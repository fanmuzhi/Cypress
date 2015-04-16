using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Ports;
using Microsoft.Win32;

namespace CypressSemiconductor.ChinaManufacturingTest.AFT
{

    public partial class MainForm : Form    
    {
        //
        //------------------Global Variables-------------------------------------------------------           
        //
        Agilent agilentDevice;
        MPQ mpqDevice;

        TestWell testWellA;
        TestWell testWellB;

        private TextBox[] dutTextBoxWellA;
        private TextBox[] dutTextBoxWellB;

        private bool testWellARunning = false;
        private bool testWellBRunning = false;

        private bool testHardwareReady = false;
        private bool SFCS_Online = true;

        delegate void SetDUTTextBoxCallback(TextBox tb, int errorCode, bool testFinished);
        delegate void SetListBoxCallback(string text);
        delegate void SetTextCallback(TextBox tb, string text);
        delegate void SetStatusCallback(TextBox tb, string text, Color color);

        private const int SERIAL_NUMBER_LENGTH = 19;
        private const int PARTNUMBER_LENGTH = 8;

        private double totalTestedNumber;
        private double totalFailedNumber;
        private double yieldRate;

        //-----------------------------------------------------------------------------------------

        private void RegisterEvent()
        {
            agilentDevice.AgilentMessageEvent += new Agilent.AgilentMessageEventHandler(agilentDevice_AgilentMessageEvent);
            mpqDevice.MPQMessageEvent += new MPQ.MPQMessageEventHandler(mpqDevice_MPQMessageEvent);
        }
        private void UnRegisterEvent()
        {
            agilentDevice.AgilentMessageEvent -= new Agilent.AgilentMessageEventHandler(agilentDevice_AgilentMessageEvent);
            mpqDevice.MPQMessageEvent -= new MPQ.MPQMessageEventHandler(mpqDevice_MPQMessageEvent);    
        }

        /// <summary>
        /// The main function: instant Agilent, MPQ class, and textboxes.
        /// </summary>
        public MainForm()                   
        {
            InitializeComponent();
            
            agilentDevice = new Agilent();
            mpqDevice = new MPQ();

            dutTextBoxWellA = new TextBox[]{
                this.textBoxDUT1,
                this.textBoxDUT2,
                this.textBoxDUT3,
                this.textBoxDUT4,
                this.textBoxDUT5,
                this.textBoxDUT6,
                this.textBoxDUT7,
                this.textBoxDUT8};

            dutTextBoxWellB = new TextBox[]{
                this.textBoxDUT9,
                this.textBoxDUT10,
                this.textBoxDUT11,
                this.textBoxDUT12,
                this.textBoxDUT13,
                this.textBoxDUT14,
                this.textBoxDUT15,
                this.textBoxDUT16};
        }


        //****************************************//
        //             Menu Strip                 //
        //****************************************//
        private void hardwareConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfig formConfig = new FormConfig();
            try
            {
                formConfig.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                formConfig.Dispose();
                HardwareInitialize();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBoxMainForm aboutBox = new AboutBoxMainForm();
            try
            {
                aboutBox.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                aboutBox.Dispose();
            }
        }

        private void toolStripMenuItemPMode_Click(object sender, EventArgs e)
        {
            toolStripMenuItemEMode.Checked = false;
            toolStripMenuItemPMode.Checked = true;
        }

        private void toolStripMenuItemEMode_Click(object sender, EventArgs e)
        {
            toolStripMenuItemEMode.Checked = true;
            toolStripMenuItemPMode.Checked = false;
        }


