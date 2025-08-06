# Everything To PLC-Sim
## Introduction
This library is a modified version of NetToPLCSim v 1.2.5 from Thomas Wiens (aka the GOAT).
It enables a direct communication with PLC-Sim via your GUI without the TCP/IP connection. The class library is also updated to .Net 8.0.

The folder "ETS7o GUI" contains several very usefull GUI parts, either from NetToPlcsim or myself.

**This library requiers S7netplus Nugget package**

## Implementation
Either include the compiled dll or the source code in your project. You  have to set the CPU target to x86 for your project.

## How to use it
### Start Service
First you have to creat an instance of PLCSimInterface. With the method
```
StartStation(int protocollType, string name, IPAddress plcsimIpAddress, int rack, int slot)
```
you create an instance of your PLC, directly connect to it and if succesfully connected, add it to its list of PLCs.
### Read Data
I highly recommend to run the loops that reads and writes in a backgroundworker (Win Forms) or Thread.
```
backgroundWorkerRead.DoWork += (s,e) => ReadPLCAsync(_cancellationTokenSPS.Token);
backgroundWorkerRead.WorkerSupportsCancellation = true;

backgroundWorkerWrite.DoWork += (s,e) => WritePLCAsync(_cancellationTokenSPS.Token);
backgroundWorkerWrite.WorkerSupportsCancellation = true;
```
There are multiple ways to read data from the PLC and there is a short description for each method. The intended way to read is the Task
```
ReadWrapperAsync(int index, int startAddress, int bytesToRead)
```
**index:** Position Index within m_PLCs of the PLC you want to read  
**startAddress:** Starting Address of the memory area  
**bytesToRead:** Number of consecutive bytes you want to read  
This task runs async and therefor can be awaited. This synchronizes request and answer for easier handling.
### Write Data
Same as reading there are multiple ways to write data to the PLC. The intended way is either the task
```
WriteByteWrapperAsync(int index, int startAddress, byte[] data)
```
**index:** Position Index within m_PLCs of the PLC you want to write  
**startAddress:** Starting Address of the memory area  
**data:** Data Bytes you want to write to the PLC

or if you just want to set a specific bit
```
WriteBitWrapperAsync(int index, int startAddress, int bitAddress, int value)
```
***value:*** can either be 0 or 1
### Closing you Form
Upon closing your Programm ```Close()``` has to be called, otherwise the background Thread to communicate with PLC-Sim will block the process and prevent closing.
## License
EverythingToPLCsim is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
