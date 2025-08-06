/*********************************************************************
 * EverythingToPLCsim, Interface to communicate with PLCSim
 * 
 * Copyright (C) 2025 Thomas Schubert
 *
 * This file is part of EverythingToPLCSim.
 *
 * EverythingToPLCSim is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 /*********************************************************************/

using S7.Net;
using S7.Net.Types;
using System;
using System.Net;

namespace ETS7o
{
    internal class PLCSimInterface
    {
        List<S7OServiceProvider> m_PLCs = new();

        private const int MaxPDUSize = 480;             // S7.Net Lib says so
        private const int DBNAME = 0;                   // I/O is DB0, if you want to read any other DB this has to be edit

        private static TaskCompletionSource<string>? responseStatus;
        private static TaskCompletionSource<byte[]>? responseRx;
        private static TaskCompletionSource<int>?    responseTx;

        public PLCSimInterface() { }

        public string GetPLCIP(int index)
        {
            return m_PLCs[index].m_PlcsimIpAdress.ToString();
        }

        public string GetPLCName(int index)
        {
            return m_PLCs[index].m_Name;
        }

        /// <summary>
        /// This method initializes a new PLC obejct an attempt to connect to the PLC
        /// A successful connection returns a 1, an Exception a 0, a failed connection a -1
        /// </summary>
        public int StartStation(int protocollType, string name, IPAddress plcsimIpAddress, int rack, int slot)
        {
            int result = 0;

            try
            {
                S7OServiceProvider srv = new S7OServiceProvider(name, plcsimIpAddress, rack, slot);
                srv.StatusUpdate += StatusUpdate;
                srv.DataReceived += ReceivedData;
                srv.DataSend     += WritedData;
                if (!srv.StartPLCSimStation(protocollType))
                    return -1;
                m_PLCs.Add(srv);
                result = 1;
            }
            catch
            {
                return 0;
            }

            return result;
        }