        private void ProdTestMode_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItemPMode.Checked)     //Produciton Mode
            {
                //tabControlMain.TabPages.Remove(tabPageDeviceSetting);

                buttonSettingLoad.Enabled = false;  //disable trackpad test parameter setting.
                buttonSettingSave.Enabled = false;

                radioButtonOffline.Enabled = false; //disable SFCS on/off option.
                radioButtonOnline.Enabled = false;
            }
            else                                    //Engineering Mode
            {
                //tabControlMain.TabPages.Add(tabPageDeviceSetting);

                buttonSettingLoad.Enabled = true;   //disable trackpad test parameter setting.
                buttonSettingSave.Enabled = true;

                radioButtonOffline.Enabled = true;  //disable SFCS on/off option.
                radioButtonOnline.Enabled = true;    
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //****************************************//
        //                MainForm                //
        //****************************************//
        private void MainForm_Load(object sender, EventArgs e)
        {
            //initialize GUI disaplay
            toolStripMenuItemEMode.CheckState = CheckState.Unchecked;
            toolStripMenuItemPMode.CheckState = CheckState.Checked;
            
            radioButtonOnline.Checked = true;
            radioButtonOffline.Checked = false;
            //tabControlMain.TabPages.Remove(tabPageDeviceSetting);

            DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";

            string[] sections = DeviceConfig.ReadModuleList();
            foreach (string section in sections)
            {
                comboBoxModule.Items.Add(section);
            }

            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey(@"software\Cypress\TrackpadTest");
                totalTestedNumber = Convert.ToDouble(readKey.GetValue("Total Tested"));
                totalFailedNumber = Convert.ToDouble(readKey.GetValue("Total Failed"));
                yieldRate = Convert.ToDouble(readKey.GetValue("Yield Rate"));
                SetManufacturingSummary();
            }
            catch
            {
                RegistryKey saveKey = Registry.LocalMachine.CreateSubKey(@"software\Cypress\TrackpadTest");

                saveKey.SetValue("Total Tested", "0");
                saveKey.SetValue("Total Failed", "0");
                saveKey.SetValue("Yield Rate", "0");
                totalTestedNumber = 0;
                totalFailedNumber = 0;
                yieldRate = 0;
                SetManufacturingSummary();
            }

            textBoxSNA.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegistryKey saveKey = Registry.LocalMachine.CreateSubKey(@"software\Cypress\TrackpadTest");

            saveKey.SetValue("Total Tested", totalTestedNumber.ToString());
            saveKey.SetValue("Total Failed", totalFailedNumber.ToString());
            saveKey.SetValue("Yield Rate", yieldRate.ToString());
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            RegisterEvent();        //register event for MPQ and Agilent device.
            HardwareInitialize();   //initialize test hardwares.
        }

        /// <summary>
        /// Show the summary info on GUI
        /// </summary>
        private void SetManufacturingSummary()
        {
            Set_TextBox(textBoxTotalTested, totalTestedNumber.ToString());
            Set_TextBox(textBoxTotalFailed, totalFailedNumber.ToString());
            Set_TextBox(textBoxYieldRate, string.Format("{0:P}", yieldRate));
        }

        //****************************************//
        //          Hardware Initialize           //
        //****************************************//
        
        /// <summary>
        /// Hardware initialize, init Agilent device and open comm for MPQ
        /// </summary>
        private void HardwareInitialize()
        {
            try
            {
                //Read Hardware Settings
                HardwareConfig.Read();

                //Agilent
                agilentDevice.InitializeU2651A(HardwareConfig.Option.U2651AtxtAddr);
                agilentDevice.InitializeU2722A(HardwareConfig.Option.U2722AtxtAddr);
                agilentDevice.InitializeU2751A_WELLA(HardwareConfig.Option.U2751AWellAtxtAddr);
                agilentDevice.InitializeU2751A_WELLB(HardwareConfig.Option.U2751AWellBtxtAddr);

                //MPQ
                mpqDevice.PortInit(HardwareConfig.Port.MPQPort);

                toolStripStatusLabelHardware.BackColor = Color.Green;
                testHardwareReady = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabelHardware.BackColor = Color.Red;
                testHardwareReady = false;
            }
        }

        private void mpqDevice_MPQMessageEvent(object sender, MPQMessageEventArgs ea)
        {
            SetListBoxStatus(ea.Message);
        }

