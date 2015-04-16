namespace Database_query_tool
{
    partial class FormDistChart
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
            this.chartDist = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBoxDist = new System.Windows.Forms.ComboBox();
            this.DistributionPoint = new System.Windows.Forms.Label();
            this.buttonAnalyze = new System.Windows.Forms.Button();
            this.textBoxLow = new System.Windows.Forms.TextBox();
            this.textBoxHigh = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.comboBoxChartType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxShowValue = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartDist)).BeginInit();
            this.SuspendLayout();
            // 
            // chartDist
            // 
            this.chartDist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chartDist.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartDist.Legends.Add(legend1);
            this.chartDist.Location = new System.Drawing.Point(0, 40);
            this.chartDist.Name = "chartDist";
            this.chartDist.Size = new System.Drawing.Size(781, 360);
            this.chartDist.TabIndex = 0;
            this.chartDist.Text = "chartDist";
            // 
            // comboBoxDist
            // 
            this.comboBoxDist.FormattingEnabled = true;
            this.comboBoxDist.Items.AddRange(new object[] {
            "All Points",
            "By sensor(mean)"});
            this.comboBoxDist.Location = new System.Drawing.Point(94, 12);
            this.comboBoxDist.Name = "comboBoxDist";
            this.comboBoxDist.Size = new System.Drawing.Size(121, 21);
            this.comboBoxDist.TabIndex = 1;
            this.comboBoxDist.Text = "All Points";
            // 
            // DistributionPoint
            // 
            this.DistributionPoint.AutoSize = true;
            this.DistributionPoint.Location = new System.Drawing.Point(4, 16);
            this.DistributionPoint.Name = "DistributionPoint";
            this.DistributionPoint.Size = new System.Drawing.Size(86, 13);
            this.DistributionPoint.TabIndex = 2;
            this.DistributionPoint.Text = "DistributionType:";
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new System.Drawing.Point(691, 9);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new System.Drawing.Size(75, 23);
            this.buttonAnalyze.TabIndex = 3;
            this.buttonAnalyze.Text = "Analyze";
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new System.EventHandler(this.buttonAnalyze_Click);
            // 
            // textBoxLow
            // 
            this.textBoxLow.Location = new System.Drawing.Point(294, 12);
            this.textBoxLow.Name = "textBoxLow";
            this.textBoxLow.Size = new System.Drawing.Size(40, 20);
            this.textBoxLow.TabIndex = 5;
            this.textBoxLow.Text = "0";
            // 
            // textBoxHigh
            // 
            this.textBoxHigh.Location = new System.Drawing.Point(364, 12);
            this.textBoxHigh.Name = "textBoxHigh";
            this.textBoxHigh.Size = new System.Drawing.Size(40, 20);
            this.textBoxHigh.TabIndex = 5;
            this.textBoxHigh.Text = "10000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(222, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "X Axis Range:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(341, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "to";
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox.Location = new System.Drawing.Point(676, 334);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(105, 66);
            this.richTextBox.TabIndex = 7;
            this.richTextBox.Text = "";
            // 
            // comboBoxChartType
            // 
            this.comboBoxChartType.FormattingEnabled = true;
            this.comboBoxChartType.Items.AddRange(new object[] {
            "Column",
            "Line",
            "Point",
            "Bar"});
            this.comboBoxChartType.Location = new System.Drawing.Point(478, 11);
            this.comboBoxChartType.Name = "comboBoxChartType";
            this.comboBoxChartType.Size = new System.Drawing.Size(86, 21);
            this.comboBoxChartType.TabIndex = 8;
            this.comboBoxChartType.Text = "Column";
            this.comboBoxChartType.SelectedValueChanged += new System.EventHandler(this.comboBoxChartType_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(415, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Chart Type:";
            // 
            // checkBoxShowValue
            // 
            this.checkBoxShowValue.AutoSize = true;
            this.checkBoxShowValue.Location = new System.Drawing.Point(575, 13);
            this.checkBoxShowValue.Name = "checkBoxShowValue";
            this.checkBoxShowValue.Size = new System.Drawing.Size(80, 17);
            this.checkBoxShowValue.TabIndex = 10;
            this.checkBoxShowValue.Text = "ShowValue";
            this.checkBoxShowValue.UseVisualStyleBackColor = true;
            this.checkBoxShowValue.CheckedChanged += new System.EventHandler(this.checkBoxShowValue_CheckedChanged);
            // 
            // FormDistChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 396);
            this.Controls.Add(this.checkBoxShowValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxChartType);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxHigh);
            this.Controls.Add(this.textBoxLow);
            this.Controls.Add(this.buttonAnalyze);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DistributionPoint);
            this.Controls.Add(this.comboBoxDist);
            this.Controls.Add(this.chartDist);
            this.Name = "FormDistChart";
            this.Text = "FormDistChart";
            this.Shown += new System.EventHandler(this.FormDistChart_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chartDist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartDist;
        private System.Windows.Forms.ComboBox comboBoxDist;
        private System.Windows.Forms.Label DistributionPoint;
        private System.Windows.Forms.Button buttonAnalyze;
        private System.Windows.Forms.TextBox textBoxLow;
        private System.Windows.Forms.TextBox textBoxHigh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.ComboBox comboBoxChartType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxShowValue;
    }
}