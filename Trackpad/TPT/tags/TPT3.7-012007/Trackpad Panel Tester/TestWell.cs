using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using CypressSemiconductor.ChinaManufacturingTest.TrackPad;

namespace CypressSemiconductor.ChinaManufacturingTest.TPT
{
    public class TestWell
    {
        //
        //----------Global Variable------------------------------------------------------------              
        //

        private const int AGILENT_POWER_CHANNEL1 = 1;
        private const int AGILENT_POWER_CHANNEL2 = 2;
        private const int AGILENT_POWER_CHANNEL3 = 3;  //Add one channel for new TPT********

        // public double VDD_PS_DEFAULT = DeviceConfig.Items.VDD_OP_MAX;  // Set DUT power supply 

        private const double VDD_PROGRAMMING = 5.00;
        private const double VDD_ZERO = 0.00;

        private const double PROGRAM_TIME_OUT = 25000;
        private const byte MPQ_PROG_VOLTAGE = 33;

        //private const byte MPQ_PROG_VOLTAGE = 50;

        private byte MPQ_POLARITY = 0;  //0 for ST and MTG, 1 for APA
        //private int MPQ_DELAY = 500;    //200 for ST and MTG, 500 for APA

        private const byte PORT_NUMBER = 8;

        //define 1 : WELLA; 2 : WELLB
        private double VDD_IO_DEFAULT;
        public double m_VDD_IO_DEFAULT
        {
            set { VDD_IO_DEFAULT = value; }
            get { return VDD_IO_DEFAULT; }
        }

        private double VDD_PS_DEFAULT;
        public double m_VDD_PS_DEFAULT
        {
            set { VDD_PS_DEFAULT = value; }
            get { return VDD_PS_DEFAULT; }
        }

        private byte currWell=1;
        public byte CurrWell
        {
            set { currWell = value; }
            get { return currWell; }
        }

        //DUT Class Array to record test log
        private DUT DUT1 = new DUT();
        private DUT DUT2 = new DUT();
        private DUT DUT3 = new DUT();
        private DUT DUT4 = new DUT();
        private DUT DUT5 = new DUT();
        private DUT DUT6 = new DUT();
        private DUT DUT7 = new DUT();
        private DUT DUT8 = new DUT();

        private DUT[] m_DUTArray;
        public DUT[] DUTArray
        {
            get { return m_DUTArray; }
        }

        private Agilent m_Agilent;        //Agilent hardware device.
        private MPQ m_MPQ;                //RPM system hardware device.
        private TrackpadConfig m_DeviceConfig;


        private SFCS m_SFCS; // Shop Floor System for Sigmatron.


        //define true : keeping reading; false : stop reading
        private volatile bool readMicroSwitchStop = false;
        private bool programTimeOut = false;

        private byte MPQ_ADDRESS1=1;
        private byte MPQ_ADDRESS2=2;

        private byte DUT14_PortEnables;     //DUT1_4_PortsEnabled MSB->LSB: DUT4,DUT3,DUT2,DUT1
        private byte DUT58_PortEnalbes;     //DUT5_8_PortsEnabled MSB->LSB: DUT8,DUT7,DUT6,DUT5

        public int PortEnalbes
        {
            get { return (DUT58_PortEnalbes << 4) + DUT14_PortEnables; }
        }


        //
        //----------------------------------------------------------------------------------------
        //

        /// <summary>
        /// Construction of TestWell Class
        /// </summary>
        /// <param name="Agilent"></param>Agilent Class, define the method of Agilent equipments.
        /// <param name="MPQ"></param>MPQ Class, define the method of MPQ programmer.
        /// <param name="wellNumber"></param>Well number, 1 for WellA, 2 for WellB
        /// <param name="serialNumber"></param>Serialnumber of 1 DUT on panel, 19 digits.
        public TestWell(Agilent Agilent, MPQ MPQ, string serialNumber, TrackpadConfig tpConfig)
        {
            m_Agilent = Agilent;
            m_MPQ = MPQ;
            m_DeviceConfig = tpConfig;

            serialNumber = serialNumber.Remove(18); //remove last digit
            m_DUTArray = new DUT[] { DUT1, DUT2, DUT3, DUT4, DUT5, DUT6, DUT7, DUT8 };

            int i = 1;
            foreach (DUT dut in m_DUTArray)
            {
                dut.SerailNumber = serialNumber + i.ToString();
                dut.RawCount = new List<int>();
                dut.Noise = new List<int>();
                dut.IDAC = new List<int>();
                dut.Global_IDAC = new List<int>();
                dut.IDACGain = new List<byte>();
                dut.Local_IDAC = new List<int>();
                dut.IDAC_Erased = new List<int>();
                i++;
            }

            DUT14_PortEnables = 0xF;    //Enable all DUTs
            DUT58_PortEnalbes = 0xF;


        }

        /// <summary>
        /// Test initialzie; Power OFF, Pneumatic OFF. 
        /// </summary>
        public void Initialize()
        {
            this.VDD_PS_DEFAULT = m_DeviceConfig.VDD_OP_PS;
            this.VDD_IO_DEFAULT = m_DeviceConfig.VDD_OP_IO;
            SetupPowerSupply(VDD_PS_DEFAULT, VDD_IO_DEFAULT);
            PowerOFF();
            PneumaticOff();

            if (m_DeviceConfig.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                MPQ_POLARITY = 1;
                //MPQ_DELAY = DelayTime.MPQ_APA_COMMAND_DELAY;
            }
            else
            {
                MPQ_POLARITY = 0;
                //MPQ_DELAY = DelayTime.MPQ_MTG_COMMAND_DELAY;
            }
        }

        #region Test Functions

        //****************************************//
        //           TPT fixture enable status    //
        //****************************************//
        public bool TPTFixtureEnableStatus()
        {
            //   bool fixtrueEnableStatus = false;
            if ((m_Agilent.ReadBit("101", "2") == "1") && (m_Agilent.ReadBit("101", "3") == "1"))
                return true;
            else
                return false;
        }

