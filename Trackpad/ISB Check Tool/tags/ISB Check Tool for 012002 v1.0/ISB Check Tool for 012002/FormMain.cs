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

        //#########################################################################//
        
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Win32.AllocConsole();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            textBoxSN.Focus();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                mm_fucntion.CurrentMeasumentEvent -= new MutliMeterFunction.MultiMeterEventHandler(mm_fucntion_CurrentMeasumentEvent);
                mm_fucntion.ResultEvent -= new MutliMeterFunction.MMResultEventHandler(mm_fucntion_ResultEvent);

                tp_function.TPStatusEvent -= new TrackpadFunction.TPEventHandler(tp_function_TPStatusEvent);
                tp_function.STEvent -= new TrackpadFunction.StateEventHandler(tp_function_STEvent);

                mm_thread.exit();
                tp_thread.exit();
            }
            catch
            { }
        }



        private void tp_function_TPStatusEvent(object sender, TPEventArgs e)
        {
            //Trackpad Power On
            if (e.Status == 0)
            {
                
            }

            //Sleep1
            if (e.Status == 1)
            {
                System.Threading.Thread.Sleep(500);

                List<STATE.states> MM_STATES = new List<STATE.states>();
                
                for (int i = 0; i < Config.MEAS_TIMES; i++)
                {
                    MM_STATES.Add(STATE.states.MM_ReadCurr);
                }
                MM_STATES.Add(STATE.states.MM_CalcSleep1Curr);

                mm_thread.en_queue(MM_STATES);
            }

            //Deep Sleep
            if (e.Status == 2)
            {
                System.Threading.Thread.Sleep(500);

                List<STATE.states> MM_STATES = new List<STATE.states>();
                
                for (int i = 0; i < Config.MEAS_TIMES; i++)
                {
                    MM_STATES.Add(STATE.states.MM_ReadCurr);
                }
                MM_STATES.Add(STATE.states.MM_CalcDeepSleepCurr);

                mm_thread.en_queue(MM_STATES);
            }

            //Trackpad Power Off
            if (e.Status == 4)
            {
                //mm_thread.en_queue(STATE.states.idle);
            }
        }

        private void tp_function_STEvent(object sender, StateMachineEventArgs e)
        {
            if (e.CurrentState == STATE.states.initialize)
            {
                if (e.PassOrFail)
                {
                    tp_ready=true;
                    if (mm_ready)
                    {
                        EnableButtonConnect(false);
                        EnableButtonSleep(true);
                        EnableButtonDeepSleep(true);
                        
                    }
                }
            }

            if (e.CurrentState == STATE.states.exit)
            {
                try
                {
                    mm_thread.exit();

                    EnableButtonConnect(true);
                    EnableButtonSleep(false);
                    EnableButtonDeepSleep(false);

                    serialNumberScanned = false;
                    SetTxtBoxSN("");
                }
                catch { }



                tp_function = null;
                tp_thread = null;

            }
        }

        private void mm_fucntion_STEvent(object sender, StateMachineEventArgs e)
        {
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

            if (e.CurrentState == STATE.states.exit)
            {
                try
                {
                    tp_thread.exit();

                    EnableButtonConnect(true);
                    EnableButtonSleep(false);
                    EnableButtonDeepSleep(false);

                    serialNumberScanned = false;
                    SetTxtBoxSN("");
                }
                catch { }



                mm_fucntion = null;
                mm_thread = null;
            }
        }



        private void mm_fucntion_CurrentMeasumentEvent(object sender, MultiMeterEventArgs e)
        {
            SetListBoxStatus(false, "Current: " + e.Current.ToString() + "uA");
        }

        private void mm_fucntion_ResultEvent(object sender, MMResultEventArgs e)
        {
            if (e.Status == 1)
            {
                sleep1_test_done = true;
                dut.IDDValue_Sleep = e.Average;

                SetListBoxStatus(false, "Sleep1 Current:" + e.Average.ToString() + "uA");
                if (e.Result)
                {
                    SetTextBoxSleepResult("PASS", Color.Green);
                    //Log.info("[" + serialNumber + "]" + "Sleep1 PASS:\t\t\t" + e.Average.ToString() + "uA");
                    
                }
                else
                {
                    SetTextBoxSleepResult("FAIL", Color.Red);
                    //Log.info("[" + serialNumber + "]" + "Sleep1 FAIL:\t\t\t" + e.Average.ToString() + "uA");
                    dut.ErrorCode += ErrorCode.ERROR_IDD_HIGH;

                    //No need to test the deep sleep.
                    deepsleep_test_done = true;
                }
            }

            if (e.Status == 2)
            {
                deepsleep_test_done = true;
                dut.IDDValue_DeepSleep = e.Average;

                SetListBoxStatus(false, "Deep Sleep Current:" + e.Average.ToString() + "uA");
                if (e.Result)
                {
                    SetTextBoxDeepSleepResult("PASS", Color.Green);
                    //Log.info("[" + serialNumber + "]" + "Deep Sleep PASS:\t\t" + e.Average.ToString() + "uA");
                    
                }
                else
                {
                    SetTextBoxDeepSleepResult("FAIL", Color.Red);
                    //Log.info("[" + serialNumber + "]" + "Deep Sleep FAIL:\t\t" + e.Average.ToString() + "uA");
                    dut.ErrorCode += ErrorCode.ERROR_IDD_LOW;

                    //No need to test the sleep1.
                    sleep1_test_done = true;
                }
            }

            tp_thread.en_queue(STATE.states.TP_PowerOff);

            if (sleep1_test_done && deepsleep_test_done)
            {
                serialNumberScanned = false;
                SetTxtBoxSN("");

                WriteTestReport();

                dut = null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////

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


        ///////////////////////////////////////////////////////////////////////////////

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
                tp_function.TPStatusEvent += new TrackpadFunction.TPEventHandler(tp_function_TPStatusEvent);
                tp_function.STEvent += new TrackpadFunction.StateEventHandler(tp_function_STEvent);


                //Instant MultiMeter Fucntion
                mm_fucntion = new MutliMeterFunction();
                mm_thread = new StatesThread(mm_fucntion);

                //Register MultiMeter Event
                mm_fucntion.CurrentMeasumentEvent += new MutliMeterFunction.MultiMeterEventHandler(mm_fucntion_CurrentMeasumentEvent);
                mm_fucntion.ResultEvent += new MutliMeterFunction.MMResultEventHandler(mm_fucntion_ResultEvent);
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
                serialNumber = textBoxSN.Text;
                string partType = serialNumber.Substring(0, Config.PARTNUMBER_LENGTH);
                string identifier = serialNumber.Substring(Config.PARTNUMBER_LENGTH, (Config.SERIAL_NUMBER_LENGTH - Config.PARTNUMBER_LENGTH));

                serialNumber = partType + identifier;

                if (validateSerialNumber(serialNumber))
                {
                    textBoxSN.Text = identifier;
                    textBoxMPN.Text = partType;

                    serialNumberScanned = true;

                    dut = new DUT();
                    dut.SerailNumber = serialNumber;

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

        private bool validateSerialNumber(string sn)
        {

            Regex r = new Regex("^[01]([0][0-9]|[1][0-9])([0][0-9][0-9])[0][01][0][1-9]([0][1-9]|[1][0-2])([0-4][0-9]|[5][0-3])[0-4][a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9]([1-8]$|$)");

            return r.IsMatch(sn);
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

    }


    public class Win32
    {
        //Allocates a new console for current process.
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        //Closes the console.
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();
    }
}
