using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CypressSemiconductor.ChinaManufacturingTest.Work_Loop;
using CypressSemiconductor.ChinaManufacturingTest.TrackPad;

namespace CypressSemiconductor.ChinaManufacturingTest.ISB_Check_Tool_for_012002
{
    public partial class FormMain : Form
    {
        private MutliMeterFunction mm_fucntion;
        private StatesThread mm_thread;

        private TrackpadFunction tp_function;
        private StatesThread tp_thread;

        private DUT dut;

        private bool tp_ready = false;
        private bool mm_ready = false;

        private bool sleep1_test_done = false;
        private bool deepsleep_test_done = false;

        private string serialNumber = "";
        private bool serialNumberScanned = false;

        private SFCS m_SFCS;

        //#########################################################################//

        #region Main Form

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            ConsoleWidget.ConsoleStream FConsole = new ConsoleWidget.ConsoleStream(richTextBoxDebug);
            Console.SetOut(FConsole);

            //Win32.AllocConsole();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            //Trace.Indent();

            m_SFCS = new SFCS();

            Config.read();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            textBoxSN.Focus();

            //Console.WriteLine("Test");
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                //tp_function.STEvent -= new TrackpadFunction.StateEventHandler(tp_function_STEvent);
                mm_fucntion.STEvent -= new MutliMeterFunction.StateEventHandler(mm_fucntion_STEvent);

