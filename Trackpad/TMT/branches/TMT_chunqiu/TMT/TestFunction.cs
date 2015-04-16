﻿using System;
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

    public class Fingers
    {
        public int ID;
        public List<Point> FingerPoints = new List<Point>();
        public bool FingerLive;
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

        //private int _fingerX;
        //public int fingerX
        //{
        //    get { return _fingerX; }
        //    set { _fingerX = value; }
        //}
        //private int _fingerY;
        //public int fingerY
        //{
        //    get { return _fingerY; }
        //    set { _fingerY = value; }
        //}
        //public List<Point> fingerPoint = new List<Point>();

        public List<Fingers> FingerRecords = new List<Fingers>();
        public int FingerCount = 0;

        private int NUM_COLS; //13  
        private int NUM_ROWS; //9
        private int RAW_DATA_READS; //20

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


        #region Check calibration

        public void ExitBootloader()
        {
            I2C_Exit_Bootloader();
        }

        public bool I2C_Check_RawCount_Value()
        {
            int nToRead = 3;
            byte[] datain_RawCount1;
            byte[] datain_RawCount2;
            bridge.WriteReg(0x28, 0x40, 50);
            System.Threading.Thread.Sleep(500);

            datain_RawCount1 = bridge.ReadReg(0x26, nToRead, 50);
            datain_RawCount2 = bridge.ReadReg(0x27, nToRead, 50);
            OnTestMessage(new TestMessageEventArgs("RawCountMax: " + datain_RawCount1[0] + " ;RawCountMin: " + datain_RawCount2[0]));
            if (datain_RawCount1[0] > m_TPCONFIGtestItems.RAW_AVG_MAX)
            {
                dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_HIGH;
                return false;
            }
            else if (datain_RawCount2[0] < m_TPCONFIGtestItems.RAW_AVG_MIN)
            {
                dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_LOW;
                return false;
            }
            else
            {
                //dut.ErrorCode = 0;
                return true;
            }

        }

        public void I2C_Calibrate_IDAC()
        {
            bridge.WriteReg(0x28, 0x80, 1000);
            OnTestMessage(new TestMessageEventArgs("Recalibrating``````"));

            if (I2C_Check_RawCount_Value())
            {
                OnTestMessage(new TestMessageEventArgs("Recalibration pass!"));
            }
            else
            {
                OnTestMessage(new TestMessageEventArgs("Recalibration fail!"));
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
            datain_FW = bridge.ReadWrite(TestCommand.Read_FW_Rev, nToRead, DelayTime.I2C_MS);
            datain_FW_Ready = true;
            FWVersionItem = true;
            //BridgePowerOff();
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
                dut.ErrorCode = ErrorCode.ERROR_FW_REV;
                //if (dut.ErrorCode == 0)
                //    dut.ErrorCode = ErrorCode.ERROR_FW_REV;
            }
            datain_FW_Ready = false;
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
                datain_RawCountX.Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS));

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
                datain_RawCountY.Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS));

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
                    templist.Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS));

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

            datain_other_IDAC = bridge.ReadWrite(TestCommand.Read_Global_IDAC, (byte)(3 + NUM_ROWS + NUM_COLS), DelayTime.I2C_MS);

            datain_other_IDAC_Ready = true;

        }

        /// <summary>
        /// IDAC Test for APA
        /// </summary>
        private void I2C_APA_Read_Global_IDAC_Test()
        {

            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));

            datain_APA_GlobalIDAC = bridge.ReadWrite(TestCommand.Read_Global_IDAC, Convert.ToByte(3 + nToRead), DelayTime.I2C_MS);
            datain_APA_GlobalIDAC_Ready = true;
        }

        /// <summary>
        /// read IDAC Gain
        /// </summary>
        private void I2C_APA_Read_IDAC_Gain_Test()
        {
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            datain_APA_IDACGain = bridge.ReadWrite(TestCommand.Read_IDACGain, Convert.ToByte(3 + nToRead), DelayTime.I2C_MS);

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
                bridge.ReadWrite(TestCommand.Read_Local_IDAC, row, (byte)(3 + NUM_COLS), DelayTime.I2C_MS);    //read 1 lines to scrap
                //System.Threading.Thread.Sleep(10);
                byte[] datain3 = bridge.ReadWrite(TestCommand.Read_Local_IDAC, row, (byte)(3 + NUM_COLS), DelayTime.I2C_MS);
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
                System.Threading.Thread.Sleep(ReadCount * 500);
                bridge.ReadWrite(TestCommand.Read_Global_IDAC, Convert.ToByte(3 + nToRead), DelayTime.I2C_MS);    //read 1 lines to scrap              
                datain_EraseIDAC = bridge.ReadWrite(TestCommand.Read_Global_IDAC, Convert.ToByte(3 + nToRead), DelayTime.I2C_MS);
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

                byte[] datain = bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS);
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
            byte[] datain1 = bridge.ReadWrite(TestCommand.Read_RawCount_X, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS);
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
            byte[] datain2 = bridge.ReadWrite(TestCommand.Read_RawCount_Y, (byte)(3 + 2 * NUM_ROWS), DelayTime.I2C_MS);
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
        }

        #endregion

        #region Enter Test Mode
        //==============================
        //  Enter Test Mode
        //==============================
        private void I2C_Exit_Bootloader()
        {
            byte[] commands = new byte[12] { 0x00, 0x00, 0xFF, 0xA5, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            bridge.Write(commands);
            System.Threading.Thread.Sleep(DelayTime.BootLoader_Exit);
            //OnTestMessage(new TestMessageEventArgs(bridge.LastError));
        }

        private void I2C_Enter_TestMode()
        {
            //byte[] commands = new byte[4] { 0x11, 0x0A, 0x0F, 0x20 };
            //byte[] commands = new byte[3] { 0x12, 0x0A, 0x0F};
            byte[] commands = new byte[3] { 0x45, 0x0A, 0x0F };
            bridge.Write(commands);

            System.Threading.Thread.Sleep(DelayTime.I2C_Enter_TestMode);
            //OnTestMessage(new TestMessageEventArgs(bridge.LastError));
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
            if (!xmlReport.WriteSingleData("Firmware_Revision", dut.FwRev.ToString()))
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
                int Status3 = bridge.SetI2CSpeed(m_TPCONFIG.I2C_ADDRESS);
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
            int fingerX;
            int fingerY;
            int fingerID;
            Point pointTemp = new Point();
            //set all fingers die
            for (int i = 0; i < FingerRecords.Count; i++)
            {
                FingerRecords[i].FingerLive = false;
            }

            readPositionStop = true;
            byte[] datain = bridge.ReadReg(0x00, 30, 10);
            if (datain.Count() < 20)            
            {
                //MessageBox.Show("eerrr");
                return;
            }
            FingerCount = datain[1] >> 4;
            try
            {
                for (int i = 0; i < FingerCount; i++)
                {
                    //calculate finger XY position and finger ID

                    int tempX = datain[2 + i * 5] >> 4;
                    fingerX = tempX * 256 + datain[3 + i * 5];
                    int tempY = datain[2 + i * 5] % 16;
                    fingerY = tempY * 256 + datain[4 + i * 5];
                    fingerID = datain[6 + i * 5];
                    pointTemp.X = Convert.ToInt32(fingerX/4);
                    pointTemp.Y = Convert.ToInt32(fingerY/4);
                    //Convert.ToInt32();
                    //store finger position
                    bool fingerIDExist = false;
                    for (int j = 0; j < FingerRecords.Count; j++)
                    {
                        if (FingerRecords[j].ID == fingerID)
                        {
                            FingerRecords[j].FingerPoints.Add(pointTemp);
                            fingerIDExist = true;
                            FingerRecords[j].FingerLive = true;
                        }
                    }
                    if (fingerIDExist == false)
                    {
                        Fingers fingersTemp = new Fingers();
                        fingersTemp.ID = fingerID;
                        fingersTemp.FingerPoints.Add(pointTemp);
                        fingersTemp.FingerLive = true;
                        FingerRecords.Add(fingersTemp);
                    }

                }
                //remove all die fingers
                for (int i = 0; i < FingerRecords.Count; i++)
                {
                    if (FingerRecords[i].FingerLive == false)
                    {
                        FingerRecords.RemoveAt(i);
                    }
                }

                dut.LeftButtonStatus = datain[1];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: position cal error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        #endregion


        public static bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        }


        #region Test data check


        public void Check_EraseIDAC()
        {
            byte nToRead = Convert.ToByte(NUM_ROWS * Math.Ceiling(NUM_COLS / 8.0));
            //string dataS = "IDAC Erased Value: ";
            byte temp;
            int k = 3;
            for (int element = 0; element < nToRead; element++)
            {
                temp = datain_EraseIDAC[k++];
                dut.IDAC_Erased.Add(temp);
                //   dataS += string.Format("{0,-3}", temp) + ", ";

            }
            //OnTestMessage(new TestMessageEventArgs(dataS));

            //check if idac erased
            foreach (int idacErasedValue in dut.IDAC_Erased)
            {
                if (idacErasedValue != 0x0C)
                {
                    idacErased = false;

                }
            }

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
                    //datain_APA_RawCount[row, i].Add(bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS));
                    // datain_APA_RawCount[row][i] = bridge.ReadWrite(TestCommand.Read_RawCount_X, row, (byte)(3 + 2 * NUM_COLS), DelayTime.I2C_MS);
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
                //bridge.ReadWrite(TestCommand.Read_Local_IDAC, row, (byte)(3 + NUM_COLS), DelayTime.I2C_MS);    //read 1 lines to scrap
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


            //foreach (int idacValue in dut.IDAC)
            //{
            //    //newIDACrow[idac_element + 1] = temp;
            //    //idac_element++;

            //    if (idacValue < m_TPCONFIGtestItems.IDAC_MIN)     //Check for low condition
            //    {
            //        if (dut.ErrorCode == 0)
            //            dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
            //    }

            //    if (idacValue > m_TPCONFIGtestItems.IDAC_MAX)     //Check for high condition
            //    {
            //        if (dut.ErrorCode == 0)
            //            dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
            //    }
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
            //OnTestMessage(new TestMessageEventArgs(dataS));

            foreach (int idacValue in dut.IDAC)
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

            datain_other_IDAC_Ready = false;

        }

        //private void IDAC_Data_Check()
        //{
        //    foreach (int idacValue in dut.IDAC)
        //    {
        //        //newIDACrow[idac_element + 1] = temp;
        //        //idac_element++;

        //        if (idacValue < m_TPCONFIGtestItems.IDAC_MIN)     //Check for low condition
        //        {
        //            if (dut.ErrorCode == 0)
        //                dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
        //        }

        //        if (idacValue > m_TPCONFIGtestItems.IDAC_MAX)     //Check for high condition
        //        {
        //            if (dut.ErrorCode == 0)
        //                dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
        //        }
        //    }

        //    datain_APA_GlobalIDAC_Ready = false;
        //    datain_APA_IDACGain_Ready = false;
        //    datain_APA_LocalIDAC_Ready = false;
        //    datain_other_IDAC_Ready = false;
        //}
        #endregion

    }
}
