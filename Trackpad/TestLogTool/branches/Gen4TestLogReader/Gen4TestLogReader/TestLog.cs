using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Threading;
using System.Data; 

namespace Gen4TestLogReader
{
    class TestLog
    {
        public List<FileInfo> TestLogs;
        public List<DUT> duts;
        public DirectoryInfo SourceDir;
        public DirectoryInfo TargetDir;
        public DirectoryInfo AchievdDir;

        public TestLog(string Sourcedirectory, string Targetdirectory, string Achievddirectory)
        {
            TestLogs = new List<FileInfo>();
            duts = new List<DUT>();

            SourceDir = new DirectoryInfo(Sourcedirectory);
            TargetDir = new DirectoryInfo(Targetdirectory);
            if(!TargetDir.Exists)
            { TargetDir.Create(); }

            AchievdDir = new DirectoryInfo(Achievddirectory);
            if (!AchievdDir.Exists)
            { AchievdDir.Create(); }

        }

        public void write_XML_report()
        {
            this.search_file();
            this.paser_file();

            if (duts.Count > 0)
            {
                foreach (DUT dut in duts)
                {

                    if (!this.TargetDir.Exists)     //check if folder exists
                    { this.TargetDir.Create(); }    //if not, create the folder


                    string filePath = this.TargetDir + dut.SERIAL_NUMBER;
                    FileInfo fI = new FileInfo(filePath + ".xml");
                    int i = 1;
                    while (fI.Exists)
                    {
                        string pathTmp = filePath + "(" + i.ToString() + ")";
                        fI = new FileInfo(pathTmp + ".xml");
                        i++;
                    }

                    FileStream fs = new FileStream(fI.FullName, FileMode.CreateNew);
                    XmlSerializer xser = new XmlSerializer(typeof(DUT));
                    xser.Serialize(fs, dut);

                    fs.Close();

                }
            }
        }


        public void write_summary_report()
        {
            //prepare the data table to put data into 
            DataTable dataTable = new DataTable("trackpad");

            dataTable.Columns.Add(new DataColumn("SerialNumber", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Statistic", typeof(string)));

            for (int i = 1; i <= 500; i++)
            {
                dataTable.Columns.Add(new DataColumn("D" + i.ToString(), typeof(string)));
            }

            //search raw data
            this.search_file();
            this.paser_file();

            //insert dut data into datatable
            if (duts.Count > 0)
            {
                foreach (DUT dut in duts)
                {
                    db_insert(dataTable, dut.SERIAL_NUMBER, "CHIP_ID", dut.CHIP_ID);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "DATE_TIME", dut.DATE_TIME);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "TEST_STATION", dut.TEST_STATION);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "TEST_RESULT", dut.TEST_RESULT);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "ERROR_CODE", dut.ERROR_CODE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "VCOM_VOLTAGE", dut.VCOM_VOLTAGE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "VAUX_VOLTAGE", dut.VAUX_VOLTAGE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "ICOM_CURRENT", dut.ICOM_CURRENT);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "IAUX_CURRENT", dut.IAUX_CURRENT);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "FW_VERSION", dut.FW_VERSION);

