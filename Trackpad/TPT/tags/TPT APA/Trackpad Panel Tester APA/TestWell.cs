using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CypressSemiconductor.ChinaManufacturingTest.AFT
{
    public class TestWell
    {
        //
        //----------Global Variable------------------------------------------------------------              
        //

        private const int AGILENT_POWER_CHANNEL1 = 1;
        private const int AGILENT_POWER_CHANNEL2 = 2;

        private double VDD_DEFAULT = HardwareConfig.PowerSupply.VDD_Default;
        private double VDD_PROGRAM = HardwareConfig.PowerSupply.VDD_Prog;
        private const int VDD_DELAY = 800;
        private int MPQ_DELAY = HardwareConfig.Port.MPQDelay;
        private const double PROGRAM_TIME_OUT = 20000;

        private const byte MPQ_IO_VOLTAGE = 33;
        private const byte MPQ_POLARITY = 1;
        
        //define 1 : WELLA; 2 : WELLB
        private byte currWell;                                  
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

        private SFCS m_SFCS;              // Shop Floor System for Sigmatron.

        //define true : keeping reading; false : stop reading
        private volatile bool readMicroSwitchStop = false;
        private bool programTimeOut = false;

        private byte MPQ_ADDRESS1;
        private byte MPQ_ADDRESS2;

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
        public TestWell(Agilent Agilent, MPQ MPQ, byte wellNumber, string serialNumber)
        {
            m_Agilent = Agilent;
            m_MPQ = MPQ;
            currWell = wellNumber;
            serialNumber = serialNumber.Remove(18);
            m_DUTArray = new DUT[] { DUT1, DUT2, DUT3, DUT4, DUT5, DUT6, DUT7, DUT8 };

            int i = 1;
            foreach (DUT dut in m_DUTArray)
            {
                dut.SerailNumber = serialNumber + i.ToString();
                dut.RawCountX = new List<int>();
                dut.NoiseX = new List<int>();
                dut.RawCountY = new List<int>();
                dut.NoiseY = new List<int>();
                dut.IDAC = new List<int>();
                dut.Global_IDAC = new List<int>();
                dut.IDACGain = new List<byte>();
                dut.IDAC_Erased = new List<int>();
                i++;
            }

            DUT14_PortEnables = 0xF;    //Enable all DUTs
            DUT58_PortEnalbes = 0xF;

            if (wellNumber == 1)        //WELLA
            {
                MPQ_ADDRESS1 = 1;
                MPQ_ADDRESS2 = 2;       
            }
            else                        //WELLB
            {
                MPQ_ADDRESS1 = 3;
                MPQ_ADDRESS2 = 4;
            }
        }

        #region Test Functions
        //****************************************//
        //        Check Test Permission           //
        //****************************************//
        /// <summary>
        /// Check Test Permission for each DUT.
        /// </summary>
        /// <returns></returns>false if cannot connect to SFCS.
        public bool CheckTestPermission()
        {
            m_SFCS = new SFCS();
            
            bool connected = m_SFCS.SFCS_Connect();
            if (!connected)
            {
                MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DUT14_PortEnables = 0x0;
                DUT58_PortEnalbes = 0x0;
                return false;
            }

            foreach (DUT dut in m_DUTArray)
            {
                string Model = dut.SerailNumber.Substring(0,8);
                string Station = "TPT";

                bool permission = m_SFCS.SFCS_PermissonCheck(dut.SerailNumber, Model, Station);
                if (!permission)
                {
                    if (dut.ErrorCode == 0)
                    { dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_SFCS_NOPERMISSION; }
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
        public bool UploadTestRecord()
        {
            bool connected = m_SFCS.SFCS_Connect();
            if (!connected)
            {
                MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DUT14_PortEnables = 0x0;
                DUT58_PortEnalbes = 0x0;
                return false;
            }

            foreach (DUT dut in m_DUTArray)
            {
                string Model = dut.SerailNumber.Substring(0, 8);
                string Station = "TPT";
                string TestLog="";

                string ErrorCode = string.Format("{0:X} ", dut.ErrorCode);

                string TestResult = "Fail";
                if (dut.ErrorCode == 0x0)
                { TestResult = "Pass"; }

                bool upload = m_SFCS.SFCS_UploadTestResult(dut.SerailNumber, Model, ErrorCode, TestLog, TestResult, Station);
                if (!upload)
                {
                    if (dut.ErrorCode == 0)
                    { dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_SFCS_UPLOADDATA; }
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

            for (byte i = 0; i < 4; i++)
            {
                if (currWell == 1)
                {
                    m_Agilent.SetRelayWellA(relayNum, relayNum);        //close relay one by one from DUT1 to DUT4, 
                }                                                                                  // DUT5 to DUT8.
                else
                {
                    m_Agilent.SetRelayWellB(relayNum, relayNum);
                }

                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL1);     //power on channel 1
                m_DUTArray[i].IDDValue = m_Agilent.MeasureChannelCurrent(AGILENT_POWER_CHANNEL1) * 1000;         //measure IDD DUT1 to DUT4
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL1);

                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL2);     //power on channel 2
                m_DUTArray[i + 4].IDDValue = m_Agilent.MeasureChannelCurrent(AGILENT_POWER_CHANNEL2) * 1000;     //measure IDD DUT5 to DUT8
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL2);

                relayNum <<= 1;
            }

            PowerOFF();

            foreach (DUT dut in m_DUTArray)     //Check Test Result
            {
                double MaxLimit, MinLimit;
                if (!workingCurrentTest)
                {
                    MaxLimit = DeviceConfig.Items.IDD_SHORT;
                    MinLimit = DeviceConfig.Items.IDD_OPEN;
                }
                else
                {
                    MaxLimit = DeviceConfig.Items.IDD_MAX;
                    MinLimit = DeviceConfig.Items.IDD_MIN;
                }

                if (dut.IDDValue <= MinLimit)     //Check for low current condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_IDD_LOW;
                }

                if (dut.IDDValue >= MaxLimit)     //Check for high current condition
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_IDD_HIGH;
                }
            }

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

            SetupPowerSupply(VDD_PROGRAM);
            PowerON();

            try
            {
                m_MPQ.Address = MPQ_ADDRESS1;
                m_MPQ.ProgrammerInit(DUT14_PortEnables);
                m_MPQ.Programming(ImageID);

                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.ProgrammerInit(DUT58_PortEnalbes);
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

            do
            {
                m_MPQ.Address = MPQ_ADDRESS1;
                m_MPQ.CheckProgrammingStatus(out ProgrammingStatus1);

                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.CheckProgrammingStatus(out ProgrammingStatus2);

                for (int i = 0; i < 4; i++)
                {
                    m_DUTArray[i].ProgrammingStatus = ProgrammingStatus1[i + 1];
                    m_DUTArray[i + 4].ProgrammingStatus = ProgrammingStatus2[i + 1];
                }

                System.Threading.Thread.Sleep(1000);    //delay 1s to decrease the check rate, free MPQ UART interrupt time. 

            }
            while ((!programTimeOut) && ((ProgrammingStatus1[0] != 0) || (ProgrammingStatus2[0] != 0)));

            t.Stop();
            t.Dispose();
            PowerOFF();
            SetupPowerSupply(VDD_DEFAULT);

            foreach (DUT dut in m_DUTArray)
            {
                if (dut.ProgrammingStatus != 0)
                {
                    if (dut.ErrorCode == 0)
                    {
                        dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_PROGRAM_FAILURE;
                    }
                }
            }
            UpdateDUT_Enables();
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
            byte I2CCommand = 0x00;
            byte I2CAddress = DeviceConfig.Items.I2C_ADDRESS;
            byte nToRead = 7;
            byte[,] RcvDataCH1, RcvDataCH2;

            TestMode_GPIO_Down();
            PowerON();

            System.Threading.Thread.Sleep(100); // for first power on calibration.

            try
            {
                m_MPQ.Address = MPQ_ADDRESS1;
                m_MPQ.I2CInit(DUT14_PortEnables, MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);


                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    m_DUTArray[port].FwRev = RcvDataCH1[port, 4];
                    m_DUTArray[port].NumCols = RcvDataCH1[port, 5];
                    m_DUTArray[port].NumRows = RcvDataCH1[port, 6];
                    m_DUTArray[port + 4].FwRev = RcvDataCH2[port, 4];
                    m_DUTArray[port + 4].NumCols = RcvDataCH2[port, 5];
                    m_DUTArray[port + 4].NumRows = RcvDataCH2[port, 6];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            PowerOFF();
            //TestMode_GPIO_Up();

            foreach (DUT dut in m_DUTArray)
            {
                if (dut.FwRev != DeviceConfig.Items.FW_INFO_FW_REV
                    || dut.NumCols != DeviceConfig.Items.FW_INFO_NUM_COLS
                    || dut.NumRows != DeviceConfig.Items.FW_INFO_NUM_ROWS)
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_FW_REV;
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
            byte I2CCommand = 0x02;
            byte I2CAddress = DeviceConfig.Items.I2C_ADDRESS;
            byte nToRead = 5;
            byte[,] RcvDataCH1, RcvDataCH2;

            TestMode_GPIO_Down();
            PowerON();

            try
            {
                //Button Release
                m_Agilent.SetChannel("202", 0x0);
                m_Agilent.SetChannel("204", 0x0);
                
                
                //Set Left Button Down
                m_Agilent.SetChannel("202", 0xFF);
                m_Agilent.SetChannel("204", 0x0);


                m_MPQ.Address = MPQ_ADDRESS1;
                m_MPQ.I2CInit(DUT14_PortEnables, MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);


                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.I2CInit(DUT58_PortEnalbes,MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    m_DUTArray[port].LeftButtonStatus = RcvDataCH1[port, 4];
                    m_DUTArray[port + 4].LeftButtonStatus = RcvDataCH2[port, 4];
                }

                //Set Right Button Down
                m_Agilent.SetChannel("202", 0x0);
                m_Agilent.SetChannel("204", 0xFF);
                m_MPQ.Address = MPQ_ADDRESS1;

                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1);


                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2);

                for (byte port = 0; port < 4; port++)
                {
                    m_DUTArray[port].RightButtonStatus = RcvDataCH1[port, 4];
                    m_DUTArray[port + 4].RightButtonStatus = RcvDataCH2[port, 4];
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Button Release
            m_Agilent.SetChannel("202", 0x0);
            m_Agilent.SetChannel("204", 0x0);

            PowerOFF();
            //TestMode_GPIO_Up();

            foreach (DUT dut in m_DUTArray)
            {
                if (dut.LeftButtonStatus != 0x04 || dut.RightButtonStatus != 0x05) //dut.LeftButtonStatus!=0x04
                {
                    if (dut.ErrorCode == 0)
                        dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_TACTILE_SWITCH;
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
        public void I2CRowCountTest()
        {
            byte I2CCommand = 0x03;
            byte I2CAddress = DeviceConfig.Items.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(4 + 2 * DeviceConfig.Items.FW_INFO_NUM_COLS);
            byte[,] RcvDataCH1, RcvDataCH2;

            int[, , ,] RawCounts = new int[8, DeviceConfig.Items.FW_INFO_NUM_ROWS, DeviceConfig.Items.FW_INFO_NUM_COLS, DeviceConfig.Items.RAW_DATA_READS];
            int[, ,] SumValues = new int[8, DeviceConfig.Items.FW_INFO_NUM_ROWS, DeviceConfig.Items.FW_INFO_NUM_COLS];
            int[, ,] MinValues = new int[8, DeviceConfig.Items.FW_INFO_NUM_ROWS, DeviceConfig.Items.FW_INFO_NUM_COLS]; //port, FW_INFO_NUM_COLS
            int[, ,] MaxValues = new int[8, DeviceConfig.Items.FW_INFO_NUM_ROWS, DeviceConfig.Items.FW_INFO_NUM_COLS]; //port, FW_INFO_NUM_COLS

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < DeviceConfig.Items.FW_INFO_NUM_ROWS; j++)
                {
                    for (int x = 0; x < DeviceConfig.Items.FW_INFO_NUM_COLS; x++)
                    {
                        SumValues[i, j, x] = 0;
                        for (int y = 0; y < DeviceConfig.Items.RAW_DATA_READS; y++)
                        {
                            RawCounts[i, j, x, y] = 0;
                        }

                        MinValues[i, j, x] = 0xFFFF;
                        MaxValues[i, j, x] = 0;
                    }
                }
            }

            TestMode_GPIO_Down();
            PowerON();

            try
            {
                //Read RawCountX*Y From MPQ1
                m_MPQ.Address = MPQ_ADDRESS1;
                m_MPQ.I2CInit(DUT14_PortEnables,MPQ_IO_VOLTAGE,MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);

                for (int i = 0; i < DeviceConfig.Items.RAW_DATA_READS; i++)
                {
                    for (byte row = 0; row < DeviceConfig.Items.FW_INFO_NUM_ROWS; row++)
                    {
                        m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH1, row);

                        int k;
                        int temp;

                        for (int port = 0; port < 4; port++)
                        {
                            k = 4;
                            for (int element = 0; element < DeviceConfig.Items.FW_INFO_NUM_COLS; element++)
                            {
                                temp = 256 * RcvDataCH1[port, k++];  //read 16 bit values out of I2C buffer
                                temp += RcvDataCH1[port, k++];
                                RawCounts[port, row, element, i] = temp;
                            }
                        }
                    }
                }

                //Read RawCountX*Y From MPQ2
                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);

                for (int i = 0; i < DeviceConfig.Items.RAW_DATA_READS; i++)
                {
                    for (byte row = 0; row < DeviceConfig.Items.FW_INFO_NUM_ROWS; row++)
                    {
                        m_MPQ.I2CRun(I2CCommand, I2CAddress, nToRead, out RcvDataCH2, row);

                        int k;
                        int temp;

                        for (int port = 0; port < 4; port++)
                        {
                            k = 4;
                            for (int element = 0; element < DeviceConfig.Items.FW_INFO_NUM_COLS; element++)
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
            
            PowerOFF();
            //TestMode_GPIO_Up();

            //Data Analysis
            //caculate RawCount average, noise
            for (int port = 0; port < 8; port++)
            {
                for (int row = 0; row < DeviceConfig.Items.FW_INFO_NUM_ROWS; row++)
                {
                    for (int element = 0; element < DeviceConfig.Items.FW_INFO_NUM_COLS; element++)
                    {
                        for (int i = 0; i < DeviceConfig.Items.RAW_DATA_READS; i++)
                        {
                            SumValues[port, row, element] += RawCounts[port, row, element, i];

                            int temp = RawCounts[port, row, element, i];

                            if (MinValues[port,row, element] > temp)
                            {
                                MinValues[port,row, element] = temp;
                            }
                            if (MaxValues[port,row, element] < temp)
                            {
                                MaxValues[port,row, element] = temp;
                            }
                        }
                        int rawData = SumValues[port, row, element] / DeviceConfig.Items.RAW_DATA_READS;
                        int noiseData = MaxValues[port, row, element] - MinValues[port, row, element];
                        //AverageX[element] = SumValuesX[port, element] / DeviceConfig.Items.RAW_DATA_READS;
                        m_DUTArray[port].RawCountX.Add(rawData);
                        m_DUTArray[port].NoiseX.Add(noiseData);
                    }
                }
            }

            //Check Test Result
            foreach (DUT dut in m_DUTArray)
            {

                foreach (int rawCount in dut.RawCountX)
                {
                    if (rawCount <= DeviceConfig.Items.RAW_AVG_MIN)     //Check for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_RAW_X_LOW;
                    }

                    if (rawCount >= DeviceConfig.Items.RAW_AVG_MAX)     //Check for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_RAW_X_HIGH;
                    }
                }

                foreach (int noise in dut.NoiseX)
                {
                    if (noise >= DeviceConfig.Items.RAW_NOISE_MAX)
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_RAW_X_NOISE_AVG;
                    }
                }
            }
            UpdateDUT_Enables();
        }

        //****************************************//
        //               IDAC Test                //
        //****************************************//
        /// <summary>
        /// I2C Command 7 test;
        /// Collect trackpad IDAC value; 
        /// Dataoutput IDAC X + IDAC Y;
        /// </summary>
        public void I2CIDACTest()
        {
            byte I2CCommand_READ_IDAC = 0x07;  //Read Global IDAC.
            byte I2CCommand_READ_GAIN = 0x08; //Read Gain.
            byte I2CCommand_ERASE_IDAC = 0x09; //Erase IDAC.
            //byte I2CCommand_RECALIBRATE = 0x0A; //Recalibrate
            byte I2CAddress = DeviceConfig.Items.I2C_ADDRESS;
            byte nToRead = Convert.ToByte(DeviceConfig.Items.FW_INFO_NUM_ROWS * Math.Ceiling(DeviceConfig.Items.FW_INFO_NUM_COLS / 8.0) + 4);
            byte[,] RcvDataCH1, RcvDataCH2;

            TestMode_GPIO_Down();
            PowerON();

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
                m_MPQ.I2CInit(DUT14_PortEnables, MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH1);


                m_MPQ.Address = MPQ_ADDRESS2;
                m_MPQ.I2CInit(DUT58_PortEnalbes, MPQ_IO_VOLTAGE, MPQ_POLARITY);
                System.Threading.Thread.Sleep(MPQ_DELAY);
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
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_GAIN, I2CAddress, nToRead, out RcvDataCH1);


                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(MPQ_DELAY);
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
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_ERASE_IDAC, I2CAddress, 4, out RcvDataCH1,true);

                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_ERASE_IDAC, I2CAddress, 4, out RcvDataCH2,true);

                System.Threading.Thread.Sleep(1000); // wait one sec to finish erasing the flash.

                //read Erased IDAC.
                m_MPQ.Address = MPQ_ADDRESS1;
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH1,true);   
                
                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH2,true);

                //read Erased IDAC.
                m_MPQ.Address = MPQ_ADDRESS1;
                System.Threading.Thread.Sleep(MPQ_DELAY);
                m_MPQ.I2CRun(I2CCommand_READ_IDAC, I2CAddress, nToRead, out RcvDataCH1);

                m_MPQ.Address = MPQ_ADDRESS2;
                System.Threading.Thread.Sleep(MPQ_DELAY);
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

            PowerOFF();
            //TestMode_GPIO_Up();

            //Check Test Result
            foreach (DUT dut in m_DUTArray)     
            {
                foreach (byte idacValue in dut.IDAC)
                {
                    if (idacValue <= DeviceConfig.Items.IDAC_MIN )     //Check global IDAC for low condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_IDAC_LOW;
                    }

                    if (idacValue >= DeviceConfig.Items.IDAC_MAX)     //Check global IDAC for high condition
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_IDAC_HIGH;
                    }   
                }

                foreach (byte gainValue in dut.IDACGain)
                {
                    if (gainValue > 4)
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_IDAC_GAIN_HIGH;
                    }
                }

                foreach (byte idacValueErased in dut.IDAC_Erased)
                {
                    if (idacValueErased != 0x0C)                     //Check if IDAC is erased.
                    {
                        if (dut.ErrorCode == 0)
                            dut.ErrorCode = DeviceConfig.ErrorCode.ERROR_IDAC_ERASED;  
                    }
                }
            }

            UpdateDUT_Enables();
        }
        #endregion

        #region Hardware Control
        
        /// <summary>
        /// Hardware initialzie; Power OFF, Pneumatic OFF. 
        /// </summary>
        public void Initialize()
        {
            SetupPowerSupply(VDD_DEFAULT);
            PowerOFF();
            PneumaticOff();

            //TestMode_GPIO_Down();
        }

        //****************************************//
        //              Micro Switch              //
        //****************************************//
        public bool ReadMicroSwitch()
        {
            bool wellClosed = false;

            string wellNumber;
            if (currWell == 1) wellNumber = "0";
            else wellNumber = "3";

            string old_wellSwitch = m_Agilent.ReadBit("103", wellNumber);

            while (!(readMicroSwitchStop | wellClosed))
            {
                string wellSwitch = m_Agilent.ReadBit("103", wellNumber);

                if ((old_wellSwitch == "1") && (wellSwitch == "0"))
                {
                    wellClosed = true;
                    return true;
                }

                old_wellSwitch = wellSwitch;

                System.Threading.Thread.Sleep(100);
            }
            return false;
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
            //m_Agilent.SetChannel("202", 0xFF);        //TBD
            //m_Agilent.SetChannel("204", 0xFF);
            m_Agilent.SetChannel("203", 0xFF);
        }

        private void TestMode_GPIO_Up()
        {
            //m_Agilent.SetChannel("202", 0);           //TBD
            //m_Agilent.SetChannel("204", 0);
            m_Agilent.SetChannel("203", 0x00);
        }



        //****************************************//
        //          Pneumatic Control             //
        //****************************************//
        public void PneumaticOn()
        {
            if (currWell == 1)
            {
                m_Agilent.SetBit("201", "0", "1");  //WellA Pneumatic ON
                m_Agilent.SetBit("201", "7", "1");  //GPIO selection chip(TS5V330) point to WELLA //1
            }
            else
            {
                m_Agilent.SetBit("201", "6", "1");  //WellB Pneumatic ON
                m_Agilent.SetBit("201", "7", "0");  //GPIO selection chip(TS5V330) point to WELLB //0
            }
        }

        public void PneumaticOff()
        {
            if (currWell == 1)
            {
                m_Agilent.SetBit("201", "0", "0");  //WellA Pneumatic OFF
            }
            else
            {
                m_Agilent.SetBit("201", "6", "0");  //WellB Pneumatic OFF
            }
        }

        //****************************************//
        //             Power Control              //
        //****************************************//
        private void SetupPowerSupply(double VDD)
        {
            try
            {
                m_Agilent.SetChannelVoltage(VDD, AGILENT_POWER_CHANNEL1);
                m_Agilent.SetChannelVoltage(VDD, AGILENT_POWER_CHANNEL2);
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
                if (currWell == 1)
                { m_Agilent.SetRelayWellA(DUT14_PortEnables, DUT58_PortEnalbes); }  //close relay on WELLA if DUT enable to test
                else 
                { m_Agilent.SetRelayWellB(DUT14_PortEnables, DUT58_PortEnalbes); }  //close relay on WELLB if DUT enable to test

                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL1);     //channel 1 power on
                m_Agilent.ActivateChannelOutput(AGILENT_POWER_CHANNEL2);     //channel 2 power on
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                System.Threading.Thread.Sleep(VDD_DELAY);
            }
        }

        private void PowerOFF()
        {
            try
            {
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL1);      //channel 1 power off
                m_Agilent.De_ActivateChannelOutput(AGILENT_POWER_CHANNEL2);      //channel 2 power off

                if (currWell == 1)
                { m_Agilent.SetRelayWellA(0x0, 0x0); }      //open all relay on WELLA
                else
                { m_Agilent.SetRelayWellB(0x0, 0x0); }      //open all relay on WELLB

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                System.Threading.Thread.Sleep(VDD_DELAY);
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
                xmlReport.writeReport(dut, System.Windows.Forms.Application.StartupPath + @"\test results\", "TPT");
            }
        }
    }
}
