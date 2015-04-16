using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Database_query_tool
{
    public partial class FormFPYChart : Form
    {
        public DataSet FPYds = new DataSet();
        List<double> FPYdata = new List<double>();
        List<string> WWdata = new List<string>();
        List<double> WWpassCount = new List<double>();
        List<double> WWfailCount = new List<double>();

        public FormFPYChart()
        {
            InitializeComponent();
        }

        private void FormFPYChart_Shown(object sender, EventArgs e)
        {
            
            int firstMonOfYear;

           
            DataView dv = new DataView(FPYds.Tables[0]);
            dv.Sort = "TestTime";
            DataTable dt = dv.ToTable();
            DateTime date;
            date = Convert.ToDateTime(dt.Rows[0]["TestTime"].ToString());           
            firstMonOfYear = FIrstMonOfYear(date);

            // first record's week of year
            int firstWeek = (date.DayOfYear - firstMonOfYear) / 7;

            double passCount = 0;
            double failCount = 0;
            double totalPassed = 0;
            double totalFailed = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime dateTemp;
                dateTemp = Convert.ToDateTime(dt.Rows[i]["TestTime"].ToString());
                int currentWeek = (dateTemp.DayOfYear - firstMonOfYear) / 7;
                if (currentWeek == firstWeek && i != (dt.Rows.Count - 1))
                {
                    if (dt.Rows[i]["ErrorCode"].ToString() == "0")
                    {
                        passCount += 1;
                        totalPassed += 1;
                    }
                    else
                    {
                        failCount += 1;
                        totalFailed += 1;
                    }
                }
                else
                {
                    double FPY = 100 * passCount / (passCount + failCount);
                    int FPYTemp = (int)(FPY * 100);
                    FPY = FPYTemp / 100.00;
                    FPYdata.Add(FPY);
                    WWdata.Add(date.Year.ToString() + " " + (firstWeek + 1).ToString());
                    WWfailCount.Add(failCount);
                    WWpassCount.Add(passCount);
                    passCount = 0;
                    failCount = 0;
                   
                    if (date.Year != dateTemp.Year)
                    {
                        firstMonOfYear = FIrstMonOfYear(dateTemp);
                    }
                    firstWeek = (dateTemp.DayOfYear - firstMonOfYear) / 7;
                    date = dateTemp;
                }

            }

            double totalFPY = totalPassed / (totalFailed + totalPassed);            
            textBoxFPY.Text = totalFPY.ToString("p");

            Series FPYSeries = new Series();
            FPYSeries.ChartType = SeriesChartType.Column;
            FPYSeries.IsValueShownAsLabel = true;
            FPYSeries.IsXValueIndexed = false;
            FPYSeries.Points.DataBindXY(WWdata, FPYdata);
            Distchart.Series.Add(FPYSeries);            
            Distchart.ChartAreas[0].AxisY.Minimum = (int)FPYdata.Min() - 5;
            Distchart.ChartAreas[0].AxisY.Title = "Percentage % ";
            Distchart.ChartAreas[0].AxisX.Title = "Work Week";
            



        }

        public int FIrstMonOfYear(DateTime date)
        {
            int firstMonOfYear;
            
            switch (date.Year.ToString())
            {
                case "2010":
                    firstMonOfYear = 3;
                    break;
                case "2011":
                    firstMonOfYear = -5;
                    break;
                case "2012":
                    firstMonOfYear = 1;
                    break;
                case "2013":
                    firstMonOfYear = 6;
                    break;
                case "2014":
                    firstMonOfYear = 5;
                    break;
                case "2015":
                    firstMonOfYear = 4;
                    break;
                case "2016":
                    firstMonOfYear = 3;
                    break;
                default:
                    firstMonOfYear = 0;
                    break;
            }

            return firstMonOfYear;

        }

        private void comboBoxInfoToShow_TextChanged(object sender, EventArgs e)
        {
            if (cmbShowType.Text == "FPY")
            {
                Distchart.Series.Clear();
                Series FPYSeries = new Series();
                FPYSeries.ChartType = SeriesChartType.Column;
                FPYSeries.IsValueShownAsLabel = true;
                FPYSeries.IsXValueIndexed = false;
                FPYSeries.Points.DataBindXY(WWdata, FPYdata);
                Distchart.Series.Add(FPYSeries);
                Distchart.ChartAreas[0].AxisY.Minimum = (int)FPYdata.Min() - 5;
                Distchart.ChartAreas[0].AxisY.Title = "Percentage % ";
                Distchart.ChartAreas[0].AxisX.Title = "Work Week";

            }
            else
            {
                Distchart.Series.Clear();
                Series passSeries = new Series();
                passSeries.Points.DataBindXY(WWdata, WWpassCount);
                passSeries.IsXValueIndexed = false;
                passSeries.IsValueShownAsLabel = true;
                Distchart.Series.Add(passSeries);

                Series failSeries = new Series();
                failSeries.Points.DataBindXY(WWdata, WWfailCount);
                failSeries.IsValueShownAsLabel = true;
                Distchart.Series.Add(failSeries);
                Distchart.ChartAreas[0].AxisY.Title = "Quantity";

            }
        }



    }
}
