using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;
using CypressSemiconductor.ChinaManufacturingTest;
using Microsoft.Office.Interop.Excel;

namespace Database_query_tool
{
    public partial class MainForm : Form
    {
        MySQLFunction mySQLFunction = new MySQLFunction();
        public string dataBase;
        public string dataType;
        public string saveFileName;

        public MainForm()
        {
            InitializeComponent();
            dateTimePicker2.Value = DateTime.Now;
            //tbTestTimeStart.Text = dateTimePicker1.Text;
        }

        #region UI callback

        delegate void SetStatusCallback(string text, Color color, int Process);
        private void setStatus(string text, Color color, int process)
        {
            if (this.statusStrip.InvokeRequired)
            {
                SetStatusCallback d1 = new SetStatusCallback(setStatus);
                this.Invoke(d1, new object[] { text, color, process });
            }
            else
            {
                toolStripProgressBar.Value = process;
                toolStripStatusLabel.Text = text;
                toolStripStatusLabel.BackColor = color;                
            }
        }


        delegate void SetDataGridViewTrackpadCallback();
        private void SetDataGridViewTrackpad()
        {
            if (this.dataGridViewTrackpad.InvokeRequired)
            {
                SetDataGridViewTrackpadCallback d2 = new SetDataGridViewTrackpadCallback(SetDataGridViewTrackpad);
                this.Invoke(d2, new object[] { });
            }
            else
            {
                dataGridViewTrackpad.DataSource = mySQLFunction.DUTds.Tables[0].DefaultView;
                dataGridViewTrackpad.Columns["TestTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                tbResultCount.Text = (dataGridViewTrackpad.Rows.Count - 1).ToString();
                setStatus("Search finished.......", Color.Green, 100);
            }
        }

        #endregion

        #region Functions

        public void searchDatabase(object command)
        {
            string cmd = (string)command;
            setStatus("Searching......." + cmd, Color.Red, 50);

            if (mySQLFunction.connect())
            {
                try
                {
                    mySQLFunction.DUTds = new DataSet();
                    MySql.Data.MySqlClient.MySqlDataAdapter mySQLData = new MySqlDataAdapter(cmd, mySQLFunction.conn);
                    mySQLData.Fill(mySQLFunction.DUTds);
                    // dataGridViewTrackpad.DataSource = mySQLFunction.DUTds.Tables[0].DefaultView;
                    SetDataGridViewTrackpad();

                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
        }



        public void SaveResults()
        {
            try
            {

                if (dataGridViewTrackpad.Columns.Count == 0)
                    return;


                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                Workbook wBook = excel.Application.Workbooks.Add(true);
                //Worksheet wSheet = (Worksheet)wBook.ActiveSheet;      
                excel.Visible = false;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Export excel file";
                saveFileDialog.ShowDialog();
                string strName = saveFileDialog.FileName;

                btSEARCH.Enabled = false;
                btCLEAR.Enabled = false;
                btSave.Enabled = false;
                ////Excel header  
                for (int i = 0; i < dataGridViewTrackpad.Columns.Count; i++)
                {


                    excel.Cells[1, i + 1] = dataGridViewTrackpad.Columns[i].HeaderText;
                }

                //save DataGridView into Excel

                if (dataGridViewTrackpad.Rows.Count > 0)
                {
                    for (int i = 0; i < (dataGridViewTrackpad.Rows.Count - 1); i++)
                    {
                        for (int j = 0; j < dataGridViewTrackpad.Columns.Count; j++)
                        {
                            string str = dataGridViewTrackpad[j, i].Value.ToString();
                            excel.Cells[i + 2, j + 1] = "'" + str;
                            //System.Threading.Thread.Sleep(10);
                        }
                        setStatus("Saving searched data ", Color.Red, (int)(i * 100 / (dataGridViewTrackpad.Rows.Count - 1)));
                    }

                }

                setStatus("save finished", Color.Green, 100);

                excel.DisplayAlerts = false;
                excel.AlertBeforeOverwriting = false;

                wBook.Save();
                excel.ActiveWorkbook.SaveCopyAs(strName);


                btSEARCH.Enabled = true;
                btCLEAR.Enabled = true;
                btSave.Enabled = true;
                //// excel.ThisWorkbook.Save();
                // excel.Application.Workbooks.Add(true).Save();
                // excel.Save(strName);
                //excel.SaveWorkspace("Libraries\\Documents\temp.xlsx");    
                excel.Quit();
                excel = null;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                btSEARCH.Enabled = true;
                btCLEAR.Enabled = true;
                btSave.Enabled = true;
            }
        }


        public void searchDatabase3()
        {
            
            System.Data.DataTable dataTableSearched = new System.Data.DataTable();
            mySQLFunction.dataTableDUT = new System.Data.DataTable();

            setStatus("Searching data ", Color.Red, 0);

            try
            {
                for (int i = 0; i < dataGridViewTrackpad.Rows.Count - 1; i++)
                {
                    StringBuilder mySQLString = new StringBuilder();
                    DeviceConfig.partType = dataGridViewTrackpad["PartType", i].Value.ToString();
                    mySQLFunction.Commonds = new DataSet();
                    mySQLString.Append("select * from " + dataBase + " where DUTID = " + dataGridViewTrackpad["Id", i].Value.ToString());
                    MySql.Data.MySqlClient.MySqlDataAdapter mySQLData = new MySqlDataAdapter(mySQLString.ToString(), mySQLFunction.conn);
                    mySQLData.Fill(mySQLFunction.Commonds);
                    dataTableSearched = mySQLFunction.Commonds.Tables[0];

                    if (i % 50 == 0)
                    {
                        setStatus("Searching data ***************", Color.Red, (int)(i * 100 / (dataGridViewTrackpad.Rows.Count - 1)));
                    }

                    DataColumn column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.AllowDBNull = false;
                    string columnName = dataGridViewTrackpad["Id", i].Value.ToString();
                    column.Caption = columnName;
                    column.ColumnName = columnName;
                    column.DefaultValue = "";

                    // Add the column to the table. 

                    mySQLFunction.dataTableDUT.Columns.Add(column);

                    for (int k = 0; k < dataTableSearched.Rows.Count; k++)
                    {

                        if (i == 0)
                        {
                            DataRow row;
                            row = mySQLFunction.dataTableDUT.NewRow();
                            row[columnName] = dataTableSearched.Rows[k][dataType].ToString();
                            mySQLFunction.dataTableDUT.Rows.Add(row);
                        }
                        else
                        {
                            mySQLFunction.dataTableDUT.Rows[k][columnName] = dataTableSearched.Rows[k][dataType].ToString();
                        }

                    }


                }
                setStatus("Search finished ", Color.Green, 100);

            }

            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


            FormDistChart DistForm = new FormDistChart();
            DistForm.distDT = mySQLFunction.dataTableDUT;
            DistForm.dataTypeString = dataType;
            DistForm.ShowDialog();
        }

        public void subSearchDatabase(string dataTable)
        {

            StringBuilder mySQLString = new StringBuilder();

            try
            {

                DeviceConfig.partType = dataGridViewTrackpad["PartType", dataGridViewTrackpad.SelectedCells[0].RowIndex].Value.ToString();
                mySQLFunction.Commonds = new DataSet();
                mySQLString.Append("select * from " + dataTable + " where DUTID = " + dataGridViewTrackpad["Id", dataGridViewTrackpad.SelectedCells[0].RowIndex].Value.ToString());
                MySql.Data.MySqlClient.MySqlDataAdapter mySQLData = new MySqlDataAdapter(mySQLString.ToString(), mySQLFunction.conn);
                mySQLData.Fill(mySQLFunction.Commonds);

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
           
            if (dataTable == "iddstandby")
            {
                string IDDSlepp1 = mySQLFunction.Commonds.Tables[0].Rows[0]["IDDSleep1"].ToString();
                string IDDDeepSleep = mySQLFunction.Commonds.Tables[0].Rows[0]["IDDDeepSleep"].ToString();
                MessageBox.Show("IDD Sleep1: " + IDDSlepp1 + "mA; IDD Deep Sleep: " + IDDDeepSleep + "uA.", "Test Result", MessageBoxButtons.OK);

            }
            else
            {
                FormGrid formGrid = new FormGrid();
                formGrid.dsFormGrid = mySQLFunction.Commonds;
                formGrid.indicateInfo = dataTable;

                formGrid.ShowDialog();
            }

           
        }



        public void saveSearchedDataToExcel(object dataTypeSave)
        {
            string dataBaseSave=dataTypeSave.ToString().ToLower();
      
            System.Data.DataTable dataTableSearched = new System.Data.DataTable();
            System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //create excel file to save
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wBook = excel.Application.Workbooks.Add(true);
            excel.Visible = false;

            
            try
            {
                for (int i = 0; i < dataGridViewTrackpad.Rows.Count - 1; i++)
                {
                    StringBuilder mySQLString = new StringBuilder();
                    DeviceConfig.partType = dataGridViewTrackpad["PartType", i].Value.ToString();
                    mySQLFunction.Commonds = new DataSet();
                    mySQLString.Append("select * from " + dataBaseSave + " where DUTID = " + dataGridViewTrackpad["Id", i].Value.ToString());
                    MySql.Data.MySqlClient.MySqlDataAdapter mySQLData = new MySqlDataAdapter(mySQLString.ToString(), mySQLFunction.conn);
                    mySQLData.Fill(mySQLFunction.Commonds);
                    dataTableSearched = mySQLFunction.Commonds.Tables[0];

                    if (i % 25 == 0)
                    {
                        setStatus("Please wait !! Searching and Saving searched data  NO."+i.ToString(), Color.Red, (int)(i * 100 / (dataGridViewTrackpad.Rows.Count - 1)));

                    }
                    
                    //save dataGridRawCount to created excel file

                    if (i == 0)
                    {
                        excel.Cells[1, 1] = "DUTID";
                        excel.Cells[1, 2] = "SerialNumber";
                        excel.Cells[1, 3] = "ErrorCode";
                        excel.Cells[1, 4] = "TestTime";
                        for (int j = 0; j < dataTableSearched.Rows.Count; j++)
                        {
                            excel.Cells[1, j + 5] = "D" + Convert.ToString(j + 1);
                        }
                    }
                    excel.Cells[i + 2, 1] = dataGridViewTrackpad["Id", i].Value.ToString();
                    excel.Cells[i + 2, 2] = dataGridViewTrackpad["SerialNumber", i].Value.ToString();
                    excel.Cells[i + 2, 3] = dataGridViewTrackpad["ErrorCode", i].Value.ToString();
                    excel.Cells[i + 2, 4] = dataGridViewTrackpad["TestTime", i].Value.ToString();

                    for (int k = 0; k < dataTableSearched.Rows.Count; k++)
                    {
                        excel.Cells[i + 2, k + 5] = dataTableSearched.Rows[k][dataTypeSave.ToString()].ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            excel.DisplayAlerts = false;
            excel.AlertBeforeOverwriting = false;

            wBook.Save();
            excel.ActiveWorkbook.SaveCopyAs(saveFileName);
            excel.Quit();
            excel = null;
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;

            setStatus("Search and save data finished", Color.Green, 100);

        }

        public void saveSearchedIDDToExcel(object dataTypeSave)
        {
            string dataBaseSave = dataTypeSave.ToString().ToLower();

            System.Data.DataTable dataTableSearched = new System.Data.DataTable();
            System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //create excel file to save
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wBook = excel.Application.Workbooks.Add(true);
            excel.Visible = false;


            try
            {
                for (int i = 0; i < dataGridViewTrackpad.Rows.Count - 1; i++)
                {
                    StringBuilder mySQLString = new StringBuilder();
                    DeviceConfig.partType = dataGridViewTrackpad["PartType", i].Value.ToString();
                    mySQLFunction.Commonds = new DataSet();
                    mySQLString.Append("select * from " + dataBaseSave + " where DUTID = " + dataGridViewTrackpad["Id", i].Value.ToString());
                    MySql.Data.MySqlClient.MySqlDataAdapter mySQLData = new MySqlDataAdapter(mySQLString.ToString(), mySQLFunction.conn);
                    mySQLData.Fill(mySQLFunction.Commonds);
                    dataTableSearched = mySQLFunction.Commonds.Tables[0];

                    if (i % 25 == 0)
                    {
                        setStatus("Please wait !! Searching and Saving searched data  NO." + i.ToString(), Color.Red, (int)(i * 100 / (dataGridViewTrackpad.Rows.Count - 1)));

                    }

                    //save dataGridRawCount to created excel file

                    if (i == 0)
                    {
                        excel.Cells[1, 1] = "DUTID";
                        excel.Cells[1, 2] = "SerialNumber";
                        excel.Cells[1, 3] = "ErrorCode";
                        excel.Cells[1, 4] = "TestTime";
                        excel.Cells[1, 5] = "Sleep1";
                        excel.Cells[1, 6] = "Deep Sleep";
                        
                    }
                    excel.Cells[i + 2, 1] = dataGridViewTrackpad["Id", i].Value.ToString();
                    excel.Cells[i + 2, 2] = dataGridViewTrackpad["SerialNumber", i].Value.ToString();
                    excel.Cells[i + 2, 3] = dataGridViewTrackpad["ErrorCode", i].Value.ToString();
                    excel.Cells[i + 2, 4] = dataGridViewTrackpad["TestTime", i].Value.ToString();
                    excel.Cells[i + 2, 5] = dataTableSearched.Rows[0]["IDDSleep1"].ToString(); 
                    excel.Cells[i + 2, 6] = dataTableSearched.Rows[0]["IDDDeepSleep"].ToString();
                   

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            excel.DisplayAlerts = false;
            excel.AlertBeforeOverwriting = false;

            wBook.Save();
            excel.ActiveWorkbook.SaveCopyAs(saveFileName);
            excel.Quit();
            excel = null;
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;

            setStatus("Search and save data finished", Color.Green, 100);

        }



        #endregion

        #region Event

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void testDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MySQLFunction mySQLFunction1 = new MySQLFunction();
                if (mySQLFunction1.connect())
                {
                    setStatus("Database connection succeed", Color.Green, 100);
                    MessageBox.Show("Database connection succeed");
                }
                else
                {
                    setStatus("Database connection failed, Please make sure your config are right", Color.Red, 0);
                    MessageBox.Show("Database connection failed, please check your network or configuration");
                }
            }
            catch
            {}

        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {



            FormConfiguration Config = new FormConfiguration();

            try
            {
                Config.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                Config.Dispose();
            }
        }

        #endregion

        private void btSEARCH_Click(object sender, EventArgs e)
        {

            StringBuilder mySQLstring = new StringBuilder();
            //Build search command

            string strDateStart = dateTimePicker1.Value.ToString("u", System.Globalization.DateTimeFormatInfo.InvariantInfo).Substring(0,10);
            
            string strDateEnd = dateTimePicker2.Value.ToString("u", System.Globalization.DateTimeFormatInfo.InvariantInfo).Substring(0,10);
            strDateStart= strDateStart.Replace("Z", "");
            strDateEnd= strDateEnd.Replace("Z","");
            strDateStart = strDateStart.Insert(10," 00:00:00");
            strDateEnd = strDateEnd.Insert(10," 23:59:59");

            if (textBoxCommand.Text.Length > 1)
            {
                mySQLstring.Append(textBoxCommand.Text.ToString());
            }
            else
            {
                if (comboBoxSearchType.Text == "FailedUnits")
                {
                    mySQLstring.Append("select * , count(distinct SerialNumber) from dut where SerialNumber IN (Select SerialNumber from dut where  ");
                   
                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }

                    mySQLstring.Append("TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME) and ErrorCode > 0) and SerialNumber NOT IN ( Select SerialNumber from dut where  ");

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }
                    //TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME) and
                    mySQLstring.Append(" ErrorCode = 0)");

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append(" and Teststation = '" + cmbTestStation.Text + "'");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append(" and PartType = '" + cmbModel.Text + "' ");
                    }

                    mySQLstring.Append(" group by SerialNumber");
                }

                if (comboBoxSearchType.Text=="UnitsWithFailLogs")
                {
                                       
                    
                    ////************* below code can search dut with repeated logs

                    mySQLstring.Append("select * from dut where SerialNumber IN (Select SerialNumber from dut where  ");
                   

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }

                   
                    mySQLstring.Append("ErrorCode > 0 and ");
                    

                    if (tbIDDValue.Text.Trim() != "")
                    {
                        mySQLstring.Append("IDDValue" + cmbIDDValue.Text + "'" + tbIDDValue.Text + "' and ");
                    }

                    if (tbFWVersion.Text.Trim() != "")
                    {
                        mySQLstring.Append("FirmwareVersion = '" + tbFWVersion.Text + "' and ");
                    }

                    mySQLstring.Append("TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME)");


                    mySQLstring.Append(") and ");

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' Order by TestTime");
                    }
                    

                }
                if (comboBoxSearchType.Text == "AllLogs")
                {
                    mySQLstring.Append("select * from dut where ");
                    if (tbSN.Text.Trim() != "")
                    {
                        mySQLstring.Append("SerialNumber='" + tbSN.Text + "' and ");
                    }

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }

                    if (tbErrorCode.Text.Trim() != "")
                    {
                        mySQLstring.Append("ErrorCode" + cmbErrorCode.Text + "'" + tbErrorCode.Text + "' and ");
                    }

                    if (tbIDDValue.Text.Trim() != "")
                    {
                        mySQLstring.Append("IDDValue" + cmbIDDValue.Text + "'" + tbIDDValue.Text + "' and ");
                    }

                    if (tbFWVersion.Text.Trim() != "")
                    {
                        mySQLstring.Append("FirmwareVersion = '" + tbFWVersion.Text + "' and ");
                    }

                    mySQLstring.Append("TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME)");
                }

                if (comboBoxSearchType.Text == "Output")
                {
                    mySQLstring.Append("select * , count(distinct SerialNumber) from dut where SerialNumber IN (Select SerialNumber from dut where  ");

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }

                    mySQLstring.Append("TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME) and ErrorCode > 0) and SerialNumber NOT IN ( Select SerialNumber from dut where  ");

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }
                    //TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME) and
                    mySQLstring.Append(" ErrorCode = 0)");

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append(" and Teststation = '" + cmbTestStation.Text + "'");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append(" and PartType = '" + cmbModel.Text + "' ");
                    }

                    mySQLstring.Append(" group by SerialNumber UNION "); 
                    
                    
                    mySQLstring.Append("select * , count(distinct SerialNumber) from dut where ");

                    if (tbSN.Text.Trim() != "")
                    {
                        mySQLstring.Append("SerialNumber='" + tbSN.Text + "' and ");
                    }

                    if (cmbTestStation.Text.Trim() != "")
                    {
                        mySQLstring.Append("Teststation = '" + cmbTestStation.Text + "' and ");
                    }

                    if (cmbModel.Text.Trim() != "")
                    {
                        mySQLstring.Append("PartType = '" + cmbModel.Text + "' and ");
                    }

                    
                    mySQLstring.Append("ErrorCode = 0 " + " and ");
                  

                    if (tbIDDValue.Text.Trim() != "")
                    {
                        mySQLstring.Append("IDDValue" + cmbIDDValue.Text + "'" + tbIDDValue.Text + "' and ");
                    }

                    if (tbFWVersion.Text.Trim() != "")
                    {
                        mySQLstring.Append("FirmwareVersion = '" + tbFWVersion.Text + "' and ");
                    }

                    mySQLstring.Append("TestTime BETWEEN CAST(\"" + strDateStart + "\" AS DATETIME)" + " AND CAST(\"" + strDateEnd + "\" AS DATETIME)");

                    mySQLstring.Append(" group by SerialNumber");
                }

            }