        //*********************************************************//
        //     Check WellID to make sure use the right fixture     //
        //*********************************************************//
        public bool WellIDCheck(string modelNumber)
        {
           
            string wellID;
            string customerID, customerProd;
            int customerID1, customerProd1, wellID1;
            customerID = modelNumber.Substring(1, 2);
            customerID1 = Convert.ToInt32(customerID) % 16;


            customerProd = modelNumber.Substring(4, 2);
            customerProd1 = Convert.ToInt32(customerProd) % 16;


            wellID = m_Agilent.ReadChannel("102");
            wellID1 = Convert.ToInt32(wellID);

            //wellIDH = Convert.ToChar(wellID.Substring(0, 1));
            //wellIDH = char.ToLower(wellIDH);

            //wellIDL = Convert.ToChar(wellID.Substring(1, 1));
            //wellIDL = char.ToLower(wellIDL);

            //if (wellIDH >= 'a')
            //    wellID1 = wellIDH - 'a' + 10;
            //else
            //    wellID1 = wellIDH - '0';

            //if (wellIDL >= 'a')
            //    wellID2 = wellIDL - 'a' + 10;
            //else
            //    wellID2 = wellIDL - '0';



            if ((customerID1 * 16 + customerProd1) == wellID1)
                return true;
            else
                return false;

        }

        //*****************************************//
        //           Check Trackpad in slot        //
        //*****************************************//

        public void CheckTrackpadInSlot()
        {
            int index = 0;
            foreach (DUT dut in m_DUTArray)
            {
                if (m_Agilent.ReadBit("103", Convert.ToString(index)) == "1")
                {
                    dut.ErrorCode = ErrorCode.ERROR_NO_TRACKPAD_IN_SLOT;
                }
                index++;
            }
            UpdateDUT_Enables();

        }

        //****************************************//
        //        Check Test Permission           //
        //****************************************//
        /// <summary>
        /// Check Test Permission for each DUT.
        /// </summary>
        /// <returns></returns>false if cannot connect to SFCS.
        public bool CheckTestPermission(string OperatorID)
        {
            m_SFCS = new SFCS();

            bool connected = m_SFCS.SFCS_Connect();
            if (!connected)
            {
                MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.error(m_SFCS.connect_error);
                DUT14_PortEnables = 0x0;
                DUT58_PortEnalbes = 0x0;
                return false;
            }

            foreach (DUT dut in m_DUTArray)
            {
                string Model = dut.SerailNumber.Substring(0, 8);
                string Station = "TPT";

                bool permission = m_SFCS.SFCS_PermissonCheck(dut.SerailNumber, Model, OperatorID, Station);
                if (!permission)
                {
                    if (dut.ErrorCode == 0)
                    { dut.ErrorCode = ErrorCode.ERROR_SFCS_NOPERMISSION; }
                }
            }
            return true;
        }
        //****************************************//
        //          Upload Test Record            //
        //****************************************//
        /// <summary>
        /// Upload the test record of each DUT to SFCS.
        /// </summary>
        /// <returns></returns>false if cannot connect to SFCS.
        public bool UploadTestRecord(string OperatorID)
        {
            bool connected = m_SFCS.SFCS_Connect();
            if (!connected)
            {
                MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.error(m_SFCS.connect_error);
                DUT14_PortEnables = 0x0;
                DUT58_PortEnalbes = 0x0;
                return false;
            }

            foreach (DUT dut in m_DUTArray)
            {
                string Model = dut.SerailNumber.Substring(0, 8);
                string Station = "TPT";
                string TestLog = "";

                string m_ErrorCode = string.Format("{0:X} ", dut.ErrorCode);

                string TestResult = "Fail";
                if (dut.ErrorCode == 0x0)
                { TestResult = "Pass"; }

                bool upload = m_SFCS.SFCS_UploadTestResult(dut.SerailNumber, Model, OperatorID, m_ErrorCode, TestLog, TestResult, Station);
                if (!upload)
                {
                    if (dut.ErrorCode == 0)
                    { dut.ErrorCode = ErrorCode.ERROR_SFCS_UPLOADDATA; }
                }
            }
            return true;
        }

