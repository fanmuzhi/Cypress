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
using System.Xml.Serialization;
using System.Reflection;
using CypressSemiconductor.ChinaManufacturingTest.TrackPad;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
{
    public class TestMessageEventArgs : EventArgs
    {
        public TestMessageEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    [XmlRootAttribute("CONFIGFILE", Namespace = "www.cypress.com", IsNullable = false)]
    public class CONFIGFILE
    {
        public int X1H;
        public int X1L;
        public int X2H;
        public int X2L;
        public int X3H;
        public int X3L;
        public int X4H;
        public int X4L;
        public int X5H;
        public int X5L;
        public int X6H;
        public int X6L;
        public int X7H;
        public int X7L;
        public int X8H;
        public int X8L;
        public int Y1H;
        public int Y1L;
        public int Y2H;
        public int Y2L;
        public int Y3H;
        public int Y3L;
        public int Y4H;
        public int Y4L;
        public int Y5H;
        public int Y5L;
        public int Y6H;
        public int Y6L;
        public int Y7H;
        public int Y7L;
        public int Y8H;
        public int Y8L;
        public int Xsym;
        public int Yorder;

    }

    public class TestFunction
    {

        #region Golbal_Varialbes

        //==============================
        //  Golbal Variables
        //==============================
        public delegate void TestMessageEventHandler(object sender, TestMessageEventArgs ea);
        public event TestMessageEventHandler TestMessageEvent;
        protected virtual void OnTestMessage(TestMessageEventArgs ea)
        {
            TestMessageEventHandler handler = TestMessageEvent;
            if (handler != null)
            {
                handler(this, ea);
            }
        }

        private USB_I2C bridge;
        public DUT dut;
        private SFCS m_SFCS;
        private TrackpadConfig m_TPCONFIG;
        private testItems m_TPCONFIGtestItems;

        public bool readPositionStop = true;
        private int _fingerX;
        public int fingerX
        {
            get { return _fingerX; }
            set { _fingerX = value; }
        }
        private int _fingerY;
        public int fingerY
        {
            get { return _fingerY; }
            set { _fingerY = value; }
        }


        private int NUM_COLS; //13  
        private int NUM_ROWS; //9
        private int RAW_DATA_READS; //20
        private int I2CReadDelay;
        public List<Point> fingerPoint = new List<Point>();
        public List<Point> targetPoint = new List<Point>();
        public int currentTarget = 0;

        private string VDD_DEFAULT;


        public bool FWVersionItem = false;
        public bool RawCountItem = false;
        public bool IDACItem = false;

        public bool idacErased = false;
        public byte[] datain_EraseIDAC;
        public bool datain_EraseIDAC_Ready = false;

        public byte[] datain_FW;
        public bool datain_FW_Ready = false;

        public List<List<byte[]>> datain_APA_RawCount = new List<List<byte[]>>();
        public bool datain_APA_RawCount_Ready = false;

        public List<byte[]> datain_RawCountX = new List<byte[]>();
        public bool datain_RawCountX_Ready = false;

        public List<byte[]> datain_RawCountY = new List<byte[]>();
        public bool datain_RawCountY_Ready = false;

        public byte[] datain_APA_GlobalIDAC;
        public bool datain_APA_GlobalIDAC_Ready = false;

        public byte[] datain_APA_IDACGain;
        public bool datain_APA_IDACGain_Ready = false;

        public List<byte[]> datain_APA_LocalIDAC = new List<byte[]>();
        public bool datain_APA_LocalIDAC_Ready = false;

        public byte[] datain_other_IDAC;
        public bool datain_other_IDAC_Ready = false;



        #endregion

        /// <summary>
        /// Constructor of TestFunction
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="UsbBridge"></param>
        public TestFunction(string serialNumber, USB_I2C UsbBridge, TrackpadConfig trackpadConfig, string testStation)
        {
            dut = new DUT();
            dut.SerailNumber = serialNumber;
            dut.RawCount = new List<int>();
            dut.RawCountX = new List<int>();
            dut.RawCountY = new List<int>();
            dut.Noise = new List<int>();
            dut.StdDev = new List<double>();

            dut.IDAC = new List<int>();
            dut.Global_IDAC = new List<int>();
            dut.IDACGain = new List<byte>();
            dut.IDAC_Erased = new List<int>();
            dut.Local_IDAC = new List<int>();

            dut.Signal = new List<int>();
            dut.SNR = new List<double>();

            dut.RawCount_Single = new List<int>();
            dut.Signal_Single = new List<int>();
            dut.Finger_Positions = new List<Point>();

            m_TPCONFIG = trackpadConfig;

            if (testStation == "TMT")
            {
                m_TPCONFIGtestItems = m_TPCONFIG.TMT;
            }
            if (testStation == "OQC")
            {
                m_TPCONFIGtestItems = m_TPCONFIG.OQC;
            }
            if (testStation == "TPT")
            {
                m_TPCONFIGtestItems = m_TPCONFIG.TPT;
            }

            NUM_COLS = m_TPCONFIGtestItems.FW_INFO_NUM_COLS;
            NUM_ROWS = m_TPCONFIGtestItems.FW_INFO_NUM_ROWS;
            RAW_DATA_READS = m_TPCONFIGtestItems.RAW_DATA_READS;

            bridge = UsbBridge;
            bridge.DeviceAddress = m_TPCONFIG.I2C_ADDRESS;

            VDD_DEFAULT = Convert.ToString(m_TPCONFIG.VDD_OP_PS);
            if (m_TPCONFIG.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                I2CReadDelay = DelayTime.I2C_MS / 2;
            }
            else
            {
                I2CReadDelay = DelayTime.I2C_MS;
            }

        }

        /// <summary>
        /// Get Address of USB_to_I2C_Bridge
        /// </summary>
        private void GetDeviceAddress()
        {
            int Status1 = bridge.GetDeviceAddress();
            if (Status1 != 0)
            {
                OnTestMessage(new TestMessageEventArgs("Error to get DUT address: " + bridge.LastError));
                //throw new Exception("Can not get device address.");
            }
            else
            {
                OnTestMessage(new TestMessageEventArgs("DUT address: " + bridge.DeviceAddress.ToString()));
            }
        }

        #region LED light test

        public void LEDLightTest()
        {

            if (MessageBox.Show("请检查LED指示灯是否亮起", "The Question", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                if (dut.ErrorCode == 0x0)
                    dut.ErrorCode = ErrorCode.ERROR_LED_LIGHT;
            }
        }

        #endregion

        #region FW Rev Test
        //==============================
        //  FW Rev Test
        //==============================
        public void I2C_Read_FW_Rev_Test()
        {

            byte nToRead = 8;
            datain_FW = bridge.ReadWrite(TestCommand.Read_FW_Rev, nToRead, I2CReadDelay);
            datain_FW_Ready = true;
            FWVersionItem = true;
            //BridgePowerOff();
        }
        #endregion

        #region RawCount Test
        //==============================
        //  RawCount Test
        //==============================
        public void RawCount_Test()
        {
            OnTestMessage(new TestMessageEventArgs("Reading Rawcount......."));
            if (m_TPCONFIG.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                I2C_APA_Read_RawCount_Test();
            }
            else
            {
                I2C_Read_RawX_Test();
                I2C_Read_RawY_Test();
            }
            RawCountItem = true;

        }

        /// <summary>
        /// read raw counts of X-axis of STG and MTG trackpad.
        /// </summary>
        private void I2C_Read_RawX_Test()
        {

            for (int i = 0; i < RAW_DATA_READS; i++)
            {
                datain_RawCountX.Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, (byte)(3 + 2 * NUM_COLS), I2CReadDelay));

            }

            datain_RawCountX_Ready = true;
        }


        /// <summary>
        /// read raw counts of Y-axis of STG and MTG trackpad.
        /// </summary>
        private void I2C_Read_RawY_Test()
        {

            for (int i = 0; i < RAW_DATA_READS; i++)
            {
                datain_RawCountY.Add(bridge.ReadWrite(TestCommand.Read_RawCount_Y, (byte)(3 + 2 * NUM_ROWS), I2CReadDelay));

            }

            datain_RawCountY_Ready = true;

        }

        /// <summary>
        /// read raw counts of all points of APA trackpad.
        /// </summary>
        private void I2C_APA_Read_RawCount_Test()
        {

            for (byte row = 0; row < NUM_ROWS; row++)
            {
                List<byte[]> templist = new List<byte[]>();
                for (int i = 0; i < RAW_DATA_READS; i++)
                {
                    templist.Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), I2CReadDelay));

                }
                datain_APA_RawCount.Add(templist);
            }
            datain_APA_RawCount_Ready = true;

        }

        /// <summary>
        /// check the raw count level and noise level.
        /// </summary>


        #endregion

        #region IDAC Test
        //==============================
        //  IDAC Test
        //==============================

        /// <summary>
        /// IDAC test entry
        /// </summary>
        public void IDAC_Test()
        {
            OnTestMessage(new TestMessageEventArgs("Reading IDAC......."));
            if (m_TPCONFIG.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                I2C_APA_Read_Global_IDAC_Test();
                I2C_APA_Read_IDAC_Gain_Test();
                I2C_APA_Read_Local_IDAC_Test();
            }
            else
            {
                I2C_Read_IDAC_Test();
            }
            IDACItem = true;
        }

        /// <summary>
        /// IDAC Test for ST and MTG
        /// </summary>
        private void I2C_Read_IDAC_Test()
        {

            datain_other_IDAC = bridge.ReadWrite(TestCommand.Read_Global_IDAC, (byte)(3 + NUM_ROWS + NUM_COLS), I2CReadDelay);

            datain_other_IDAC_Ready = true;

        }

        /// <summary>
        /// IDAC Test for APA
        /// </summary>
        private void I2C_APA_Read_Global_IDAC_Test()
        {

            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));

            datain_APA_GlobalIDAC = bridge.ReadWrite(TestCommand.Read_Global_IDAC, Convert.ToByte(3 + nToRead), I2CReadDelay);
            datain_APA_GlobalIDAC_Ready = true;
        }

        /// <summary>
        /// read IDAC Gain
        /// </summary>
        private void I2C_APA_Read_IDAC_Gain_Test()
        {
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            datain_APA_IDACGain = bridge.ReadWrite(TestCommand.Read_IDACGain, Convert.ToByte(3 + nToRead), I2CReadDelay);

            datain_APA_IDACGain_Ready = true;

        }

        /// <summary>
        /// read Local IDAC
        /// </summary>
        private void I2C_APA_Read_Local_IDAC_Test()
        {


            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));

            // read Local_IDAC Value
            for (byte row = 0; row < NUM_ROWS; row++)
            {
                bridge.ReadWrite(TestCommand.Read_Local_IDAC, row, (byte)(3 + NUM_COLS), I2CReadDelay);    //read 1 lines to scrap
                //System.Threading.Thread.Sleep(10);
                byte[] datain3 = bridge.ReadWrite(TestCommand.Read_Local_IDAC, row, (byte)(3 + NUM_COLS), I2CReadDelay);
                datain_APA_LocalIDAC.Add(datain3);

            }
            datain_APA_LocalIDAC_Ready = true;

        }

        public void I2C_APA_EraseIDAC()
        {

            dut.IDAC_Erased.Clear();
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));

            int ReadCount = 1;
            do
            {

                dut.IDAC_Erased.Clear();
                bridge.Write(TestCommand.Erase_IDAC);
                System.Threading.Thread.Sleep(ReadCount * DelayTime.IDAC_ERASE);
                bridge.ReadWrite(TestCommand.Read_Global_IDAC, Convert.ToByte(3 + nToRead), I2CReadDelay);    //read 1 lines to scrap              
                datain_EraseIDAC = bridge.ReadWrite(TestCommand.Read_Global_IDAC, Convert.ToByte(3 + nToRead), I2CReadDelay);
                string dataS = "IDAC Erased Value: ";
                byte temp;
                int k = 3;
                for (int element = 0; element < nToRead; element++)
                {
                    temp = datain_EraseIDAC[k++];
                    dut.IDAC_Erased.Add(temp);
                    dataS += string.Format("{0,-3}", temp) + ", ";

                }
                OnTestMessage(new TestMessageEventArgs(dataS));
                idacErased = true;
                //check if idac erased
                foreach (int idacErasedValue in dut.IDAC_Erased)
                {
                    if (idacErasedValue != 0x0C)
                    {
                        idacErased = false;

                    }
                }
                ++ReadCount;

            } while (!idacErased && ReadCount < 5);

            datain_EraseIDAC_Ready = true;

        }




        #endregion

        #region Signal Test
        //==============================
        //  Signal Test
        //==============================

        public void I2C_Signal_Test()
        {
            if (m_TPCONFIG.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                I2C_APA_Signal_Test();
            }
            else
            {
                I2C_ST_MTG_Signal_Test();
            }
        }


        /// <summary>
        /// Signal Test for APA.
        /// </summary>
        private void I2C_APA_Signal_Test()
        {
            dut.Signal_Single.Clear();
            dut.RawCount_Single.Clear();

            for (byte row = 0; row < NUM_ROWS; row++)
            {

                byte[] datain = bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), I2CReadDelay);
                int temp;
                int k = 3;
                for (int element = 0; element < NUM_COLS; element++)
                {
                    temp = 256 * datain[k++];
                    temp += datain[k++];
                    dut.RawCount_Single.Add(temp);
                }

            }

            for (int i = 0; i < dut.RawCount.Count; i++)
            {
                int temp = dut.RawCount_Single[i] - dut.RawCount[i];
                dut.Signal_Single.Add(temp);

                //caculate MAX Signal
                if (dut.Signal.Count > i)
                {
                    if (dut.Signal[i] <= (temp))
                        dut.Signal[i] = (temp);
                }
                else
                    dut.Signal.Add(0);

                //caculate MAX SNR
                if (dut.SNR.Count <= i)
                { dut.SNR.Add(0); }
                else
                {
                    if (dut.Noise[i] != 0)
                        dut.SNR[i] = dut.Signal[i] / dut.Noise[i];
                    else
                        dut.SNR[i] = dut.Signal[i];

                }
            }
        }

        /// <summary>
        /// Signal Test for ST and MTG.
        /// </summary>
        private void I2C_ST_MTG_Signal_Test()
        {
            dut.Signal_Single.Clear();
            dut.RawCount_Single.Clear();

            //Read RawCount X Single
            byte[] datain1 = bridge.ReadWrite(TestCommand.Read_RawCount_X, (byte)(3 + 2 * NUM_COLS), I2CReadDelay);
            int temp1;
            int k1 = 3;
            for (int element = 0; element < NUM_COLS; element++)
            {
                temp1 = 256 * datain1[k1++];
                temp1 += datain1[k1++];
                //if (MaxRawCountsX[element] < temp1)
                //{ MaxRawCountsX[element] = temp1; }
                dut.RawCount_Single.Add(temp1);
            }

            //Read RawCount Y Single
            byte[] datain2 = bridge.ReadWrite(TestCommand.Read_RawCount_Y, (byte)(3 + 2 * NUM_ROWS), I2CReadDelay);
            int temp2;
            int k2 = 3;
            for (int element = 0; element < NUM_ROWS; element++)
            {
                temp2 = 256 * datain2[k2++];
                temp2 += datain2[k2++];
                //if (MaxRawCountsY[element] < temp2)
                //{ MaxRawCountsY[element] = temp2; }
                dut.RawCount_Single.Add(temp2);
            }


            for (int i = 0; i < dut.RawCount.Count; i++)
            {
                int temp = dut.RawCount_Single[i] - dut.RawCount[i];
                dut.Signal_Single.Add(temp);

                //caculate MAX Signal
                if (dut.Signal.Count > i)
                {
                    if (dut.Signal[i] <= (temp))
                        dut.Signal[i] = (temp);
                }
                else
                    dut.Signal.Add(0);

                //caculate MAX SNR
                if (dut.SNR.Count <= i)
                { dut.SNR.Add(0); }
                else
                {
                    if (dut.Noise[i] != 0)
                        dut.SNR[i] = dut.Signal[i] / dut.Noise[i];
                    else
                        dut.SNR[i] = dut.Signal[i];

                }
            }

            if (dut.ErrorCode == ErrorCode.ERROR_SNR_LOW)
            {
                dut.ErrorCode = 0x00;
            }
            foreach (int SNRTemp in dut.SNR)
            {
                if (SNRTemp < 5)
                {
                    if (dut.ErrorCode == 0x00)
                    {
                        dut.ErrorCode = ErrorCode.ERROR_SNR_LOW;
                    }
                }
            }

        }

        public void I2C_ST_MTG_Signal_Check()
        {
            List<int> order = new List<int>();
            for (int i = 8; i < dut.Signal.Count(); i++)
            {
                int k = 1;
                for (int j = 8; j < dut.Signal.Count(); j++)
                {
                    if (dut.Signal[j] > dut.Signal[i])
                    {
                        k = k + 1;
                    }
                }
                order.Add(k);
            }

            //// store the singnal in order from high to low
            //for (int i = 0; i < dut.Signal.Count()-1;i++ )
            //{
            //    int k = i;
            //    for (int j = i+1; j < dut.Signal.Count(); j++)
            //    {
            //        if(dut.Signal[j]>dut.Signal[k])
            //        {
            //            k = j;
            //        }
            //    }
            //    int t = dut.Signal[k];
            //    dut.Signal[k] = dut.Signal[i];
            //    dut.Signal[i] = t;
            //}

            //Read the CONFIG XML file
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FileStream fs = new FileStream(path + @"\Config.xml", FileMode.Open);
            XmlSerializer xser = new XmlSerializer(typeof(CONFIGFILE));
            CONFIGFILE cfg = (CONFIGFILE)xser.Deserialize(fs);
            fs.Close();

            // X limit
            if (dut.Signal[0] > cfg.X1H || dut.Signal[0] < cfg.X1L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[1] > cfg.X2H || dut.Signal[1] < cfg.X2L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[2] > cfg.X3H || dut.Signal[2] < cfg.X3L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[3] > cfg.X4H || dut.Signal[3] < cfg.X4L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[4] > cfg.X5H || dut.Signal[4] < cfg.X5L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[5] > cfg.X6H || dut.Signal[5] < cfg.X6L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[6] > cfg.X7H || dut.Signal[6] < cfg.X7L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[7] > cfg.X8H || dut.Signal[7] < cfg.X8L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            //Y limit 
            if (dut.Signal[8] > cfg.Y1H || dut.Signal[8] < cfg.Y1L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[9] > cfg.Y2H || dut.Signal[9] < cfg.Y2L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[10] > cfg.Y3H || dut.Signal[10] < cfg.Y3L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[11] > cfg.Y4H || dut.Signal[11] < cfg.Y4L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[12] > cfg.Y5H || dut.Signal[12] < cfg.Y5L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[13] > cfg.Y6H || dut.Signal[13] < cfg.Y6L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[14] > cfg.Y7H || dut.Signal[14] < cfg.Y7L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
            if (dut.Signal[15] > cfg.Y8H || dut.Signal[15] < cfg.Y8L)
            {
                dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }


            ////X sensor test
            //if (Math.Abs(dut.Signal[0] - dut.Signal[7]) > cfg.Xsym || Math.Abs(dut.Signal[1] - dut.Signal[6]) > cfg.Xsym || Math.Abs(dut.Signal[2] - dut.Signal[5]) > cfg.Xsym || Math.Abs(dut.Signal[4] - dut.Signal[3]) > cfg.Xsym)
            //{
            //    dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            //}
            //if (dut.Signal[3] < dut.Signal[0] || dut.Signal[4] < dut.Signal[0])
            //{
            //    dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            //}

            //Y sensor test

            //int Ycount = 0;
            //if ((dut.Signal[15] - dut.Signal[14]) < cfg.Yorder || (dut.Signal[13] - dut.Signal[14]) < cfg.Yorder || (dut.Signal[12] - dut.Signal[14]) < cfg.Yorder || (dut.Signal[11] - dut.Signal[14]) < cfg.Yorder || (dut.Signal[10] - dut.Signal[14]) < cfg.Yorder || (dut.Signal[9] - dut.Signal[14]) < cfg.Yorder)
            //{
            //    dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            //}
            //if ((dut.Signal[15] - dut.Signal[8]) < cfg.Yorder || (dut.Signal[13] - dut.Signal[8]) < cfg.Yorder || (dut.Signal[12] - dut.Signal[8]) < cfg.Yorder || (dut.Signal[11] - dut.Signal[8]) < cfg.Yorder || (dut.Signal[10] - dut.Signal[8]) < cfg.Yorder || (dut.Signal[9] - dut.Signal[8]) < cfg.Yorder)
            //{
            //    dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            //}

            
        }

        public bool MTG_Signal_Check()
        {
            bool SignalTestR = true;           
            //Read the CONFIG XML file
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FileStream fs = new FileStream(path + @"\Config.xml", FileMode.Open);
            XmlSerializer xser = new XmlSerializer(typeof(CONFIGFILE));
            CONFIGFILE cfg = (CONFIGFILE)xser.Deserialize(fs);
            fs.Close();

            // X limit
            if (dut.Signal[0] > cfg.X1H || dut.Signal[0] < cfg.X1L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[1] > cfg.X2H || dut.Signal[1] < cfg.X2L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[2] > cfg.X3H || dut.Signal[2] < cfg.X3L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[3] > cfg.X4H || dut.Signal[3] < cfg.X4L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[4] > cfg.X5H || dut.Signal[4] < cfg.X5L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[5] > cfg.X6H || dut.Signal[5] < cfg.X6L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[6] > cfg.X7H || dut.Signal[6] < cfg.X7L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[7] > cfg.X8H || dut.Signal[7] < cfg.X8L)
            {
                SignalTestR = false;
            }
            //Y limit 
            if (dut.Signal[8] > cfg.Y1H || dut.Signal[8] < cfg.Y1L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[9] > cfg.Y2H || dut.Signal[9] < cfg.Y2L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[10] > cfg.Y3H || dut.Signal[10] < cfg.Y3L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[11] > cfg.Y4H || dut.Signal[11] < cfg.Y4L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[12] > cfg.Y5H || dut.Signal[12] < cfg.Y5L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[13] > cfg.Y6H || dut.Signal[13] < cfg.Y6L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[14] > cfg.Y7H || dut.Signal[14] < cfg.Y7L)
            {
                SignalTestR = false;
            }
            if (dut.Signal[15] > cfg.Y8H || dut.Signal[15] < cfg.Y8L)
            {
                SignalTestR = false;
            }

            return SignalTestR;
        }

        #endregion

        #region Enter Test Mode
        //==============================
        //  Enter Test Mode
        //==============================
        private void I2C_Exit_Bootloader()
        {
            bool success = false;
            byte[] commands = new byte[12] { 0x00, 0x00, 0xFF, 0xA5, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            for (int i = 0; i < 5; i++)
            {
                byte[] readData = bridge.ReadAfterWrite(commands, Convert.ToByte(5), DelayTime.BootLoader_Exit);
                if (readData.Count() > 0)
                {
                    if (readData[0] >= 0x80)
                    {
                        success = true;
                        return;
                    }
                }

            }


            //OnTestMessage(new TestMessageEventArgs(bridge.LastError));
        }

        private void I2C_Enter_TestMode()
        {
            if (dut.SerailNumber.Substring(0, 8) == "10100400" || dut.SerailNumber.Substring(0, 8) == "10100500")
            {

                byte[] commands = new byte[4] { 0x00, 0xE0, 0x0A, 0x0F };// only for 10100400 and 10100500               
                bridge.Write(commands);
                System.Threading.Thread.Sleep(DelayTime.I2C_Enter_TestMode);

            }
            else
            {
                byte[] commands = new byte[3] { 0x45, 0x0A, 0x0F };
                if (dut.SerailNumber.Substring(0, 8) == "11400200") // only for 11400200
                {
                    commands = new byte[3] { 0x30, 0x0A, 0x0F };
                }
                bridge.Write(commands);
                System.Threading.Thread.Sleep(DelayTime.I2C_Enter_TestMode);
            }


        }

        private void SMB_Enter_TestMode()
        {
            byte[] commands = new byte[4] { 0xF0, 0x02, 0x0A, 0x0F };
            bridge.Write(commands);
            System.Threading.Thread.Sleep(DelayTime.SMBUS_Enter_TestMode);
        }

        public void Enter_TestMode()
        {
            if (m_TPCONFIG.TRACKPAD_BOOTLOADER)
            {
                I2C_Exit_Bootloader();
            }

            if (m_TPCONFIG.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_I2C)
            {
                I2C_Enter_TestMode();
            }
            if (m_TPCONFIG.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_SMB)
            {
                SMB_Enter_TestMode();
            }
        }
        #endregion

        #region Test Log
        //==============================
        //  Write Test Log
        //==============================
        public void OutputData()
        {
            //TestLog testlog = new TestLog();
            //testlog.Write(dut, System.Windows.Forms.Application.StartupPath + @"\test results\");

            XmlReport xmlReport = new XmlReport();

            //Open XML Report
            if (!xmlReport.OpenReport(dut.SerailNumber, System.Windows.Forms.Application.StartupPath + @"\test results\", Manufacture.testStation))
            { Log.error(xmlReport.LastError); }

            if (!xmlReport.WriteSingleData("Serial_Number", dut.SerailNumber))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSingleData("Test_Station", Manufacture.testStation))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSingleData("Error_Code", string.Format("{0:X} ", dut.ErrorCode)))
            { Log.error(xmlReport.LastError); }

            string time = System.DateTime.Now.ToString("u", DateTimeFormatInfo.InvariantInfo);
            if (!xmlReport.WriteSingleData("Test_Time", time))
            { Log.error(xmlReport.LastError); }

            if (!xmlReport.WriteSingleData("IDD_Value", dut.IDDValue.ToString()))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSingleData("Firmware_Revision", dut.FwRev.ToString()+"."+dut.FwVer.ToString()))
            { Log.error(xmlReport.LastError); }

            if (!xmlReport.WriteSerialData("Raw_Count_Averages", dut.RawCount))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSerialData("Raw_Count_Noise", dut.Noise))
            { Log.error(xmlReport.LastError); }

            if (!xmlReport.WriteSerialData("IDAC_Value", dut.IDAC))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSerialData("Global_IDAC_Value", dut.Global_IDAC))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSerialData("IDAC_Gain_Value", dut.IDACGain))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSerialData("Local_IDAC_Value", dut.Local_IDAC))
            { Log.error(xmlReport.LastError); }

            if (!xmlReport.WriteSerialData("Signal_Data", dut.Signal))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSerialData("SNR_Data", dut.SNR))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSerialData("Fingers", dut.Finger_Positions))
            { Log.error(xmlReport.LastError); }

            //Close XML Report
            if (!xmlReport.CloseReport())
            { Log.error(xmlReport.LastError); }
        }
        #endregion

        #region Hardware Control
        //==============================
        //  Hardware Control
        //==============================
        public void BridgePowerOff()
        {
            int Status = bridge.PowerOff();
            if (Status != 0)
            {
                OnTestMessage(new TestMessageEventArgs("Error in setting power: " + bridge.LastError));
            }
            else
            {
                OnTestMessage(new TestMessageEventArgs("Successful to power off."));
            }
        }

        public void BridgePowerOn()
        {
            int Status1 = bridge.SetPower(VDD_DEFAULT);
            int Status2 = bridge.PowerOn();
            if ((Status1 != 0) || (Status2 != 0))
            {
                OnTestMessage(new TestMessageEventArgs("Error in setting power: " + bridge.LastError));
            }
            else
            {
                OnTestMessage(new TestMessageEventArgs("Successful to set power to " + VDD_DEFAULT));
                //Set I2C Speed to 400KHz//
                int Status3 = bridge.SetI2CSpeed(m_TPCONFIG.I2C_SPEED);
                if (Status3 != 0)
                {
                    OnTestMessage(new TestMessageEventArgs("Error in setting I2C speed " + bridge.LastError));
                    return;
                }
                else
                {
                    OnTestMessage(new TestMessageEventArgs("Successful to set I2C speed to " + m_TPCONFIG.I2C_SPEED.ToString()));
                    System.Threading.Thread.Sleep(DelayTime.POWER_ON_MS);
                }
            }
        }
        #endregion

        #region SFCS control
        //==============================
        //  SFCS control
        //==============================
        public bool CheckPemission()
        {

            m_SFCS = new SFCS();

            bool connected = m_SFCS.SFCS_Connect();
            if (!connected)
            {
                MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string Model = dut.SerailNumber.Substring(0, 8);
            //string Station = "TMT";

            bool permission = m_SFCS.SFCS_PermissonCheck(dut.SerailNumber, Model, Manufacture.operatorID, Manufacture.testStation);
            if (!permission)
            {
                if (dut.ErrorCode == 0)
                { dut.ErrorCode = ErrorCode.ERROR_SFCS_NOPERMISSION; }
            }
            return true;
        }

        public bool UploadResult()
        {
            bool connected = m_SFCS.SFCS_Connect();
            if (!connected)
            {
                MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string Model = dut.SerailNumber.Substring(0, 8);
            //string Station = "TMT";
            string TestLog = "";

            string errorCode = string.Format("{0:X} ", dut.ErrorCode);

            string TestResult = "Fail";
            if (dut.ErrorCode == 0x0)
            { TestResult = "Pass"; }

            bool upload = m_SFCS.SFCS_UploadTestResult(dut.SerailNumber, Model, Manufacture.operatorID, errorCode, TestLog, TestResult, Manufacture.testStation);
            if (!upload)
            {
                if (dut.ErrorCode == 0)
                { dut.ErrorCode = ErrorCode.ERROR_SFCS_UPLOADDATA; }
            }

            return true;
        }
        #endregion

        #region finger Position test

        public void ReadPosition()
        {
            readPositionStop = true;
            byte[] datain = bridge.ReadWrite(TestCommand.Read_Postion_XY, Convert.ToByte(10), I2CReadDelay);
            fingerX = datain[4] + datain[3] * 256;
            fingerY = datain[6] + datain[5] * 256;

            byte[] datain2 = bridge.ReadWrite(TestCommand.Read_Tactile_Switch, Convert.ToByte(5), I2CReadDelay);
            dut.LeftButtonStatus = datain2[3];
        }



        #endregion


        public static bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        }


        #region Test data check


        public void Check_EraseIDAC()
        {
            //byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            ////string dataS = "IDAC Erased Value: ";
            //byte temp;
            //int k = 3;
            //for (int element = 0; element < nToRead; element++)
            //{
            //    temp = datain_EraseIDAC[k++];
            //    dut.IDAC_Erased.Add(temp);
            // //   dataS += string.Format("{0,-3}", temp) + ", ";

            //}
            ////OnTestMessage(new TestMessageEventArgs(dataS));

            ////check if idac erased
            //foreach (int idacErasedValue in dut.IDAC_Erased)
            //{
            //    if (idacErasedValue != 0x0C)
            //    {
            //        idacErased = false;

            //    }
            //}

            //System.Threading.Thread.Sleep(500);
            if (!idacErased)
            {
                if (dut.ErrorCode == 0x0)
                    dut.ErrorCode = ErrorCode.ERROR_IDAC_ERASED;
            }
            datain_EraseIDAC_Ready = false;
        }


        public void Check_FWVersion()
        {
            string dataS = "FW Info: ";
            dut.FwRev = datain_FW[3];
            dut.NumCols = datain_FW[4];
            dut.NumRows = datain_FW[5];
            dut.FwVer = datain_FW[6];

            dataS += dut.FwRev + "; " + dut.NumCols + "; " + dut.NumRows + "; " + dut.FwVer;

            OnTestMessage(new TestMessageEventArgs(dataS));

            if (dut.FwRev != m_TPCONFIGtestItems.FW_INFO_FW_REV ||
                dut.NumCols != m_TPCONFIGtestItems.FW_INFO_NUM_COLS ||
                dut.NumRows != m_TPCONFIGtestItems.FW_INFO_NUM_ROWS ||
                dut.FwVer != m_TPCONFIGtestItems.FW_INFO_FW_VER)
            {
                if (dut.ErrorCode == 0)
                    dut.ErrorCode = ErrorCode.ERROR_FW_REV;
            }
            datain_FW_Ready = false;
        }

        public void Cal_APA_RawCount_Noise()
        {
            int[, ,] RawCounts = new int[NUM_ROWS, NUM_COLS, RAW_DATA_READS];

            for (byte row = 0; row < NUM_ROWS; row++)
            {
                for (int i = 0; i < RAW_DATA_READS; i++)
                {
                    List<byte[]> tempList = datain_APA_RawCount[row];
                    byte[] datain = tempList[i];
                    //datain_APA_RawCount[row, i].Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), I2CReadDelay));
                    // datain_APA_RawCount[row][i] = bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), I2CReadDelay);
                    string dataS = "Raw Count Value: ";
                    int temp;
                    int k = 3;
                    for (int element = 0; element < NUM_COLS; element++)
                    {
                        temp = 256 * datain[k++];
                        temp += datain[k++];
                        RawCounts[row, element, i] = temp;
                        dataS += string.Format("{0, -5}", temp) + ", ";
                    }
                    //OnTestMessage(new TestMessageEventArgs(dataS));
                }
            }

            int[,] SumValues = new int[NUM_ROWS, NUM_COLS]; //FW_INFO_NUM_COLS
            int[,] MinValues = new int[NUM_ROWS, NUM_COLS];
            int[,] MaxValues = new int[NUM_ROWS, NUM_COLS];

            for (int j = 0; j < NUM_ROWS; j++)
            {
                for (int i = 0; i < NUM_COLS; i++)
                {
                    MinValues[j, i] = 0xFFFF;
                    MaxValues[j, i] = 0;
                    SumValues[j, i] = 0;
                }
            }

            string AverageXString = "Average data: ";
            string NoiseXString = "Noise: ";

            //caculate average RawCount
            for (int row = 0; row < NUM_ROWS; row++)
            {
                for (int element = 0; element < NUM_COLS; element++)
                {
                    for (int i = 0; i < RAW_DATA_READS; i++)
                    {
                        int temp = RawCounts[row, element, i];
                        SumValues[row, element] += RawCounts[row, element, i];

                        if (MinValues[row, element] > temp)
                        { MinValues[row, element] = temp; }
                        if (MaxValues[row, element] < temp)
                        { MaxValues[row, element] = temp; }
                    }
                    int rawCount = SumValues[row, element] / RAW_DATA_READS;
                    dut.RawCount.Add(rawCount);
                    //baseRawCount.Add(rawCount);

                    int noise = MaxValues[row, element] - MinValues[row, element];
                    dut.Noise.Add(noise);
                    //baseNoise.Add(noise);

                    AverageXString += string.Format("{0,-4}", rawCount) + ", ";
                    NoiseXString += string.Format("{0,-3}", noise) + ",";
                }
            }
            OnTestMessage(new TestMessageEventArgs(AverageXString));
            OnTestMessage(new TestMessageEventArgs(NoiseXString));

            Rawcount_Noise_Data_Check();
        }

        public void Cal_RawCountX_Noise()
        {

            int[,] RawCountsX = new int[NUM_COLS, RAW_DATA_READS];

            //BridgePowerOn();
            for (int i = 0; i < RAW_DATA_READS; i++)
            {
                //bridge.ReadWrite(TestCommand.Read_RawCount_X, (byte)(3 + 2 * NUM_COLS)); //read 1 lines to scrap
                byte[] datain;
                datain = datain_RawCountX[i];
                string dataS = "RawCount X:";
                int temp;
                int k = 3;
                for (int element = 0; element < NUM_COLS; element++)
                {
                    temp = 256 * datain[k++];
                    temp += datain[k++];
                    RawCountsX[element, i] = temp;
                    dataS += string.Format("{0,-4}", temp) + ", ";
                }
                //OnTestMessage(new TestMessageEventArgs(dataS));
            }
            //BridgePowerOff();


            int[] SumValuesX = new int[NUM_COLS]; //FW_INFO_NUM_COLS
            int[] MinValuesX = new int[NUM_COLS];
            int[] MaxValuesX = new int[NUM_COLS];

            for (int i = 0; i < NUM_COLS; i++)
            {
                MinValuesX[i] = 0xFFFF;
                MaxValuesX[i] = 0;
            }

            string AverageXString = "Average X data: ";
            string NoiseXString = "Noise X: ";

            //caculate average X
            for (int element = 0; element < NUM_COLS; element++)
            {
                for (int i = 0; i < RAW_DATA_READS; i++)
                {
                    int temp = RawCountsX[element, i];
                    SumValuesX[element] += RawCountsX[element, i];

                    if (MinValuesX[element] > temp)
                    { MinValuesX[element] = temp; }
                    if (MaxValuesX[element] < temp)
                    { MaxValuesX[element] = temp; }
                }
                int rawCount = SumValuesX[element] / RAW_DATA_READS;
                dut.RawCountX.Add(rawCount);
                dut.RawCount.Add(rawCount);
                int noise = MaxValuesX[element] - MinValuesX[element];
                dut.Noise.Add(noise);

                AverageXString += string.Format("{0,-4}", rawCount) + ", ";
                NoiseXString += string.Format("{0,-3}", noise) + ",";
            }
            OnTestMessage(new TestMessageEventArgs(AverageXString));
            OnTestMessage(new TestMessageEventArgs(NoiseXString));
        }

        public void Cal_RawCountY_Noise()
        {

            int[,] RawCountsY = new int[NUM_ROWS, RAW_DATA_READS];

            //BridgePowerOn();

            for (int i = 0; i < RAW_DATA_READS; i++)
            {
                byte[] datain;
                datain = datain_RawCountY[i];
                string dataS = "RawCount Y:";
                int temp;
                int k = 3;
                for (int element = 0; element < NUM_ROWS; element++)
                {
                    temp = 256 * datain[k++];
                    temp += datain[k++];
                    RawCountsY[element, i] = temp;
                    dataS += string.Format("{0,-4}", temp) + ", ";
                }
                //OnTestMessage(new TestMessageEventArgs(dataS));
            }
            //BridgePowerOff();


            int[] SumValuesY = new int[NUM_ROWS]; //FW_INFO_NUM_ROWS
            int[] MinValuesY = new int[NUM_ROWS];
            int[] MaxValuesY = new int[NUM_ROWS];

            for (int i = 0; i < NUM_ROWS; i++)
            {
                MinValuesY[i] = 0xFFFF;
                MaxValuesY[i] = 0;
            }

            string AverageYString = "Average Y data: ";
            string NoiseYString = "Noise Y: ";

            //caculate average Y
            for (int element = 0; element < NUM_ROWS; element++)
            {
                for (int i = 0; i < RAW_DATA_READS; i++)
                {
                    int temp = RawCountsY[element, i];
                    SumValuesY[element] += temp;

                    if (MinValuesY[element] > temp)
                    { MinValuesY[element] = temp; }
                    if (MaxValuesY[element] < temp)
                    { MaxValuesY[element] = temp; }
                }
                int rawCount = SumValuesY[element] / RAW_DATA_READS;
                dut.RawCountY.Add(rawCount);
                dut.RawCount.Add(rawCount);
                int noise = MaxValuesY[element] - MinValuesY[element];
                dut.Noise.Add(noise);

                AverageYString += string.Format("{0,-4}", rawCount) + ", ";
                NoiseYString += string.Format("{0,-3}", noise) + ",";
            }
            OnTestMessage(new TestMessageEventArgs(AverageYString));
            OnTestMessage(new TestMessageEventArgs(NoiseYString));

            Rawcount_Noise_Data_Check();
        }

        private void Rawcount_Noise_Data_Check()
        {
            //int rawcount_element = 0;
            foreach (int rawCount in dut.RawCount)
            {
                //newRawCountRow[rawcount_element + 1] = rawCount;
                //rawcount_element++;

                if (rawCount <= m_TPCONFIGtestItems.RAW_AVG_MIN)     //Check for low condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_LOW;
                }

                if (rawCount >= m_TPCONFIGtestItems.RAW_AVG_MAX)     //Check for high condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_HIGH;
                }
            }
            //dataTable_RawCount.Rows.Add(newRawCountRow);

            //check noise
            //DataRow newNoiseRow = dataTable_Noise.NewRow();
            //newNoiseRow["Noise"] = "Y" + (row + 1).ToString();

            // int noise_element = 0;
            foreach (int noise in dut.Noise)
            {
                //newNoiseRow[noise_element + 1] = noise;
                //noise_element++;

                if (noise >= m_TPCONFIGtestItems.RAW_NOISE_MAX)
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_NOISE_AVG;
                }
            }

            datain_APA_RawCount_Ready = false;
            datain_RawCountX_Ready = false;
            datain_RawCountY_Ready = false;
        }

        public void Check_APA_GlobalIDAC()
        {
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            byte[] Global_IDAC = new byte[nToRead];

            string dataS = "Global IDAC Value: ";
            byte temp;
            int k = 3;
            for (int element = 0; element < nToRead; element++)
            {
                temp = datain_APA_GlobalIDAC[k++];
                Global_IDAC[element] = temp;
                dataS += string.Format("{0,-3}", temp) + ", ";
                dut.Global_IDAC.Add(temp);
            }
            OnTestMessage(new TestMessageEventArgs(dataS));
            foreach (int idacValue in dut.Global_IDAC)
            {
                //newIDACrow[idac_element + 1] = temp;
                //idac_element++;

                if (idacValue < m_TPCONFIGtestItems.IDAC_MIN)     //Check for low condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
                }

                if (idacValue > m_TPCONFIGtestItems.IDAC_MAX)     //Check for high condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
                }
            }
            datain_APA_GlobalIDAC_Ready = false;


        }

        public void Check_APA_IDACGain()
        {
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            byte[] IDACGains = new byte[nToRead];
            byte[] datain2 = datain_APA_IDACGain;
            string dataS2 = "IDAC Gain Value: ";
            string GainMutiplyS = "IDAC Gain Multiply: ";
            byte temp2;
            int k2 = 3;
            for (int element = 0; element < nToRead; element++)
            {
                temp2 = datain2[k2++];

                switch (temp2 & 0x60)
                {
                    case 0x60:
                        IDACGains[element] = 1;
                        break;
                    case 0x40:
                        IDACGains[element] = 2;
                        break;
                    case 0x20:
                        IDACGains[element] = 4;
                        break;
                    case 0x00:
                        IDACGains[element] = 8;
                        break;
                    default:
                        IDACGains[element] = 1;
                        break;
                }

                dut.IDACGain.Add(IDACGains[element]);
                dataS2 += string.Format("{0,-3}", temp2) + ", ";
                GainMutiplyS += string.Format("{0,-3}", IDACGains[element]) + ", ";
            }
            OnTestMessage(new TestMessageEventArgs(dataS2));
            OnTestMessage(new TestMessageEventArgs(GainMutiplyS));

            foreach (byte gain in dut.IDACGain)
            {
                if (gain > m_TPCONFIGtestItems.IDAC_GAIN_MAX)
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_IDAC_GAIN_HIGH;
                }
            }
            datain_APA_IDACGain_Ready = false;

        }

        public void Check_APA_LocalIDAC_TotalIDAC()
        {
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            int[,] Local_IDAC = new int[NUM_ROWS, NUM_COLS];

            // read Local_IDAC Value
            for (byte row = 0; row < NUM_ROWS; row++)
            {
                //bridge.ReadWrite(TestCommand.Read_Local_IDAC, row, (byte)(3 + NUM_COLS), I2CReadDelay);    //read 1 lines to scrap
                //System.Threading.Thread.Sleep(10);
                byte[] datain3 = datain_APA_LocalIDAC[row];

                string dataLocalIDACstring = "Local IDAC Value: ";
                byte temp3;
                int k3 = 3;
                for (int element = 0; element < NUM_COLS; element++)
                {
                    temp3 = datain3[k3++];
                    Local_IDAC[row, element] = temp3;
                    dut.Local_IDAC.Add(temp3);
                    dataLocalIDACstring += temp3.ToString() + ", ";

                }
                OnTestMessage(new TestMessageEventArgs(dataLocalIDACstring));

            }

            foreach (byte localIDAC in dut.Local_IDAC)
            {
                if (localIDAC < m_TPCONFIGtestItems.LOCAL_IDAC_MIN || localIDAC > m_TPCONFIGtestItems.LOCAL_IDAC_MAX)
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_LOCAL_IDAC;
                }
            }

            ////
            //List<int> IDACMap = new List<int>();
            //string[] idacmaparray = m_TPCONFIG.IDACMap.Split(' ');
            //foreach (string aa in idacmaparray)
            //{
            //    int bb = Convert.ToInt32(aa);
            //    IDACMap.Add(bb);
            //}
            ////int[] IDACMap = { 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1 };
            //for (int row = 0; row < NUM_ROWS; row++)
            //{
            //    string dataTotalIDACstring = "Total IDAC Value: ";
            //    for (int element = 0; element < NUM_COLS; element++)
            //    {

            //        Local_IDAC[row, element] = (Local_IDAC[row, element] + dut.Global_IDAC[2 * row + IDACMap[element]]) * Convert.ToInt32(dut.IDACGain[2 * row + IDACMap[element]]);
            //        dut.IDAC.Add(Local_IDAC[row, element]);
            //        dataTotalIDACstring += Local_IDAC[row, element].ToString() + ", ";

            //    }
            //    OnTestMessage(new TestMessageEventArgs(dataTotalIDACstring));

            //}

            datain_APA_LocalIDAC_Ready = false;

        }

        public void Check_Other_IDAC()
        {
            int[] SumValuesIDAC = new int[40];
            byte[] IDACValues = new byte[NUM_ROWS + NUM_COLS];

            byte[] datain = datain_other_IDAC;

            string dataS = "IDAC: ";
            byte temp;
            int k = 3;

            for (int element = 0; element < (NUM_ROWS + NUM_COLS); element++)
            {
                temp = datain[k++];
                IDACValues[element] = temp;
                dut.IDAC.Add(temp);
                dataS += string.Format("{0,-3}", temp) + ", ";
            }
            OnTestMessage(new TestMessageEventArgs(dataS));


            if (m_TPCONFIG.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_MTG)
            {
                //check IDAC X axis
                foreach (int idacValue in dut.IDAC.GetRange(0, NUM_COLS))
                {

                    if (idacValue < m_TPCONFIGtestItems.LOCAL_IDAC_MIN)     //Check for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
                    }

                    if (idacValue > m_TPCONFIGtestItems.LOCAL_IDAC_MAX)     //Check for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
                    }
                }
                foreach (int idacValue in dut.IDAC.GetRange(NUM_COLS, NUM_ROWS))
                {
                    // check IDAX Y axis
                    if (idacValue < m_TPCONFIGtestItems.IDAC_MIN)     //Check for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
                    }

                    if (idacValue > m_TPCONFIGtestItems.IDAC_MAX)     //Check for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
                    }
                }
            }
            else
            {
                foreach (int idacValue in dut.IDAC.GetRange(0, NUM_COLS))
                {
                    //newIDACrow[idac_element + 1] = temp;
                    //idac_element++;

                    if (idacValue < m_TPCONFIGtestItems.IDAC_MIN)     //Check for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
                    }

                    if (idacValue > m_TPCONFIGtestItems.IDAC_MAX)     //Check for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
                    }
                }
            }


            datain_other_IDAC_Ready = false;

        }


        #endregion

    }
}
