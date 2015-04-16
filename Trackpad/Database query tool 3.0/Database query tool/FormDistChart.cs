using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CypressSemiconductor.ChinaManufacturingTest;

namespace Database_query_tool
{
    public partial class FormDistChart : Form
    {
        List<int> distData = new List<int>();
        List<int> valueList = new List<int>();
        List<int> countList = new List<int>();
        List<double> sensorAve = new List<double>();
        List<int> sensorNum = new List<int>();
        public DataTable distDT = new DataTable();
        Series DistSeries = new Series();
        public string dataTypeString;
        int limitLow;
        int limitHigh;
        double ave = 0;
        double StDev = 0;
        double Cpk = 0;

        public FormDistChart()
        {
            InitializeComponent();
        }

        private void FormDistChart_Shown(object sender, EventArgs e)
        {
            buttonAnalyze.Enabled = false;

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


            for (int i = 0; i < distDT.Rows.Count; i++)
            {
                double sensorSum = 0;
                for (int j = 0; j < distDT.Columns.Count; j++)
                {
                    if (distDT.Rows[i][j].ToString() != "" && distDT.Rows[i][j] != null)
                    {                        
                        int temp = Convert.ToInt32(distDT.Rows[i][j]);
                        sensorSum += temp;
                        distData.Add(Convert.ToInt32(distDT.Rows[i][j]));
                    }

                }
                sensorAve.Add(Convert.ToDouble((sensorSum / distDT.Columns.Count).ToString("F01")));
                
                sensorNum.Add(i+1);
            }
            //remove all the abnormal points
            double ave = StatisticsData.Ave(distData.ToArray());
            double StDev = StatisticsData.Std(distData.ToArray());

            switch (dataTypeString)
            {
                case "RawCountAverage":
                    limitLow = DeviceConfig.Items.RAW_AVG_MIN;
                    limitHigh = DeviceConfig.Items.RAW_AVG_MAX;
                    break;
                case "RawCountNoise":
                    limitLow = 0;
                    limitHigh = DeviceConfig.Items.RAW_NOISE_MAX;
                    break;
                default:
                    limitLow = DeviceConfig.Items.IDAC_MIN;
                    limitHigh = DeviceConfig.Items.IDAC_MAX;
                    break;
            }

            distData.RemoveAll(delegate(int t) { return (t > (ave + 6 * StDev) || t < (ave - 6 * StDev)) && (t > limitHigh || t < limitLow); });

            buttonAnalyze.Enabled = true;

        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {

            string senderM=sender.ToString();

            valueList = new List<int>();
            countList = new List<int>();

            for (int k = (Convert.ToInt32(textBoxLow.Text) > distData.Min() ? Convert.ToInt32(textBoxLow.Text) : distData.Min()); k < (Convert.ToInt32(textBoxHigh.Text) > distData.Max() ? distData.Max() : Convert.ToInt32(textBoxHigh.Text)); k++)
            {

                List<int> temp = distData.FindAll(delegate(int t) { return t == k; });
                if (temp.Count > 0)
                {
                    valueList.Add(k);
                    countList.Add(temp.Count);
                }
                temp.Clear();

            }

           
            ave = StatisticsData.Ave(distData.ToArray());
            StDev = StatisticsData.Std(distData.ToArray());
            if (dataTypeString == "RawCountNoise")
            {
                Cpk = StatisticsData.Cpu(limitLow, limitHigh, ave, StDev);
            }
            else
            {
                Cpk = StatisticsData.Cpk(limitLow, limitHigh, ave, StDev); 
            }            
            

            richTextBox.Text = "\nMean: " + ave.ToString("F02") + "\nStd Dev: " + StDev.ToString("F02") + "\nCpk: " + Cpk.ToString("F02");            
            chartDist.Series.Clear();
            DistSeries = new Series();   
            
            DistSeries.Name = dataTypeString;
            DistSeries.Legend = "Legend1";
            DistSeries.Color = Color.Blue;

            if (comboBoxDist.Text == "All Points")
            {
                DistSeries.Points.DataBindXY(valueList, countList);
                chartDist.ChartAreas[0].AxisY.Minimum = 0;
                //chartDist.ChartAreas[0].AxisY.Maximum = countList.Max() + 5;
                //chartDist.ChartAreas[0].AxisX.Minimum = Convert.ToInt32(textBoxLow.Text);
                //chartDist.ChartAreas[0].AxisX.Maximum = Convert.ToInt32(textBoxHigh.Text);
            }
            else if (comboBoxDist.Text == "By sensor(mean)")
            {
                int index = Convert.ToInt32(textBoxLow.Text);
                int length = Math.Abs(Convert.ToInt32(textBoxHigh.Text) - Convert.ToInt32(textBoxLow.Text));
                if (length > sensorNum.Count - index)
                {
                    length = sensorNum.Count - index;
                }
                
                DistSeries.Points.DataBindXY(sensorNum.GetRange(index,length), sensorAve.GetRange(index,length));
                
                chartDist.ChartAreas[0].AxisY.Minimum = sensorAve.Min() - 5;
                //chartDist.ChartAreas[0].AxisY.Maximum = sensorAve.Max() + 5;
                //chartDist.ChartAreas[0].AxisX.Minimum = Convert.ToInt32(textBoxLow.Text);
                //chartDist.ChartAreas[0].AxisX.Maximum = Convert.ToInt32(textBoxHigh.Text);
            }
            
            DistSeries.IsXValueIndexed = false;
            DistSeries.IsValueShownAsLabel = checkBoxShowValue.Checked;
            switch (comboBoxChartType.Text)
            {
                case "Column":
                    DistSeries.ChartType = SeriesChartType.Column;
                    break;
                case "Line":
                    DistSeries.ChartType = SeriesChartType.Line;
                    break;
                case "Bar":
                    DistSeries.ChartType=SeriesChartType.Bar;
                    break;
                case "Point":
                    DistSeries.ChartType=SeriesChartType.Point;
                    break;
                default:
                    DistSeries.ChartType = SeriesChartType.Column;
                    break;
            }
            
            chartDist.Series.Add(DistSeries);

        }

        private void comboBoxChartType_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (comboBoxChartType.Text)
            {
                case "Column":
                    DistSeries.ChartType = SeriesChartType.Column;
                    break;
                case "Line":
                    DistSeries.ChartType = SeriesChartType.Line;
                    break;
                case "Bar":
                    DistSeries.ChartType = SeriesChartType.Bar;
                    break;
                case "Point":
                    DistSeries.ChartType = SeriesChartType.Point;
                    break;
                default:
                    DistSeries.ChartType = SeriesChartType.Column;
                    break;
            }
        }

        private void checkBoxShowValue_CheckedChanged(object sender, EventArgs e)
        {
            DistSeries.IsValueShownAsLabel = checkBoxShowValue.Checked;
        }
    }
}