        //****************************************//
        //              Current Test              //
        //****************************************//
        /// <summary>
        /// Trackpad current test;
        /// </summary>
        /// <param name="workingCurrentTest"></param> true for trapckpad current test with production code; false for short and open test.
        public void IDDTest(bool workingCurrentTest)
        {
            byte relayNum = 0x01;
            m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL1);      //power off
            m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL2);
            m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL3);

            for (byte i = 0; i < 4; i++)
            {

                m_Agilent.SetRelayWellA(relayNum, relayNum);        //close relay one by one from DUT1 to DUT4, 
                System.Threading.Thread.Sleep(DelayTime.AGILENT_RELAY_CLOSE);


                // DUT1 to DUT4.
                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL1);     //power on channel 1
                System.Threading.Thread.Sleep(DelayTime.POWER_ON_DELAY);

                List<double> IDD14 = new List<double>();
                for (byte j = 0; j < 5; j++)
                {
                    IDD14.Add(m_Agilent.MeasureChannelCurrent(AGILENT_POWER_CHANNEL1) * 1000);
                    System.Threading.Thread.Sleep(20);
                }
                m_DUTArray[i].IDDValue = IDD14.Max();         //measure IDD DUT1 to DUT4
                
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL1);
                System.Threading.Thread.Sleep(DelayTime.POWER_OFF_DELAY);

                // DUT5 to DUT8.
                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL2);     //power on channel 2
                System.Threading.Thread.Sleep(DelayTime.POWER_ON_DELAY);


                List<double> IDD58 = new List<double>();
                for (byte j = 0; j < 5; j++)
                {
                    IDD58.Add(m_Agilent.MeasureChannelCurrent(AGILENT_POWER_CHANNEL2) * 1000);
                    System.Threading.Thread.Sleep(20);
                }
                m_DUTArray[i + 4].IDDValue = IDD58.Max();     //measure IDD DUT5 to DUT8
                

                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL2);
                System.Threading.Thread.Sleep(DelayTime.POWER_OFF_DELAY);

                relayNum <<= 1;
            }

            PowerOFF();

            foreach (DUT dut in m_DUTArray)     //Check Test Result
            {
                double MaxLimit, MinLimit;
                if (!workingCurrentTest)
                {
                    MaxLimit = m_DeviceConfig.IDD_SHORT;
                    MinLimit = m_DeviceConfig.IDD_OPEN;
                }
                else
                {
                    MaxLimit = m_DeviceConfig.IDD_MAX;
                    MinLimit = m_DeviceConfig.IDD_MIN;
                }

                if (dut.IDDValue <= MinLimit)     //Check for low current condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_IDD_LOW;
                }

                if (dut.IDDValue >= MaxLimit)     //Check for high current condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_IDD_HIGH;
                }
            }

            PowerON();

            UpdateDUT_Enables();
        }

        //****************************************//
        //            Programming IC              //
        //****************************************//
        /// <summary>
        /// Programming PSOC IC by MPQ programmer
        /// </summary>
        /// <param name="ImageID"></param> indicate firmware# in MPQ(i.e. 1 for produciton code)
        public void ProgrammingIC(byte ImageID)
        {
            byte[] ProgrammingStatus1;
            byte[] ProgrammingStatus2;
            bool MPQ1Stop = false;
            bool MPQ2Stop = false;


            SetupPowerSupply(VDD_PROGRAMMING, VDD_PROGRAMMING);
            PowerON();
            System.Threading.Thread.Sleep(500);
            try
            {
                m_MPQ.Address = MPQ_ADDRESS1;
                m_MPQ.ProgrammerInit(MPQ_PROG_VOLTAGE, DUT14_PortEnables);
                m_MPQ.Programming(ImageID);

                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.ProgrammerInit(MPQ_PROG_VOLTAGE, DUT58_PortEnalbes);
                m_MPQ.Programming(ImageID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            programTimeOut = false;
            System.Timers.Timer t = new System.Timers.Timer(PROGRAM_TIME_OUT);
            t.AutoReset = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();
            try
            {
                do
                {
                    m_MPQ.Address = MPQ_ADDRESS1;
                    m_MPQ.CheckProgrammingStatus(out ProgrammingStatus1);
                    if (ProgrammingStatus1[1] == 0 || ProgrammingStatus1[2] == 0 || ProgrammingStatus1[3] == 0 || ProgrammingStatus1[4] == 0)
                    {
                        MPQ1Stop = true;
                    }
                    if (ProgrammingStatus1[1] > 2 && ProgrammingStatus1[2] > 2 && ProgrammingStatus1[3] > 2 && ProgrammingStatus1[4] > 2)
                    {
                        MPQ1Stop = true;
                    }

                    System.Threading.Thread.Sleep(DelayTime.CHECK_PROGRAMMING_STATUS);

                    m_MPQ.Address = MPQ_ADDRESS2;
                    m_MPQ.CheckProgrammingStatus(out ProgrammingStatus2);
                    if (ProgrammingStatus2[1] == 0 || ProgrammingStatus2[2] == 0 || ProgrammingStatus2[3] == 0 || ProgrammingStatus2[4] == 0)
                    {
                        MPQ2Stop = true;
                    }
                    if (ProgrammingStatus2[1] > 2 && ProgrammingStatus2[2] > 2 && ProgrammingStatus2[3] > 2 && ProgrammingStatus2[4] > 2)
                    {
                        MPQ2Stop = true;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        m_DUTArray[i].ProgrammingStatus = ProgrammingStatus1[i + 1];
                        m_DUTArray[i + 4].ProgrammingStatus = ProgrammingStatus2[i + 1];
                    }

                    System.Threading.Thread.Sleep(DelayTime.CHECK_PROGRAMMING_STATUS);    //delay 1s to decrease the check rate, free MPQ UART interrupt time. 

                }
                while ((!programTimeOut) && (!(MPQ1Stop && MPQ2Stop)));
            }
            catch (Exception ex)
            {
                m_MPQ = null;
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                foreach (DUT dut in m_DUTArray)
                {
                    if (dut.ErrorCode == 0)
                    {
                        dut.ErrorCode = ErrorCode.ERROR_PROGRAM_FAILURE;
                    }                    
                }
            }
            

            t.Stop();
            t.Dispose();
            PowerOFF();

            foreach (DUT dut in m_DUTArray)
            {
                if (dut.ProgrammingStatus != 0)
                {
                    if (dut.ErrorCode == 0)
                    {
                        dut.ErrorCode = ErrorCode.ERROR_PROGRAM_FAILURE;
                    }
                }
            }

            SetupPowerSupply(VDD_PS_DEFAULT, VDD_IO_DEFAULT);
            UpdateDUT_Enables();
        }

        //****************************************//
        //        Test Model Selection            //
        //****************************************//
        public void EnterIntoTestMode()
        {

            try
            {
                TestMode_GPIO_Down();
                PowerON();

                byte[] I2CCommand;
                if (m_DeviceConfig.TRACKPAD_BOOTLOADER)
                {
                    //Exit Bootloader
                    I2CCommand = new byte[12] { 0x00, 0x00, 0xFF, 0xA5, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
                    byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
                    byte[,] RcvDataCH1, RcvDataCH2;

                    m_MPQ.Address = MPQ_ADDRESS1;
                    m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                    System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, 0, out RcvDataCH1, true);

                    System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                    m_MPQ.Address = MPQ_ADDRESS2;
                    m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                    System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, 0, out RcvDataCH2, true);

                    System.Threading.Thread.Sleep(DelayTime.BootLoader_Exit);

                }

                if (m_DeviceConfig.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_I2C)
                {
                    //I2C command to enter test mode
                    //I2CCommand = new byte[4] { 0x11, 0x0A, 0x0F, 0x20 };
                    //I2CCommand = new byte[3] { 0x12, 0x0A, 0x0F };
                    
                    I2CCommand = new byte[3] { 0x47, 0x0A, 0x0F };
                    byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
                    byte[,] RcvDataCH1, RcvDataCH2;

                    m_MPQ.Address = MPQ_ADDRESS1;
                    m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                    System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, 0, out RcvDataCH1, true);

                    System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                    m_MPQ.Address = MPQ_ADDRESS2;
                    m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                    System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, 0, out RcvDataCH2, true);

                    System.Threading.Thread.Sleep(DelayTime.I2C_Enter_TestMode);
                }

                if (m_DeviceConfig.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_SMB)
                {
                    //************not support in TPT yet**************************
                    ////I2C command to enter test mode
                    ////I2CCommand = new byte[4] { 0x11, 0x0A, 0x0F, 0x20 };
                    //I2CCommand = new byte[4] { 0xF0, 0x02, 0x0A, 0x0F };
                    //byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
                    //byte[,] RcvDataCH1, RcvDataCH2;

                    //m_MPQ.Address = MPQ_ADDRESS1;
                    //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                    //System.Threading.Thread.Sleep(MPQ_DELAY);
                    //m_MPQ.I2CRun(I2CCommand, I2CAddress, 0, out RcvDataCH1, true);

                    //m_MPQ.Address = MPQ_ADDRESS2;
                    //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                    //System.Threading.Thread.Sleep(MPQ_DELAY);
                    //m_MPQ.I2CRun(I2CCommand, I2CAddress, 0, out RcvDataCH2, true);

                    //System.Threading.Thread.Sleep(DelayTime.SMBUS_Enter_TestMode);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);  
            }



        }
        

        public void ExitTestMode()
        {
            PowerOFF();
            TestMode_GPIO_Up();
        }

        //****************************************//
        //               FW Rev Test              //
        //****************************************//
        /// <summary>
        /// I2C Command 0 test;
        /// Send address, offset and commdand to trackpad(i.e. 0x04 0x00 0x00);
        /// Return firware revision, column number and row number of trackpad;
        /// (i.e. for fwRev=01, colsNum(X)=9, rowNum(Y)=8, return 0x04 0x00 0x00 0x01 0x09 0x08);
        /// </summary>
        public void I2CFWRevCheck()
        {
            byte[] I2CCommand = new byte[2] { 0x00, TestCommand.Read_FW_Rev };
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = 7;
            byte[,] RcvDataCH1, RcvDataCH2;

            try
            {
                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    m_DUTArray[port].FwRev = RcvDataCH1[port, 4];
                    m_DUTArray[port].NumCols = RcvDataCH1[port, 5];
                    m_DUTArray[port].NumRows = RcvDataCH1[port, 6];
                    m_DUTArray[port].FwVer = RcvDataCH1[port, 7];
                    m_DUTArray[port + 4].FwRev = RcvDataCH2[port, 4];
                    m_DUTArray[port + 4].NumCols = RcvDataCH2[port, 5];
                    m_DUTArray[port + 4].NumRows = RcvDataCH2[port, 6];
                    m_DUTArray[port + 4].FwVer = RcvDataCH2[port, 7];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (DUT dut in m_DUTArray)
            {
                if (dut.FwRev != m_DeviceConfig.TPT.FW_INFO_FW_REV
                    || dut.NumCols != m_DeviceConfig.TPT.FW_INFO_NUM_COLS
                    || dut.NumRows != m_DeviceConfig.TPT.FW_INFO_NUM_ROWS || dut.FwVer != m_DeviceConfig.TPT.FW_INFO_FW_VER)
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = ErrorCode.ERROR_FW_REV;
                }
            }

            UpdateDUT_Enables();
        }

        //****************************************//
        //              Buttons Test              //
        //****************************************//
        /// <summary>
        /// I2C Command 2 test;
        /// Test "Left button down", "Right button down", "Buttons release" functions;
        /// </summary>
        public void I2CSwitchTest()
        {


            if (m_DeviceConfig.TRACKPAD_TACTILESWITCH == TPCONFIG.TP_CLICK_PAD)
            {

                I2C_CP_SwitchTest();  //Click_PAD button test
            }

            if (m_DeviceConfig.TRACKPAD_TACTILESWITCH == TPCONFIG.TP_NORMAL_BUTTON)
            { 
                //Normal 
            }

            if (m_DeviceConfig.TRACKPAD_TACTILESWITCH == TPCONFIG.TP_REMOTE_CONTROL)
            { 
                //RC
            }


            UpdateDUT_Enables();
        }


        /// <summary>
        /// Click pad switch test
        /// </summary>
        private void I2C_CP_SwitchTest()
        {
            byte[] I2CCommand = new byte[2] { 0x00, TestCommand.Read_Tactile_Switch };
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = 5;
            byte[,] RcvDataCH1, RcvDataCH2;

            try
            {
                //Button Release
                m_Agilent.SetChannel("202", 0x0);
                System.Threading.Thread.Sleep(DelayTime.AGILENT_GPIO_DELAY);
                //m_Agilent.SetChannel("204", 0x0);

                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    m_DUTArray[port].LeftButtonStatus = RcvDataCH1[port, 4];
                    m_DUTArray[port + 4].LeftButtonStatus = RcvDataCH2[port, 4];
                }

                foreach (DUT dut in m_DUTArray)
                {
                    if (dut.LeftButtonStatus != 0x00)
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_TACTILE_SWITCH;
                    }
                }


                //Set Left Button Down
                m_Agilent.SetChannel("202", 0xFF);
                System.Threading.Thread.Sleep(DelayTime.AGILENT_GPIO_DELAY);

                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    m_DUTArray[port].LeftButtonStatus = RcvDataCH1[port, 4];
                    m_DUTArray[port + 4].LeftButtonStatus = RcvDataCH2[port, 4];
                }

                //Check results
                foreach (DUT dut in m_DUTArray)
                {
                    if (dut.LeftButtonStatus != 0x04)
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_TACTILE_SWITCH;
                    }
                }

                m_Agilent.SetChannel("202", 0x0);
                System.Threading.Thread.Sleep(DelayTime.AGILENT_GPIO_DELAY);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //****************************************//
        //             RawCount Test              //
        //****************************************//
        /// <summary>
        /// RawCount Test, choose which test function based on trackpad model.
        /// </summary>
        public void I2CRowCountTest()
        {
            if (m_DeviceConfig.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                I2CRowCount_XY_Test();
            }
            else
            {

                I2CRowCountXTest();
                I2CRowCountYTest();
            }

            //Check Test Result
            foreach (DUT dut in m_DUTArray)
            {

                foreach (int rawCount in dut.RawCount)
                {
                    if (rawCount < m_DeviceConfig.TPT.RAW_AVG_MIN)     //Check for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_LOW;
                    }

                    if (rawCount > m_DeviceConfig.TPT.RAW_AVG_MAX)     //Check for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_HIGH;
                    }
                }

                foreach (int noise in dut.Noise)
                {
                    if (noise > m_DeviceConfig.TPT.RAW_NOISE_MAX)
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_RAW_COUNT_NOISE_AVG;
                    }
                }
            }
            UpdateDUT_Enables();
        }

        //****************************************//
        //             RawCount X*Y Test          //
        //****************************************//
        /// <summary>
        /// I2C Command 3 Test;
        /// Collect trackpad RawCount X, caculate average value and noise value;
        /// Dataout from sensor 1 to sensor max;
        /// </summary>
        private void I2CRowCount_XY_Test()
        {
            byte[] I2CCommand;
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(4 + 2 * m_DeviceConfig.TPT.FW_INFO_NUM_COLS);
            byte[,] RcvDataCH1, RcvDataCH2;

            int[, , ,] RawCounts = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS, m_DeviceConfig.TPT.FW_INFO_NUM_COLS, m_DeviceConfig.TPT.RAW_DATA_READS];
            int[, ,] SumValues = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS, m_DeviceConfig.TPT.FW_INFO_NUM_COLS];
            int[, ,] MinValues = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS, m_DeviceConfig.TPT.FW_INFO_NUM_COLS]; //port, FW_INFO_NUM_COLS
            int[, ,] MaxValues = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS, m_DeviceConfig.TPT.FW_INFO_NUM_COLS]; //port, FW_INFO_NUM_COLS

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; j++)
                {
                    for (int x = 0; x < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; x++)
                    {
                        SumValues[i, j, x] = 0;
                        for (int y = 0; y < m_DeviceConfig.TPT.RAW_DATA_READS; y++)
                        {
                            RawCounts[i, j, x, y] = 0;
                        }

                        MinValues[i, j, x] = 0xFFFF;
                        MaxValues[i, j, x] = 0;
                    }
                }
            }

            try
            {
                //Read RawCountX*Y From MPQ1
                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);

                for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                {
                    for (byte row = 0; row < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; row++)
                    {

                        I2CCommand = new byte[3] { 0x00, TestCommand.Read_RawCount_X, row };
                        m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                        int k;
                        int temp;

                        for (int port = 0; port < 4; port++)
                        {
                            k = 4;
                            for (int element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; element++)
                            {
                                temp = 256 * RcvDataCH1[port, k++];  //read 16 bit values out of I2C buffer
                                temp += RcvDataCH1[port, k++];
                                RawCounts[port, row, element, i] = temp;
                            }
                        }
                    }
                }

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                //Read RawCountX*Y From MPQ2
                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);

                for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                {
                    for (byte row = 0; row < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; row++)
                    {
                        I2CCommand = new byte[3] { 0x00, TestCommand.Read_RawCount_X, row };
                        m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                        int k;
                        int temp;

                        for (int port = 0; port < 4; port++)
                        {
                            k = 4;
                            for (int element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; element++)
                            {
                                temp = 256 * RcvDataCH2[port, k++];  //read 16 bit values out of I2C buffer
                                temp += RcvDataCH2[port, k++];
                                RawCounts[port + 4, row, element, i] = temp;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Data Analysis
            //caculate RawCount average, noise
            for (int port = 0; port < PORT_NUMBER; port++)
            {
                for (int row = 0; row < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; row++)
                {
                    for (int element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; element++)
                    {
                        for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                        {
                            SumValues[port, row, element] += RawCounts[port, row, element, i];

                            int temp = RawCounts[port, row, element, i];

                            if (MinValues[port, row, element] > temp)
                            {
                                MinValues[port, row, element] = temp;
                            }
                            if (MaxValues[port, row, element] < temp)
                            {
                                MaxValues[port, row, element] = temp;
                            }
                        }
                        int rawData = SumValues[port, row, element] / m_DeviceConfig.TPT.RAW_DATA_READS;
                        int noiseData = MaxValues[port, row, element] - MinValues[port, row, element];
                        //AverageX[element] = SumValuesX[port, element] / DeviceConfig.Items.RAW_DATA_READS;
                        m_DUTArray[port].RawCount.Add(rawData);
                        m_DUTArray[port].Noise.Add(noiseData);
                    }
                }
            }
        }

        //****************************************//
        //             RawCount X Test            //
        //****************************************//
        /// <summary>
        /// I2C Command 3 Test;
        /// Collect trackpad RawCount X, caculate average value and noise value;
        /// Dataout from sensor 1 to sensor max;
        /// </summary>
        private void I2CRowCountXTest()
        {
            byte[] I2CCommand = new byte[2] { 0x00, TestCommand.Read_RawCount_X };
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(4 + 2 * m_DeviceConfig.TPT.FW_INFO_NUM_COLS);
            byte[,] RcvDataCH1, RcvDataCH2;

            int[, ,] RawCountsX = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_COLS, m_DeviceConfig.TPT.RAW_DATA_READS]; //port, FW_INFO_NUM_COLS, RAW_DATA_READS
            int[,] SumValuesX = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_COLS]; //FW_INFO_NUM_COLS
            int[,] MinValuesX = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_COLS]; //port, FW_INFO_NUM_COLS
            int[,] MaxValuesX = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_COLS]; //port, FW_INFO_NUM_COLS

            for (int i = 0; i < PORT_NUMBER; i++)
            {
                for (int j = 0; j < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; j++)
                {
                    SumValuesX[i, j] = 0;
                    for (int x = 0; x < m_DeviceConfig.TPT.RAW_DATA_READS; x++)
                    {
                        RawCountsX[i, j, x] = 0;
                    }

                    MinValuesX[i, j] = 0xFFFF;
                    MaxValuesX[i, j] = 0;
                }
            }

            try
            {
                //Read RawCountX From MPQ1
                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);

                for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                {
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                    byte k;
                    int temp;
                    for (byte port = 0; port < 4; port++)
                    {
                        k = 4;
                        for (byte element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; element++)
                        {
                            temp = 256 * RcvDataCH1[port, k++];  //read 16 bit values out of I2C buffer
                            temp += RcvDataCH1[port, k++];
                            RawCountsX[port, element, i] = temp;
                        }
                    }
                }

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                //Read RawCountX From MPQ2
                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);

                for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                {
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                    byte k;
                    int temp;
                    for (byte port = 0; port < 4; port++)
                    {
                        k = 4;
                        for (byte element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; element++)
                        {
                            temp = 256 * RcvDataCH2[port, k++];  //read 16 bit values out of I2C buffer
                            temp += RcvDataCH2[port, k++];
                            RawCountsX[port + 4, element, i] = temp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Data Analysis
            //caculate average X
            for (byte port = 0; port < PORT_NUMBER; port++)
            {
                for (int element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_COLS; element++)
                {
                    for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                    {
                        SumValuesX[port, element] += RawCountsX[port, element, i];

                        int temp = RawCountsX[port, element, i];

                        if (MinValuesX[port, element] > temp)
                        {
                            MinValuesX[port, element] = temp;
                        }
                        if (MaxValuesX[port, element] < temp)
                        {
                            MaxValuesX[port, element] = temp;
                        }
                    }
                    //AverageX[element] = SumValuesX[port, element] / DeviceConfig.Items.RAW_DATA_READS;
                    m_DUTArray[port].RawCount.Add(SumValuesX[port, element] / m_DeviceConfig.TPT.RAW_DATA_READS);
                    m_DUTArray[port].Noise.Add(MaxValuesX[port, element] - MinValuesX[port, element]);
                }
            }
        }

        //****************************************//
        //             RawCount Y Test            //
        //****************************************//
        /// <summary>
        /// I2C Command 4 Test;
        /// Collect trackpad rawcount Y, caculate average value and noise value;
        /// Data output from sensor 1 to sensor max;
        /// </summary>
        private void I2CRowCountYTest()
        {
            byte[] I2CCommand = new byte[2] { 0x00, TestCommand.Read_RawCount_Y };
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(4 + 2 * m_DeviceConfig.TPT.FW_INFO_NUM_ROWS);
            byte[,] RcvDataCH1, RcvDataCH2;

            int[, ,] RawCountsY = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS, m_DeviceConfig.TPT.RAW_DATA_READS]; //port, FW_INFO_NUM_COLS, RAW_DATA_READS
            int[,] SumValuesY = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS]; //FW_INFO_NUM_COLS
            int[,] MinValuesY = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS]; //port, FW_INFO_NUM_COLS
            int[,] MaxValuesY = new int[PORT_NUMBER, m_DeviceConfig.TPT.FW_INFO_NUM_ROWS]; //port, FW_INFO_NUM_COLS

            for (int i = 0; i < PORT_NUMBER; i++)
            {
                for (int j = 0; j < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; j++)
                {
                    SumValuesY[i, j] = 0;
                    for (int x = 0; x < m_DeviceConfig.TPT.RAW_DATA_READS; x++)
                    {
                        RawCountsY[i, j, x] = 0;
                    }

                    MinValuesY[i, j] = 0xFFFF;
                    MaxValuesY[i, j] = 0;
                }
            }

            try
            {
                //Read RawCountY From MPQ1
                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);

                for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                {
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                    byte k;
                    int temp;
                    for (byte port = 0; port < 4; port++)
                    {
                        k = 4;
                        for (byte element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; element++)
                        {
                            temp = 256 * RcvDataCH1[port, k++];  //read 16 bit values out of I2C buffer
                            temp += RcvDataCH1[port, k++];
                            RawCountsY[port, element, i] = temp;
                        }
                    }
                }

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                //Read RawCountY From MPQ2
                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);

                for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                {
                    m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                    byte k;
                    int temp;
                    for (byte port = 0; port < 4; port++)
                    {
                        k = 4;
                        for (byte element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; element++)
                        {
                            temp = 256 * RcvDataCH2[port, k++];  //read 16 bit values out of I2C buffer
                            temp += RcvDataCH2[port, k++];
                            RawCountsY[port + 4, element, i] = temp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Data Analysis
            //caculate average Y
            for (byte port = 0; port < 8; port++)
            {
                for (int element = 0; element < m_DeviceConfig.TPT.FW_INFO_NUM_ROWS; element++)
                {
                    for (int i = 0; i < m_DeviceConfig.TPT.RAW_DATA_READS; i++)
                    {
                        SumValuesY[port, element] += RawCountsY[port, element, i];

                        int temp = RawCountsY[port, element, i];

                        if (MinValuesY[port, element] > temp)
                        {
                            MinValuesY[port, element] = temp;
                        }
                        if (MaxValuesY[port, element] < temp)
                        {
                            MaxValuesY[port, element] = temp;
                        }
                    }
                    //AverageX[element] = SumValuesX[port, element] / DeviceConfig.Items.RAW_DATA_READS;
                    m_DUTArray[port].RawCount.Add(SumValuesY[port, element] / m_DeviceConfig.TPT.RAW_DATA_READS);
                    m_DUTArray[port].Noise.Add(MaxValuesY[port, element] - MinValuesY[port, element]);
                }
            }
        }

        //****************************************//
        //               IDAC Test                //
        //****************************************//

        public void I2CIDACTest()
        {
            if (m_DeviceConfig.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
            {
                I2CIDAC_XmultiplyY_Test();
            }
            else
            {
                I2CIDAC_XplusY_Test();
            }
        }


        private void I2CIDAC_XmultiplyY_Test()
        {
            byte[] I2CCommand_READ_IDAC = new byte[2] { 0x00, TestCommand.Read_Global_IDAC };  //Read Global IDAC.
            byte[] I2CCommand_READ_GAIN = new byte[2] { 0x00, TestCommand.Read_IDACGain }; //Read Gain.
            byte[] I2CCommand_ERASE_IDAC = new byte[2] { 0x00, TestCommand.Erase_IDAC }; //Erase IDAC.
            //byte I2CCommand_RECALIBRATE = 0x0A; //Recalibrate
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(m_DeviceConfig.TPT.FW_INFO_NUM_ROWS * Math.Ceiling(m_DeviceConfig.TPT.FW_INFO_NUM_COLS / 8.0) + 4);
            byte[,] RcvDataCH1, RcvDataCH2;

            try
            {
                ////recalibrate IDAC
                //m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables);
                //m_MPQ.I2CRun(I2CCommand3, I2CAddress, 4, out RcvDataCH1);
                //m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes);
                //m_MPQ.I2CRun(I2CCommand3, I2CAddress, 4, out RcvDataCH2);

                //read Global IDAC.
                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    for (byte element = 0; element < (nToRead - 4); element++)
                    {
                        m_DUTArray[port].Global_IDAC.Add(RcvDataCH1[port, element + 4]);
                        m_DUTArray[port + 4].Global_IDAC.Add(RcvDataCH2[port, element + 4]);
                    }
                }

                //read Gain.
                m_MPQ.Address = MPQ_ADDRESS1;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_GAIN, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_GAIN, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    for (byte element = 0; element < (nToRead - 4); element++)
                    {
                        switch (RcvDataCH1[port, element + 4] & 0x60)
                        {
                            case 0x60:
                                m_DUTArray[port].IDACGain.Add(1);
                                break;
                            case 0x40:
                                m_DUTArray[port].IDACGain.Add(2);
                                break;
                            case 0x20:
                                m_DUTArray[port].IDACGain.Add(4);
                                break;
                            case 0x00:
                                m_DUTArray[port].IDACGain.Add(8);
                                break;
                            default:
                                m_DUTArray[port].IDACGain.Add(1);
                                break;
                        }

                        switch (RcvDataCH2[port, element + 4] & 0x60)
                        {
                            case 0x60:
                                m_DUTArray[port + 4].IDACGain.Add(1);
                                break;
                            case 0x40:
                                m_DUTArray[port + 4].IDACGain.Add(2);
                                break;
                            case 0x20:
                                m_DUTArray[port + 4].IDACGain.Add(4);
                                break;
                            case 0x00:
                                m_DUTArray[port + 4].IDACGain.Add(8);
                                break;
                            default:
                                m_DUTArray[port + 4].IDACGain.Add(1);
                                break;
                        }
                    }
                }

                //combine IDAC value
                foreach (DUT dut in m_DUTArray)
                {
                    int i = 0;
                    foreach (byte gain in dut.IDACGain)
                    {
                        dut.IDAC.Add(dut.Global_IDAC[i] * gain);
                        i++;
                    }
                }
                //Erase IDAC
                m_MPQ.Address = MPQ_ADDRESS1;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_ERASE_IDAC, I2CAddress, 4, out RcvDataCH1, true);
                
                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_ERASE_IDAC, I2CAddress, 4, out RcvDataCH2, true);

                System.Threading.Thread.Sleep(DelayTime.IDAC_ERASE); // wait one sec to finish erasing the flash.

                //read Erased IDAC.
                m_MPQ.Address = MPQ_ADDRESS1;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH1, true);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH2, true);

                //read Erased IDAC.
                m_MPQ.Address = MPQ_ADDRESS1;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    for (byte element = 0; element < (nToRead - 4); element++)
                    {
                        m_DUTArray[port].IDAC_Erased.Add(RcvDataCH1[port, element + 4]);
                        m_DUTArray[port + 4].IDAC_Erased.Add(RcvDataCH2[port, element + 4]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Check Test Result
            foreach (DUT dut in m_DUTArray)
            {
                foreach (byte idacValue in dut.IDAC)
                {
                    if (idacValue < m_DeviceConfig.TPT.IDAC_MIN)     //Check global IDAC for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
                    }

                    if (idacValue > m_DeviceConfig.TPT.IDAC_MAX)     //Check global IDAC for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
                    }
                }

                foreach (byte gainValue in dut.IDACGain)
                {
                    if (gainValue > 4)
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_GAIN_HIGH;
                    }
                }

                foreach (byte idacValueErased in dut.IDAC_Erased)
                {
                    if (idacValueErased != 0x0C)                     //Check if IDAC is erased.
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_ERASED;
                    }
                }
            }

            UpdateDUT_Enables();
        }

        /// <summary>
        /// I2C Command 7 test;
        /// Collect trackpad IDAC value; 
        /// Dataoutput IDAC X + IDAC Y;
        /// </summary>
        private void I2CIDAC_XplusY_Test()
        {
            byte[] I2CCommand = new byte[2] { 0x00, TestCommand.Read_Global_IDAC };
            byte I2CAddress = m_DeviceConfig.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(m_DeviceConfig.TPT.FW_INFO_NUM_COLS + m_DeviceConfig.TPT.FW_INFO_NUM_ROWS + 4);
            byte[,] RcvDataCH1, RcvDataCH2;

            try
            {
                m_MPQ.Address = MPQ_ADDRESS1;
                //m_MPQ.I2CInit(DUT14_PortEnables, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);

                System.Threading.Thread.Sleep(DelayTime.MPQ_SWITCH_DELAY);

                m_MPQ.Address = MPQ_ADDRESS2;
                //m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_POLARITY);
                System.Threading.Thread.Sleep(DelayTime.MPQ_INIT_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    for (byte element = 0; element < (nToRead - 4); element++)
                    {
                        m_DUTArray[port].IDAC.Add(RcvDataCH1[port, element + 4]);
                        m_DUTArray[port + 4].IDAC.Add(RcvDataCH2[port, element + 4]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Check Test Result
            foreach (DUT dut in m_DUTArray)
            {
                foreach (byte idacValue in dut.IDAC)
                {
                    if (idacValue <= m_DeviceConfig.TPT.IDAC_MIN)     //Check for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_LOW;
                    }

                    if (idacValue >= m_DeviceConfig.TPT.IDAC_MAX)     //Check for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = ErrorCode.ERROR_IDAC_HIGH;
                    }
                }
            }

            UpdateDUT_Enables();
        }
        #endregion

        #region Hardware Control



        //****************************************//
        //              Micro Switch              //
        //****************************************//
        public bool ReadMicroSwitch()
        {
            bool wellAClosed = false;
            //  bool wellBClosed = false;

            string wellNumberA = "0";
            string wellNumberB = "1";

            try
            {
                string old_wellSwitchA = m_Agilent.ReadBit("101", wellNumberA);
                string old_wellSwitchB = m_Agilent.ReadBit("101", wellNumberB);

                while (!(readMicroSwitchStop | wellAClosed))
                {
                    string wellSwitchA = m_Agilent.ReadBit("101", wellNumberA);
                    string wellSwitchB = m_Agilent.ReadBit("101", wellNumberB);

                    if ((old_wellSwitchA == "1") && (wellSwitchA == "0") && (wellSwitchB == "0"))
                    {
                        wellAClosed = true;
                        currWell = 1;
                        MPQ_ADDRESS1 = 1;
                        MPQ_ADDRESS2 = 2;
                        return true;
                    }

                    //if ((old_wellSwitchB == "1") && (wellSwitchB == "0"))  **********************************
                    //{
                    //    wellBClosed = true;
                    //    currWell = 2;
                    //    MPQ_ADDRESS1 = 3;
                    //    MPQ_ADDRESS2 = 4;

                    //    return true;
                    //}

                    old_wellSwitchA = wellSwitchA;
                    old_wellSwitchB = wellSwitchB;

                    System.Threading.Thread.Sleep(DelayTime.READ_MIRCO_SWITCH);
                }
                return false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public void StopReadMicroSwitch()
        {
            readMicroSwitchStop = true;
        }


        //****************************************//
        //          Accss Test Mode               //
        //****************************************//
        private void TestMode_GPIO_Down()
        {
            try
            {
                //m_Agilent.SetChannel("202", 0xFF);        //TBD
                //m_Agilent.SetChannel("204", 0xFF);
                m_Agilent.SetChannel("203", 0xFF);
                System.Threading.Thread.Sleep(DelayTime.AGILENT_GPIO_DELAY);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestMode_GPIO_Up()
        {
            try
            {
                //m_Agilent.SetChannel("202", 0);           //TBD
                //m_Agilent.SetChannel("204", 0);
                m_Agilent.SetChannel("203", 0x00);
                System.Threading.Thread.Sleep(DelayTime.AGILENT_GPIO_DELAY);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        //****************************************//
        //          Pneumatic Control             //
        //****************************************//
        public void PneumaticOn()
        {
            try
            {
                if (currWell == 1)
                {
                    m_Agilent.SetBit("201", "0", "1");  //WellA Pneumatic ON
                    //m_Agilent.SetBit("201", "6", "1");  //WellB Pneumatic ON
                    m_Agilent.SetBit("201", "7", "1");  //GPIO selection chip(TS5V330) point to WELLA
                }
                else
                {
                    //m_Agilent.SetBit("201", "0", "1");  //WellA Pneumatic ON
                    m_Agilent.SetBit("201", "6", "1");  //WellB Pneumatic ON
                    m_Agilent.SetBit("201", "7", "0");  //GPIO selection chip(TS5V330) point to WELLB
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void PneumaticOff()
        {
            try
            {
                if (currWell == 1)
                {
                    m_Agilent.SetBit("201", "0", "0");  //WellA Pneumatic OFF
                    //m_Agilent.SetBit("201", "6", "0");  //WellB Pneumatic OFF
                }
                else
                {
                    //m_Agilent.SetBit("201", "0", "0");  //WellA Pneumatic OFF
                    m_Agilent.SetBit("201", "6", "0");  //WellB Pneumatic OFF
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //****************************************//
        //             Power Control              //
        //****************************************//
        private void SetupPowerSupply(double vdd_ps, double vdd_io)
        {
            try
            {
                m_Agilent.SetChannelVoltage(vdd_ps, AGILENT_POWER_CHANNEL1);
                m_Agilent.SetChannelVoltage(vdd_ps, AGILENT_POWER_CHANNEL2);
                m_Agilent.SetChannelVoltage(vdd_io, AGILENT_POWER_CHANNEL3);// add channel3 IO power voltage******************************
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PowerON()
        {
            try
            {

                m_Agilent.SetRelayWellA(DUT14_PortEnables, DUT58_PortEnalbes);  //close relay on WELLA if DUT enable to test
                System.Threading.Thread.Sleep(DelayTime.AGILENT_RELAY_CLOSE);


                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL1);     //channel 1 power on
                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL2);     //channel 2 power on
                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL3);     //channel 3 power on  ***************************
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                System.Threading.Thread.Sleep(DelayTime.POWER_ON_DELAY);
                
            }
        }

        private void PowerOFF()
        {
            try
            {
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL1);      //channel 1 power off
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL2);      //channel 2 power off
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL3);      //channel 3 power off  ******************


                m_Agilent.SetRelayWellA(0x0, 0x0);      //open all relay on WELLA
                System.Threading.Thread.Sleep(DelayTime.AGILENT_RELAY_CLOSE);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                System.Threading.Thread.Sleep(DelayTime.POWER_OFF_DELAY);
            }
        }

        #endregion

        private void UpdateDUT_Enables()
        {
            int i;
            byte j;

            for (i = 0, j = 1; i < 4; i++, j <<= 1)
            {

                if (m_DUTArray[i].ErrorCode != 0)
                {
                    DUT14_PortEnables &= (byte)(~j);
                }

                if (m_DUTArray[i + 4].ErrorCode != 0)
                {
                    DUT58_PortEnalbes &= (byte)(~j);
                }
            }
        }

        private void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            programTimeOut = true;
        }


        //****************************************//
        //               Test Record              //
        //****************************************//
        /// <summary>
        /// Write Test Log;
        /// Write test log of each DUT in seprated file;
        /// Single txt file with file name is the serial number;
        /// </summary>
        public void TestLog()
        {
            foreach (DUT dut in m_DUTArray)
            {
                //TestLog testlog = new TestLog();
                //testlog.Write(dut, System.Windows.Forms.Application.StartupPath + @"\test results\");

                XmlReport xmlReport = new XmlReport();
                //xmlReport.writeReport(dut, System.Windows.Forms.Application.StartupPath + @"\test results\", "TPT");

                //Open XML Report
                if (!xmlReport.OpenReport(dut.SerailNumber, System.Windows.Forms.Application.StartupPath + @"\test results\", "TPT"))
                { Log.error(xmlReport.LastError); }

                if (!xmlReport.WriteSingleData("Serial_Number", dut.SerailNumber))
                { Log.error(xmlReport.LastError); }
                if (!xmlReport.WriteSingleData("Test_Station", "TPT"))
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

                //if (!xmlReport.WriteSerialData("Signal_Data", dut.Signal))
                //{ Log.error(xmlReport.LastError); }
                //if (!xmlReport.WriteSerialData("SNR_Data", dut.SNR))
                //{ Log.error(xmlReport.LastError); }

                //Close XML Report
                if (!xmlReport.CloseReport())
                { Log.error(xmlReport.LastError); }
            }
        }
    }
}
