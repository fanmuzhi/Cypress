using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;


namespace CypressSemiconductor.ChinaManufacturingTest.TrackPad
{
    public class DUT
    {
        // Serial Number
        private string m_SerialNumber;
        public string SerailNumber
        {
            set { m_SerialNumber = value; }
            get { return m_SerialNumber; }
        }

        //Error Code
        private int m_ErrorCode;
        public int ErrorCode
        {
            set { m_ErrorCode = value; }
            get { return m_ErrorCode; }
        }

        // IDD Value
        private double m_IDDValue;
        public double IDDValue
        {
            set { m_IDDValue = value; }
            get { return m_IDDValue; }
        }

        // IDD Sleep Value
        private double m_SleepIDDValue;
        public double IDDValue_Sleep
        {
            set { m_SleepIDDValue = value; }
            get { return m_SleepIDDValue; }
        }

        // IDD Deep Sleep Value
        private double m_DeepSleepIDDValue;
        public double IDDValue_DeepSleep
        {
            set { m_DeepSleepIDDValue = value; }
            get { return m_DeepSleepIDDValue; }
        }

        //Programming Status
        private byte m_ProgrammingStatus;
        public byte ProgrammingStatus
        {
            set { m_ProgrammingStatus = value; }
            get { return m_ProgrammingStatus; }
        }

        //Left Button Status
        private byte m_LeftButtonStatus;
        public byte LeftButtonStatus
        {
            set { m_LeftButtonStatus = value; }
            get { return m_LeftButtonStatus; }
        }

        //Right Button Status
        private byte m_RightButtonStatus;
        public byte RightButtonStatus
        {
            set { m_RightButtonStatus = value; }
            get { return m_RightButtonStatus; }
        }

        //Firmware Revision
        private byte m_FwRev;
        public byte FwRev
        {
            set { m_FwRev = value; }
            get { return m_FwRev; }
        }

        //Firmware Version
        private byte m_FwVer;
        public byte FwVer
        {
            set { m_FwVer = value; }
            get { return m_FwVer; }
        }

        //Num of Column
        private byte m_NumCols;
        public byte NumCols
        {
            set { m_NumCols = value; }
            get { return m_NumCols; }
        }

        //Num of Row
        private byte m_NumRows;
        public byte NumRows
        {
            set { m_NumRows = value; }
            get { return m_NumRows; }
        }

        //Raw Count 
        private List<int> m_RawCount;
        public List<int> RawCount
        {
            set { m_RawCount = value; }
            get { return m_RawCount; }
        }
        private List<int> m_RawCountX;
        public List<int> RawCountX
        {
            set { m_RawCountX = value; }
            get { return m_RawCountX; }
        }
        private List<int> m_RawCountY;
        public List<int> RawCountY
        {
            set { m_RawCountY = value; }
            get { return m_RawCountY; }
        }

        private List<int> m_RawCount_Single;
        public List<int> RawCount_Single
        {
            set { m_RawCount_Single = value; }
            get { return m_RawCount_Single; }
        }

        //Signal 
        private List<int> m_Signal;
        public List<int> Signal
        {
            set { m_Signal = value; }
            get { return m_Signal; }
        }

        //Finger positions 
        private List<Point> m_Finger_Positions;
        public List<Point> Finger_Positions
        {
            set { m_Finger_Positions = value; }
            get { return m_Finger_Positions; }
        }
        private List<int> m_Signal_Single;
        public List<int> Signal_Single
        {
            set { m_Signal_Single = value; }
            get { return m_Signal_Single; }
        }

        //SNR 
        private List<double> m_SNR;
        public List<double> SNR
        {
            set { m_SNR = value; }
            get { return m_SNR; }
        }

        //Noise 
        private List<int> m_Noise;
        public List<int> Noise
        {
            set { m_Noise = value; }
            get { return m_Noise; }
        }

        //StdDev of Noise
        private List<double> m_StdDev;
        public List<double> StdDev
        {
            set { m_StdDev = value; }
            get { return m_StdDev; }
        }

        //IDAC
        private List<int> m_IDAC;
        public List<int> IDAC
        {
            set { m_IDAC = value; }
            get { return m_IDAC; }
        }

        //IDAC Gain
        private List<byte> m_IDACGain;
        public List<byte> IDACGain
        {
            set { m_IDACGain = value; }
            get { return m_IDACGain; }
        }

        //IDAC Erased
        private List<int> m_IDAC_Erased;
        public List<int> IDAC_Erased
        {
            set { m_IDAC_Erased = value; }
            get { return m_IDAC_Erased; }
        }

        //Global IDAC
        private List<int> m_Global_IDAC;
        public List<int> Global_IDAC
        {
            set { m_Global_IDAC = value; }
            get { return m_Global_IDAC; }
        }

        //Local IDAC
        private List<int> m_Local_IDAC;
        public List<int> Local_IDAC
        {
            set { m_Local_IDAC = value; }
            get { return m_Local_IDAC; }
        }

    }
}