            //Searching data
            Thread searchThread;
            searchThread = new Thread(searchDatabase);
            searchThread.IsBackground = true;
            searchThread.Start(mySQLstring.ToString());


        }



        private void btCLEAR_Click(object sender, EventArgs e)
        {
            dataGridViewTrackpad.Columns.Clear();
        }

        private void buttonRawCount_Click(object sender, EventArgs e)
        {

            subSearchDatabase("rawcountaverage");

        }

        private void buttonNoise_Click(object sender, EventArgs e)
        {

            subSearchDatabase("rawcountnoise");
        }

        private void buttonIDAC_Click(object sender, EventArgs e)
        {

            subSearchDatabase("idacvalue");
        }

        private void buttonIDD_Click(object sender, EventArgs e)
        {

            subSearchDatabase("iddstandby");
        }

        private void btSave_Click(object sender, EventArgs e)
        {

            System.Globalization.CultureInfo oldCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            //object m_objOpt = System.Reflection.Missing.Value;


            //Thread searchThread;
            //searchThread = new Thread(SaveResults);
            //searchThread.IsBackground = true;
            //searchThread.Start();

            SaveResults();


            System.Threading.Thread.CurrentThread.CurrentCulture = oldCI;
        }


        private void dataGridViewTrackpad_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            tbResultCount.Text = (dataGridViewTrackpad.Rows.Count - 1).ToString();
        }

        private void saveSelectedRawCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Export excel file";
            saveFileDialog.ShowDialog();
            saveFileName = saveFileDialog.FileName;
            
            
            Thread searchThread;
            searchThread = new Thread(saveSearchedDataToExcel);
            searchThread.IsBackground = true;
            searchThread.Start("RawCountAverage");

            //saveSearchedDataToExcel("rawcountaverage", "RawCountAverage");"rawcountnoise", "RawCountNoise""idacvalue", "IDACValue"

        }

        private void saveSelectedRawCountNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Export excel file";
            saveFileDialog.ShowDialog();
            saveFileName = saveFileDialog.FileName;
            
            Thread searchThread;
            searchThread = new Thread(saveSearchedDataToExcel);
            searchThread.IsBackground = true;
            searchThread.Start("RawCountNoise");
            
            //saveSearchedDataToExcel("RawCountNoise");
        }

        private void saveSelectedIDACValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Export excel file";
            saveFileDialog.ShowDialog();
            saveFileName = saveFileDialog.FileName;
            
            Thread searchThread;
            searchThread = new Thread(saveSearchedDataToExcel);
            searchThread.IsBackground = true;
            searchThread.Start("IDACValue");
            
            //saveSearchedDataToExcel("IDACValue");
        }

        private void saveSelectedIDDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Export excel file";
            saveFileDialog.ShowDialog();
            saveFileName = saveFileDialog.FileName;

            Thread searchThread;
            searchThread = new Thread(saveSearchedIDDToExcel);
            searchThread.IsBackground = true;
            searchThread.Start("iddstandby");

            //saveSearchedDataToExcel("IDACValue");
        }

        private void passYieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFPYChart FPYForm = new FormFPYChart();
            try
            {

                FPYForm.FPYds = mySQLFunction.DUTds;

                FPYForm.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FPYForm.Close();
            }
        }

        private void rawCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataBase = "rawcountaverage";
            dataType = "RawCountAverage";
            Thread searchThread;
            searchThread = new Thread(searchDatabase3);
            searchThread.IsBackground = true;
            searchThread.Start();

        }

        private void noiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataBase = "rawcountnoise";
            dataType = "RawCountNoise";
            Thread searchThread;
            searchThread = new Thread(searchDatabase3);
            searchThread.IsBackground = true;
            searchThread.Start();
        }

        private void iDACToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataBase = "idacvalue";
            dataType = "IDACValue";
            Thread searchThread;
            searchThread = new Thread(searchDatabase3);
            searchThread.IsBackground = true;
            searchThread.Start();
        }






    }
}
