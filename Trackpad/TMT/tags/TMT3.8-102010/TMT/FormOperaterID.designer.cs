namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
{
    partial class FormOperaterID
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
            this.textBoxWorkerID = new System.Windows.Forms.TextBox();
            this.confirmWorkerID = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxWorkStation = new System.Windows.Forms.ComboBox();
            this.comboBoxWorkSite = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxWorkerID
            // 
            this.textBoxWorkerID.Location = new System.Drawing.Point(12, 141);
            this.textBoxWorkerID.Name = "textBoxWorkerID";
            this.textBoxWorkerID.Size = new System.Drawing.Size(188, 20);
            this.textBoxWorkerID.TabIndex = 0;
            this.textBoxWorkerID.TextChanged += new System.EventHandler(this.textBoxWorkerID_TextChanged);
            // 
            // confirmWorkerID
            // 
            this.confirmWorkerID.Location = new System.Drawing.Point(58, 178);
            this.confirmWorkerID.Name = "confirmWorkerID";
            this.confirmWorkerID.Size = new System.Drawing.Size(100, 29);
            this.confirmWorkerID.TabIndex = 1;
            this.confirmWorkerID.Text = "OK";
            this.confirmWorkerID.UseVisualStyleBackColor = true;
            this.confirmWorkerID.Click += new System.EventHandler(this.confirmWorkerID_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "OperatorID";
            // 
            // comboBoxWorkStation
            // 
            this.comboBoxWorkStation.FormattingEnabled = true;
            this.comboBoxWorkStation.Items.AddRange(new object[] {
            "TPT",
            "TMT",
            "OQC"});
            this.comboBoxWorkStation.Location = new System.Drawing.Point(13, 89);
            this.comboBoxWorkStation.Name = "comboBoxWorkStation";
            this.comboBoxWorkStation.Size = new System.Drawing.Size(188, 21);
            this.comboBoxWorkStation.TabIndex = 3;
            this.comboBoxWorkStation.SelectedIndexChanged += new System.EventHandler(this.comboBoxWorkStation_SelectedIndexChanged);
            // 
            // comboBoxWorkSite
            // 
            this.comboBoxWorkSite.FormattingEnabled = true;
            this.comboBoxWorkSite.Items.AddRange(new object[] {
            "CM 1",
            "CM 2",
            "Others"});
            this.comboBoxWorkSite.Location = new System.Drawing.Point(12, 37);
            this.comboBoxWorkSite.Name = "comboBoxWorkSite";
            this.comboBoxWorkSite.Size = new System.Drawing.Size(188, 21);
            this.comboBoxWorkSite.TabIndex = 4;
            this.comboBoxWorkSite.SelectedIndexChanged += new System.EventHandler(this.comboBoxWorkSite_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Manufaturing Site";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Test Station";
            // 
            // FormOperaterID
            // 
            this.AcceptButton = this.confirmWorkerID;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 219);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxWorkSite);
            this.Controls.Add(this.comboBoxWorkStation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.confirmWorkerID);
            this.Controls.Add(this.textBoxWorkerID);
            this.Name = "FormOperaterID";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please enter your information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxWorkerID;
        private System.Windows.Forms.Button confirmWorkerID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxWorkStation;
        private System.Windows.Forms.ComboBox comboBoxWorkSite;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}