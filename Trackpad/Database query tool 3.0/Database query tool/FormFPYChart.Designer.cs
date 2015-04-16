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
            this.textBoxFPY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxInfoToShow = new System.Windows.Forms.ComboBox();
            this.FPYchart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBoxErrorCode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FPYchart)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxFPY
            // 
            this.textBoxFPY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFPY.Location = new System.Drawing.Point(827, 334);
            this.textBoxFPY.Name = "textBoxFPY";
            this.textBoxFPY.Size = new System.Drawing.Size(79, 20);
            this.textBoxFPY.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(827, 317);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Total FPY:";
            // 
            // comboBoxInfoToShow
            // 
            this.comboBoxInfoToShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxInfoToShow.FormattingEnabled = true;
            this.comboBoxInfoToShow.Items.AddRange(new object[] {
            "FPY",
            "Output",
            "Error Sum",
            "Error Trend",
            "Error Trend(%)"});
            this.comboBoxInfoToShow.Location = new System.Drawing.Point(827, 292);
            this.comboBoxInfoToShow.Name = "comboBoxInfoToShow";
            this.comboBoxInfoToShow.Size = new System.Drawing.Size(79, 21);
            this.comboBoxInfoToShow.TabIndex = 5;
            this.comboBoxInfoToShow.Text = "FPY";
            this.comboBoxInfoToShow.TextChanged += new System.EventHandler(this.comboBoxInfoToShow_TextChanged);
            // 
            // FPYchart
            // 
            chartArea1.AxisX.Interval = 1;
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.Name = "ChartArea1";
            this.FPYchart.ChartAreas.Add(chartArea1);
            this.FPYchart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.AutoFitMinFontSize = 15;
            legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend1.Name = "Legend1";
            this.FPYchart.Legends.Add(legend1);
            this.FPYchart.Location = new System.Drawing.Point(0, 0);
            this.FPYchart.Name = "FPYchart";
            this.FPYchart.Size = new System.Drawing.Size(921, 403);
            this.FPYchart.TabIndex = 0;
            this.FPYchart.Text = "FPY chart";
            // 
            // comboBoxErrorCode
            // 
            this.comboBoxErrorCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxErrorCode.FormattingEnabled = true;
            this.comboBoxErrorCode.Location = new System.Drawing.Point(827, 375);
            this.comboBoxErrorCode.Name = "comboBoxErrorCode";
            this.comboBoxErrorCode.Size = new System.Drawing.Size(79, 21);
            this.comboBoxErrorCode.TabIndex = 5;
            this.comboBoxErrorCode.SelectedIndexChanged += new System.EventHandler(this.comboBoxErrorCode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(827, 358);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Single Error Trend:";
            // 
            // FormFPYChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 403);
            this.Controls.Add(this.comboBoxErrorCode);
            this.Controls.Add(this.comboBoxInfoToShow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxFPY);
            this.Controls.Add(this.FPYchart);
            this.Name = "FormFPYChart";
            this.Text = "FPY Chart";
            this.Shown += new System.EventHandler(this.FormFPYChart_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.FPYchart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFPY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxInfoToShow;
        private System.Windows.Forms.DataVisualization.Charting.Chart FPYchart;
        private System.Windows.Forms.ComboBox comboBoxErrorCode;
        private System.Windows.Forms.Label label2;
    }
}