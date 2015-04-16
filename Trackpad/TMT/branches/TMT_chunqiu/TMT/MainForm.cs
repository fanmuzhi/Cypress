using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using CypressSemiconductor.ChinaManufacturingTest.TrackPad;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
{
    public partial class MainForm : Form
    {
        //
        //-----Global Variable--------------------------
        //
        
        TestFunction testFunction;
        USB_I2C bridge;
        
        TrackpadConfig tpConfig;
        testItems tpConfigTestItems;
        // string lastPartType = " ";

        System.Timers.Timer t;
        Stopwatch timer = new Stopwatch();
        private const double TIME_OUT = 5000;
        private const int PARTNUMBER_LENGTH = 8;
        private const int SERIAL_NUMBER_LENGTH = 19;

        private bool bridgeReady = false;
        private bool test_running = false;
        private bool signalTestFlag = true;
        private bool passwordConfirmed = false;

        private string serialNumber = "";
       private bool serialNumberScanned = true;

        private const byte EngineeringMode = 1;
        private const byte ProductionMode = 2;
        private const byte DebugMode = 3;

        static SolidBrush pointB1 = new SolidBrush(Color.Red);
        public Pen penB1 = new Pen(pointB1, 1);

        static SolidBrush pointB2 = new SolidBrush(Color.Blue);
        Pen penB2 = new Pen(pointB2, 1);

        static SolidBrush pointB3 = new SolidBrush(Color.Purple);
        Pen penB3 = new Pen(pointB3, 1);

        static SolidBrush pointB4 = new SolidBrush(Color.Green);
        Pen penB4 = new Pen(pointB4, 1);

        static SolidBrush pointB5 = new SolidBrush(Color.Black);
        Pen penB5 = new Pen(pointB5, 1);

        List<Pen> PenBrushes = new List<Pen>();
       
        
        public class Mode
        {
            public System.Drawing.Size UISize;
            public bool SignalTest;
            public bool PostionTest;
            public byte ModeType;

            public Mode(byte mode)
            {
                if (mode == EngineeringMode)
                {
                    UISize = new System.Drawing.Size(620, 430);
                    SignalTest = false;
                    PostionTest = false;
                    ModeType = EngineeringMode;

                }

                if (mode == ProductionMode)
                {
                    UISize = new System.Drawing.Size(620, 184);
                    SignalTest = false;
                    PostionTest = false;
                    ModeType = ProductionMode;

                }

                if (mode == DebugMode)
                {
                    UISize = new System.Drawing.Size(620, 430);
                    SignalTest = true;
                    PostionTest = true;
                    ModeType = DebugMode;

                }
            }
        }

        private Mode currentMode = new Mode(DebugMode);

       



        //
        //----------------------------------------------
        //

        #region UI CALLBCK

        delegate void SetListBoxCallback(string text);
        delegate void SetTxtBoxCallback(int errorCode);
        delegate void SetTxtBoxCallbackString(string message);
        delegate void SetChartFormCallback(DUT tmpData);
        delegate void SetGridDisplayCallback(DUT dt);
        delegate void SetTabCallback(int tabIndex);
        delegate void SetSNBoxCallback(string text);        
        delegate void SetFocus();
       

        private void SetTxtBoxSN(string text)
        {
            if (this.textBoxSN.InvokeRequired)
            {
                SetSNBoxCallback s1 = new SetSNBoxCallback(SetTxtBoxSN);
                this.Invoke(s1, new object[] { text });

            }
            else
            {
                textBoxSN.Text = text;
                buttonStart.Focus();
               // textBoxSN.Focus();
            }


        }

        private void SetTxtBoxStatus(int errorCode)
        {
            if (this.textBoxStatus.InvokeRequired)
            {
                SetTxtBoxCallback d0 = new SetTxtBoxCallback(SetTxtBoxStatus);
                this.Invoke(d0, new object[] { errorCode });
            }
            else
            {
                if (test_running)
                {
                    textBoxStatus.Text = "TESTING";
                    textBoxStatus.BackColor = Color.Yellow;
                }
                else
                {
                    if (errorCode != 0)
                    {
                        textBoxStatus.Text = textBoxStatus.Text = string.Format("{0:X} ", errorCode) + ": " + ErrorCode.ReportErrorCodes(errorCode);
                        textBoxStatus.BackColor = Color.Red;
                    }
                    else
                    {
                        textBoxStatus.Text = "PASS";
                        textBoxStatus.BackColor = Color.Green;
                    }
                }
            }
        }

        private void SetTxtBoxStatus(string message)
        {
            if (this.textBoxStatus.InvokeRequired)
            {
                SetTxtBoxCallbackString d0 = new SetTxtBoxCallbackString(SetTxtBoxStatus);
                this.Invoke(d0, new object[] { message });
            }
            else
            {
                if (test_running)
                { return; }
                else
                {
                    textBoxStatus.Text = message;
                }
            }
        }


        private void SetTabControlIndex(int tabIndex)
        {
            if (this.tabControlMain.InvokeRequired)
            {
                SetTabCallback dx = new SetTabCallback(SetTabControlIndex);
                this.Invoke(dx, new object[] { tabIndex });
            }
            else
                tabControlMain.SelectedIndex = tabIndex;
                //buttonPass.Focus();
        }

        private void SetTabPassFocus()
        {
            if (this.buttonPass.InvokeRequired)
            {
                SetFocus dx = new SetFocus(SetTabPassFocus);
                this.Invoke(dx, new object[] { });
            }
            else
            {
                buttonPass.Focus();
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

        private void SetListBoxPorts(string text)
        {
            if (this.listBoxPorts.InvokeRequired)
            {
                SetListBoxCallback d2 = new SetListBoxCallback(SetListBoxPorts);
                this.Invoke(d2, new object[] { text });
            }
            else
            {
                listBoxPorts.Items.Add(text);
            }
        }

        

        #endregion

        #region SYS Config

        /// <summary>
        /// load TMT config
        /// </summary>
        private void loadSysConfig()
        {

            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey(@"software\Cypress\TrackpadTest");
                Manufacture.operatorID = readKey.GetValue("Test ID").ToString();
                Manufacture.testStation = readKey.GetValue("Test Station").ToString();
                Manufacture.testSite = readKey.GetValue("Test Site").ToString();

                byte mode = Convert.ToByte(readKey.GetValue("Current Mode"));
                if (mode != 0)
                    currentMode = new Mode(mode);
                else
                    currentMode = new Mode(ProductionMode);
            }
            catch
            {
                FormOperaterID formWorkerID = new FormOperaterID();
                formWorkerID.ShowDialog();

                Manufacture.operatorID = formWorkerID.WorkerID;
                Manufacture.testStation = formWorkerID.WorkStation;
                Manufacture.testSite = formWorkerID.WorkSite;

                currentMode = new Mode(ProductionMode);
            }

            finally
            {

                toolStripStatusLabelTestID.Text = "Test ID: " + Manufacture.operatorID;
                toolStripStatusLabelTestSite.Text = "Test Site: " + Manufacture.testSite;
                toolStripStatusLabelTestStation.Text = "Test Station: " + Manufacture.testStation;

                this.ClientSize = currentMode.UISize;
                if (currentMode.ModeType == ProductionMode)
                {
                    productionModeToolStripMenuItem.Checked = true;
                    engineeringModeToolStripMenuItem.Checked = false;
                    debugModeToolStripMenuItem.Checked = false;
                    tabControlMain.Visible = false;
                }
                if (currentMode.ModeType == EngineeringMode)
                {
                    productionModeToolStripMenuItem.Checked = false;
                    engineeringModeToolStripMenuItem.Checked = true;
                    debugModeToolStripMenuItem.Checked = false;
                    tabControlMain.Visible = true;
                }
                if (currentMode.ModeType == DebugMode)
                {
                    productionModeToolStripMenuItem.Checked = false;
                    engineeringModeToolStripMenuItem.Checked = false;
                    debugModeToolStripMenuItem.Checked = true;
                    tabControlMain.Visible = true;
                }
            }

        }

        /// <summary>
        /// save TMT config
        /// </summary>
        private void saveSysConfig()
        {
            RegistryKey saveKey = Registry.LocalMachine.CreateSubKey(@"software\Cypress\TrackpadTest");

            saveKey.SetValue("Test ID", Manufacture.operatorID);
            saveKey.SetValue("Test Station", Manufacture.testStation);
            saveKey.SetValue("Test Site", Manufacture.testSite);
            saveKey.SetValue("Current Mode", currentMode.ModeType.ToString());
        }

        #endregion

        //
        //---------------------------------Main Form Controls--------------------------------------------------
        //
        public MainForm()
        {
            InitializeComponent();
            PositionPanel.Size = new Size(Convert.ToInt32(resolutionX.Text) / 4, Convert.ToInt32(resolutionY.Text) / 4);

            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;

            loadSysConfig();
           // textBoxSN.Text = "do not need";
           // textBoxSN.Focus();
            PenBrushes.Add(penB1);
            PenBrushes.Add(penB2);
            PenBrushes.Add(penB3);
            PenBrushes.Add(penB4);
            PenBrushes.Add(penB5);
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

            bridge = new USB_I2C();

            //define a timer to get bridge device when TIME_OUT
            t = new System.Timers.Timer(TIME_OUT);
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();

            int OK = GetDevice();
            if (OK == 0) { t.Stop(); }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (test_running)
            {
                DialogResult status = MessageBox.Show("Test is Runnung. Do you want to close?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (status == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    bridge.ClosePort();
                    bridge = null;
                }
            }
            else
            {
                bridge.ClosePort();
                bridge = null;
            }

            saveSysConfig();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //textBoxSN.Focus();
            buttonStart.Focus();

        }

        private void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int OK = GetDevice();
            if (OK == 0) { t.Stop(); }
        }

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
        //
        //---------------------------------------------------------------------------------------------------
        //

        #region UI Event

        //select grid view items
        


        //stop position view
       

        private void buttonPass_Click(object sender, EventArgs e)
        {
            testFunction.readPositionStop = false;
            tabControlMain.SelectedIndex = 0;
        }

        private void buttonFail_Click(object sender, EventArgs e)
        {
            testFunction.readPositionStop = false;
            tabControlMain.SelectedIndex = 0;
            testFunction.dut.ErrorCode = ErrorCode.ERROR_No_Signal;
        }
        //change resolution settings of trackpad.
        private void resolutionX_TextChanged(object sender, EventArgs e)
        {
            PositionPanel.Size = new Size(Convert.ToInt32(resolutionX.Text) / 2, Convert.ToInt32(resolutionY.Text) / 2);
        }

        //Serial Number scanned in.
       

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

        //Open the brige port.
        private void listBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxPorts.SelectedItems == null)
            { return; }

            try
            {
                int status = bridge.OpenPort(listBoxPorts.SelectedItem.ToString());
                if (status != 0)
                {
                    SetTxtBoxStatus("Error in open port: " + bridge.LastError);
                    Log.error("Error in open port: " + bridge.LastError);
                    bridge.PowerOff();
                    bridge.ClosePort();
                    bridgeReady = false;
                }
                else
                {
                    SetTxtBoxStatus("Successful to open port: " + listBoxPorts.SelectedItem.ToString());
                    this.listBoxPorts.SelectedIndexChanged -= new System.EventHandler(this.listBoxPorts_SelectedIndexChanged);
                    listBoxPorts.SetSelected(0, false);
                    bridgeReady = true;
                }
            }
            catch
            {
                //buttonStart.Focus();
                return; 
            }

            //buttonStart.Focus();
        }

        private int GetDevice()
        {
            int Status = 1;
            try
            {
                string[] ports = bridge.GetPorts();
                if (ports.Length < 1)
                {
                    //SetListBoxStatus("No I2C Device Found: " + bridge.LastError);
                    Status = 1;
                }
                else
                {
                    foreach (string port in ports)
                    {
                        SetListBoxPorts(port);
                        SetTxtBoxStatus("Device Found: " + port);
                        Status = 0;
                    }
                }
            }
            catch
            { }
            return Status;
        }

        //show manufacturing info
        private void manufacturingInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormOperaterID formWorkerID = new FormOperaterID();
                formWorkerID.ShowDialog();

                Manufacture.operatorID = formWorkerID.WorkerID;
                Manufacture.testStation = formWorkerID.WorkStation;
                Manufacture.testSite = formWorkerID.WorkSite;

                toolStripStatusLabelTestSite.Text = "Test Site: " + Manufacture.testSite;
                toolStripStatusLabelTestStation.Text = "Test Station: " + Manufacture.testStation;
                toolStripStatusLabelTestID.Text = "Test ID: " + Manufacture.operatorID;

            }
            catch { }
        }


        //show configuratrion panel.
        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (passwordConfirmed)
            {

                FormTrackpadConfig formTPConfig = new FormTrackpadConfig();
                try
                {
                    formTPConfig.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    formTPConfig.Dispose();
                }

            }
            else
                MessageBox.Show("Please LogIn from the file Menu first\n Or you can contact your test engineer ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //exit test program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //log out of engineering mode
        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            passwordConfirmed = false;
            this.configurationToolStripMenuItem.Enabled = false;
        }

        //log in the engineering mode
        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
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
                    this.configurationToolStripMenuItem.Enabled = true;
                }
                else
                {
                    passwordConfirmed = false;
                    this.configurationToolStripMenuItem.Enabled = false;

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

        //about box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            try
            {
                aboutBox.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.info(ex.Message);
            }
            finally
            {
                aboutBox.Dispose();
            }
        }

        //production mode view
        private void productionModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMode = new Mode(ProductionMode);
            this.ClientSize = currentMode.UISize;
            tabControlMain.Visible = false;

            productionModeToolStripMenuItem.Checked = true;
            engineeringModeToolStripMenuItem.Checked = false;
            debugModeToolStripMenuItem.Checked = false;
        }

        //engineer mode view
        private void engineeringModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMode = new Mode(EngineeringMode);
            this.ClientSize = currentMode.UISize;
            tabControlMain.Visible = true;

            productionModeToolStripMenuItem.Checked = false;
            engineeringModeToolStripMenuItem.Checked = true;
            debugModeToolStripMenuItem.Checked = false;
        }

        //debug mode view
        private void debugModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMode = new Mode(DebugMode);
            this.ClientSize = currentMode.UISize;
            tabControlMain.Visible = true;

            productionModeToolStripMenuItem.Checked = false;
            engineeringModeToolStripMenuItem.Checked = false;
            debugModeToolStripMenuItem.Checked = true;
        }

        //clear serial number
      
        //show the scroll message
        private void testFunction_TestMessageEvent(object sender, TestMessageEventArgs ea)
        {
            SetListBoxStatus(ea.Message);
        }

        //click the start button
        //check the hardware, serial number, online or offline, then start the test process
        private void buttonStart_Click(object sender, EventArgs e)
        {
            string partType = "11600200";
            currentMode = new Mode(EngineeringMode);
            this.ClientSize = currentMode.UISize;
            tabControlMain.Visible = true;

            productionModeToolStripMenuItem.Checked = false;
            engineeringModeToolStripMenuItem.Checked = true;
            debugModeToolStripMenuItem.Checked = false;
            
            //timer.Start();
            timer.Restart();

            if (serialNumber != "")
            {
                partType = serialNumber.Substring(0, PARTNUMBER_LENGTH);
            }
           
            //if another test instance is running, then quit this test.
            if (test_running)
            {
                MessageBox.Show("Test is running", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ////show the workID panel if work ID is not valid.
            //while (DeviceConfig.Items.trackpad_ShopFloor == DeviceConfig.TPCONFIG.TP_TEST_ONLINE && TestFunction.WorkerID == "")
            //{
            //    FormOperaterID formWorkerID = new FormOperaterID();
            //    formWorkerID.ShowDialog();
            //    TestFunction.WorkerID = formWorkerID.WorkerID;
            //}

            //check if serial number is valid
            if (true)
            {
                //Check Part type in Production.ini
                try
                {

                    FormTrackpadConfig formTPConfig = new FormTrackpadConfig();
                    tpConfig = formTPConfig.read(partType);


                    if (Manufacture.testStation == "TMT")
                    {
                        tpConfigTestItems = tpConfig.TMT;
                    }

                    if (Manufacture.testStation == "OQC")
                    {
                        tpConfigTestItems = tpConfig.OQC;
                    }

                    if (Manufacture.testStation == "TPT")
                    {
                        tpConfigTestItems = tpConfig.TPT;
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.error(ex.Message);
                    return;
                }

                //Check if bridge is ready
                if (!bridgeReady)
                {
                    MessageBox.Show("Please open the bridge first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Clear UI info
                listBoxStatus.Items.Clear();
                textBoxStatus.Text = "";
                textBoxStatus.BackColor = Color.White;

                //start thread of test functions
                testFunction = new TestFunction(serialNumber, bridge, tpConfig, Manufacture.testStation);

                //subscribe the listView or not
                if (currentMode.ModeType != ProductionMode)
                {
                    testFunction.TestMessageEvent += new TestFunction.TestMessageEventHandler(testFunction_TestMessageEvent);
                }

                else
                {
                    testFunction.TestMessageEvent -= new TestFunction.TestMessageEventHandler(testFunction_TestMessageEvent);
                }


                Task readTask = new Task(testProcess);
                readTask.Start();

                Task processTask = new Task(dataProcess);
                processTask.Start();

                //start test process in test main thread
                //testReadMainThread = new Thread(testProcess);
                //testReadMainThread.IsBackground = true;
                //testReadMainThread.Start();

                //testDataMainThread = new Thread(dataProcess);
                //testDataMainThread.IsBackground = true;
                //testDataMainThread.Start();

                test_running = true;

                //wait for the test main thread alive.
                //while (!testReadMainThread.IsAlive)
                //{
                //    testReadMainThread.Join();
                //    //System.Threading.Thread.Sleep(10);
                //}
            }
            else
            {
                MessageBox.Show("Please input serial number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        /// <summary>
        /// Main Test Process, Start in a new thread.
        /// </summary>
        private void testProcess()
        {

            try
            { testFunction.BridgePowerOff(); }  //try to stop the bridge if it's in use.
            catch
            { }

            try
            {

                testFunction.idacErased = false;
                testFunction.IDACItem = false;
                testFunction.RawCountItem = false;
                testFunction.FWVersionItem = false;
                //power on trackpad and enter into test mode.
                testFunction.BridgePowerOn();
                System.Threading.Thread.Sleep(500);
                testFunction.ExitBootloader();
                System.Threading.Thread.Sleep(100);
                testFunction.Enter_TestMode();
                System.Threading.Thread.Sleep(100);
               // System.Threading.Thread.Sleep(1000);
                //clear text box message, show "testing".
                //testFunction.dut.ErrorCode = ErrorCode.EEROR_SYSTEM_ERROR;
                //SetTxtBoxStatus(testFunction.dut.ErrorCode);                
                // FW check
                if (tpConfigTestItems.ReadFW)
                {
                    testFunction.I2C_Read_FW_Rev_Test();
                }
                testFunction.BridgePowerOff();
                testFunction.BridgePowerOn();
                System.Threading.Thread.Sleep(500);
                testFunction.ExitBootloader();
                if (checkBoxRecal.Checked & !testFunction.I2C_Check_RawCount_Value())
                {
                    testFunction.I2C_Calibrate_IDAC();
                    testFunction.BridgePowerOff();
                    testFunction.BridgePowerOn();
                    System.Threading.Thread.Sleep(500);
                    testFunction.ExitBootloader();
                    testFunction.I2C_Check_RawCount_Value();
                }


                //Position test
                SetTabControlIndex(1);
                SetTabPassFocus();

                while (testFunction.readPositionStop)
                {                   
                    testFunction.ReadPosition();
                    PositionPanel.Invalidate();
                }
                testFunction.BridgePowerOff();

                SetTxtBoxStatus(testFunction.dut.ErrorCode); 
                if (testFunction.dut.ErrorCode == 0)
                {
                    System.Threading.Thread.Sleep(500);
                    testFunction.BridgePowerOn();
                    System.Threading.Thread.Sleep(1000);
                    testFunction.ExitBootloader();
                    // System.Threading.Thread.Sleep(1000);
                    //clear text box message, show "testing".
                    SetTxtBoxStatus(testFunction.dut.ErrorCode);

                    if (checkBoxRecal.Checked & !testFunction.I2C_Check_RawCount_Value())
                    {
                        testFunction.I2C_Calibrate_IDAC();
                        testFunction.BridgePowerOff();
                        testFunction.BridgePowerOn();
                        System.Threading.Thread.Sleep(1000);
                        testFunction.ExitBootloader();
                        testFunction.I2C_Check_RawCount_Value();
                    }
                    testFunction.BridgePowerOff();
                }
               
                test_running = false;   //stop test
                SetTxtBoxStatus(testFunction.dut.ErrorCode); 
            }
            catch (Exception ex)
            {
                test_running = false;
                SetTxtBoxStatus(ErrorCode.EEROR_SYSTEM_ERROR);
                MessageBox.Show(ex.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.error(ex.Message);
                bridge.PowerOff();
                //bridge.ClosePort();

            }

            finally
            {

            }
        }

        private void dataProcess()
        {

            try
            {
                //clear all test data DUT stored.
                testFunction.dut.RawCount.Clear();
                testFunction.dut.Signal.Clear();
                testFunction.dut.Noise.Clear();
                testFunction.dut.IDACGain.Clear();
                testFunction.dut.Global_IDAC.Clear();
                testFunction.dut.Local_IDAC.Clear();


                //check test permission if online
                if (Manufacture.testSite == "CM 1")
                {
                    if (!testFunction.CheckPemission())
                    {
                        test_running = false;
                        return;
                    }
                }
                while (test_running)
                {
                    System.Threading.Thread.Sleep(100);
                }
               


                //write into xml test log.xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                testFunction.OutputData();

                //upload test result to damn sigmatron shop floor system
                if (Manufacture.testSite == "CM 1")
                {
                    testFunction.UploadResult();
                }

                SetTxtBoxStatus(testFunction.dut.ErrorCode); //show pass fail
                //SetDisplayChart(testFunction.dut); //show charts
            }
            catch (Exception ex)
            {
                test_running = false;
                SetTxtBoxStatus(ErrorCode.EEROR_SYSTEM_ERROR);
                MessageBox.Show(ex.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.error(ex.Message);

            }

            finally
            {
                SetTxtBoxSN("");
                serialNumberScanned = false;
                timer.Stop();
                // MessageBox.Show("elapsed" + timer.Elapsed.TotalSeconds, " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetListBoxStatus("Test time(s): " + timer.Elapsed.TotalSeconds);
                
            }
        }

        /// <summary>
        /// Show finger position of trackpad.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionPanelPaint(object sender, PaintEventArgs e)
        {

            //draw cursor line 

            try
            {
                for (int i = 0; i < testFunction.FingerRecords.Count(); i++)
                {
                    e.Graphics.DrawLines(PenBrushes[i], testFunction.FingerRecords[i].FingerPoints.ToArray());
                }

            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message + "painting error");
            }
            textBoxFingers.Text = testFunction.FingerCount.ToString();
            

            try
            {
                if ((testFunction.dut.LeftButtonStatus & 0x01) == 0x01)
                {
                    textBoxSwitch.BackColor = Color.Green;
                }
                else
                {
                    textBoxSwitch.BackColor = Color.White;
                }

            }

            catch
            { }

        }

        private void textBoxSN_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSN.Text.Length == SERIAL_NUMBER_LENGTH)
            {
                serialNumber = textBoxSN.Text;
                string partType = serialNumber.Substring(0, PARTNUMBER_LENGTH);
                string identifier = serialNumber.Substring(PARTNUMBER_LENGTH, (SERIAL_NUMBER_LENGTH - PARTNUMBER_LENGTH));

                serialNumber = partType + identifier;

                if (validateSerialNumber(serialNumber))
                {
                    textBoxSN.Text = identifier;
                    textBoxMPN.Text = partType;

                    serialNumberScanned = true;
                }
                else
                {

                    MessageBox.Show("invalid serial number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxSN.Text = "";
                    textBoxMPN.Text = "";

                    serialNumberScanned = false;
                }

            }
        }

        private void textBoxSN_MouseDown(object sender, MouseEventArgs e)
        {
            textBoxSN.Text = "";
            textBoxMPN.Text = "";
        }

       

        private void buttonStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//F2...F12
            {
                buttonStart_Click(sender, e);
            }
        }

        

    }
}
