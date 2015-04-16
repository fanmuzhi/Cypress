using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CypressSemiconductor.ChinaManufacturingTest;
using System.IO;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace My_Trackpad_Database
{
    public partial class mainForm : Form
    {
        private string logsFilePath;
        private ConsoleWidget.ConsoleStream FConsole;

        private DateTime startTime;
        private DateTime stopTime;

        private Queue<DUT_Str> QueueDUTs;

        private bool readFinished;

        private int fileNumer;
        
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            //set output console to rich text box
            try
            {
                FConsole = new ConsoleWidget.ConsoleStream(richTextBoxDebug);
                Console.SetOut(FConsole);

                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                Trace.AutoFlush = true;
                //Trace.Indent();

                QueueDUTs = new Queue<DUT_Str>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FConsole.Close();
            }
            catch { }
        }

        #region Insert Data into mySQL

        private async Task FileToDatabase()
        {
            readFinished = false;
            startTime = DateTime.Now;

            fileNumer = 0;

            //get directory information
            DirectoryInfo directoryInfo = new DirectoryInfo(logsFilePath);
            //if the directory is not exists, return.
            if (!directoryInfo.Exists)
            { return; }

            Task t1=new Task((o) => ParseFile(o), directoryInfo);
            t1.Start();

            Task t2 = new Task(() => DataToDB());
            t2.Start();

            await Task.WhenAll(new Task[] { t1, t2 });

            Trace.WriteLine("Totoal " + fileNumer.ToString() + " files.");
            stopTime = DateTime.Now;
            string elapsedTime = (stopTime - startTime).TotalMilliseconds.ToString();
            Trace.WriteLine("Elapsed Time: " + elapsedTime + " ms");
            
        }

        private void ParseFile(object directoryInfo)
        {
            //ThreadPool.SetMaxThreads(30, 30);
            
            //FileIOPermission fileIOPerm1 = new FileIOPermission(FileIOPermissionAccess.AllAccess, di.FullName);
            //fileIOPerm1.AllFiles = FileIOPermissionAccess.Read;

            Stack<DirectoryInfo> pending = new Stack<DirectoryInfo>();
            pending.Push((DirectoryInfo)directoryInfo);

            Trace.WriteLine("working....");

            List<Task> tasks = new List<Task>();

            while (pending.Count != 0)
            {
                DirectoryInfo path = pending.Pop();
                FileInfo[] files = null;
                try
                {
                    files = path.GetFiles();

                    fileNumer += files.Length;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Get FIles Error: " + ex.Message);
                }
                if (files != null && files.Length != 0)
                {

                    //add one task for one folder, not for one file.

                    Task t = new Task(() =>
                        {
                            foreach (FileInfo file in files)
                            {
                                //Trace.WriteLine("file: " + file.Name); 

                                if (validateFile(file))
                                {

                                    //parse xml
                                    XmlReadLogs xmlread = new XmlReadLogs();
                                    DUT_Str dut = xmlread.read(file.FullName);
                                    if (dut.SerailNumber != "")
                                    {
                                        lock (QueueDUTs)
                                        {
                                            QueueDUTs.Enqueue(dut);
                                        }
                                    }
                                }

                                else
                                {
                                    //Trace.WriteLine("Invalid file: " + file.FullName);
                                }
                            }

                        });

                    t.Start();

                    tasks.Add(t);

                }
                try
                {
                    DirectoryInfo[] directorys = path.GetDirectories();
                    foreach (DirectoryInfo subdir in directorys) pending.Push(subdir);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Get Directories Error: " + ex.Message);
                }
            }

            Task.WaitAll(tasks.ToArray());

            readFinished = true;

            //Trace.WriteLine("Search Finished");

        }


        private void DataToDB()
        {
            
            List<Task> tasks = new List<Task>();

            while (!readFinished || (QueueDUTs.Count > 0))
            {
                if (QueueDUTs.Count != 0)
                {
                    DUT_Str dut = QueueDUTs.Dequeue();

                    Task t = new Task(() =>
                        {
                            //connect to db
                            MySQLUtil db = new MySQLUtil();
                            db.connect();

                            if (db.insert_into_table(dut))
                            {
                                string str = String.Format("ADD: {0,-20} {1, -6} {2, -10} {3, -5} {4, -20}", dut.SerailNumber, dut.ErrorCode, dut.PartType, dut.IDDValue, dut.TestTime);
                                Trace.WriteLine(str);

                                //File.Move(logsFilePath.ToString() + @"\" + file.Name, directoryInfoAchieved.ToString() + @"\" + file.Name);
                                //System.Threading.Thread.Sleep(100);
                            }
                            else
                            {
                                Trace.WriteLine("Insert Into Table Error: " + db.LastError);
                            }

                            //disconnet
                            db.disconnect();
                        });

                    t.Start();
                    tasks.Add(t);
                }
            }

            Task.WaitAll(tasks.ToArray());

        }

        private bool validateFile(FileInfo file)
        {
            Regex r = new Regex("^([x][m][l])|([X][M][L])|([x][m][l])$");

            return r.IsMatch(file.Extension);
        }

        #endregion

        private void toolStripButtonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.ShowDialog();
                //textBoxPath.Text = folderBrowserDialog1.SelectedPath;
                logsFilePath = folderBrowserDialog1.SelectedPath;

                Trace.WriteLine("Path: " + logsFilePath);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Browse folder Error: " + ex.Message);
            }
        }

        private void toolStripButtonInsert_Click(object sender, EventArgs e)
        {
            FileToDatabase();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            XMLPaser();
        }

        private async Task XMLPaser()
        {

            DUT dut = new DUT();

            dut.SerialNumber = "1030030011111111";

            dut.RawCount.Add("100");

            dut.RawCount.Add("102");

            dut.IDAC.Add("98");

            ObjectXMLSerializer<DUT>.Save(dut, @"D:\test.xml");

            //Task t = new Task(() => ObjectXMLSerializer<DUT_Str>.Save(dut, "D:\test.xml"));

            //await t;


            dut = ObjectXMLSerializer<DUT>.Load(@"D:\test.xml");

            MessageBox.Show(dut.SerialNumber);

            foreach (string rc in dut.RawCount)
            {
                MessageBox.Show(rc);
            }
        
        }

    }

}
