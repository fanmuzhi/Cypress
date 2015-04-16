using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    [XmlRootAttribute("DUT_Str", Namespace = "", IsNullable = false)]
    public class DUT_Str
    {
        public DUT_Str()
        {       
        }
        
        // Serial Number
        private string m_SerialNumber;
        public string SerailNumber
        {
            set { m_SerialNumber = value; }
            get { return m_SerialNumber; }
        }

        //Error Code
        private string m_ErrorCode;
        public string ErrorCode
        {
            set { m_ErrorCode = value; }
            get { return m_ErrorCode; }
        }

        //Part Type
        private string m_PartType;
        public string PartType
        {
            set { m_PartType = value; }
            get { return m_PartType; }
        }

        //Test Station
        private string m_TestStation;
        public string TestStation
        {
            set { m_TestStation = value; }
            get { return m_TestStation; }
        }

        //Test Time
        private string m_TestTime;
        public string TestTime
        {
            set { m_TestTime = value; }
            get { return m_TestTime; }
        }

        // IDD Value
        private string m_IDDValue;
        public string IDDValue
        {
            set { m_IDDValue = value; }
            get { return m_IDDValue; }
        }

        // Sleep1 IDD Value
        private string m_IDDValueSleep1;
        public string IDDValueSleep1
        {
            set { m_IDDValueSleep1 = value; }
            get { return m_IDDValueSleep1; }
        }

        // Deep Sleep IDD Value
        private string m_IDDValueDeepSleep;
        public string IDDValueDeepSleep
        {
            set { m_IDDValueDeepSleep = value; }
            get { return m_IDDValueDeepSleep; }
        }

        //Programming Status
        private string m_ProgrammingStatus;
        public string ProgrammingStatus
        {
            set { m_ProgrammingStatus = value; }
            get { return m_ProgrammingStatus; }
        }

        //Left Button Status
        private string m_LeftButtonStatus;
        public string LeftButtonStatus
        {
            set { m_LeftButtonStatus = value; }
            get { return m_LeftButtonStatus; }
        }

        //Right Button Status
        private string m_RightButtonStatus;
        public string RightButtonStatus
        {
            set { m_RightButtonStatus = value; }
            get { return m_RightButtonStatus; }
        }

        //Firmware Revision
        private string m_FwRev;
        public string FwRev
        {
            set { m_FwRev = value; }
            get { return m_FwRev; }
        }

        //Num of Column
        private string m_NumCols;
        public string NumCols
        {
            set { m_NumCols = value; }
            get { return m_NumCols; }
        }

        //Num of Row
        private string m_NumRows;
        public string NumRows
        {
            set { m_NumRows = value; }
            get { return m_NumRows; }
        }

        //Raw Count X
        private List<string> m_RawCountX;
        public List<string> RawCountX
        {
            set { m_RawCountX = value; }
            get { return m_RawCountX; }
        }

        //Signal X
        private List<string> m_SignalX;
        public List<string> SignalX
        {
            set { m_SignalX = value; }
            get { return m_SignalX; }
        }

        //SNR X
        private List<string> m_SNRX;
        public List<string> SNRX
        {
            set { m_SNRX = value; }
            get { return m_SNRX; }
        }

        //Raw Count Y
        private List<string> m_RawCountY;
        public List<string> RawCountY
        {
            set { m_RawCountY = value; }
            get { return m_RawCountY; }
        }

        //Noise X
        private List<string> m_NoiseX;
        public List<string> NoiseX
        {
            set { m_NoiseX = value; }
            get { return m_NoiseX; }
        }

        //Noise Y
        private List<string> m_NoiseY;
        public List<string> NoiseY
        {
            set { m_NoiseY = value; }
            get { return m_NoiseY; }
        }

        //StdDev X
        private List<string> m_StdDevX;
        public List<string> StdDevX
        {
            set { m_StdDevX = value; }
            get { return m_StdDevX; }
        }

        //StdDev Y
        private List<string> m_StdDevY;
        public List<string> StdDevY
        {
            set { m_StdDevY = value; }
            get { return m_StdDevY; }
        }


        //IDAC
        private List<string> m_IDAC;
        public List<string> IDAC
        {
            set { m_IDAC = value; }
            get { return m_IDAC; }
        }

        //IDAC Gain
        private List<string> m_IDACGain;
        public List<string> IDACGain
        {
            set { m_IDACGain = value; }
            get { return m_IDACGain; }
        }

        //IDAC Erased
        private List<string> m_IDAC_Erased;
        public List<string> IDAC_Erased
        {
            set { m_IDAC_Erased = value; }
            get { return m_IDAC_Erased; }
        }

        //IDAC
        private List<string> m_Global_IDAC;
        public List<string> Global_IDAC
        {
            set { m_Global_IDAC = value; }
            get { return m_Global_IDAC; }
        }

        //IDAC
        private List<string> m_Local_IDAC;
        public List<string> Local_IDAC
        {
            set { m_Local_IDAC = value; }
            get { return m_Local_IDAC; }
        }

    }
}