                mm_thread.exit();
                tp_thread.exit();
            }
            catch
            { }
        }

        #endregion

        //#########################################################################//

        #region State Change

        /// <summary>
        /// The StateMachine EVENT of class TP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tp_function_STEvent(object sender, StateMachineEventArgs e)
        {
            //trackpad thread init
            if (e.CurrentState == STATE.states.initialize)
            {
                if (e.PassOrFail)
                {
                    tp_ready = true;
                    if (mm_ready)
                    {
                        EnableButtonConnect(false);
                        EnableButtonSleep(true);
                        EnableButtonDeepSleep(true);

                    }
                }
            }

            //Trackpad thread exit
            if (e.CurrentState == STATE.states.exit)
            {

                if (mm_ready)
                {
                    try
                    {
                        EnableButtonConnect(true);
                        EnableButtonSleep(false);
                        EnableButtonDeepSleep(false);

                        serialNumberScanned = false;
                        SetTxtBoxSN("");

                        mm_thread.exit();
                    }
                    catch { }
                }


                tp_ready = false;
                tp_function = null;
                tp_thread = null;

            }

            //Trackpad Power On
            if (e.CurrentState == STATE.states.TP_PowerOn)
            {

            }

            //Sleep1
            if (e.CurrentState == STATE.states.TP_SendSleep1Command)
            {
                System.Threading.Thread.Sleep(Config.Delay.EnterSleepMode);

                List<STATE.states> MM_STATES = new List<STATE.states>();

                for (int i = 0; i < Config.MEAS_TIMES; i++)
                {
                    MM_STATES.Add(STATE.states.MM_ReadCurr);
                }
                MM_STATES.Add(STATE.states.MM_CalcSleep1Curr);

                mm_thread.en_queue(MM_STATES);
            }

            //Deep Sleep
            if (e.CurrentState == STATE.states.TP_SendDeepSleepCommand)
            {
                System.Threading.Thread.Sleep(Config.Delay.EnterSleepMode);

                List<STATE.states> MM_STATES = new List<STATE.states>();

                for (int i = 0; i < Config.MEAS_TIMES; i++)
                {
                    MM_STATES.Add(STATE.states.MM_ReadCurr);
                }
                MM_STATES.Add(STATE.states.MM_CalcDeepSleepCurr);

                mm_thread.en_queue(MM_STATES);
            }

            //Trackpad Power Off
            if (e.CurrentState == STATE.states.TP_PowerOff)
            {
                //mm_thread.en_queue(STATE.states.idle);
            }

        }

        /// <summary>
        /// The StateMachine EVENT of class MultiMeter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mm_fucntion_STEvent(object sender, StateMachineEventArgs e)
        {
            
            //Multi-Meter Init
            if (e.CurrentState == STATE.states.initialize)
            {
                if (e.PassOrFail)
                {
                    mm_ready = true;
                    if (tp_ready)
                    {

                        EnableButtonConnect(false);
                        EnableButtonSleep(true);
                        EnableButtonDeepSleep(true);
                    }
                }
            }

            //Multi-Meter Exit
            if (e.CurrentState == STATE.states.exit)
            {

                if (tp_ready)
                {
                    try
                    {
                        EnableButtonConnect(true);
                        EnableButtonSleep(false);
                        EnableButtonDeepSleep(false);

                        serialNumberScanned = false;
                        SetTxtBoxSN("");

                        tp_thread.exit();
                    }
                    catch { }
                }
                

                mm_fucntion = null;
                mm_thread = null;
                mm_ready = false;
            }

            //sleep1 current
            if (e.CurrentState == STATE.states.MM_CalcSleep1Curr)
            {
                sleep1_test_done = true;
                dut.IDDValue_Sleep = Convert.ToDouble(e.Message);

                SetListBoxStatus(false, "Sleep1 Current:" + e.Message + "uA");
                if (e.PassOrFail)
                {
                    SetTextBoxSleepResult("PASS", Color.Green);
                    //Log.info("[" + serialNumber + "]" + "Sleep1 PASS:\t\t\t" + e.Average.ToString() + "uA");

                }
                else
                {
                    SetTextBoxSleepResult("FAIL", Color.Red);
                    //Log.info("[" + serialNumber + "]" + "Sleep1 FAIL:\t\t\t" + e.Average.ToString() + "uA");

                    if (dut.ErrorCode == 0)
                    {
                        dut.ErrorCode += ErrorCode.ERROR_IDD_HIGH;
                    }

                    //No need to test the deep sleep.
                    deepsleep_test_done = true;
                }

                tp_thread.en_queue(STATE.states.TP_PowerOff);

                if (sleep1_test_done && deepsleep_test_done)
                {
                    serialNumberScanned = false;
                    SetTxtBoxSN("");

                    WriteTestReport();

                    if (Config.ONLINE)
                    {
                        UploadResult();
                    }

                    dut = null;
                }
            }

            //deep sleep current
            if (e.CurrentState==STATE.states.MM_CalcDeepSleepCurr)
            {
                deepsleep_test_done = true;
                dut.IDDValue_DeepSleep = Convert.ToDouble(e.Message);

                SetListBoxStatus(false, "Deep Sleep Current:" + e.Message + "uA");
                if (e.PassOrFail)
                {
                    SetTextBoxDeepSleepResult("PASS", Color.Green);
                    //Log.info("[" + serialNumber + "]" + "Deep Sleep PASS:\t\t" + e.Average.ToString() + "uA");

                }
                else
                {
                    SetTextBoxDeepSleepResult("FAIL", Color.Red);
                    //Log.info("[" + serialNumber + "]" + "Deep Sleep FAIL:\t\t" + e.Average.ToString() + "uA");

                    if (dut.ErrorCode == 0)
                    {
                        dut.ErrorCode = ErrorCode.ERROR_IDD_LOW;
                    }

                    //No need to test the sleep1.
                    sleep1_test_done = true;
                }


                tp_thread.en_queue(STATE.states.TP_PowerOff);

                if (sleep1_test_done && deepsleep_test_done)
                {
                    serialNumberScanned = false;
                    SetTxtBoxSN("");

                    WriteTestReport();

                    if (Config.ONLINE)
                    {
                        UploadResult();
                    }

                    dut = null;
                }
            }

            if (e.CurrentState == STATE.states.MM_ReadCurr)
            {
                SetListBoxStatus(false, "Current: " + e.Message + "uA");
            }

        }

        #endregion

        //#########################################################################//

        #region UI CallBack

        delegate void SetListBoxCallback(bool clr, string text);
        private void SetListBoxStatus(bool clr, string text)
        {
            if (this.listBoxStatus.InvokeRequired)
            {
                SetListBoxCallback d1 = new SetListBoxCallback(SetListBoxStatus);
                this.Invoke(d1, new object[] { clr, text });
            }
            else
            {
                if (clr)
                {
                    listBoxStatus.Items.Clear();
                }
                else
                {
                    listBoxStatus.Items.Add(text);
                    int countNum = listBoxStatus.Items.Count;
                    listBoxStatus.SetSelected(countNum - 1, true);
                }
            }
        }


        delegate void EnableButtonCallback(bool enable);
        private void EnableButtonConnect(bool enable)
        {
            if (this.buttonConnect.InvokeRequired)
            {
                EnableButtonCallback d1 = new EnableButtonCallback(EnableButtonConnect);
                this.Invoke(d1, new object[] { enable });
            }
            else
            {
                this.buttonConnect.Enabled = enable;
            }
        }

        private void EnableButtonSleep(bool enable)
        {
            if (this.buttonSleep.InvokeRequired)
            {
                EnableButtonCallback d1 = new EnableButtonCallback(EnableButtonSleep);
                this.Invoke(d1, new object[] { enable });
            }
            else
            {
                this.buttonSleep.Enabled = enable;
            }
        }

        private void EnableButtonDeepSleep(bool enable)
        {
            if (this.buttonDeepSleep.InvokeRequired)
            {
                EnableButtonCallback d1 = new EnableButtonCallback(EnableButtonDeepSleep);
                this.Invoke(d1, new object[] { enable });
            }
            else
            {
                this.buttonDeepSleep.Enabled = enable;
            }
        }

        delegate void SetTextBoxCallback(string text, Color color);
        private void SetTextBoxSleepResult(string text, Color color)
        {
            if (this.textBoxSleepResult.InvokeRequired)
            {
                SetTextBoxCallback d1 = new SetTextBoxCallback(SetTextBoxSleepResult);
                this.Invoke(d1, new object[] { text, color });
            }
            else
            {
                this.textBoxSleepResult.Text = text;
                this.textBoxSleepResult.BackColor = color;
            }
        }

        private void SetTextBoxDeepSleepResult(string text, Color color)
        {
            if (this.textBoxDeepSleepResult.InvokeRequired)
            {
                SetTextBoxCallback d1 = new SetTextBoxCallback(SetTextBoxDeepSleepResult);
                this.Invoke(d1, new object[] { text, color });
            }
            else
            {
                this.textBoxDeepSleepResult.Text = text;
                this.textBoxDeepSleepResult.BackColor = color;
            }
        }

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

        #endregion

        //#########################################################################//

        #region UI Function

        private void buttonSleep_Click(object sender, EventArgs e)
        {

            SetListBoxStatus(true, "");
            SetListBoxStatus(false, "Sleep1 IDD Testing");
            SetTextBoxSleepResult("", Color.White);

            sleep1_test_done = false;

            if (!serialNumberScanned)
            {
                MessageBox.Show("Please input serial number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<STATE.states> TP_STATES = new List<STATE.states>();

            TP_STATES.Add(STATE.states.TP_PowerOn);
            TP_STATES.Add(STATE.states.TP_SendSleep1Command);

            tp_thread.en_queue(TP_STATES);

        }

        private void buttonDeepSleep_Click(object sender, EventArgs e)
        {

            SetListBoxStatus(true, "");
            SetListBoxStatus(false, "Deep Sleep IDD Testing");
            SetTextBoxDeepSleepResult("", Color.White);

            deepsleep_test_done = false;

            if (!serialNumberScanned)
            {
                MessageBox.Show("Please input serial number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<STATE.states> TP_STATES = new List<STATE.states>();

            TP_STATES.Add(STATE.states.TP_PowerOn);
            TP_STATES.Add(STATE.states.TP_SendDeepSleepCommand);

            tp_thread.en_queue(TP_STATES);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {

                //Instant Trackpad Function
                tp_function = new TrackpadFunction();
                tp_thread = new StatesThread(tp_function);

                //Register Trackpad Event
                 tp_function.STEvent += new TrackpadFunction.StateEventHandler(tp_function_STEvent);


                //Instant MultiMeter Fucntion
                mm_fucntion = new MutliMeterFunction();
                mm_thread = new StatesThread(mm_fucntion);

                //Register MultiMeter Event
                mm_fucntion.STEvent += new MutliMeterFunction.StateEventHandler(mm_fucntion_STEvent);

                tp_thread.run();
                mm_thread.run();

            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);

                tp_function = null;
                tp_thread = null;


                mm_fucntion = null;
                mm_thread = null;
            }


        }

        private void textBoxSerialNumber_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSN.Text.Length == Config.SERIAL_NUMBER_LENGTH)
            {
                dut = new DUT();
                
                serialNumber = textBoxSN.Text;
                string partType = serialNumber.Substring(0, Config.PARTNUMBER_LENGTH);
                string identifier = serialNumber.Substring(Config.PARTNUMBER_LENGTH, (Config.SERIAL_NUMBER_LENGTH - Config.PARTNUMBER_LENGTH));

                serialNumber = partType + identifier;

                textBoxSN.Text = identifier;
                textBoxMPN.Text = partType;

                serialNumberScanned = true;
                dut.SerailNumber = serialNumber;

                if (validateSerialNumber(serialNumber))
                {
                    SetTextBoxSleepResult("", Color.White);
                    SetTextBoxDeepSleepResult("", Color.White);
                    
                    sleep1_test_done = false;
                    deepsleep_test_done = false;
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

        #endregion

        //#########################################################################//

        private bool validateSerialNumber(string sn)
        {

            Regex r = new Regex("^[01]([0][0-9]|[1][0-9])([0][0-9][0-9])[0][01][0][1-9]([0][1-9]|[1][0-5])([0-4][0-9]|[5][0-3])[0-4][a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]([1-8]$|$)");

            if (!r.IsMatch(sn))
            {
                return false;
            }
            
            if(Config.ONLINE)
            {
                if (!CheckPemission())
                {
                    return false;
                }
            }

            return true;
        }

        private void WriteTestReport()
        {
            XmlReport xmlReport = new XmlReport();

            //Open XML Report
            xmlReport.OpenReport(dut.SerailNumber, System.Windows.Forms.Application.StartupPath + @"\test results\", "IDD StandBy");


            xmlReport.WriteSingleData("Serial_Number", dut.SerailNumber);

            xmlReport.WriteSingleData("Test_Station", "IDD StandBy");

            xmlReport.WriteSingleData("Error_Code", string.Format("{0:X} ", dut.ErrorCode));


            string time = System.DateTime.Now.ToString("u", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            xmlReport.WriteSingleData("Test_Time", time);


            xmlReport.WriteSingleData("IDD_Sleep1_Value", dut.IDDValue_Sleep.ToString());
            xmlReport.WriteSingleData("IDD_Deep_Sleep_Value", dut.IDDValue_DeepSleep.ToString());

            //Close XML Report
            xmlReport.CloseReport();


        }


        /// <summary>
        /// SFCS Check Permission
        /// </summary>
        /// <param name="WorkerID"></param>
        /// <returns></returns>
        private bool CheckPemission()
        {
            try
            {
                bool connected = m_SFCS.SFCS_Connect();
                if (!connected)
                {
                    MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string Model = dut.SerailNumber.Substring(0, 8);
                string WorkerID = "001";
                string Station = "TMT";

                bool permission = m_SFCS.SFCS_PermissonCheck(dut.SerailNumber, Model, WorkerID, Station);
                if (!permission)
                {
                    if (dut.ErrorCode == 0)
                    { dut.ErrorCode = ErrorCode.ERROR_SFCS_NOPERMISSION; }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Check Permission Error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// SFCS Upload Result
        /// </summary>
        /// <returns></returns>
        private bool UploadResult()
        {
            try
            {
                bool connected = m_SFCS.SFCS_Connect();
                if (!connected)
                {
                    MessageBox.Show(m_SFCS.connect_error, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string Model = dut.SerailNumber.Substring(0, 8);
                string WorkerID = "001";
                string Station = "TMT";
                string TestLog = "";

                string errorCode = string.Format("{0:X} ", dut.ErrorCode);

                string TestResult = "Fail";
                if (dut.ErrorCode == 0x0)
                { TestResult = "Pass"; }

                bool upload = m_SFCS.SFCS_UploadTestResult(dut.SerailNumber, Model, WorkerID, errorCode, TestLog, TestResult, Station);
                if (!upload)
                {
                    if (dut.ErrorCode == 0)
                    { dut.ErrorCode = ErrorCode.ERROR_SFCS_UPLOADDATA; }
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Upload test result error: " + ex.Message);
                return false;

            }
        }

    }
}
