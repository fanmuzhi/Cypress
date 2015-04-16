using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace Database_query_tool
{
    public partial class FormFPYChart : Form
    {
        public DataSet FPYds = new DataSet();
        List<double> FPYdata = new List<double>();
        List<string> WWdata = new List<string>();
        List<double> WWpassCount = new List<double>();
        List<double> WWfailCount = new List<double>();
        List<string> ErrorList = new List<string>();
        List<int> errorCount = new List<int>();
        List<int> errorTrendCount = new List<int>();
        List<double> errorTrendPercent = new List<double>();
        List<string> errorCodeList = new List<string>();
        List<Series> errorSeriesList = new List<Series>();
        List<Series> errorSeriesListPercent = new List<Series>();
        int maxErrorCount = 0;
        double maxErrorPercent = 0;

        DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;


        public FormFPYChart()
        {
            InitializeComponent();
        }

        private void FormFPYChart_Shown(object sender, EventArgs e)
        {

            Calendar cal = dfi.Calendar;
            DataView dv = new DataView(FPYds.Tables[0]);
            dv.Sort = "TestTime";
            DataTable dt = dv.ToTable();
            DateTime date;
            date = Convert.ToDateTime(dt.Rows[0]["TestTime"].ToString());
            // List<string> WWErrorList = new List<string>();



            // first record's week of year
            int formerWeek = cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            double passCount = 0;
            double failCount = 0;
            double totalPassed = 0;
            double totalFailed = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime dateTemp;
                dateTemp = Convert.ToDateTime(dt.Rows[i]["TestTime"].ToString());
                int currentWeek = cal.GetWeekOfYear(dateTemp, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                if (currentWeek == formerWeek && i != (dt.Rows.Count - 1))
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
                        ErrorList.Add(dt.Rows[i]["ErrorCode"].ToString());
                        // WWErrorList.Add(dt.Rows[i]["ErrorCode"].ToString());
                    }
                }
                else
                {
                    double FPY = 100 * passCount / ((passCount + failCount) == 0 ? 1 : (passCount + failCount));
                    int FPYTemp = (int)(FPY * 100);
                    FPY = FPYTemp / 100.00;
                    FPYdata.Add(FPY);
                    WWdata.Add(formerWeek.ToString() + " " + date.Year.ToString());
                    WWfailCount.Add(failCount);
                    WWpassCount.Add(passCount);
                    passCount = 0;
                    failCount = 0;
                    formerWeek = cal.GetWeekOfYear(dateTemp, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                    date = dateTemp;
                    //WWErrorList = new List<string>();
                    //First day of new week
                    if (dt.Rows[i]["ErrorCode"].ToString() == "0")
                    {
                        passCount += 1;
                        totalPassed += 1;
                    }
                    else
                    {
                        failCount += 1;
                        totalFailed += 1;
                        ErrorList.Add(dt.Rows[i]["ErrorCode"].ToString());
                        //WWErrorList.Add(dt.Rows[i]["ErrorCode"].ToString());
                    }

                }

            }


            string[] errorCode = { "10", "11", "12", "21", "31", "32", "41", "51", "52", "53", "54", "55", "61", "62", "63", "64", "65", "81", "91", "92", "A1" };
            foreach (string errorStr in errorCode)
            {
                List<string> temp = ErrorList.FindAll(delegate(string t) { return t == errorStr; });
                if (temp.Count > 0)
                {
                    errorCount.Add(temp.Count);
                    errorCodeList.Add(errorStr);
                    comboBoxErrorCode.Items.Add(errorStr);
                }

                temp.Clear();
            }


            //build error trend list
            foreach (string error in errorCodeList)
            {
                FindErrorList(error);
                Series errorSeries = new Series();
                errorSeries.Name = error;
                errorSeries.Legend = "Legend1";                
                errorSeries.ChartType = SeriesChartType.Line;
                errorSeries.Points.DataBindXY(WWdata, errorTrendCount);
                errorSeries.IsXValueIndexed = false;
                errorSeries.IsValueShownAsLabel = true;                
                errorSeriesList.Add(errorSeries);

                Series errorSeriesP = new Series();
                errorSeriesP.Name = error;
                errorSeriesP.Legend = "Legend1";
                errorSeriesP.ChartType = SeriesChartType.Line;
                errorSeriesP.Points.DataBindXY(WWdata, errorTrendPercent);
                errorSeriesP.IsXValueIndexed = false;
                errorSeriesP.IsValueShownAsLabel = true;
                errorSeriesListPercent.Add(errorSeriesP);
            }


            double totalFPY = totalPassed / (totalFailed + totalPassed);
            textBoxFPY.Text = totalFPY.ToString("p");

            Series FPYSeries = new Series();
            FPYSeries.LegendText = "FPY   ";
            FPYSeries.ChartType = SeriesChartType.Column;
            FPYSeries.IsValueShownAsLabel = true;
            FPYSeries.IsXValueIndexed = false;
            FPYSeries.Points.DataBindXY(WWdata, FPYdata);
            FPYchart.Series.Add(FPYSeries);
            FPYchart.ChartAreas[0].AxisY.Minimum = (int)FPYdata.Min() - 5 > 0 ? ((int)FPYdata.Min() - 5) : (int)FPYdata.Min();
            FPYchart.ChartAreas[0].AxisY.Maximum = FPYdata.Max()+1;
            FPYchart.ChartAreas[0].AxisY.Title = "Percentage % ";
            FPYchart.ChartAreas[0].AxisX.Title = "Work Week";
            
        }



        private void comboBoxInfoToShow_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxInfoToShow.Text == "FPY")
            {
                FPYchart.Series.Clear();
                Series FPYSeries = new Series();
                FPYSeries.Name = "FPY";
                FPYSeries.Legend = "Legend1";
                FPYSeries.Color = Color.Blue;
                FPYSeries.ChartType = SeriesChartType.Line;
                FPYSeries.IsValueShownAsLabel = true;
                FPYSeries.IsXValueIndexed = false;
                FPYSeries.Points.DataBindXY(WWdata, FPYdata);
                FPYchart.Series.Add(FPYSeries);
                FPYchart.ChartAreas[0].AxisY.Minimum = (int)FPYdata.Min() - 5 > 0 ? ((int)FPYdata.Min() - 5) : (int)FPYdata.Min();
                FPYchart.ChartAreas[0].AxisY.Maximum = FPYdata.Max();
                FPYchart.ChartAreas[0].AxisY.Title = "Percentage % ";
                FPYchart.ChartAreas[0].AxisX.Title = "Work Week";

            }
            else if (comboBoxInfoToShow.Text == "Output")
            {
                FPYchart.Series.Clear();
                Series passSeries = new Series();
                passSeries.Name = "Pass";
                passSeries.Legend = "Legend1";
                passSeries.Color = Color.Blue;
                passSeries.Points.DataBindXY(WWdata, WWpassCount);
                passSeries.IsXValueIndexed = false;
                passSeries.IsValueShownAsLabel = true;
                FPYchart.Series.Add(passSeries);
                FPYchart.ChartAreas[0].AxisY.Minimum = 0;
                FPYchart.ChartAreas[0].AxisY.Maximum = Math.Max(WWpassCount.Max(),WWfailCount.Max());

                Series failSeries = new Series();
                failSeries.Name = "Fail";
                failSeries.Legend = "Legend1";
                failSeries.Color = Color.Red;
                failSeries.Points.DataBindXY(WWdata, WWfailCount);
                failSeries.IsValueShownAsLabel = true;
                FPYchart.Series.Add(failSeries);
                FPYchart.ChartAreas[0].AxisY.Title = "Quantity";
                FPYchart.ChartAreas[0].AxisY.Minimum = 0;
                FPYchart.ChartAreas[0].AxisY.Maximum = Math.Max(WWpassCount.Max(), WWfailCount.Max());

            }
            else if (comboBoxInfoToShow.Text == "Error Sum")
            {

                FPYchart.Series.Clear();
                Series errorSeries = new Series();
                errorSeries.Name = "ErrorCode";
                errorSeries.Legend = "Legend1";
                errorSeries.Color = Color.Blue;
                errorSeries.ChartType = SeriesChartType.Column;
                errorSeries.IsValueShownAsLabel = true;
                errorSeries.IsXValueIndexed = false;
                errorSeries.Points.DataBindXY(errorCodeList, errorCount);
                FPYchart.Series.Add(errorSeries);
                FPYchart.ChartAreas[0].AxisY.Minimum = 0;
                FPYchart.ChartAreas[0].AxisY.Maximum = errorCount.Max();
                FPYchart.ChartAreas[0].AxisY.Title = "Quantity";
                FPYchart.ChartAreas[0].AxisX.Title = "ErrorCode";

            }
            else if (comboBoxInfoToShow.Text == "Error Trend")
            {
                FPYchart.Series.Clear();
                foreach (Series errorSeries in errorSeriesList)
                {
                    
                    FPYchart.Series.Add(errorSeries);
                    FPYchart.ChartAreas[0].AxisY.Minimum = 0;
                    FPYchart.ChartAreas[0].AxisY.Maximum = maxErrorCount;
                    
                }
            }
            else if (comboBoxInfoToShow.Text == "Error Trend(%)")
            {
                FPYchart.Series.Clear();
                foreach (Series errorSeries in errorSeriesListPercent)
                {
                    
                    FPYchart.Series.Add(errorSeries);
                    FPYchart.ChartAreas[0].AxisY.Minimum = 0;
                    FPYchart.ChartAreas[0].AxisY.Maximum = maxErrorPercent;
                    
                }
            }

            
        }




        private void comboBoxErrorCode_SelectedIndexChanged(object sender, EventArgs e)
        {

            FPYchart.Series.Clear();
            

            
            if (comboBoxInfoToShow.Text == "Error Trend")
            {
                FPYchart.ChartAreas[0].AxisY.Minimum = 0;
                FPYchart.ChartAreas[0].AxisY.Maximum = maxErrorCount;
                FPYchart.ChartAreas[0].AxisY.Title = "Quantity";
                FPYchart.ChartAreas[0].AxisX.Title = "Work Week";
                FPYchart.Series.Add(errorSeriesList[comboBoxErrorCode.SelectedIndex]);
            }
            else if (comboBoxInfoToShow.Text == "Error Trend(%)")
            {
                FPYchart.ChartAreas[0].AxisY.Title = "Percentage(%)";
                FPYchart.ChartAreas[0].AxisX.Title = "Work Week";
                FPYchart.Series.Add(errorSeriesListPercent[comboBoxErrorCode.SelectedIndex]);
                FPYchart.ChartAreas[0].AxisY.Maximum = maxErrorPercent;
                
            }
           
            
        }


        public void FindErrorList(String ErrorCode)
        {

            Calendar cal = dfi.Calendar;
            DataView dv = new DataView(FPYds.Tables[0]);
            dv.Sort = "TestTime";
            DataTable dt = dv.ToTable();
            DateTime date;
            date = Convert.ToDateTime(dt.Rows[0]["TestTime"].ToString());
            //List<string> WWErrorList = new List<string>();
            // first record's week of year
            int formerWeek = cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int errorC = 0;
            errorTrendCount = new List<int>();
            errorTrendPercent = new List<double>();
            int j = 0; // for wwfailcount and wwpasscount
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime dateTemp;
                dateTemp = Convert.ToDateTime(dt.Rows[i]["TestTime"].ToString());
                int currentWeek = cal.GetWeekOfYear(dateTemp, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

                if (currentWeek == formerWeek && i != (dt.Rows.Count - 1))
                {
                    if (dt.Rows[i]["ErrorCode"].ToString() == ErrorCode)
                    {
                        errorC += 1;
                    }

                }
                else
                {
                    errorTrendCount.Add(errorC);


                    double errPercent = 100 * errorC / ((WWpassCount[j] + WWfailCount[j]) == 0 ? 1 : (WWpassCount[j] + WWfailCount[j]));
                    int FPYTemp = (int)(errPercent * 100);
                    errPercent = FPYTemp / 100.00;
                    errorTrendPercent.Add(errPercent);                    
                    j++;

                    errorC = 0;
                    formerWeek = cal.GetWeekOfYear(dateTemp, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                    date = dateTemp;
                    if (dt.Rows[i]["ErrorCode"].ToString() == ErrorCode)
                    {
                        errorC += 1;
                    }

                }

            }
            maxErrorCount = (maxErrorCount > errorTrendCount.Max() ? maxErrorCount : errorTrendCount.Max());
            maxErrorPercent = (maxErrorPercent > errorTrendPercent.Max() ? maxErrorPercent : errorTrendPercent.Max());
        }

    }
}
