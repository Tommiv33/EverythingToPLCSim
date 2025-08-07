﻿/*********************************************************************
 * NetToPLCsim, Netzwerkanbindung fuer PLCSIM
 * 
 * Copyright (C) 2011-2016 Thomas Wiens, th.wiens@gmx.de
 *
 * This file is part of NetToPLCsim.
 *
 * NetToPLCsim is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 /*********************************************************************/

using System.Net;
using System.ComponentModel;

namespace ETS7o.Types
{
    public class StationData : INotifyPropertyChanged
    {
        private string m_Name;
        private IPAddress m_PlcsimIpAddress;
        private int m_PlcsimRackNumber;
        private int m_PlcsimSlotNumber;
        private bool m_TsapCheckEnabled;
        private bool m_Connected;
        private string m_Status;

        public event PropertyChangedEventHandler PropertyChanged;

        public StationData()
        {
            m_Name = String.Empty;
            m_PlcsimIpAddress = IPAddress.None;
            m_PlcsimRackNumber = 0;
            m_PlcsimSlotNumber = 1;
            m_TsapCheckEnabled = false;
            m_Status = StationStatus.READY.ToString();
        }

        public StationData(string name, IPAddress plcsimIpAddress, int rack, int slot, bool tsapCheckEnabled)
        {
            m_Name = name;
            m_PlcsimIpAddress = plcsimIpAddress;
            m_PlcsimRackNumber = rack;
            m_PlcsimSlotNumber = slot;
            m_TsapCheckEnabled = tsapCheckEnabled;
            m_Status = StationStatus.READY.ToString();
        }

        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        public IPAddress PlcsimIpAddress
        {
            get { return m_PlcsimIpAddress; }
            set
            {
                m_PlcsimIpAddress = value;
                this.NotifyPropertyChanged("PlcsimIpAddress");
            }
        }

        public int PlcsimRackNumber
        {
            get { return m_PlcsimRackNumber; }
            set
            {
                m_PlcsimRackNumber = value;
                this.NotifyPropertyChanged("PlcsimRackSlot");
            }
        }

        public int PlcsimSlotNumber
        {
            get { return m_PlcsimSlotNumber; }
            set
            {
                m_PlcsimSlotNumber = value;
                this.NotifyPropertyChanged("PlcsimRackSlot");
            }
        }

        public string PlcsimRackSlot
        {
            get { return m_PlcsimRackNumber.ToString() + "/" + m_PlcsimSlotNumber.ToString(); }
        }

        public bool TsapCheckEnabled
        {
            get { return m_TsapCheckEnabled; }
            set
            {
                m_TsapCheckEnabled = value;
                this.NotifyPropertyChanged("TsapCheckEnabled");
            }
        }

        public bool Connected
        {
            get { return m_Connected; }
            set
            {
                m_Connected = value;
                this.NotifyPropertyChanged("Connected");
            }
        }

        public string Status
        {
            get { return m_Status; }
            set
            {
                m_Status = value;
                this.NotifyPropertyChanged("Status");
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public StationData ShallowCopy()
        {
            return (StationData)this.MemberwiseClone();
        }
    }
}
