using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cypress_Link_SQL2005;
using CypressSemiconductor.ChinaManufacturingTest;
using CypressSemiconductor.ChinaManufacturingTest.TrackPad;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace GoogleGlass
{
    public partial class GlassTestOQC : Form
    {
        #region call MTK

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);   

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string IpClassName, string IpWindowsName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        private static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", EntryPoint = "GetWindow")]
        public static extern IntPtr GetWindow(IntPtr hwnd, IntPtr wCmd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder ClassName, int nMaxCount);

        const int WM_GETTEXT = 0X000D;
        const int WM_SETTEXT = 0x000C;
        const int WM_CLICK = 0x00F5;
        const int WM_KEYDOWN = 0x0100;
        const int VK_F5 = 0x0074;
        IntPtr GW_CHILD = new IntPtr(5);
        IntPtr GW_HWNDNEXT = new IntPtr(2);
        public bool findEdit = false;
        public IntPtr EdithWnd = new IntPtr(0);
        public IntPtr StatusWnd = new IntPtr(0);
        public int retval = 0; //增加一个返回值用来判断操作是否成功 
        public IntPtr ParenthWnd = new IntPtr(0);
        #endregion

        #region components test

        private string serialNumber = "";
        private bool serialNumberScanned = false;

        private double LED_Volt = 2.0;
        private int LED_Channal = 1;
        private int LED_Delay = 200;
        private double ALS_Volt = 2.8;
        private int ALS_Channal = 2;
        private int ALS_Delay = 500;
        private double RES_Volt = 4.0;
        private int RES_Channal = 3;
        private int RES_Delay = 200;

        private string partType;
        private SFCS m_SFCS;
        private DUT dut;
        private Agilent agilentDevice;
        private bool instrument_initiated = false;
        USB_I2C bridge;

        delegate void SetListBoxCallback(string text);
        delegate void SetTxtBoxCallback(int errorCode);

        #endregion
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
        private void SetTxtBoxStatus(int errorCode)
        {
            if (this.textBoxStatus.InvokeRequired)
            {
                SetTxtBoxCallback d0 = new SetTxtBoxCallback(SetTxtBoxStatus);
                this.Invoke(d0, new object[] { errorCode });
            }
            else
            {

                if (errorCode != 0)
                {
                    textBoxStatus.Clear();
                    textBoxStatus.Text = "Fail:" + errorCode.ToString() + "--" + ErrorCode.ReportErrorCodes(errorCode);
                    textBoxStatus.BackColor = Color.Red;
                }
                else
                {
                    textBoxStatus.Clear();
                    textBoxStatus.Text = "PASS";
                    textBoxStatus.BackColor = Color.Green;
                }

            }
        }

        public GlassTestOQC()
        {
            InitializeComponent();


        }

        private void textBoxSN_TextChanged(object sender, EventArgs e)
        {

            if (textBoxSN.Text.Length == Config.SERIAL_NUMBER_LENGTH)
            {
                dut = new DUT();
                serialNumber = textBoxSN.Text;
                partType = serialNumber.Substring(0, Config.PARTNUMBER_LENGTH);
                string identifier = serialNumber.Substring(Config.PARTNUMBER_LENGTH, (Config.SERIAL_NUMBER_LENGTH - Config.PARTNUMBER_LENGTH));

                serialNumber = partType + identifier;

                textBoxSN.Text = identifier;
                textBoxMPN.Text = partType;

                serialNumberScanned = true;
                dut.SerailNumber = serialNumber;
                buttonStart.Enabled = true;
                //StartTest();
                //buttonStart.Focus();
            }


        }

        private void buttonInit_Click(object sender, EventArgs e)
        {
            try
            {
                agilentDevice.InitializeU2722A(Config.U2722_ADDRESS);
                string[] ports = bridge.GetPorts();
                if (ports.Length < 1)
                {
                    MessageBox.Show("No USB_I2C bridge found", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int status = bridge.OpenPort(ports[0].ToString());
                    if (status != 0)
                    {
                        MessageBox.Show("Error in open port:", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.error("Error in open port: " + bridge.LastError);
                        bridge.PowerOff();
                        bridge.ClosePort();
                        //bridgeReady = false;
                    }
                    else
                    {
                        instrument_initiated = true;
                        //buttonInit.BackColor = Color.Green;

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Init_Bridge()
        {
            try
            {
                //agilentDevice.InitializeU2722A(Config.U2722_ADDRESS);
                string[] ports = bridge.GetPorts();
                if (ports.Length < 1)
                {
                    MessageBox.Show("No USB_I2C bridge found", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    int status = bridge.OpenPort(ports[0].ToString());
                    if (status != 0)
                    {
                        MessageBox.Show("Error in open port:", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.error("Error in open port: " + bridge.LastError);
                        bridge.PowerOff();
                        bridge.ClosePort();
                        //bridgeReady = false;
                    }
                    else
                    {
                        //instrument_initiated = true;
                        //buttonInit.BackColor = Color.Green;

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void GoogleGlass_Load(object sender, EventArgs e)
        {
            Config.read();
            agilentDevice = new Agilent();
            m_SFCS = new SFCS();
            buttonStart.Enabled = false;
            bridge = new USB_I2C();
            bridge.DeviceAddress = Config.I2C_ADDRESS;
            Manufacture.operatorID = "google";
            Manufacture.testStation = "ALS";
            Manufacture.testSite = "Sigma";
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            listBoxStatus.Items.Clear();
            textBoxStatus.Text = "";
            textBoxStatus.BackColor = Color.White;
            //SetTxtBoxStatus(dut.ErrorCode);
            textBoxSN.Enabled = false;
            if (Config.ONLINE)
            {
                if (CheckPemission())
                {
                    testDUT();
                    UploadResult();
                }
            }
            else
            {
                testDUT();
            }
            SetTxtBoxStatus(dut.ErrorCode);
            textBoxSN.Clear();
            textBoxSN.Enabled = true;
            OutputData();
            buttonStart.Enabled = false;
            textBoxSN.Focus();
        }

        public void testDUT()
        {
            if (instrument_initiated == false)
            {
                //MessageBox.Show("Instruments not initiated", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
                if (partType == "11500400")
                {
                    Init_Bridge();
                    try
                    {
                        agilentDevice.InitializeU2722A(Config.U2722_ADDRESS);
                        instrument_initiated = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dut.ErrorCode = ErrorCode.EEROR_SYSTEM_ERROR;
                        return;
                    }                    
                    
                }
                if (partType == "11500500")
                {
                    try
                    {
                        agilentDevice.InitializeU2722A(Config.U2722_ADDRESS);
                        instrument_initiated = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dut.ErrorCode = ErrorCode.EEROR_SYSTEM_ERROR;
                        return;
                    }
                    
                }
            }
            if (partType == "11500400")
            {
                SetListBoxStatus("Optics POD Test");
                //LED test
                // “Testing/reading LED current consumption:”
                SetListBoxStatus("Testing/reading LED current consumption:");
                agilentDevice.SetChannelVoltage(LED_Volt, LED_Channal);
                agilentDevice.ActivateChannelOutput(LED_Channal);
                System.Threading.Thread.Sleep(LED_Delay);

                List<double> CurrentLED = new List<double>();
                for (byte j = 0; j < 5; j++)
                {
                    CurrentLED.Add(agilentDevice.MeasureChannelCurrent(LED_Channal) * 1000);
                    System.Threading.Thread.Sleep(LED_Delay);
                    SetListBoxStatus(CurrentLED[j].ToString());
                }
                agilentDevice.De_ActivateChannelOutput(LED_Channal);
                dut.LED = CurrentLED.Average();
                if (dut.LED < Config.LED_MIN)
                {
                    dut.ErrorCode = ErrorCode.ERROR_LED_LOW;
                }
                else if (dut.LED > Config.LED_MAX)
                {
                    dut.ErrorCode = ErrorCode.ERROR_LED_HIGH;
                }



                //ALS test

                try
                { BridgePowerOff(); }  //try to stop the bridge if it's in use.
                catch
                {
                    dut.ErrorCode = ErrorCode.ERROR_NO_TRACKPAD_IN_SLOT;
                }
                try
                {
                    SetListBoxStatus("Reading ALS");
                    BridgePowerOn();
                    System.Threading.Thread.Sleep(ALS_Delay);
                    bridge.Write(0xEC);
                    //agilentDevice.De_ActivateChannelOutput(ALS_Channal);
                    System.Threading.Thread.Sleep(ALS_Delay);
                    byte[] dataOn = bridge.Read(2);
                    dut.ALS_light = Convert.ToInt32(Convert.ToInt32(dataOn[0]) * 256 + Convert.ToInt32(dataOn[1]));
                    SetListBoxStatus(dut.ALS_light.ToString());

                    //SetListBoxStatus("Testing/reading ALS output with LED illumination");
                    //agilentDevice.SetChannelVoltage(ALS_Volt, ALS_Channal);
                    //agilentDevice.ActivateChannelOutput(ALS_Channal);
                    //System.Threading.Thread.Sleep(ALS_Delay * 2);
                    //byte[] dataOn = bridge.Read(2);
                    //agilentDevice.De_ActivateChannelOutput(ALS_Channal);
                    //dut.ALS_light = Convert.ToInt32(Convert.ToInt32(dataOn[1]) * 256 + Convert.ToInt32(dataOn[0]));
                    //SetListBoxStatus(dut.ALS_light.ToString());
                    //dut.ALS=

                    if (dataOn.Count() == 2)
                    {
                        
                        if (dut.ALS_light < Config.ALS_LIGHT)
                        {
                            dut.ErrorCode = ErrorCode.ERROR_ALS_LOW;
                        }
                    }
                    else
                    {
                        dut.ErrorCode = ErrorCode.ERROR_NO_ALS;
                    }


                }
                catch
                {
                    dut.ErrorCode = ErrorCode.ERROR_NO_TRACKPAD_IN_SLOT;
                }
                finally
                {
                    BridgePowerOff();
                }
            }
            if (partType == "11500500")
            {

                try
                {
                    SetListBoxStatus("Main POD Test");
                    SetListBoxStatus("Testing/reading Thermal Resistor values(KOhm)");
                    agilentDevice.SetChannelVoltage(RES_Volt, RES_Channal);
                    agilentDevice.ActivateChannelOutput(RES_Channal);
                    System.Threading.Thread.Sleep(RES_Delay);

                    List<double> CurrentR = new List<double>();
                    for (byte j = 0; j < 5; j++)
                    {
                        CurrentR.Add(agilentDevice.MeasureChannelCurrent(RES_Channal) * 1000);
                        System.Threading.Thread.Sleep(RES_Delay);
                        SetListBoxStatus(Convert.ToString(RES_Volt / CurrentR[j] - 10));//use 12 in IQC fixture in US Jabil
                    }
                    agilentDevice.De_ActivateChannelOutput(RES_Channal);
                    dut.Resistor = Convert.ToInt32(RES_Volt / CurrentR.Average() - 10);//use 12 in IQC fixture in US Jabil
                    if (dut.Resistor < Config.RESISTOR_MIN)
                    {
                        dut.ErrorCode = ErrorCode.ERROR_RES_LOW;
                    }
                    else if (dut.Resistor > Config.RESISTOR_MAX)
                    {
                        dut.ErrorCode = ErrorCode.ERROR_RES_HIGH;
                    }
                }
                catch (Exception ex)
                {
                    dut.ErrorCode = ErrorCode.ERROR_NO_TRACKPAD_IN_SLOT;
                }
                System.Threading.Thread.Sleep(200);
            }
            if (checkBoxCallMTK.Checked)
            {
                CallMTK();
            }
            

        }

        public void CallMTK()
        {
            //int retval = 0; //增加一个返回值用来判断操作是否成功 

            //下面的这些参数都可以用Spy++查到 
            string lpszParentClass = "WindowsForms10.Window.8.app.0.378734a"; //整个窗口的类名 
            //string lpszParentWindow = "Cypress Touchscreen Test Executive - 11500500OQC"; //窗口标题 
            string lpszClass = "WindowsForms10.EDIT.app.0.378734a"; //需要查找的子窗口的类名，也就是输入框             
            string text = "";
            StringBuilder title = new StringBuilder(255);
            StringBuilder className = new StringBuilder(255);
            StringBuilder StatusString = new StringBuilder(255);

            //IntPtr ParenthWnd = new IntPtr(0);
            //IntPtr EdithWnd = new IntPtr(0);
            //IntPtr StatusWnd = new IntPtr(0);

            //查到窗体，得到整个窗体 

            if (retval == 0)
            {
                ParenthWnd = FindWindow(lpszParentClass, null);
            }
            

            try
            {
                while (!ParenthWnd.Equals(IntPtr.Zero) && (retval == 0))
                {
                    GetWindowText(ParenthWnd, title, title.Capacity);
                    string APItitle = title.ToString();
                    if (APItitle.Contains("Cypress Touchscreen"))
                    {
                        retval = retval + 1;
                    }
                    else
                    {
                        ParenthWnd = FindWindow(lpszParentClass, null);
                    }
                    System.Threading.Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Find MTK", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            #region find edit and status

            try
            {
                if (retval > 0)
                {
                    //得到Serial number这个子窗体，并设置其内容 

                    //bool findEdit = false;
                    bool findStatus = false;
                    IntPtr EdithWnd1 = new IntPtr(0);
                    IntPtr EdithWnd2 = new IntPtr(0);
                    IntPtr EdithWnd3 = new IntPtr(0);
                    IntPtr EdithWnd4 = new IntPtr(0);

                    EdithWnd1 = GetWindow(ParenthWnd, GW_CHILD);
                    while (!EdithWnd1.Equals(IntPtr.Zero) && !findEdit)
                    {
                        GetClassName(EdithWnd1, className, 255);
                        if (className.ToString() == "WindowsForms10.SysTabControl32.app.0.378734a")
                        {
                            EdithWnd2 = GetWindow(EdithWnd1, GW_CHILD);
                            while (!EdithWnd2.Equals(IntPtr.Zero) && !findEdit)
                            {
                                GetClassName(EdithWnd2, className, 255);
                                if (className.ToString() == "WindowsForms10.Window.8.app.0.378734a")
                                {
                                    EdithWnd3 = GetWindow(EdithWnd2, GW_CHILD);
                                    while (!EdithWnd3.Equals(IntPtr.Zero) && !findEdit)
                                    {
                                        GetClassName(EdithWnd3, className, 255);
                                        if (className.ToString() == "WindowsForms10.Window.8.app.0.378734a")
                                        {
                                            EdithWnd4 = GetWindow(EdithWnd3, GW_CHILD);
                                            while (!EdithWnd4.Equals(IntPtr.Zero) && !findEdit)
                                            {
                                                GetClassName(EdithWnd4, className, 255);
                                                if (className.ToString() == "WindowsForms10.EDIT.app.0.378734a")
                                                {
                                                    EdithWnd = GetWindow(EdithWnd4, GW_HWNDNEXT);
                                                    GetClassName(EdithWnd, className, 255);
                                                    if (className.ToString() == "WindowsForms10.EDIT.app.0.378734a")
                                                    {
                                                        findEdit = true;
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Can not find MTK");
                                                    }

                                                }
                                                else
                                                {
                                                    if (className.ToString() == "WindowsForms10.STATIC.app.0.378734a" && findStatus == false)
                                                    {
                                                        StatusWnd = EdithWnd4;
                                                        findStatus = true;
                                                    }
                                                    EdithWnd4 = GetWindow(EdithWnd4, GW_HWNDNEXT);
                                                }
                                                System.Threading.Thread.Sleep(20);
                                            }
                                            EdithWnd3 = GetWindow(EdithWnd3, GW_HWNDNEXT);
                                        }
                                        else
                                        {
                                            EdithWnd3 = GetWindow(EdithWnd3, GW_HWNDNEXT);
                                        }
                                        System.Threading.Thread.Sleep(20);
                                    }
                                    EdithWnd2 = GetWindow(EdithWnd2, GW_HWNDNEXT);
                                }
                                else
                                {
                                    EdithWnd2 = GetWindow(EdithWnd2, GW_HWNDNEXT);
                                }
                                System.Threading.Thread.Sleep(20);
                            }
                            EdithWnd1 = GetWindow(EdithWnd1, GW_HWNDNEXT);
                        }
                        else
                        {
                            EdithWnd1 = GetWindow(EdithWnd1, GW_HWNDNEXT);
                        }
                        System.Threading.Thread.Sleep(20);

                    }

                    if (!findEdit)
                    {
                        MessageBox.Show("Can not find SN input");
                    }

                }
                else
                {
                    MessageBox.Show("could not find input", "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dut.ErrorCode = ErrorCode.ERROR_MTK_FAIL;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Find input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            # endregion


            try
            {
                if (!EdithWnd.Equals(IntPtr.Zero))
                {
                    SetForegroundWindow(ParenthWnd);
                    System.Threading.Thread.Sleep(200);
                    //调用SendMessage方法设置其内容 
                    SendMessage(EdithWnd, WM_SETTEXT, (IntPtr)0, serialNumber);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Send SN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            try
            {
                if (!ParenthWnd.Equals(IntPtr.Zero))
                {
                    //SetForegroundWindow(ParenthWnd);
                    PostMessage(EdithWnd, WM_KEYDOWN, VK_F5, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: F5", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            System.Threading.Thread.Sleep(5000);

            bool testFinish = false;
            int i = 0;
            while (!testFinish && (i < 100) && (retval > 0))
            {
                GetWindowText(StatusWnd, StatusString, 255);
                string temp = StatusString.ToString();
                if (temp.Contains("0x"))
                {
                    testFinish = true;
                    dut.ErrorCode = ErrorCode.ERROR_MTK_FAIL;
                }
                else if (temp.Contains("PASS"))
                {
                    testFinish = true;
                }
                else
                {
                    i++;
                }
                System.Threading.Thread.Sleep(1000);
            }
            SetForegroundWindow(this.Handle);
            System.Threading.Thread.Sleep(100);
            //GetWindowText(StatusWnd, StatusString, 255);
            //string ss = StatusString.ToString() + "vv";


        }


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

                if (!xmlReport.WriteSingleData("ALS_dark_Value", dut.ALS_dark.ToString()))
                { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSingleData("ALS_light_Value", dut.ALS_light.ToString()))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSingleData("LED_Value", dut.LED.ToString()))
            { Log.error(xmlReport.LastError); }
            if (!xmlReport.WriteSingleData("Res_Value", dut.Resistor.ToString()))
            { Log.error(xmlReport.LastError); }

            //Close XML Report
            if (!xmlReport.CloseReport())
            { Log.error(xmlReport.LastError); }
        }

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

        public void BridgePowerOff()
        {
            int Status = bridge.PowerOff();
            if (Status != 0)
            {
                MessageBox.Show("Error in power off:", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void BridgePowerOn()
        {
            int Status1 = bridge.SetPower("3.3");
            int Status2 = bridge.PowerOn();
            if ((Status1 != 0) || (Status2 != 0))
            {
                MessageBox.Show("Error in power on:", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                //Set I2C Speed to 400KHz//
                int Status3 = bridge.SetI2CSpeed(2);
                if (Status3 != 0)
                {
                    MessageBox.Show("Error in Set I2C Speed:", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                }
            }
        }

        private void GoogleGlass_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (instrument_initiated)
            {
                try
                {
                    //BridgePowerOff();
                    agilentDevice.De_ActivateChannelOutput(LED_Channal);
                    agilentDevice.De_ActivateChannelOutput(ALS_Channal);
                    agilentDevice.De_ActivateChannelOutput(RES_Channal);
                }
                catch (Exception ex)
                {

                }
            }

        }



    }
}
