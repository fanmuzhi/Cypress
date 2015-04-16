using System;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Windows.Forms; // for Application.StartupPath
using System.Runtime.InteropServices;
using System.Text;
using CypressSemiconductor.ChinaManufacturingTest;

namespace CypressSemiconductor.ChinaManufacturingTest.AFT
{
    /// <summary>
    /// Persistent settings
    /// </summary>
    public class HardwareConfig
    {
        public class Port
        {
            public static string MPQPort = "COM1";
            public static string ScannerPort = "COM2";
        }

        public class Option
        {
            
            public static string U2722AtxtAddr = "USB0::2391::16664::MY48440005::0::INSTR";
            public static string U2651AtxtAddr = "USB0::2391::6936::TW49265003::0::INSTR";
            public static string U2751AWellAtxtAddr = "USB0::2391::15640::MY48250045::0::INSTR";
            public static string U2751AWellBtxtAddr = "USB0::2391::15640::my48250146::0::INSTR";
        }

        /// <summary>
        /// Read the settings from disk. 
        /// </summary>
        public static void Read()
        {
            IniFile ini = new IniFile(Application.StartupPath + @"\Hardware Settings.ini");
            
            Port.MPQPort = ini.GetString("Port", "MPQPortName", Port.MPQPort);
            Port.ScannerPort = ini.GetString("Port", "ScannerPortName", Port.ScannerPort);
            
            Option.U2651AtxtAddr = ini.GetString("Option", "U2651A", Option.U2651AtxtAddr);
            Option.U2722AtxtAddr = ini.GetString("Option", "U2722A", Option.U2722AtxtAddr);
            Option.U2751AWellAtxtAddr = ini.GetString("Option", "U2751AWellA", Option.U2751AWellAtxtAddr);
            Option.U2751AWellBtxtAddr = ini.GetString("Option", "U2751AWellB", Option.U2751AWellBtxtAddr);
        }

        /// <summary>
        /// Write the settings to disk. 
        /// </summary>
        public static void Write()
        {
            IniFile ini = new IniFile(Application.StartupPath + @"\Hardware Settings.ini");

            ini.WriteValue("Port", "MPQPortName", Port.MPQPort);
            ini.WriteValue("Port", "ScannerPortName", Port.ScannerPort);

            ini.WriteValue("Option", "U2651A", Option.U2651AtxtAddr);
            ini.WriteValue("Option", "U2722A", Option.U2722AtxtAddr);
            ini.WriteValue("Option", "U2751AWellA", Option.U2751AWellAtxtAddr);
            ini.WriteValue("Option", "U2751AWellB", Option.U2751AWellBtxtAddr);
        }
    }
}

