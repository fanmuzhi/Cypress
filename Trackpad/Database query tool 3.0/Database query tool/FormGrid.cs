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

namespace Database_query_tool
{
    public partial class FormGrid : Form
    {
        public DataSet dsFormGrid = new DataSet();
        public string indicateInfo; //what to show? Rawcount or noise or IDAC
        

        public FormGrid()
        {
            InitializeComponent();

        }

        delegate void SetGridDisplayCallback(DataSet dt);

        private void SetGridDisplay(DataSet dt)
        {
            
            
            if (this.dataGridViewForm.InvokeRequired)
            {
                SetGridDisplayCallback d = new SetGridDisplayCallback(SetGridDisplay);
                this.dataGridViewForm.Invoke(d, new object[] { dt });

            }
            else
            {
                

                if (dataGridViewForm.Columns.Count != DeviceConfig.Items.FW_INFO_NUM_COLS ||
                    dataGridViewForm.Rows.Count != DeviceConfig.Items.FW_INFO_NUM_ROWS)   //  initiate dataGridViewIntegrated;
                {

                    //clear data grid columns and rows
                    if (dataGridViewForm.Columns.Count != 0)
                    {
                        dataGridViewForm.Columns.Clear();                        
                    }
                    if (dataGridViewForm.Rows.Count != 0)
                    {
                        dataGridViewForm.Rows.Clear();
                    }
                   

                    //structure new data grid
                    for (int j = 0; j < DeviceConfig.Items.FW_INFO_NUM_COLS; j++)
                    {
                        dataGridViewForm.Columns.Add("Columns", Convert.ToString(j + 1));
                        dataGridViewForm.Columns[j].Width = 40;
                    }
                    dataGridViewForm.Rows.Add(DeviceConfig.Items.FW_INFO_NUM_ROWS - 1);
                    for (int i = 0; i < DeviceConfig.Items.FW_INFO_NUM_ROWS; i++)
                    {
                        dataGridViewForm.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
                    }
                    dataGridViewForm.RowHeadersWidth = 50;
                }
                dataGridViewForm.Rows[0].Selected = false;
                List<int> DisplayInfo = new List<int>(); //dt.RawCount;

           
                string indicateInfoTemp = "";
                if (indicateInfo == "idacvalue")
                    indicateInfoTemp = "IDACValue";
                else if (indicateInfo == "rawcountaverage")
                    indicateInfoTemp = "RawCountAverage";
                else if (indicateInfo == "rawcountnoise")
                    indicateInfoTemp = "RawCountNoise";


                try
                {
                    for (int i = 0; i < dsFormGrid.Tables[0].Rows.Count; i++)
                    {
                        string temp = dsFormGrid.Tables[0].Rows[i][indicateInfoTemp].ToString();
                        DisplayInfo.Add(Convert.ToInt32(temp));
                    }                    

                    for (int i = DisplayInfo.Count; i < DeviceConfig.Items.FW_INFO_NUM_COLS * DeviceConfig.Items.FW_INFO_NUM_ROWS; i++)
                    {
                        DisplayInfo.Add(-1);
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    for (int i = 0; i < DeviceConfig.Items.FW_INFO_NUM_ROWS; i++)
                    {
                        for (int j = 0; j < DeviceConfig.Items.FW_INFO_NUM_COLS; j++)
                        {
                            dataGridViewForm[j, (DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i)].Value = DisplayInfo[j + i * DeviceConfig.Items.FW_INFO_NUM_COLS];

                            int DisplayInfoTemp = DisplayInfo[j + i * DeviceConfig.Items.FW_INFO_NUM_COLS];
                         

                            switch (indicateInfo)
                            {
                                case "rawcountnoise":
                                    if (DisplayInfoTemp >= DeviceConfig.Items.RAW_NOISE_MAX)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else if (DisplayInfoTemp == -1)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Black;
                                    else
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                case "idacvalue":
                                    if (DisplayInfoTemp <= 100 && DisplayInfoTemp > 0)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else if (DisplayInfoTemp == -1)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Black;
                                    else
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                case "rawcountaverage":
                                    if (DisplayInfoTemp >= DeviceConfig.Items.RAW_AVG_MAX)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Green;
                                    else if (DisplayInfoTemp == -1)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Black;
                                    else if (DisplayInfoTemp < DeviceConfig.Items.RAW_AVG_MIN)
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.Red;
                                    else
                                        dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;

                                default:
                                    dataGridViewForm[j, DeviceConfig.Items.FW_INFO_NUM_ROWS - 1 - i].Style.BackColor = Color.White;
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
                finally
                {

                    //return;
                }

            }
        }


        private void FormGrid_Shown(object sender, EventArgs e)
        {

            try
            {
                
                DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";
                if (!DeviceConfig.Read())
                {
                    MessageBox.Show("Cannot find " + DeviceConfig.partType + " in Production.ini", "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                return;
            }
            
            
            
            try
            {
               // dataGridViewForm.Columns.Clear();
               // dataGridViewForm.DataSource = dsFormGrid.Tables[0].DefaultView;
                SetGridDisplay(dsFormGrid);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
