/*********************************************************************
 * EverythingToPLCsim, Interface to communicate with PLCSim
 * 
 * Copyright (C) 2011-2016 Thomas Wiens, th.wiens@gmx.de
 * Modified 2025 Thomas Schubert
 *
 * This file is part of EverythingToPLCSim.
 *
 * EverythingToPLCSim is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 /*********************************************************************/

using PlcsimS7online;
using System.Data;
using System.Net;
using System.Runtime.InteropServices;

namespace ETS7o
{
    public enum PlcsimProtocolType
    {
        S7comm = 0,             // Used for Step7 V5 Plcsim, and TIA-Plcsim for 1200/1500 when using absolute address mode (put/get) -> 0x32 protocol header
        S7commPlus              // Used for TIA-Plcsimusing new protocol used for 1200/1500 -> 0x72 protocol header
    }

    public class S7OServiceProvider
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(IntPtr hwnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public PlcS7onlineMsgPump m_PlcS7onlineMsgPump;
        IntPtr m_PlcS7onlineMsgPump_Handle;
        AutoResetEvent m_autoEvent_MsgPumpThreadStart;
        AutoResetEvent m_autoEvent_ConnectPlcsim;
        bool m_ConnectPlcsimSuccess;
        public int m_PlcsimRackNumber;
        public int m_PlcsimSlotNumber;
        public IPAddress m_PlcsimIpAdress;
        public bool m_S7ProtocolVersionDetected;
        public string m_Name;

        public event Action<S7OServiceProvider, byte[]>? StatusUpdate;
        public event Action<S7OServiceProvider, byte[]>? DataReceived;
        public event Action<S7OServiceProvider, byte[]>? DataSend;
        public event Action<S7OServiceProvider, int>? ConnectionStatusChanged;

        private byte[] ConnectRequest = { 50, 1, 0, 0, 255, 255, 0, 8, 0, 0, 240, 0, 0, 3, 0, 3, 3, 192 };
        private byte[] StatusRequest  = { 50, 7, 0, 0, 0, 0, 0, 8, 0, 8, 0, 1, 18, 4, 17, 68, 1, 0, 255, 9, 0, 4, 4, 36, 0, 0 };

        public S7OServiceProvider(string name, IPAddress plcsimIp, int plcsimRackNumber, int plcsimSlotNumber)
        {
            m_PlcS7onlineMsgPump_Handle = IntPtr.Zero;
            m_PlcsimIpAdress = plcsimIp;
            m_PlcsimRackNumber = plcsimRackNumber;
            m_PlcsimSlotNumber = plcsimSlotNumber;
            m_Name = name;
        }

        ~S7OServiceProvider()
        {
            ExitPlcsimMessagePump();
        }

        public void IsoLog(string message)
        {
            Console.WriteLine(message);
        }

        public void SendDataWrapper(byte[] data)
        {
            try
            {
                PlcS7onlineMsgPump.WndProcMessage msg = new PlcS7onlineMsgPump.WndProcMessage();
                msg.pdu = data;
                msg.pdulength = data.Length;

                byte[] res = null;

                // Test if we have to generate our own answer
                res = S7ProtoHook.RequestExchange(data);
                if (res == null)
                {
                    SendDataToPlcsim(msg);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void OnDropConnection()
        {
            ExitPlcsimMessagePump();
        }

        public void ExitPlcsimMessagePump()
        {
            SendMessage(m_PlcS7onlineMsgPump_Handle, PlcS7onlineMsgPump.WM_M_EXIT, IntPtr.Zero, IntPtr.Zero);
        }

        public bool StartPLCSimStation(int protocollType)
        {
            bool plcsim_success = false;

            // 0x32 = S7comm
            // 0x72 = S7commPlus (1200/1500)
            if (m_S7ProtocolVersionDetected == false)
            {
                string message = String.Empty;
                
                if (protocollType == 0x72)
                {
                    plcsim_success = InitPlcsim(PlcsimProtocolType.S7commPlus);
                    message = "Connecting to Plcsim using S7Comm-Plus mode for 1200/1500";
                }
                else
                {
                    plcsim_success = InitPlcsim(PlcsimProtocolType.S7comm);
                    message = "Connecting to Plcsim using S7Comm mode for 300/400 or 1200/1500 (not optimized)";
                }
                m_S7ProtocolVersionDetected = true;
            }

            if(plcsim_success)
            {
                PlcS7onlineMsgPump.WndProcMessage msg = new PlcS7onlineMsgPump.WndProcMessage();
                msg.pdu = ConnectRequest;
                msg.pdulength = ConnectRequest.Length;

                SendDataToPlcsim(msg);
            }

            return plcsim_success;
        }

        public void UpdateStatus()
        {
            PlcS7onlineMsgPump.WndProcMessage msg = new PlcS7onlineMsgPump.WndProcMessage();
            msg.pdu = StatusRequest;
            msg.pdulength = StatusRequest.Length;

            SendDataToPlcsim(msg);
        }

        private bool InitPlcsim(PlcsimProtocolType plcsimVersion)
        {
            m_autoEvent_MsgPumpThreadStart = new AutoResetEvent(false);
            StartPlcS7onlineMsgPump(plcsimVersion);
            m_autoEvent_MsgPumpThreadStart.WaitOne();       // Wait until the message pumpe thread has started
            try
            {
                m_ConnectPlcsimSuccess = false;
                m_autoEvent_ConnectPlcsim = new AutoResetEvent(false);
                SendMessage(m_PlcS7onlineMsgPump_Handle, PlcS7onlineMsgPump.WM_M_CONNECTPLCSIM, IntPtr.Zero, IntPtr.Zero);
                m_autoEvent_ConnectPlcsim.WaitOne();        // Wait until a connect success or connect error was received
            }
            catch
            {
                return false;
            }
            return m_ConnectPlcsimSuccess;
        }

        private void SendDataToPlcsim(PlcS7onlineMsgPump.WndProcMessage msg)
        {
            Int32 length = msg.pdu.Length + 4;
            byte[] buffer = new byte[length];

            byte[] pduLengthBytes = BitConverter.GetBytes(msg.pdulength);
            Buffer.BlockCopy(pduLengthBytes, 0, buffer, 0, pduLengthBytes.Length);
            Buffer.BlockCopy(msg.pdu, 0, buffer, pduLengthBytes.Length, msg.pdu.Length);

            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.Copy(buffer, 0, ptr, length);

            SendMessage(m_PlcS7onlineMsgPump_Handle, PlcS7onlineMsgPump.WM_M_SENDDATA, IntPtr.Zero, ptr);

            Marshal.FreeHGlobal(ptr);
        }

        private void StartPlcS7onlineMsgPump(PlcsimProtocolType plcsimVersion)
        {
            Thread PlcS7onlineMsgPumpThread = new Thread(StartPlcS7onlineMsgPumpThread);
            if (plcsimVersion == PlcsimProtocolType.S7commPlus)
            {
                m_PlcS7onlineMsgPump = new PlcS7onlineMsgPumpTia(m_PlcsimIpAdress, m_PlcsimRackNumber, m_PlcsimSlotNumber);
            }
            else 
            {
                m_PlcS7onlineMsgPump = new PlcS7onlineMsgPumpS7(m_PlcsimIpAdress, m_PlcsimRackNumber, m_PlcsimSlotNumber);
            }
            m_PlcS7onlineMsgPump.eventOnDataFromPlcsimReceived += new PlcS7onlineMsgPump.OnDataFromPlcsimReceived(OnDataFromPlcsimReceived);
            PlcS7onlineMsgPumpThread.Start();
        }

        private void StartPlcS7onlineMsgPumpThread()
        {
            m_PlcS7onlineMsgPump_Handle = m_PlcS7onlineMsgPump.Handle;
            m_autoEvent_MsgPumpThreadStart.Set();
            m_PlcS7onlineMsgPump.Run();
        }

        private void OnDataFromPlcsimReceived(PlcS7onlineMsgPump.MessageFromPlcsim message)
        {
            switch (message.type)
            {
                case PlcS7onlineMsgPump.MessageFromPlcsimType.Pdu:
                    // Some telegrams may have to be modified by Nettoplcsim, e.g. response to connection setup
                    // 30.1.2016: This should be no longer needed
                    //S7ProtoHook.ResponseExchange(ref message.pdu);
                    if (message.pdu.Length == 15 || (message.pdu.Length == 12 && message.pdu[10] == 133))
                        DataSend.Invoke(this, message.pdu);
                    else if (message.pdu.Length == 54 && message.pdu[1] == 7)
                        StatusUpdate.Invoke(this, message.pdu);
                    else
                        DataReceived.Invoke(this, message.pdu);
                    break;
                case PlcS7onlineMsgPump.MessageFromPlcsimType.ConnectError:
                    m_ConnectPlcsimSuccess = false;
                    m_autoEvent_ConnectPlcsim.Set();
                    if (ConnectionStatusChanged != null)
                        ConnectionStatusChanged.Invoke(this, 2);
                    break;
                case PlcS7onlineMsgPump.MessageFromPlcsimType.ConnectSuccess:
                    m_ConnectPlcsimSuccess = true;
                    m_autoEvent_ConnectPlcsim.Set();
                    if (ConnectionStatusChanged != null)
                        ConnectionStatusChanged.Invoke(this, 1);
                    break;
                //default:
                    // don't care about other messages at this time
                    //System.Diagnostics.Debug.Print("OnDataFromPlcsimReceived(): Type=" + message.type.ToString() + " Message=" + message.textmessage);
            }
        }
    }
}