        private void agilentDevice_AgilentMessageEvent(object sender, AgilentMessageEventArgs ea)
        {
            SetListBoxStatus(ea.Message);
        }

        //****************************************//
        //          Main Test Process             //
        //****************************************//

        /// <summary>
        /// The test sequences for trackpad 
        /// </summary>
        /// <param name="ob"></param>The testwell class
        private void testProcess(object ob)
        {
            TestWell testWell = (TestWell)ob;

            testWell.Initialize();

            if (testWell.ReadMicroSwitch())
            {
                Show_Test_Result(testWell, false);  //clear test result
                
                Thread.Sleep(300);
                SetListBoxStatus("Well Closed");

                testWell.PneumaticOn();
                Thread.Sleep(300);

                if (SFCS_Online && testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("***********Check Permission*************");
                    if (!testWell.CheckTestPermission())
                    {
                        testWell.PneumaticOff();
                        return;
                    }
                    Show_Test_Result(testWell, false);
                }
                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("*************IDD Testing***************");
                    testWell.IDDTest(false);
                    Show_Test_Result(testWell, false);
                }
                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("*********Programming Prod Code*********");
                    testWell.ProgrammingIC(1);
                    Show_Test_Result(testWell, false);
                }
                //if (testWell.PortEnalbes != 0)
                //{
                //    SetListBoxStatus("*************Switch Testing**************");
                //    testWell.I2CSwitchTest();
                //    Show_Test_Result(testWell, false);
                //}

                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("**********Fimware Rev Check************");
                    testWell.I2CFWRevCheck();
                    Show_Test_Result(testWell, false);
                }
                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("**********Row Count X*Y Testing**********");
                    testWell.I2CRowCountTest();
                    Show_Test_Result(testWell, false);
                }
                //if (testWell.PortEnalbes != 0)
                //{
                //    SetListBoxStatus("*********Programming Prod Code*********");
                //    testWell.ProgrammingIC(1);
                //    Show_Test_Result(testWell, false);
                //}
                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("*************IDD Testing***************");
                    testWell.IDDTest(true);
                    Show_Test_Result(testWell, false);
                }
                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("*************IDAC Testing**************");
                    testWell.I2CIDACTest();
                    
                    if (SFCS_Online)
                    { Show_Test_Result(testWell, false); }
                    else
                    { Show_Test_Result(testWell, true); }
                }

                if (SFCS_Online)
                {
                    SetListBoxStatus("*********Upload Test Results***********");
                    if (!testWell.UploadTestRecord())
                    {
                        testWell.PneumaticOff();
                        return;
                    }
                    Show_Test_Result(testWell, true);
                }

                testWell.TestLog();

                //cacalute pass/fail count
                int currentTested = 0;
                int currentFailed = 0;
                foreach (DUT dut in testWell.DUTArray)
                {
                    if (dut.ErrorCode != 0)
                    {
                        currentFailed++;
                    }
                    currentTested++;
                }
                totalTestedNumber += currentTested;
                totalFailedNumber += currentFailed;
                yieldRate = (totalTestedNumber - totalFailedNumber) / totalTestedNumber;
                SetManufacturingSummary();

                //release fixture lock.
                testWell.PneumaticOff();

                if (testWell.CurrWell == 1)
                {
                    //set status box
                    if (currentFailed != 0)
                    { Set_StatusBox(textBoxWellAStatus, "Total Failed:" + currentFailed.ToString(), Color.Red); }
                    else
                    { Set_StatusBox(textBoxWellAStatus, "PASS", Color.Green); }
                    
                    testWellARunning = false;
                    Set_TextBox(textBoxSNA, "");    //clear serial number of wellA, get focused.
                }    
                else
                {
                    //set status box
                    if (currentFailed != 0)
                    { Set_StatusBox(textBoxWellBStatus, "Total Failed:" + currentFailed.ToString(), Color.Red); }
                    else
                    { Set_StatusBox(textBoxWellBStatus, "PASS", Color.Green); }
                    
                    testWellBRunning = false;
                    Set_TextBox(textBoxSNB, "");    //clear serial number of wellB, get focused.
                }    
            }    
        }

        //****************************************//
        //            User Interface              //
        //****************************************//
        private void Show_Test_Result(TestWell tW, bool testFinished)
        {
            if (tW.CurrWell == 1)
            {
                //testWellARunning = !testFinished;
                int i = 0;
                foreach (TextBox tb in dutTextBoxWellA)
                {
                    int errorCode = tW.DUTArray[i].ErrorCode;
                    SetDUTTextBox(tb, errorCode, testFinished);
                    i++;
                }
            }
            else
            {
                //testWellBRunning = !testFinished;
                int i = 0;
                foreach (TextBox tb in dutTextBoxWellB)
                {
                    int errorCode = tW.DUTArray[i].ErrorCode;
                    SetDUTTextBox(tb, errorCode, testFinished);
                    i++;
                }
            }
        }

        private void Set_StatusBox(TextBox tb, string text, Color color)
        {
            if (tb.InvokeRequired)
            {
                SetStatusCallback s = new SetStatusCallback(Set_StatusBox);
                this.Invoke(s, new object[] { tb, text,color });
            }
            else
            {
                tb.Text = text;
                tb.BackColor = color;
            }
        }

        private void Set_TextBox(TextBox tb, string text)
        {
            if (tb.InvokeRequired)
            {
                SetTextCallback s = new SetTextCallback(Set_TextBox);
                this.Invoke(s, new object[] { tb, text });
            }
            else
            {
                tb.Text = text;
                tb.Focus();     //prevent loosing focus when scanning barcode.
            }
        }

        private void SetListBoxStatus(string text)
        {
            if (this.listBoxStatus.InvokeRequired)
            {
                SetListBoxCallback d1 = new SetListBoxCallback(SetListBoxStatus);
                this.Invoke(d1, new object[] { text });
            }
            else
            {
                listBoxStatus.Items.Add(text);
                int countNum = listBoxStatus.Items.Count;
                listBoxStatus.SetSelected(countNum - 1, true);
            }
        }

        /// <summary>
        /// Set TextBox's color and text
        /// to show the test result on mainform
        /// </summary>
        /// <param name="tb"></param> The textbox to set
        /// <param name="errorCode"></param> test result, 0 for pass, fail if not 0
        /// <param name="testFinished"></param> indicate test status, in testing or test finished.
        private void SetDUTTextBox(TextBox tb, int errorCode, bool testFinished)
        {
            if (tb.InvokeRequired)
            {
                SetDUTTextBoxCallback d = new SetDUTTextBoxCallback(SetDUTTextBox);
                this.Invoke(d, new object[] { tb, errorCode, testFinished });
            }
            else
            {
                tb.Text = string.Format("{0:X} ", errorCode);
                if (errorCode != 0)
                    tb.BackColor = Color.Red;
                else if (testFinished)
                    tb.BackColor = Color.Green;
                else
                    tb.BackColor = Color.White;
            }
        }


        //****************************************//
        //            Device Setting              //
        //****************************************//
        private void buttonSettingLoad_Click(object sender, EventArgs e)
        {
            try
            {
                DeviceConfig.partType = comboBoxModule.SelectedItem.ToString();
            }
            catch
            {
                return; 
            }
            SettingLoad();
        }

        private void SettingLoad()
        {
            try
            {
                //DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";
                DeviceConfig.Read();

                textBoxVddOpMax.Text = DeviceConfig.Items.VDD_OP_MAX.ToString();
                textBoxVddOpMin.Text = DeviceConfig.Items.VDD_OP_MIN.ToString();
                textBoxIddMax.Text = DeviceConfig.Items.IDD_MAX.ToString();
                textBoxIddMin.Text = DeviceConfig.Items.IDD_MIN.ToString();
                textBoxIddOpen.Text = DeviceConfig.Items.IDD_OPEN.ToString();
                textBoxIddShort.Text = DeviceConfig.Items.IDD_SHORT.ToString();

                textBoxI2CAddress.Text = DeviceConfig.Items.I2C_ADDRESS.ToString();

                textBoxRawCountMax.Text = DeviceConfig.Items.RAW_AVG_MAX.ToString();
                textBoxRawCountMin.Text = DeviceConfig.Items.RAW_AVG_MIN.ToString();
                textBoxRawDataReads.Text = DeviceConfig.Items.RAW_DATA_READS.ToString();
                textBoxNoiseMax.Text = DeviceConfig.Items.RAW_NOISE_MAX.ToString();
                textBoxStdDevMax.Text = DeviceConfig.Items.RAW_STD_DEV_MAX.ToString();

                textBoxFwRev.Text = DeviceConfig.Items.FW_INFO_FW_REV.ToString();
                textBoxNumCols.Text = DeviceConfig.Items.FW_INFO_NUM_COLS.ToString();
                textBoxNumRows.Text = DeviceConfig.Items.FW_INFO_NUM_ROWS.ToString();

                textBoxIdacMax.Text = DeviceConfig.Items.IDAC_MAX.ToString();
                textBoxIdacMin.Text = DeviceConfig.Items.IDAC_MIN.ToString();

                //labelDeviceSettingStatus.Text = "Load Success!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSettingSave_Click(object sender, EventArgs e)
        {
            try
            {
                DeviceConfig.Items.FW_INFO_FW_REV = Convert.ToByte(textBoxFwRev.Text);
                DeviceConfig.Items.FW_INFO_NUM_COLS = Convert.ToByte(textBoxNumCols.Text);
                DeviceConfig.Items.FW_INFO_NUM_ROWS = Convert.ToByte(textBoxNumRows.Text);

                DeviceConfig.Items.I2C_ADDRESS = Convert.ToByte(textBoxI2CAddress.Text);

                DeviceConfig.Items.IDAC_MAX = Convert.ToByte(textBoxIdacMax.Text);
                DeviceConfig.Items.IDAC_MIN = Convert.ToByte(textBoxIdacMin.Text);

                DeviceConfig.Items.RAW_AVG_MAX = Convert.ToInt32(textBoxRawCountMax.Text);
                DeviceConfig.Items.RAW_AVG_MIN = Convert.ToInt32(textBoxRawCountMin.Text);
                DeviceConfig.Items.RAW_DATA_READS = Convert.ToInt32(textBoxRawDataReads.Text);
                DeviceConfig.Items.RAW_NOISE_MAX = Convert.ToInt32(textBoxNoiseMax.Text);
                DeviceConfig.Items.RAW_STD_DEV_MAX = Convert.ToInt32(textBoxStdDevMax.Text);

                DeviceConfig.Items.IDD_MAX = Convert.ToDouble(textBoxIddMax.Text);
                DeviceConfig.Items.IDD_MIN = Convert.ToDouble(textBoxIddMin.Text);
                DeviceConfig.Items.IDD_OPEN = Convert.ToDouble(textBoxIddOpen.Text);
                DeviceConfig.Items.IDD_SHORT = Convert.ToDouble(textBoxIddShort.Text);

                DeviceConfig.Items.VDD_OP_MAX = Convert.ToDouble(textBoxVddOpMax.Text);
                DeviceConfig.Items.VDD_OP_MIN = Convert.ToDouble(textBoxVddOpMin.Text);

                //DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";
                DeviceConfig.partType = comboBoxModule.SelectedItem.ToString();
                DeviceConfig.Write();

                //labelDeviceSettingStatus.Text = "Save Success!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);   
            }
        }

        //private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (tabControlMain.SelectedIndex != 2)
        //    {
        //        labelDeviceSettingStatus.Text = "";
        //    }
        //}

        private void textBoxSNA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSNA.Text.Length == SERIAL_NUMBER_LENGTH)
            {
                string serialNumber = textBoxSNA.Text;

                //Go to Produciton Mode First.
                if (toolStripMenuItemEMode.Checked)
                {
                    toolStripMenuItemEMode.CheckState = CheckState.Unchecked;
                    toolStripMenuItemPMode.CheckState = CheckState.Checked;    
                }

                //Check test parameter setting
                DeviceConfig.partType = serialNumber.Substring(0, PARTNUMBER_LENGTH);
                //DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";
                if (!DeviceConfig.Read())
                {
                    MessageBox.Show("Cannot find " + DeviceConfig.partType.ToString() + " in Production.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Set_TextBox(textBoxMPNA, "");
                    Set_TextBox(textBoxSNA, "");
                    return;
                }

                //Check test hardware
                if (!testHardwareReady)
                {
                    MessageBox.Show("Hardware is not ready, check hardware config", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Set_TextBox(textBoxMPNA, "");
                    Set_TextBox(textBoxSNA, "");
                    SettingLoad();
                    return;
                }

                if (checkBoxWellA.Checked && !testWellARunning && !testWellBRunning)
                {
                    testWellARunning = true;

                    Set_StatusBox(textBoxWellAStatus, "", Color.White);
                    Set_TextBox(textBoxMPNA, DeviceConfig.partType);
                    Set_TextBox(textBoxSNA, serialNumber.Substring(PARTNUMBER_LENGTH + 1, (SERIAL_NUMBER_LENGTH - PARTNUMBER_LENGTH-1)));
                    SettingLoad();

                    testWellA = new TestWell(agilentDevice, mpqDevice, 1, serialNumber);
                    Thread testThread1 = new Thread(testProcess);
                    testThread1.Start(testWellA);

                }

                else MessageBox.Show("Well A is not selected or in testing...", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxSNB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSNB.Text.Length == 19)
            {
                string serialNumber = textBoxSNB.Text;

                //Go to Produciton Mode First.
                if (toolStripMenuItemEMode.Checked)
                {
                    toolStripMenuItemEMode.CheckState = CheckState.Unchecked;
                    toolStripMenuItemPMode.CheckState = CheckState.Checked;
                }

                //Check test parameter setting
                DeviceConfig.partType = serialNumber.Substring(0, 8);
                //DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";
                if (!DeviceConfig.Read())
                {
                    MessageBox.Show("Cannot find " + DeviceConfig.partType.ToString() + " in Production.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Set_TextBox(textBoxMPNB, "");
                    Set_TextBox(textBoxSNB, "");
                    return;
                }

                //Check test hardware
                if (!testHardwareReady)
                {
                    MessageBox.Show("Hardware is not ready, check hardware config", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Set_TextBox(textBoxMPNB, "");
                    Set_TextBox(textBoxSNB, "");
                    return;
                }

                if (checkBoxWellB.Checked && !testWellARunning && !testWellBRunning)
                {
                    testWellBRunning = true;

                    Set_StatusBox(textBoxWellBStatus, "", Color.White);
                    Set_TextBox(textBoxMPNB, DeviceConfig.partType);
                    Set_TextBox(textBoxSNB, serialNumber.Substring(PARTNUMBER_LENGTH + 1, (SERIAL_NUMBER_LENGTH - PARTNUMBER_LENGTH-1)));
                    SettingLoad();

                    testWellB = new TestWell(agilentDevice, mpqDevice, 2, serialNumber);
                    Thread testThread2 = new Thread(testProcess);
                    testThread2.Start(testWellB);

                }

                else MessageBox.Show("Well B is not selected or in testing...", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radioButtonOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOnline.Checked)
            {
                SFCS_Online = true;
                radioButtonOffline.Checked = false;
                toolStripStatusLabelSFCS.BackColor = Color.Green;

            }
        }

        private void radioButtonOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOffline.Checked)
            {
                SFCS_Online = false;
                radioButtonOnline.Checked = false;
                toolStripStatusLabelSFCS.BackColor = Color.Red;
            }
        }
    } 
}
