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
using System.Timers;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CypressSemiconductor.ChinaManufacturingTest.TrackPad;

namespace CypressSemiconductor.ChinaManufacturingTest.TPT
{

    public partial class MainForm : Form    
    {
        //
        //------------------Global Variables-------------------------------------------------------           
        //
        TrackpadConfig tpConfig;
        //testItems tpConfigTestItems;
        
        Agilent agilentDevice;
        MPQ mpqDevice;

        TestWell testWellA;
        //TestWell testWellB;
        string lastPartType = " ";

        private TextBox[] dutTextBoxWellA;
        //private TextBox[] dutTextBoxWellB;

        private bool testWellARunning = false;
        //private bool testWellBRunning = false;

        private bool testHardwareReady = false;
        //private bool SFCS_Online = true;

        private bool passwordConfirmed = false;

        delegate void SetDUTTextBoxCallback(TextBox tb, int errorCode, bool testFinished);
        delegate void SetListBoxCallback(string text);
        delegate void SetTextCallback(TextBox tb, string text);
        delegate void SetStatusCallback(TextBox tb, string text, Color color);
        delegate void SetProcessBarCallback(int Num);

        private const int SERIAL_NUMBER_LENGTH = 19;
        private const int PARTNUMBER_LENGTH = 8;

        private double totalTestedNumber;
        private double totalFailedNumber;
        private double yieldRate;

        private string testID;
        private string testSite;
        private string testStation;

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
        }


