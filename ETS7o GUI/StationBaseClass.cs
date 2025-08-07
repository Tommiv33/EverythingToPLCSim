/*********************************************************************
 * EverythingToPLCsim, Interface to communicate with PLCSim
 * 
 * Copyright (C) 2011-2016 Thomas Wiens, th.wiens@gmx.de
 * Copyright (C) 2025 Thomas Schubert
 *
 * This file is part of EverythingToPLCSim.
 *
 * EverythingToPLCSim is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 /*********************************************************************/

using ini;
using ETS7o.Tools;

namespace ETS7oGUI
{
	/// <SUMMARY>
	/// This class is the base class for handling PLC stations. 
	/// Its main purpose is to preserve usefull GUI functions from NetToPLCSim. 
	/// Inherit it to your primary Class to implement them
	/// </SUMMARY>
    public class StationBaseClass
    {
        public Config m_Conf = new Config();

        #region Station Handling

        public string AddNewStation(bool isSim)
        {
            string result = "";

            FormStationEdit dlg = new FormStationEdit(isSim);

            string station_name;
            int station_nr = 1;
            bool found = false;
            do
            {
                // Enter a default Station dummy name
                station_name = System.String.Format("PLC#{0:000}", station_nr);
                if (m_Conf.IsStationNameUnique(station_name) == false)
                {
                    station_nr++;
                }
                else
                {
                    found = true;
                }
            } while (found == false);

            dlg.Station.Name = station_name;
            dlg.ShowDialog();

            if (dlg.DialogResult == DialogResult.OK)
            {
                if (m_Conf.IsStationNameUnique(station_name) == false)
                {
                    MessageBox.Show("Station with name '" + dlg.Station.Name + "' already exists.\r\nPlease use a unique name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                StationData station = new StationData(dlg.Station.Name, dlg.Station.PlcsimIpAddress,
                    dlg.Station.PlcsimRackNumber, dlg.Station.PlcsimSlotNumber, dlg.Station.TsapCheckEnabled);

                m_Conf.Stations.Add(station);
                result = dlg.Station.Name;
            }
            else
                result = "Error";

            return result;
        }

        public string ModifyStation(int selectedRow, bool isSim)
        {
            string result = "";

            if (selectedRow < 0)
            {
                return "";
            }
            else
            {
                FormStationEdit dlg = new FormStationEdit(isSim);

                dlg.Station = m_Conf.Stations[selectedRow].ShallowCopy();

                dlg.ShowDialog();
                if (dlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    if (m_Conf.IsStationNameUniqueExcept(dlg.Station.Name, selectedRow) == false)
                    {
                        MessageBox.Show("Station with name '" + dlg.Station.Name + "' already exists.\r\nPlease use a unique name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return "";
                    }
                    m_Conf.Stations[selectedRow] = dlg.Station;
                    result = dlg.Station.Name;
                }
            }

            return result;
        }

        public bool DeleteSelectedStations(int selectedRow)
        {
            bool result = false;

            if (selectedRow >= 0)
            {
                m_Conf.Stations.RemoveAt(selectedRow);
                result = true;
            }
            return result;
        }

        #endregion

        #region Ini / Config

        public string LoadIni()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Browse Ini Files";
                ofd.DefaultExt = "ini";
                ofd.Filter = "ini files (*.ini)|*.ini|All files (*.*)|*.*";

                DialogResult result = ofd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        return LoadIniFile(ofd.FileName); ;
                    }
                    catch (Exception ex)
                    {
                        //ShowError("Error", "An error occurred while reading ini file: " + ex.Message);
                    }
                }

                return "";
            }
        }

        private string LoadIniFile(string file)
        {
            int stationNr = 1;
            string section;
            int rack = 0, slot = 2;
            bool firstOk = false;
            bool readOk = true;
            bool err;

            IniFile ini = new IniFile(file);
            while (readOk)
            {
                err = false;
                section = "Station_" + stationNr;
                StationData station = new StationData();
                station.Name = ini.IniReadValue(section, "Name");

                if (station.Name == System.String.Empty)
                {
                    readOk = false;
                    break;
                }

                try
                {
                    station.PlcsimIpAddress = IPAddress.Parse(ini.IniReadValue(section, "PlcsimIpAddress"));
                    if (ini.IniReadValue(section, "TsapCheckEnabled") == "True")
                    {
                        station.TsapCheckEnabled = true;
                    }
                    else
                    {
                        station.TsapCheckEnabled = false;
                    }

                    rack = Convert.ToInt32(ini.IniReadValue(section, "PlcsimRackNumber"));
                    slot = Convert.ToInt32(ini.IniReadValue(section, "PlcsimSlotNumber"));
                }
                catch
                {
                    err = true;
                }

                if (firstOk == false && err == false)
                {
                    m_Conf.Stations.Clear();
                    firstOk = true;
                    m_ConfigName = file;
                    //SetFormText(file);
                }

                if (err == false && station.Name != System.String.Empty && m_Conf.IsStationNameUnique(station.Name) &&
                    rack >= 0 && rack <= 3 && slot >= 0 && slot <= 18)
                {
                    station.PlcsimRackNumber = rack;
                    station.PlcsimSlotNumber = slot;
                    m_Conf.Stations.Add(station);
                }
                else
                {
                    MessageBox.Show("Error in station-configuration ini-file in section '" + section + "'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    readOk = false;
                }
                stationNr++;
            }

            return file;
        }

        public string SaveIni()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Browse Ini Files";
                sfd.DefaultExt = "ini";
                sfd.Filter = "ini files (*.ini)|*.ini|All files (*.*)|*.*";
                string Time = System.DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss");
                sfd.FileName = Time;
                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        SaveConfigToFile(sfd.FileName);
                        m_ConfigName = sfd.FileName;
                        //SetFormText(m_ConfigName);
                    }
                    catch (Exception ex)
                    {
                        //ShowError("Error", "An error occurred while writing ini file: " + ex.Message);
                    }
                }
            }

            return m_ConfigName;
        }

        private void SaveConfigToFile(string file)
        {
            IniFile ini = new IniFile(file);
            ini.DeleteFileIfExists();

            int stationNr = 1;
            string section;

            foreach (StationData st in m_Conf.Stations)
            {
                section = "Station_" + stationNr;
                ini.IniWriteValue(section, "Name", st.Name);
                ini.IniWriteValue(section, "PlcsimIpAddress", st.PlcsimIpAddress.ToString());
                ini.IniWriteValue(section, "PlcsimRackNumber", st.PlcsimRackNumber.ToString());
                ini.IniWriteValue(section, "PlcsimSlotNumber", st.PlcsimSlotNumber.ToString());
                ini.IniWriteValue(section, "TsapCheckEnabled", st.TsapCheckEnabled.ToString());
                stationNr++;
            }
        }

        #endregion

    }
}
