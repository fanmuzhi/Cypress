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
        double positionPanelScale = 1;
        int radius = 60;
        int fingerCount = 0;

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
        private bool serialNumberScanned = false;

        private const byte EngineeringMode = 1;
        private const byte ProductionMode = 2;
        private const byte DebugMode = 3;
        


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
                    UISize = new System.Drawing.Size(667, 652);
                    SignalTest = false;
                    PostionTest = true;
                    ModeType = EngineeringMode;

                }

                if (mode == ProductionMode)
                {
                    UISize = new System.Drawing.Size(667, 184);
                    SignalTest = false;
                    PostionTest = false;
                    ModeType = ProductionMode;

                }

                if (mode == DebugMode)
                {
                    UISize = new System.Drawing.Size(667, 652);
                    SignalTest = true;
                    PostionTest = false;
                    ModeType = DebugMode;

                }
            }
        }

        private Mode currentMode = new Mode(ProductionMode);  

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
                textBoxSN.Focus();
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



        private void SetGridDisplay(DUT dt)
        {
            if (this.dataGridViewIntegrated.InvokeRequired)
            {
                SetGridDisplayCallback d = new SetGridDisplayCallback(SetGridDisplay);
                this.dataGridViewIntegrated.Invoke(d, new object[] { dt });
            }
            else
            {
                if (dataGridViewIntegrated.Columns.Count != tpConfig.TMT.FW_INFO_NUM_COLS ||
                    dataGridViewIntegrated.Rows.Count != tpConfig.TMT.FW_INFO_NUM_ROWS)   //  initiate dataGridViewIntegrated;
                {

                    //clear data grid columns and rows
                    dataGridViewIntegrated.Columns.Clear();
                    dataGridViewIntegrated.Rows.Clear();

                    //structure new data grid
                    for (int j = 0; j < tpConfig.TMT.FW_INFO_NUM_COLS; j++)
                    {
                        dataGridViewIntegrated.Columns.Add("Columns", Convert.ToString(j + 1));
                        dataGridViewIntegrated.Columns[j].Width = 35;
                    }
                    dataGridViewIntegrated.Rows.Add(tpConfig.TMT.FW_INFO_NUM_ROWS - 1);
                    for (int i = 0; i < tpConfig.TMT.FW_INFO_NUM_ROWS; i++)
                    {
                        dataGridViewIntegrated.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
                    }
                    dataGridViewIntegrated.RowHeadersWidth = 50;
                }
                dataGridViewIntegrated.Rows[0].Selected = false;
                List<int> DisplayInfo = new List<int>(); //dt.RawCount;


                try
                {
                    switch (ComboBoxDisplayInfo.Text)
                    {
                        case "Noise":
                            //  DisplayInfo.Clear();
                            foreach (int temp in dt.Noise)
                            { DisplayInfo.Add(temp); }
                            break;
                        case "Local IDAC":
                            // DisplayInfo.Clear();
                            foreach (int temp in dt.Local_IDAC)
                            { DisplayInfo.Add(temp); }
                            break;
                        case "Global IDAC":
                            // DisplayInfo.Clear();
                            foreach (int temp in dt.Global_IDAC)   //IDAC is equal to Global IDAC for APA.
                            { DisplayInfo.Add(temp); }
                            break;
                        case "Gain":
                            // DisplayInfo.Clear();
                            foreach (byte gain in dt.IDACGain)
                            { DisplayInfo.Add(Convert.ToInt32(gain)); }
                            break;
                        case "Signal":
                            // DisplayInfo.Clear();
                            foreach (int temp in dt.Signal_Single)
                            { DisplayInfo.Add(temp); }

                            break;
                        case "SNR":
                            //int i = 0;
                            // DisplayInfo.Clear();
                            foreach (double SNRTemp in dt.SNR)
                            { DisplayInfo.Add(Convert.ToInt32(SNRTemp)); }
                            break;
                        default:
                            // DisplayInfo.Clear();
                            foreach (int temp in dt.RawCount_Single)
                            { DisplayInfo.Add(temp); }
                            break;
                    }

                    for (int i = DisplayInfo.Count; i < tpConfig.TMT.FW_INFO_NUM_COLS * tpConfig.TMT.FW_INFO_NUM_ROWS; i++)
                    {
                        DisplayInfo.Add(0);
                    }
                }
                catch
                {
                    //if (tabControl1.SelectedIndex == 2)
                    //{
                    //    MessageBox.Show("Please make sure you selected Signal Test item.\n" + ex.Message, "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    //}

                    //ComboBoxDisplayInfo.SelectedIndex = 0;
                }

                try
                {
                    for (int i = 0; i < tpConfig.TMT.FW_INFO_NUM_ROWS; i++)
                    {
                        for (int j = 0; j < tpConfig.TMT.FW_INFO_NUM_COLS; j++)
                        {
                            dataGridViewIntegrated[j, (tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i)].Value = DisplayInfo[j + i * tpConfig.TMT.FW_INFO_NUM_COLS];

                            int DisplayInfoTemp = DisplayInfo[j + i * tpConfig.TMT.FW_INFO_NUM_COLS];

                            switch (ComboBoxDisplayInfo.Text)
                            {
                                case "Noise":
                                    if (DisplayInfoTemp >= 13)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else if (DisplayInfoTemp > 6 && DisplayInfoTemp < 13)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.LightGreen;
                                    else
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                case "Signal":
                                    if (DisplayInfoTemp >= 40)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else if (DisplayInfoTemp > 20 && DisplayInfoTemp < 40)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.LightGreen;
                                    else
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                case "Local IDAC":
                                    if (DisplayInfoTemp >= 30 || DisplayInfoTemp <= 2)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                case "SNR":
                                    if (DisplayInfoTemp <= 5)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                case "RawCount":
                                    if (DisplayInfoTemp >= 160)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else if (DisplayInfoTemp > 120 && DisplayInfoTemp < 160)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.LightGreen;
                                    else if (DisplayInfoTemp < 80)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Red;
                                    else
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                default:
                                    if (DisplayInfoTemp == 0)
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Black;
                                    else
                                        dataGridViewIntegrated[j, tpConfig.TMT.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    //if (tabControl1.SelectedIndex == 2)
                    //{
                    //    MessageBox.Show("Please make sure you selected Signal Test item.\n" + ex.Message, "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    //}
                    //ComboBoxDisplayInfo.SelectedIndex = 0;
                }
                finally
                {

                    //return;
                }

            }
        }

        //private void SetGridDisplay_Noise(DUT dt)
        //{
        //    if (this.dataGridViewIntegrated.InvokeRequired)
        //    {
        //        SetGridDisplayCallback d = new SetGridDisplayCallback(SetGridDisplay_Noise);
        //        this.dataGridViewNoise.Invoke(d, new object[] { dt });
        //    }
        //    else
        //    {
        //        DataView dv = new DataView(dt);
        //        dataGridViewNoise.DataSource = dv;
        //    }
        //}

        //private void SetGridDisplay_IDAC(DUT dt)
        //{
        //    if (this.dataGridViewIntegrated.InvokeRequired)
        //    {
        //        SetGridDisplayCallback d = new SetGridDisplayCallback(SetGridDisplay_IDAC);
        //        this.dataGridViewIDAC.Invoke(d, new object[] { dt });
        //    }
        //    else
        //    {
        //        DataView dv = new DataView(dt);
        //        dataGridViewIDAC.DataSource = dv;
        //    }
        //}

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

                    debugModeToolStripMenuItem.Checked = false;
                    tabControlMain.Visible = false;
                }
                if (currentMode.ModeType == EngineeringMode)
                {
                    productionModeToolStripMenuItem.Checked = false;

                    debugModeToolStripMenuItem.Checked = false;
                    tabControlMain.Visible = true;
                }
                if (currentMode.ModeType == DebugMode)
                {
                    productionModeToolStripMenuItem.Checked = false;

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
            Manufacture.operatorID = "123";
            Manufacture.testStation = "TMT";
            Manufacture.testSite = "Others";

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
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            saveSysConfig();
            loadSysConfig();
            textBoxSN.Focus();
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
            textBoxSN.Focus();
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
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!test_running)    //&&LoopReading.Checked
                { SetGridDisplay(testFunction.dut); }
            }
            catch
            { }
        }

        //stop grid view
        private void StopButton_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedIndex = 0;
            signalTestFlag = false;
        }

        //stop position view
        private void StopDisplay_Click(object sender, EventArgs e)
        {
            testFunction.readPositionStop = false;
            tabControlMain.SelectedIndex = 0;
            if (textBoxSwitchL.BackColor != Color.Green || textBoxSwitchR.BackColor != Color.Green)
            {                
                testFunction.dut.ErrorCode = ErrorCode.ERROR_TACTILE_SWITCH;
            }
            else if (testFunction.currentTarget < testFunction.targetPoint.Count())
            {
                testFunction.dut.ErrorCode = ErrorCode.ERROR_FINGER_MOVE;
            }
        }

        //change resolution settings of trackpad.
        private void resolutionX_TextChanged(object sender, EventArgs e)
        {
            int positonPanelX = Convert.ToInt32(Convert.ToInt32(resolutionX.Text) / positionPanelScale);
            int positonPanelY = Convert.ToInt32(Convert.ToInt32(resolutionY.Text) / positionPanelScale);
            PositionPanel.Size = new Size(positonPanelX, positonPanelY);
        }

        //Serial Number scanned in.
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
            { return; }
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

                Manufacture.operatorID = "123";
                Manufacture.testStation = "TMT";
                Manufacture.testSite = "Others";

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
            OQCModeWithFingerTestToolStripMenuItem.Checked = false;
            debugModeToolStripMenuItem.Checked = false;
        }

        //engineer mode view
        
        //debug mode view
        private void debugModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMode = new Mode(DebugMode);
            this.ClientSize = currentMode.UISize;
            tabControlMain.Visible = true;

            productionModeToolStripMenuItem.Checked = false;
            OQCModeWithFingerTestToolStripMenuItem.Checked = false;
            debugModeToolStripMenuItem.Checked = true;
        }

        private void OQCModeWithFingerTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMode = new Mode(EngineeringMode);
            this.ClientSize = currentMode.UISize;
            tabControlMain.Visible = true;

            productionModeToolStripMenuItem.Checked = false;
            OQCModeWithFingerTestToolStripMenuItem.Checked = true;
            debugModeToolStripMenuItem.Checked = false;
        }

        //clear serial number
        private void textBoxSN_MouseDown(object sender, MouseEventArgs e)
        {
            textBoxSN.Text = "";
            textBoxMPN.Text = "";
            //textBoxSN.Focus();
        }

        //show the scroll message
        private void testFunction_TestMessageEvent(object sender, TestMessageEventArgs ea)
        {
            SetListBoxStatus(ea.Message);
        }

        //click the start button
        //check the hardware, serial number, online or offline, then start the test process
        private void buttonStart_Click(object sender, EventArgs e)
        {
            //timer.Start();
            timer.Restart();
            string partType = serialNumber.Substring(0, PARTNUMBER_LENGTH);
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
            if (serialNumberScanned)
            {
                //Check Part type in Production.ini
                try
                {

                    FormTrackpadConfig formTPConfig = new FormTrackpadConfig();
                    tpConfig = formTPConfig.read(serialNumber.Substring(0, PARTNUMBER_LENGTH));


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
                    resolutionX.Text = Convert.ToString(tpConfig.RESOLUTION_X);
                    resolutionY.Text = Convert.ToString(tpConfig.RESOLUTION_Y);
                    if (tpConfig.RESOLUTION_X > 600 || tpConfig.RESOLUTION_Y > 325)
                    {
                        positionPanelScale = 2;
                        while ((tpConfig.RESOLUTION_X / positionPanelScale) > 600 || (tpConfig.RESOLUTION_Y / positionPanelScale) > 325)
                        {
                            positionPanelScale = positionPanelScale * 2;
                        }
                    }
                    else
                    {
                        positionPanelScale = 0.5;
                        while ((tpConfig.RESOLUTION_X / positionPanelScale) * 2 < 600 && (tpConfig.RESOLUTION_Y / positionPanelScale) * 2 < 325)
                        {
                            positionPanelScale = positionPanelScale * 0.5;
                        }
                    }
                    int positonPanelX = Convert.ToInt32(Convert.ToInt32(resolutionX.Text) / positionPanelScale);
                    int positonPanelY = Convert.ToInt32(Convert.ToInt32(resolutionY.Text) / positionPanelScale);
                    PositionPanel.Size = new Size(positonPanelX, positonPanelY);
                    textBoxSwitchL.BackColor = Color.White;
                    textBoxSwitchR.BackColor = Color.White;
                    //testFunction.targetPoint = new List<Point>(new Point[] { new Point(60, 60), new Point(tpConfig.RESOLUTION_X - 60, tpConfig.RESOLUTION_Y - 60) });

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
                testFunction.targetPoint = new List<Point>(new Point[] { new Point(radius, radius), new Point(radius, tpConfig.RESOLUTION_Y - radius), new Point(tpConfig.RESOLUTION_X - radius, radius), new Point(tpConfig.RESOLUTION_X - radius, tpConfig.RESOLUTION_Y - radius), new Point(radius, radius) });
                //radius = Convert.ToInt32(radius / positionPanelScale);
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

                test_running = true;


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
                testFunction.Enter_TestMode();

                //clear text box message, show "testing".
                SetTxtBoxStatus(testFunction.dut.ErrorCode);

                // Erase IDAC
                if (tpConfigTestItems.EraseIDAC)
                {
                    testFunction.I2C_APA_EraseIDAC();
                    testFunction.BridgePowerOff();

                    //recalibrate the trackpad by powering on
                    testFunction.BridgePowerOn();
                    testFunction.Enter_TestMode();
                }
                //if (serialNumber.Substring(0, PARTNUMBER_LENGTH) == "10100400" || serialNumber.Substring(0, PARTNUMBER_LENGTH) == "10100500")
                //{
                //    testFunction.LEDLightTest();
                //}

                // FW check
                if (tpConfigTestItems.ReadFW)
                {
                    testFunction.I2C_Read_FW_Rev_Test();
                }

                //Signal test
                if (currentMode.SignalTest)
                {
                    signalTestFlag = true;

                    //initialize the baseline and IDAC value.
                    
                }
                testFunction.RawCount_Test();
                testFunction.IDAC_Test();
                while (currentMode.SignalTest && signalTestFlag)
                {
                    SetTabControlIndex(1);
                    testFunction.I2C_Signal_Test();
                    SetGridDisplay(testFunction.dut);
                }


                //Position test
                while (currentMode.PostionTest && testFunction.readPositionStop)
                {
                    SetTabControlIndex(2);
                    testFunction.ReadPosition();
                    PositionPanel.Invalidate();
                }

                // Rawcount test and Noise test
                //if (tpConfigTestItems.ReadRawCount && (!currentMode.SignalTest))
                //{
                //    testFunction.RawCount_Test();
                //    SetGridDisplay(testFunction.dut);
                //}

                //IDAC test
                //if (tpConfigTestItems.ReadIDAC && (!currentMode.SignalTest))
                //{
                //    testFunction.IDAC_Test();
                //}

                //Erase IDAC again after test.
                if (tpConfigTestItems.EraseIDAC)
                {
                    testFunction.I2C_APA_EraseIDAC();
                }

                testFunction.BridgePowerOff();
                test_running = false;   //stop test

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
                testFunction.dut.RawCountX.Clear();
                testFunction.dut.RawCountY.Clear();
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
                bool dataProcessing = true;
                while (test_running || dataProcessing || testFunction.datain_EraseIDAC_Ready ||
                    testFunction.datain_FW_Ready || testFunction.datain_APA_RawCount_Ready ||
                    testFunction.datain_RawCountX_Ready || testFunction.datain_RawCountY_Ready ||
                    testFunction.datain_APA_GlobalIDAC_Ready || testFunction.datain_APA_IDACGain_Ready ||
                    testFunction.datain_APA_LocalIDAC_Ready || testFunction.datain_other_IDAC_Ready)
                {
                    dataProcessing = false;
                    if (testFunction.datain_EraseIDAC_Ready)
                    {
                        testFunction.datain_EraseIDAC_Ready = false;
                        Task t = new Task(testFunction.Check_EraseIDAC);
                        t.Start();
                        // testFunction.Check_EraseIDAC();
                        dataProcessing = true;
                    }
                    if (testFunction.datain_FW_Ready)
                    {
                        testFunction.datain_FW_Ready = false;
                        Task t1 = new Task(testFunction.Check_FWVersion);
                        t1.Start();
                        // testFunction.Check_FWVersion();
                        dataProcessing = true;
                    }

                    if (testFunction.datain_APA_RawCount_Ready)
                    {
                        testFunction.datain_APA_RawCount_Ready = false;
                        Task t2 = new Task(testFunction.Cal_APA_RawCount_Noise);
                        t2.Start();
                        // testFunction.Cal_APA_RawCount_Noise();
                        dataProcessing = true;
                    }
                    if (testFunction.datain_RawCountX_Ready)
                    {
                        testFunction.datain_RawCountX_Ready = false;
                        Task t3 = new Task(testFunction.Cal_RawCountX_Noise);
                        t3.Start();
                        // testFunction.Cal_RawCountX_Noise();
                        dataProcessing = true;
                    }
                    if (testFunction.datain_RawCountY_Ready)
                    {
                        testFunction.datain_RawCountY_Ready = false;
                        Task t4 = new Task(testFunction.Cal_RawCountY_Noise);
                        t4.Start();
                        // testFunction.Cal_RawCountY_Noise();
                        dataProcessing = true;
                    }

                    if (testFunction.datain_APA_GlobalIDAC_Ready)
                    {
                        testFunction.datain_APA_GlobalIDAC_Ready = false;
                        Task t5 = new Task(testFunction.Check_APA_GlobalIDAC);
                        t5.Start();
                        // testFunction.Check_APA_GlobalIDAC();
                        dataProcessing = true;
                    }
                    if (testFunction.datain_APA_IDACGain_Ready)
                    {
                        testFunction.datain_APA_IDACGain_Ready = false;
                        Task t6 = new Task(testFunction.Check_APA_IDACGain);
                        t6.Start();
                        //testFunction.Check_APA_IDACGain();
                        dataProcessing = true;
                    }
                    if (testFunction.datain_APA_LocalIDAC_Ready)
                    {
                        testFunction.datain_APA_LocalIDAC_Ready = false;
                        Task t7 = new Task(testFunction.Check_APA_LocalIDAC_TotalIDAC);
                        t7.Start();
                        //  testFunction.Check_APA_LocalIDAC_TotalIDAC();
                        dataProcessing = true;
                    }
                    if (testFunction.datain_other_IDAC_Ready)
                    {
                        testFunction.datain_other_IDAC_Ready = false;
                        Task t8 = new Task(testFunction.Check_Other_IDAC);
                        t8.Start();
                        //testFunction.Check_Other_IDAC();
                        dataProcessing = true;
                    }
                    System.Threading.Thread.Sleep(10);

                }
                if (currentMode.ModeType == DebugMode)
                {
                    testFunction.I2C_ST_MTG_Signal_Check();
                }

                if (tpConfigTestItems.ReadFW)
                {
                    if (!testFunction.FWVersionItem)
                    {
                        testFunction.dut.ErrorCode = ErrorCode.EEROR_SYSTEM_ERROR;
                    }
                }
                if (tpConfigTestItems.ReadIDAC)
                {
                    if (!testFunction.IDACItem)
                    {
                        testFunction.dut.ErrorCode = ErrorCode.EEROR_SYSTEM_ERROR;
                    }
                }
                if (tpConfigTestItems.ReadRawCount)
                {
                    if (!testFunction.RawCountItem)
                    {
                        testFunction.dut.ErrorCode = ErrorCode.EEROR_SYSTEM_ERROR;
                    }
                }

                if (testFunction.dut.ErrorCode != ErrorCode.EEROR_SYSTEM_ERROR)
                {
                    //write into xml test log.xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    testFunction.OutputData();

                    //upload test result to damn sigmatron shop floor system
                    if (Manufacture.testSite == "CM 1")
                    {
                        testFunction.UploadResult();
                    }
                }


                SetTxtBoxStatus(testFunction.dut.ErrorCode); //show pass fail

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
            SolidBrush pointB = new SolidBrush(Color.Blue);
            Pen penB = new Pen(pointB, 1);
            SolidBrush pointC = new SolidBrush(Color.Red);
            Pen penC = new Pen(pointC, 1);
            int finger1X;
            int finger1Y;
            int radiusTemp = Convert.ToInt32(radius / positionPanelScale);

            if (testFunction.fingerX < 3000 && testFunction.fingerY < 3000 && testFunction.fingerX > 0 && testFunction.fingerY > 0)
            {
                try
                {
                    finger1X = Convert.ToInt32(testFunction.fingerX / positionPanelScale);
                    finger1Y = Convert.ToInt32(testFunction.fingerY / positionPanelScale);
                    //finger1Y = (Convert.ToInt32(resolutionY.Text) - testFunction.fingerY) * 2;
                    CoordinateX.Text = Convert.ToString(testFunction.fingerX);
                    CoordinateY.Text = Convert.ToString(testFunction.fingerY);
                }
                catch (Exception)
                {
                    finger1X = 0;
                    finger1Y = 0;

                }

                try
                {
                    Point[] fingerpositions = new Point[testFunction.fingerPoint.Count];
                    Point fingerPosition = new Point();

                    fingerPosition.X = finger1X;
                    fingerPosition.Y = finger1Y;
                    testFunction.fingerPoint.Add(fingerPosition);
                    testFunction.dut.Finger_Positions.Add(new Point(testFunction.fingerX, testFunction.fingerY));

                    int jj = 0;
                    foreach (Point fingertemp in fingerpositions)
                    {
                        fingerpositions[jj] = testFunction.fingerPoint[jj];
                        jj++;
                    }

                    e.Graphics.FillRectangle(pointB, finger1X, finger1Y, 1, 1);
                    for (int i = 1; i <= 10; i++)
                    {
                        e.Graphics.FillRectangle(pointB, finger1X + i, finger1Y, 1, 1);
                        e.Graphics.FillRectangle(pointB, finger1X - i, finger1Y, 1, 1);
                        e.Graphics.FillRectangle(pointB, finger1X, finger1Y + i, 1, 1);
                        e.Graphics.FillRectangle(pointB, finger1X, finger1Y - i, 1, 1);
                    }
                    if (fingerpositions.Length > 1)
                    {
                        foreach (Point temp in fingerpositions)
                        {
                            int tempX = temp.X;
                            int tempY = temp.Y;
                            for (int i = 1; i <= 2; i++)
                            {
                                e.Graphics.FillRectangle(pointB, tempX + i, tempY, 1, 1);
                                e.Graphics.FillRectangle(pointB, tempX - i, tempY, 1, 1);
                                e.Graphics.FillRectangle(pointB, tempX, tempY + i, 1, 1);
                                e.Graphics.FillRectangle(pointB, tempX, tempY - i, 1, 1);
                            }
                        }
                        
                        //e.Graphics.DrawLines(penB, fingerpositions);

                    }

                    int finger2X = Convert.ToInt32(testFunction.targetPoint[testFunction.currentTarget].X / positionPanelScale);
                    int finger2Y = Convert.ToInt32(testFunction.targetPoint[testFunction.currentTarget].Y / positionPanelScale);
                    if (Math.Pow((finger2X - finger1X) * positionPanelScale, 2) + Math.Pow((finger2Y - finger1Y) * positionPanelScale, 2) < Math.Pow(radius, 2))
                    {
                        fingerCount++;
                        if (fingerCount > 5)
                        {
                            if (testFunction.currentTarget >= (testFunction.targetPoint.Count() - 1) && textBoxSwitchL.BackColor == Color.Green && textBoxSwitchR.BackColor == Color.Green)
                            {

                                testFunction.readPositionStop = false;
                                tabControlMain.SelectedIndex = 0;

                            }
                            else if (testFunction.currentTarget < testFunction.targetPoint.Count() - 1)
                            {
                                testFunction.currentTarget++;
                            }

                            finger2X = Convert.ToInt32(testFunction.targetPoint[testFunction.currentTarget].X / positionPanelScale);
                            finger2Y = Convert.ToInt32(testFunction.targetPoint[testFunction.currentTarget].Y / positionPanelScale);
                            fingerCount = 0;
                        }

                    }
                    e.Graphics.DrawEllipse(penC, finger2X - radiusTemp, finger2Y - radiusTemp, radiusTemp, radiusTemp);

                }
                catch
                {

                }
            }
            else
            {
                int jj = 0;
                Point[] fingerpositions = new Point[testFunction.fingerPoint.Count];
                foreach (Point fingertemp in fingerpositions)
                {
                    fingerpositions[jj] = testFunction.fingerPoint[jj];
                    jj++;
                }
                if (fingerpositions.Length > 1)
                {
                    //e.Graphics.DrawLines(penB, fingerpositions);
                    foreach (Point temp in fingerpositions)
                    {
                        int tempX = temp.X;
                        int tempY = temp.Y;
                        for (int i = 1; i <= 2; i++)
                        {
                            e.Graphics.FillRectangle(pointB, tempX + i, tempY, 1, 1);
                            e.Graphics.FillRectangle(pointB, tempX - i, tempY, 1, 1);
                            e.Graphics.FillRectangle(pointB, tempX, tempY + i, 1, 1);
                            e.Graphics.FillRectangle(pointB, tempX, tempY - i, 1, 1);
                        }
                    }
                }
                    
                int finger2X = Convert.ToInt32(testFunction.targetPoint[testFunction.currentTarget].X / positionPanelScale);
                int finger2Y = Convert.ToInt32(testFunction.targetPoint[testFunction.currentTarget].Y / positionPanelScale);
                e.Graphics.DrawEllipse(penC, finger2X - radiusTemp, finger2Y - radiusTemp, radiusTemp, radiusTemp);

            }



            //draw Swtich status
            try
            {
                if (tpConfig.TRACKPAD_TACTILESWITCH == 1)
                {
                    if ((testFunction.dut.LeftButtonStatus & 0x04) == 0x04)
                    {
                        textBoxSwitchL.BackColor = Color.Green;
                    }
                    else if ((testFunction.dut.LeftButtonStatus & 0x01) == 0x01)
                    {
                        textBoxSwitchR.BackColor = Color.Green;
                    }
                }
                else if (tpConfig.TRACKPAD_TACTILESWITCH == 2)
                {
                    if ((testFunction.dut.LeftButtonStatus & 0x04) == 0x04)
                    {
                        textBoxSwitchL.BackColor = Color.Green;
                        textBoxSwitchR.BackColor = Color.Green;
                    }
                }
                

            }

            catch
            { }

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            testFunction.fingerPoint.Clear();
            testFunction.dut.Finger_Positions.Clear();
            testFunction.currentTarget = 0;
            textBoxSwitchL.BackColor = Color.White;
            textBoxSwitchR.BackColor = Color.White;
        }       

    }
}