        //****************************************//
        //             Menu Strip                 //
        //****************************************//
        private void deviceConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTrackpadConfig formTPConfig = new FormTrackpadConfig();
            try
            {
                formTPConfig.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.error(ex.Message);
            }
            finally
            {
                formTPConfig.Dispose();
            }

        }
        
        
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
                Log.error(ex.Message);
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
                Log.error(ex.Message);
            }
            finally
            {
                aboutBox.Dispose();
            }
        }

        private void toolStripMenuItemPMode_Click(object sender, EventArgs e)
        {
            passwordConfirmed = false;
            toolStripMenuItemEMode.Checked = false;
            toolStripMenuItemPMode.Checked = true;
        }

        private void toolStripMenuItemEMode_Click(object sender, EventArgs e)
        {
            FormPassword formPassword = new FormPassword();
            try
            {
                formPassword.MinimizeBox = false;
                formPassword.MaximizeBox = false;
                formPassword.ShowDialog();
                if (formPassword.Password == "cypress")
                {
                    passwordConfirmed = true;
                    toolStripMenuItemEMode.Checked = true;
                    toolStripMenuItemPMode.Checked = false;                    

                }
                else
                {
                    passwordConfirmed = false;
                    toolStripMenuItemEMode.Checked = false;
                    toolStripMenuItemPMode.Checked = true;

                    MessageBox.Show("Wrong Password!\n Please contact your test engineer ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.error(ex.Message);
            }
            finally
            {
                formPassword.Dispose();
            }
        }

        private void ProdTestMode_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItemPMode.Checked)     //Produciton Mode
            {
                
               
                //tabControlMain.TabPages.Remove(tabPageDeviceSetting);

                //buttonSettingLoad.Enabled = false;  //disable trackpad test parameter setting.
                //buttonSettingSave.Enabled = false;

                //radioButtonOffline.Enabled = false; //disable SFCS on/off option.
                //radioButtonOnline.Enabled = false;

                deviceConfigToolStripMenuItem.Visible = false;
                hardwareConfigToolStripMenuItem.Visible = false;
            }
            else                                    //Engineering Mode
            {
                //tabControlMain.TabPages.Add(tabPageDeviceSetting);

                //buttonSettingLoad.Enabled = true;   //disable trackpad test parameter setting.
                //buttonSettingSave.Enabled = true;

                //radioButtonOffline.Enabled = true;  //disable SFCS on/off option.
                //radioButtonOnline.Enabled = true;

                deviceConfigToolStripMenuItem.Visible = true;
                hardwareConfigToolStripMenuItem.Visible = true;
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void manufacturingInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormOperaterID formWorkerID = new FormOperaterID();
                formWorkerID.ShowDialog();

                testID = formWorkerID.WorkerID;
                testStation = formWorkerID.WorkStation;
                testSite = formWorkerID.WorkSite;

                toolStripStatusLabelTestSite.Text = "Test Site: " + testSite;
                toolStripStatusLabelTestStation.Text = "Test Station: " + testStation;
                toolStripStatusLabelTestID.Text = "Test ID: " + testID;

            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
        }


        //****************************************//
        //                MainForm                //
        //****************************************//

        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
        
        
        private void MainForm_Load(object sender, EventArgs e)
        {

            Process process = RunningInstance();
            if (process != null)
            {
                DialogResult status = MessageBox.Show("Another Instance of Current Software is Already Running. Still Continue?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (status == DialogResult.No)
                {
                    this.Close();
                    return;
                }
            }

            //initialize GUI disaplay
            toolStripMenuItemEMode.CheckState = CheckState.Unchecked;
            toolStripMenuItemPMode.CheckState = CheckState.Checked;
            
            //tabControlMain.TabPages.Remove(tabPageDeviceSetting);

            //DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";

            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey(@"software\Cypress\TrackpadTest");
                totalTestedNumber = Convert.ToInt32(readKey.GetValue("Total Tested"));
                totalFailedNumber = Convert.ToInt32(readKey.GetValue("Total Failed"));
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


            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey(@"software\Cypress\TrackpadTest");
                testID = readKey.GetValue("Test ID").ToString();
                testStation = readKey.GetValue("Test Station").ToString();
                testSite = readKey.GetValue("Test Site").ToString();
            }
            catch
            {
                FormOperaterID formWorkerID = new FormOperaterID();
                formWorkerID.ShowDialog();

                testID = formWorkerID.WorkerID;
                testStation = formWorkerID.WorkStation;
                testSite = formWorkerID.WorkSite;
            }

            toolStripStatusLabelTestSite.Text = "Test Site: " + testSite;
            toolStripStatusLabelTestStation.Text = "Test Station: " + testStation;
            toolStripStatusLabelTestID.Text = "Test ID: " + testID;

            textBoxSNA.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (testWellARunning)
            {
                DialogResult status = MessageBox.Show("Test is Runnung. Do you want to close?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (status == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    exitTPT();
                }
            }
            else
            {
                exitTPT();
            }
        }

        private void exitTPT()
        {
            try
            {
                RegistryKey saveKey = Registry.LocalMachine.CreateSubKey(@"software\Cypress\TrackpadTest");

                saveKey.SetValue("Total Tested", totalTestedNumber.ToString());
                saveKey.SetValue("Total Failed", totalFailedNumber.ToString());
                saveKey.SetValue("Yield Rate", yieldRate.ToString());

                saveKey.SetValue("Test ID", testID);
                saveKey.SetValue("Test Station", testStation);
                saveKey.SetValue("Test Site", testSite);


                agilentDevice = null;
                mpqDevice = null;
                testWellA = null;
            }
            catch(Exception ex)
            {
                Log.error(ex.Message);
            }
        
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
                agilentDevice.InitializeU2751A_WELLA(HardwareConfig.Option.U2751AtxtAddr);
                //agilentDevice.InitializeU2751A_WELLB(HardwareConfig.Option.U2751AWellBtxtAddr);

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
                Log.error(ex.Message);
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

            if (!testWell.TPTFixtureEnableStatus()) // check if this TPT is enabled or bad*********************
            {
                MessageBox.Show("This TPT is not enabled or a bad one, please contact your test engineer", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                testWellARunning = false;
                testWell.PneumaticOff();
                return;
            }

            //if (!testWell.WellIDCheck(textBoxMPNA.Text)) // check if it is the right fixture*********************
            //{
            //    MessageBox.Show("This is not the right fixture to test the board", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    testWellARunning = false;
            //    testWell.PneumaticOff();
            //    return;
            //}

            if (testWell.ReadMicroSwitch())
            {
                Show_Test_Result(testWell, false);  //clear test result

                Thread.Sleep(DelayTime.MICRO_SWITCH_ON);
                SetListBoxStatus("Well Closed");

                testWell.PneumaticOn();
                Thread.Sleep(DelayTime.PNEUMATIC_ON);

                testWell.CheckTrackpadInSlot(); // check if there is trackpad in each of eight slots *******************************

                if (testSite == "CM 1")
                {
                    SetListBoxStatus("***********Check Permission*************");
                    //if (TestWell.OperatorID == "")
                    //{
                        //FormOperatorID formOperatorID = new FormOperatorID();
                        //formOperatorID.ShowDialog();
                        //TestWell.OperatorID = formOperatorID.OperatorID;
                    //}

                    if (!testWell.CheckTestPermission(testID))
                    {
                                               
                        Set_StatusBox(textBoxWellAStatus, "Fail to connect SFCS", Color.Red);
                        testWellARunning = false;
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

                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("**********Enter Test Mode************");
                    testWell.EnterIntoTestMode();
                }
                if (testWell.PortEnalbes != 0 && tpConfig.TPT.ReadFW)
                {
                    SetListBoxStatus("**********Fimware Rev Check************");
                    testWell.I2CFWRevCheck();
                    Show_Test_Result(testWell, false);
                }
                if (testWell.PortEnalbes != 0 && tpConfig.TPT.SwitchTest)
                {
                    SetListBoxStatus("**********Switch Button Test************");
                    testWell.I2CSwitchTest();
                    Show_Test_Result(testWell, false);
                }

                if (testWell.PortEnalbes != 0)
                {
                    SetListBoxStatus("*************IDD Testing***************");
                    testWell.IDDTest(true);
                    Show_Test_Result(testWell, false);
                }

                if (testWell.PortEnalbes != 0 && tpConfig.TPT.ReadRawCount)
                {
                    SetListBoxStatus("**********Row Count Testing**********");
                    testWell.I2CRowCountTest();
                    Show_Test_Result(testWell, false);
                }
                if (testWell.PortEnalbes != 0 && tpConfig.TPT.ReadIDAC)
                {
                    SetListBoxStatus("*************IDAC Testing**************");
                    testWell.I2CIDACTest();
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
                    SetListBoxStatus("**********Exit Test Mode************");
                    testWell.ExitTestMode();
                }
                if (testSite == "CM 1")
                {
                    SetListBoxStatus("*********Upload Test Results***********");
                    if (!testWell.UploadTestRecord(testID))
                    {
                        Set_StatusBox(textBoxWellAStatus, "Fail to connect SFCS", Color.Red);
                        testWellARunning = false;
                        testWell.PneumaticOff();
                        return;
                    }
                }

                testWell.TestLog();
                Show_Test_Result(testWell, true);

                //cacalute pass/fail count
                double currentTested = 0;
                double currentFailed = 0;
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

                //set status box
                if (currentFailed != 0)
                { Set_StatusBox(textBoxWellAStatus, "Total Failed:" + currentFailed.ToString(), Color.Red); }
                else
                { Set_StatusBox(textBoxWellAStatus, "PASS", Color.Green); }
                
                testWellARunning = false;
                Set_TextBox(textBoxSNA, "");    //clear serial number of wellA, get focused.
  
            }    
        }

        //****************************************//
        //            User Interface              //
        //****************************************//
        private void Show_Test_Result(TestWell tW, bool testFinished)
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

        //private void SetProcessBar(int Num)
        //{
        //    if (this.statusStrip1.InvokeRequired)
        //    {
        //        SetProcessBarCallback p = new SetProcessBarCallback(SetProcessBar);
        //        this.Invoke(p, new object[] { Num });
        //    }
        //    else
        //    {
        //        this.toolStripStatusProgressBar.Value = Num;
        //    }
        //}

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
                tb.Text = string.Format("0x{0:X} ", errorCode);
                if (errorCode != 0)
                    tb.BackColor = Color.Red;
                else if (testFinished)
                    tb.BackColor = Color.Green;
                else
                    tb.BackColor = Color.White;
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
                string partType = serialNumber.Substring(0, PARTNUMBER_LENGTH);
                string identifier = serialNumber.Substring(PARTNUMBER_LENGTH, (SERIAL_NUMBER_LENGTH - PARTNUMBER_LENGTH));

                serialNumber = partType + identifier;

                //check if serial number is valid
                if (!validateSerialNumber(serialNumber))
                {
                    MessageBox.Show("invalid serial number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Set_TextBox(textBoxMPNA, "");
                    Set_TextBox(textBoxSNA, "");

                    return;
                }

                Set_TextBox(textBoxMPNA, partType);
                Set_TextBox(textBoxSNA, identifier);

                //Go to Produciton Mode.
                if (toolStripMenuItemEMode.Checked)
                {
                    toolStripMenuItemEMode.CheckState = CheckState.Unchecked;
                    toolStripMenuItemPMode.CheckState = CheckState.Checked;    
                }

                //Check test hardware
                if (!testHardwareReady)
                {
                    MessageBox.Show("Hardware is not ready, check hardware config", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Set_TextBox(textBoxMPNA, "");
                    Set_TextBox(textBoxSNA, "");

                    return;
                }

                if (!testWellARunning)
                {
                    try
                    {
                        if (lastPartType != partType)
                        {
                            FormTrackpadConfig formTPConfig = new FormTrackpadConfig();
                            tpConfig = formTPConfig.read(partType);
                        }
                        lastPartType = partType;
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.error(ex.Message);
                        return;
                    }

                    testWellARunning = true;

                    Set_StatusBox(textBoxWellAStatus, "", Color.White);

                    testWellA = new TestWell(agilentDevice, mpqDevice, serialNumber, tpConfig);
                    Thread testThread1 = new Thread(testProcess);
                    testThread1.Start(testWellA);

                }

                else
                {
                    MessageBox.Show("Current Well is in testing...", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



            }



        }

        private bool validateSerialNumber(string sn)
        {

            //string pattern = "^[0-1][0-2][0-9][0][0-9][1-9]"; //part type, eg: 102005

            //pattern += "[0][0-3]";      //eg: 00, 01
            //pattern += "[0-1][0-9]";    // revision, eg: 03.
            //pattern += "[1][1-9]";      // year, from 11 to 19.
            //pattern += "[0-5][1-9]";    // week, from 01 to 59.
            //pattern += "[0-3]";         // manufacturing site.
            //pattern += "[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]"; //panel number.
            //pattern += "[1-8]$";         //dut number.

            Regex r = new Regex("^[01]([0][0-9]|[1][0-9])([0][0-9][0-9])[0][01][0][1-9]([0][1-9]|[1][0-5])([0-4][0-9]|[5][0-3])[0-4][a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]([1-8]$|$)");

            return r.IsMatch(sn);
        }


    
    } //end of MainForm Class 

} //end of NameSpace
