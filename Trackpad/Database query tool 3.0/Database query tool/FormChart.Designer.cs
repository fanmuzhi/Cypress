namespace Database_query_tool
{
    partial class FormFPYChart
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.LegendCellColumn legendCellColumn1 = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
            this.Distchart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.Distchart)).BeginInit();
            this.SuspendLayout();
            // 
            // Distchart
            // 
            this.Distchart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.Interval = 1;
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.Name = "ChartArea1";
            this.Distchart.ChartAreas.Add(chartArea1);
            legendCellColumn1.Name = "Column1";
            legend1.CellColumns.Add(legendCellColumn1);
            legend1.Name = "Legend1";
            this.Distchart.Legends.Add(legend1);
            this.Distchart.Location = new System.Drawing.Point(2, 44);
            this.Distchart.Name = "Distchart";
            this.Distchart.Size = new System.Drawing.Size(919, 361);
            this.Distchart.TabIndex = 0;
            this.Distchart.Text = "FPY chart";
            // 
            // FormFPYChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 403);
            this.Controls.Add(this.Distchart);
            this.Name = "FormFPYChart";
            this.Text = "FPY Chart";
            this.Shown += new System.EventHandler(this.FormFPYChart_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Distchart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart Distchart;
    }
}