        /// <summary>
        /// This method requests the Status from the first PLC in the list and awaits the response.
        /// It returns the status as a string
        /// </summary>
        public string GetPLCStatus()
        {
            return Task.Run(() => GetPLCStatusAsync(0)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// This async Task requests the Status of a specified PLC and awaits the response.
        /// It returns the status as a string
        /// </summary>
        public async Task<string> GetPLCStatusAsync(int index)
        {
            if (index + 1 > m_PLCs.Count)
                return "Index out of range";

            try
            {
                responseStatus = new();

                m_PLCs[index].UpdateStatus();
                
                return await responseStatus.Task;
            }
            catch (Exception ex)
            {
                throw new PlcException(ErrorCode.WriteData, ex);
            }
        }

        /// <summary>
        /// This method has to be called when closing the application,
        /// otherwise the process is blocked by the MsgPump Thread 
        /// </summary>
        public void Close()
        {
            foreach(S7OServiceProvider srv in m_PLCs)
            {
                srv.ExitPlcsimMessagePump();
            }
        }

        #region Callback

        public static void StatusUpdate(S7OServiceProvider station, byte[] data)
        {
            if (responseStatus != null && !responseStatus.Task.IsCompleted)
            {
                string result = station.m_Name + ";";

                switch(data[37])
                {
                    case 3:
                        result += "Stop";
                        break;

                    case 8:
                        result += "Run";
                        break;

                    default:
                        result += "Error";
                        break;
                }

                responseStatus.SetResult(result);
            }
        }

        public static void ReceivedData(S7OServiceProvider station, byte[] data)
        {
            if (responseRx != null && !responseRx.Task.IsCompleted)
            {
                if ((ReadWriteErrorCode)data[14] == ReadWriteErrorCode.Success)
                {
                    byte[] buffer = new byte[data.Length - 18];

                    Buffer.BlockCopy(data, 18, buffer, 0, buffer.Length);

                    responseRx.SetResult(buffer); 
                }
                else
                {
                    byte[] err = { data[14] };
                    responseRx.SetResult(err);
                }
            }
        }

        public static void WritedData(S7OServiceProvider station, byte[] data)
        {
            if (responseTx != null && !responseTx.Task.IsCompleted)
            {
                int result = -1;

                if (data.Length == 15)
                {
                    if ((ReadWriteErrorCode)data[14] == ReadWriteErrorCode.Success)
                        result = 1;
                    else
                        result = data[14];
                }

                responseTx.SetResult(result);
            }
        }

        #endregion

        #region Read / Write Handling

        #region Read

        /// <summary>
        /// This method reads the data from the first PLC in the list and awaits the response.
        /// It returns the read data as byte[], return value null means no connected PLC
        /// </summary>
        public byte[]? ReadWrapper(int startAddress, int bytesToRead)
        {
            return Task.Run(() => ReadWrapperAsync(0, startAddress, bytesToRead)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// This async Task reads the data from PLCSim and awaits the response.
        /// It returns the read data as byte[], return value null means wrong index
        /// </summary>
        /// <param name="index">Position Index within m_PLCs of the PLC you want to read.</param>
        /// <param name="startAddress">Starting Address of the memory area</param>
        /// <param name="bytesToRead">Number of consecutive bytes you want to read</param>
        public async Task<byte[]?> ReadWrapperAsync(int index, int startAddress, int bytesToRead)
        {
            if (index + 1 > m_PLCs.Count)
                return null;

            try
            {
                responseRx = new();

                // Send the message
                ReadBytesRequest(index, DataType.Output, DBNAME, startAddress, bytesToRead);

                // Await the response
                return await responseRx.Task;
            }
            catch
            {
                byte[] error = {255};
                return error;
            }
        }

        private void ReadBytesRequest(int index, DataType dataType, int db, int startByteAdr, int bytesToRead)
        {
            try
            {
                var dataToSend = BuildReadBytesPackage(dataType, db, startByteAdr, bytesToRead);

                m_PLCs[index].SendDataWrapper(dataToSend);
            }
            catch (Exception ex)
            {
                throw new PlcException(ErrorCode.WriteData, ex);
            }
        }

        private byte[] BuildReadBytesPackage(DataType dataType, int db, int startByteAdr, int bytesToRead)
        {
            // first create the header
            int packageSize = 24;
            var packageData = new byte[packageSize];
            var package = new MemoryStream(packageData);

            // This overload doesn't allocate the byte array, it refers to assembly's static data segment
            package.Write(new byte[] { 0x32, 1, 0, 0 });
            package.Write(new byte[] { 0, 0 });
            package.Write(new byte[] { 0, 0x0e });
            package.Write(new byte[] { 0, 0 });
            package.Write(new byte[] { 0x04, 0x01, 0x12, 0x0a, 0x10, 0x02 });
            package.Write(Word.ToByteArray((ushort)bytesToRead));
            package.Write(Word.ToByteArray((ushort)db));
            package.WriteByte((byte)dataType);
            var overflow = (int)(startByteAdr * 8 / 0xffffU); // handles words with address bigger than 8191
            package.WriteByte((byte)overflow);
            package.Write(Word.ToByteArray((ushort)(startByteAdr * 8)));

            return packageData;
        }
        #endregion

        #region Write

        /// <summary>
        /// This method is only for testing purposes, it directly writes to the first PLC in the list without return value
        /// </summary>
        public void WriteByteToPLC(int startAddress, byte[] data)
        {
            WriteBytes(0, DataType.Input, DBNAME, startAddress, data);
        }

        /// <summary>
        /// This method writes the data to the first PLC in the list and awaits the response.
        /// A positiv return value indicates a successful write attempt
        /// </summary>
        public int WriteByteWrapper(int startAddress, byte[] data)
        {
            return Task.Run(() => WriteByteWrapperAsync(0, startAddress, data)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// This async Task is the main way to write data to PLCSim.
        /// A positiv return value indicates a successful write attempt
        /// </summary>
        /// <param name="index">Position Index within m_PLCs of the PLC you want to read.</param>
        /// <param name="startAddress">Starting Address of the memory area</param>
        /// <param name="data">Data Bytes you want to write to the PLC</param>
        public async Task<int> WriteByteWrapperAsync(int index, int startAddress, byte[] data)
        {
            if (index + 1 > m_PLCs.Count)
                return -2;

            try
            {
                responseTx = new();

                WriteBytes(index, DataType.Input, DBNAME, startAddress, data);

                return await responseTx.Task;
            }
            catch
            {
                return -1;
            }
        }

        private void WriteBytes(int index, DataType dataType, int db, int startByteAdr, ReadOnlySpan<byte> value)
        {
            int localIndex = 0;
            while (value.Length > 0)
            {
                var maxToWrite = Math.Min(value.Length, MaxPDUSize - 28);//TODO tested only when the MaxPDUSize is 480
                WriteBytesWithASingleRequest(index, dataType, db, startByteAdr + localIndex, value.Slice(0, maxToWrite));
                value = value.Slice(maxToWrite);
                localIndex += maxToWrite;
            }
        }

        private void WriteBytesWithASingleRequest(int index, DataType dataType, int db, int startByteAdr, ReadOnlySpan<byte> value)
        {
            try
            {
                var dataToSend = BuildWriteBytesPackage(dataType, db, startByteAdr, value);

                m_PLCs[index].SendDataWrapper(dataToSend);
            }
            catch (Exception ex)
            {
                throw new PlcException(ErrorCode.WriteData, ex);
            }
        }

        private byte[] BuildWriteBytesPackage(DataType dataType, int db, int startByteAdr, ReadOnlySpan<byte> value)
        {
            int varCount = value.Length;
            // first create the header
            int packageSize = 28 + varCount;
            var packageData = new byte[packageSize];
            var package = new MemoryStream(packageData);

            // This overload doesn't allocate the byte array, it refers to assembly's static data segment
            package.Write(new byte[] {0x32, 1, 0, 0 });
            package.Write(Word.ToByteArray((ushort)(varCount - 1)));
            package.Write(new byte[] { 0, 0x0e });
            package.Write(Word.ToByteArray((ushort)(varCount + 4)));
            package.Write(new byte[] { 0x05, 0x01, 0x12, 0x0a, 0x10, 0x02 });
            package.Write(Word.ToByteArray((ushort)varCount));
            package.Write(Word.ToByteArray((ushort)db));
            package.WriteByte((byte)dataType);
            var overflow = (int)(startByteAdr * 8 / 0xffffU); // handles words with address bigger than 8191
            package.WriteByte((byte)overflow);
            package.Write(Word.ToByteArray((ushort)(startByteAdr * 8)));
            package.Write(new byte[] { 0, 4 });
            package.Write(Word.ToByteArray((ushort)(varCount * 8)));

            // now join the header and the data
            package.Write(value);

            return packageData;
        }

        /// <summary>
        /// This method is only for testing purposes, it directly writes to the first PLC in the list without return value
        /// </summary>
        public void WriteBitToPLC(int startAddress, int bitAddress, bool value)
        {
            WriteBits(0, DataType.Input, DBNAME, startAddress, bitAddress, value);
        }

        /// <summary>
        /// This method writes a single bit to the first PLC in the list and awaits the response.
        /// A positiv return value indicates a successful write attempt
        /// </summary>
        public int WriteBitWrapper(int startAddress, int bitAddress, int value)
        {
            return Task.Run(() => WriteBitWrapperAsync(0, startAddress, bitAddress, value)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// This async Task is the main way to write data to PLCSim.
        /// A positiv return value indicates a successful write attempt
        /// </summary>
        /// <param name="index">Position Index within m_PLCs of the PLC you want to read.</param>
        /// <param name="startAddress">Starting Address of the memory area</param>
        /// <param name="value">Bit you want to write to the PLC</param>
        public async Task<int> WriteBitWrapperAsync(int index, int startAddress, int bitAddress, int value)
        {
            if (index + 1 > m_PLCs.Count)
                return -2;

            if (value < 0 || value > 1)
                throw new ArgumentException("Value must be 0 or 1", nameof(value));

            try
            {
                responseTx = new();

                WriteBits(index, DataType.Input, DBNAME, startAddress, bitAddress, value == 1);

                return await responseTx.Task;
            }
            catch
            {
                return -1;
            }
        }

        private void WriteBits(int index, DataType dataType, int db, int startByteAdr, int bitAddress, bool value)
        {
            if (bitAddress < 0 || bitAddress > 7)
                throw new InvalidAddressException(string.Format("Addressing Error: You can only reference bitwise locations 0-7. Address {0} is invalid", bitAddress));

            WriteBitsWithASingleRequest(index, dataType, db, startByteAdr, bitAddress, value);
        }

        private void WriteBitsWithASingleRequest(int index, DataType dataType, int db, int startByteAdr, int bitAddress, bool value)
        {
            try
            {
                var dataToSend = BuildWriteBitPackage(dataType, db, startByteAdr, bitAddress, value);

                m_PLCs[index].SendDataWrapper(dataToSend);
            }
            catch (Exception ex)
            {
                throw new PlcException(ErrorCode.WriteData, ex);
            }
        }

        private byte[] BuildWriteBitPackage(DataType dataType, int db, int startByteAdr, int bitAdr, bool bitValue)
        {
            var value = new[] { bitValue ? (byte)1 : (byte)0 };
            int varCount = 1;
            // first create the header
            int packageSize = 28 + varCount;
            var packageData = new byte[packageSize];
            var package = new MemoryStream(packageData);

            package.Write(new byte[] {0x32, 1, 0, 0 });
            package.Write(Word.ToByteArray((ushort)(varCount - 1)));
            package.Write(new byte[] { 0, 0x0e });
            package.Write(Word.ToByteArray((ushort)(varCount + 4)));
            package.Write(new byte[] { 0x05, 0x01, 0x12, 0x0a, 0x10, 0x01 }); //ending 0x01 is used for writing a sinlge bit
            package.Write(Word.ToByteArray((ushort)varCount));
            package.Write(Word.ToByteArray((ushort)(db)));
            package.WriteByte((byte)dataType);
            var overflow = (int)(startByteAdr * 8 / 0xffffU); // handles words with address bigger than 8191
            package.WriteByte((byte)overflow);
            package.Write(Word.ToByteArray((ushort)(startByteAdr * 8 + bitAdr)));
            package.Write(new byte[] { 0, 0x03 }); //ending 0x03 is used for writing a sinlge bit
            package.Write(Word.ToByteArray((ushort)(varCount)));

            // now join the header and the data
            package.Write(value);

            return packageData;
        }

        #endregion
        #endregion

    }

    internal enum ReadWriteErrorCode : byte
    {
        Reserved = 0x00,
        HardwareFault = 0x01,
        AccessingObjectNotAllowed = 0x03,
        AddressOutOfRange = 0x05,
        DataTypeNotSupported = 0x06,
        DataTypeInconsistent = 0x07,
        ObjectDoesNotExist = 0x0a,
        Success = 0xff
    }
}