                    db_insert(dataTable, dut.SERIAL_NUMBER, "GLOBAL_IDAC", dut.GLOBAL_IDAC);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "ELAPSED_TIME", dut.ELAPSED_TIME);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "LOCAL_IDAC", dut.LOCAL_IDAC);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "NOISE", dut.NOISE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "RAW_DATA", dut.RAW_DATA);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "BASELINE", dut.BASELINE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "SIGNAL", dut.SIGNAL);

                    db_insert(dataTable, dut.SERIAL_NUMBER, "SELFCAP_NOISE", dut.SELFCAP_NOISE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "SELFCAP_RAWDATE", dut.SELFCAP_RAWDATE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "SELFCAP_BASELINE", dut.SELFCAP_BASELINE);
                    db_insert(dataTable, dut.SERIAL_NUMBER, "SELFCAP_SIGNAL", dut.SELFCAP_SIGNAL);



                }

                //export2xml.export2XML(dataTable, TargetDir + @"summary.xml");

                StringBuilder sb = new StringBuilder();

                string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dataTable.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                    ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(TargetDir + @"summary.csv", sb.ToString());


            }

        }

        private void db_insert(DataTable dt, string SerialNumber, string Statistic, List<string> data)
        {
            DataRow newRow = dt.NewRow();

            newRow["SerialNumber"] = SerialNumber;
            newRow["Statistic"] = Statistic;

            int i = 0;
            foreach (string singleData in data)
            {
                newRow[2 + i] = singleData;
                i++;
            }

            dt.Rows.Add(newRow);
        }

        private void db_insert(DataTable dt, string SerialNumber, string Statistic, string data)
        {
            DataRow newRow = dt.NewRow();

            newRow["SerialNumber"] = SerialNumber;
            newRow["Statistic"] = Statistic;

            newRow["D1"] = data;

            dt.Rows.Add(newRow);
        }



        private void paser_file()
        {
            duts.Clear();
            
            if (TestLogs.Count <= 0)
            {
                Thread.Sleep(2000);
                return;
            }
            foreach (FileInfo log in TestLogs)
            {
                StreamReader sr = new StreamReader(log.OpenRead());

                while (!sr.EndOfStream)
                {
                    DUT dut = new DUT();

                    //read ", .header"+ Product Data + ", .end"
                    string ProductContent="";
                    for (int i = 0; i < 5; i++)
                    {
                        ProductContent += sr.ReadLine();
                    }

                    Match m = Gen4Pattern.ProductionDataPattern.Match(ProductContent);
                    if (m.Success)
                    {
                        //Console.WriteLine(m.Groups.Count.ToString());
                        dut.DATE_TIME = m.Groups["TestDate"] + " " + m.Groups["TestTime"];
                        dut.SW_VERSION = m.Groups["SWVersion"].ToString();
                        dut.OPERATOR = m.Groups["Operator"].ToString();
                        dut.TEST_STATION = m.Groups["TestStaion"].ToString();
                        dut.TEST_FACILITY = m.Groups["TestFacility"].ToString();
                        dut.CONFIG_FILE = m.Groups["ConfigFile"].ToString();
                        dut.EXECUTION_MODE = m.Groups["TestMode"].ToString();
                        dut.SENSOR_ROWS = m.Groups["RowNumber"].ToString();
                        dut.SENSOR_COLUMNS = m.Groups["ColumnNumber"].ToString();
                        dut.TEST_RESULT = m.Groups["TestResult"].ToString();
                    }
                    else
                    {
                        Console.WriteLine("Fail to read product content");
                        break;
                    }

                    //read ", .header"+ Product Data + ", .end"
                    string EngineeringContent = "";
                    EngineeringContent+=sr.ReadLine();
                    if (EngineeringContent != ", .engineering data")
                    {
                        break;
                    }
                    else
                    {
                        string result = "";
                        do
                        {
                            result = sr.ReadLine();

                            EngineeringContent += result;

                        } while (result != ", .end");

                        //Console.WriteLine(EngineeringContent);
                    }

                    //Match the first line of Engineering Data
                    Match m1 = Gen4Pattern.EngineeringLine1Pattern.Match(EngineeringContent);
                    if (m1.Success)
                    {
                        dut.SERIAL_NUMBER = m1.Groups["SerialNumber"].ToString();
                        dut.CHIP_ID = m1.Groups["ChipID"].ToString();
                        dut.ERROR_CODE = m1.Groups["ErrorCode"].ToString();
                        if (dut.ERROR_CODE == "0x00")
                        {
                            dut.ERROR_CODE = "0";
                        }
                        dut.ERROR_MESSAGE = m1.Groups["ErrorMessage"].ToString();
                    }
                    else
                    {
                        Console.WriteLine("Fail to read engineering content");
                        break;
                    }

                    //Match the VCOM Voltage
                    Match m2 = Gen4Pattern.VCOMVoltagePattern.Match(EngineeringContent);
                    if (m2.Success)
                    {
                        dut.VCOM_VOLTAGE = m2.Groups["VCOMVoltage"].ToString();
                    }

                    //Match the VAUXVoltage
                    Match m3 = Gen4Pattern.VAUXVoltagePattern.Match(EngineeringContent);
                    if (m3.Success)
                    {
                        dut.VAUX_VOLTAGE = m3.Groups["VAUXVoltage"].ToString();
                    }

                    //Match the ICOMCurrent
                    Match m4 = Gen4Pattern.ICOMCurrentPattern.Match(EngineeringContent);
                    if (m4.Success)
                    {
                        dut.ICOM_CURRENT = m4.Groups["ICOMCurrent"].ToString();
                    }

                    //Match the IAUXCurrent
                    Match m5 = Gen4Pattern.IAUXCurrentPattern.Match(EngineeringContent);
                    if (m5.Success)
                    {
                        dut.IAUX_CURRENT = m5.Groups["IAUXCurrent"].ToString();
                    }

                    //Match the ElapsedTime
                    Match m6 = Gen4Pattern.ElapsedTimePattern.Match(EngineeringContent);
                    if (m6.Success)
                    {
                        dut.ELAPSED_TIME = m6.Groups["ElapsedTime"].ToString();
                    }

                    //Match the Global_IDAC
                    Match m7 = Gen4Pattern.GolbalIDACPattern.Match(EngineeringContent);
                    if (m7.Success)
                    {
                        dut.GLOBAL_IDAC = m7.Groups["Global_IDAC"].ToString();
                    }

                    //Match the FW_Version
                    Match m8 = Gen4Pattern.FWVersionPattern.Match(EngineeringContent);
                    if (m8.Success)
                    {
                        dut.FW_VERSION = m8.Groups["FW_Version"].ToString();
                    }

                    //Match the FW_Revision
                    Match m9 = Gen4Pattern.FWRevisionPattern.Match(EngineeringContent);
                    if (m9.Success)
                    {
                        dut.FW_REVISION = m9.Groups["FW_Revision"].ToString();
                    }

                    //Match the Noise
                    Match m10 = Gen4Pattern.NoisePattern.Match(EngineeringContent);
                    while (m10.Success)
                    {
                        string temp = m10.Groups["Noise"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.NOISE.Add(result.Trim());
                        }
                        m10 = m10.NextMatch();
                    }

                    //Match the RawData
                    Match m11 = Gen4Pattern.RawDataPattern.Match(EngineeringContent);
                    while (m11.Success)
                    {
                        string temp = m11.Groups["RawData"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.RAW_DATA.Add(result.Trim());
                        }
                        m11 = m11.NextMatch();
                    }

                    //Match the Local iDAC Data
                    Match m12 = Gen4Pattern.LocalIDACPattern.Match(EngineeringContent);
                    while (m12.Success)
                    {
                        string temp = m12.Groups["Local_IDAC"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.LOCAL_IDAC.Add(result.Trim());
                        }
                        m12=m12.NextMatch();
                    }

                    //Match the Baseline
                    Match m13 = Gen4Pattern.BaselinePattern.Match(EngineeringContent);
                    while (m13.Success)
                    {
                        string temp = m13.Groups["Baseline"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.BASELINE.Add(result.Trim());
                        }
                        m13 = m13.NextMatch();
                    }

                    //Match the SelfCapNoise
                    Match m14 = Gen4Pattern.SelfCapNoisePattern.Match(EngineeringContent);
                    while (m14.Success)
                    {
                        string temp = m14.Groups["SelfCapNoise"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.SELFCAP_NOISE.Add(result.Trim());
                        }
                        m14 = m14.NextMatch();
                    }

                    //Match the SelfCapRawData
                    Match m15 = Gen4Pattern.SelfCapRawDataPattern.Match(EngineeringContent);
                    while (m15.Success)
                    {
                        string temp = m15.Groups["SelfCapRawData"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.SELFCAP_RAWDATE.Add(result.Trim());
                        }
                        m15 = m15.NextMatch();
                    }

                    //Match the SelfCapBaseLine
                    Match m16 = Gen4Pattern.SelfCapBaseLinePattern.Match(EngineeringContent);
                    while (m16.Success)
                    {
                        string temp = m16.Groups["SelfCapBaseLine"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.SELFCAP_BASELINE.Add(result.Trim());
                        }
                        m16 = m16.NextMatch();
                    }

                    //Match the SelfCapSignal
                    Match m17 = Gen4Pattern.SelfCapSignalPattern.Match(EngineeringContent);
                    while (m17.Success)
                    {
                        string temp = m17.Groups["SelfCapSignal"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.SELFCAP_SIGNAL.Add(result.Trim());
                        }
                        m17 = m17.NextMatch();
                    }

                    //Match the Signal
                    Match m18 = Gen4Pattern.SignalPattern.Match(EngineeringContent);
                    while (m18.Success)
                    {
                        string temp = m18.Groups["Signal"].ToString();
                        string[] results = temp.Split(new Char[] { ',' });

                        foreach (string result in results)
                        {
                            dut.SIGNAL.Add(result.Trim());
                        }
                        m18 = m18.NextMatch();
                    }

                    duts.Add(dut);

                    //debug info
                    Console.WriteLine("SerialNumber : " + dut.SERIAL_NUMBER + " Test Result: " + dut.TEST_RESULT + " Error Code: " + dut.ERROR_CODE);



                    
                    //string printLocalIDAC = "";
                    //foreach (string localIDAC in dut.LOCAL_IDAC)
                    //{
                    //    printLocalIDAC += localIDAC + ";";
                    //}
                    //Console.WriteLine(printLocalIDAC);

                    //string printBaseline = "";
                    //foreach (string baseline in dut.BASELINE)
                    //{
                    //    printBaseline += baseline;
                    //}
                    //Console.WriteLine("BaseLine: " + printBaseline);

                    //string printRawData = "";
                    //foreach (string rawdata in dut.RAW_DATA)
                    //{
                    //    printRawData += rawdata+";";

                    //}
                    //Console.WriteLine("Raw Data: " + printRawData);

                    //string printSelfCAPNoise = "";
                    //foreach (string selfCapNoise in dut.SELFCAP_NOISE)
                    //{
                    //    printSelfCAPNoise += selfCapNoise + ";";
                    //}
                    //Console.WriteLine("Self CAP Noise: " + printSelfCAPNoise);

                }

                //move the file
                sr.Close();

                FileInfo fI = new FileInfo(this.AchievdDir + @"\" + log.Name);
                int j = 1;
                while (fI.Exists)
                {
                    fI = new FileInfo(this.AchievdDir + @"\" + "Retest_(" + j.ToString() + ")" + log.Name);
                    j++;
                }

                File.Move(log.FullName, fI.FullName);

            }
            
            
        }




        private void search_file()
        {
            TestLogs.Clear();
            
            Stack<DirectoryInfo> pending = new Stack<DirectoryInfo>();
            pending.Push(SourceDir);
            while (pending.Count != 0)
            {
                DirectoryInfo path = pending.Pop();
                FileInfo[] files = null;
                try
                {
                    files = path.GetFiles();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                if (files != null && files.Length != 0)
                {
                    foreach (FileInfo file in files)
                    {
                        //Console.WriteLine("file: " + file.Name); 

                        if (this.isVaild(file))
                        {
                            //ThreadPool.QueueUserWorkItem(new WaitCallback(FileToDB), file);
                            TestLogs.Add(file);
                        }
                    }
                }
                try
                {
                    DirectoryInfo[] directorys = path.GetDirectories();
                    foreach (DirectoryInfo subdir in directorys) pending.Push(subdir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private bool isVaild(FileInfo file)
        {
            if (file.Extension == ".csv" || file.Extension == ".CSV")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
