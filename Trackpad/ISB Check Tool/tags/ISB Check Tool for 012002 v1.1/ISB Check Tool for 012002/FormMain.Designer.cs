namespace CypressSemiconductor.ChinaManufacturingTest.ISB_Check_Tool_for_012002
{
    partial class FormMain
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
            this.buttonSleep = new System.Windows.Forms.Button();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.buttonDeepSleep = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxDeepSleepResult = new System.Windows.Forms.TextBox();
            this.textBoxSleepResult = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxMPN = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSN = new System.Windows.Forms.TextBox();
            this.richTextBoxDebug = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSleep
            // 
            this.buttonSleep.Enabled = false;
            this.buttonSleep.Location = new System.Drawing.Point(15, 76);
            this.buttonSleep.Name = "buttonSleep";
            this.buttonSleep.Size = new System.Drawing.Size(91, 39);
            this.buttonSleep.TabIndex = 0;
            this.buttonSleep.Text = "Sleep";
            this.buttonSleep.UseVisualStyleBackColor = true;
            this.buttonSleep.Click += new System.EventHandler(this.buttonSleep_Click);
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.Location = new System.Drawing.Point(15, 21);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(282, 225);
            this.listBoxStatus.TabIndex = 2;
            // 
            // buttonDeepSleep
            // 
            this.buttonDeepSleep.Enabled = false;
            this.buttonDeepSleep.Location = new System.Drawing.Point(15, 121);
            this.buttonDeepSleep.Name = "buttonDeepSleep";
            this.buttonDeepSleep.Size = new System.Drawing.Size(91, 39);
            this.buttonDeepSleep.TabIndex = 3;
            this.buttonDeepSleep.Text = "Deep Sleep";
            this.buttonDeepSleep.UseVisualStyleBackColor = true;
            this.buttonDeepSleep.Click += new System.EventHandler(this.buttonDeepSleep_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(15, 30);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(91, 39);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxStatus);
            this.groupBox1.Location = new System.Drawing.Point(13, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 259);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonConnect);
            this.groupBox2.Controls.Add(this.buttonSleep);
            this.groupBox2.Controls.Add(this.buttonDeepSleep);
            this.groupBox2.Location = new System.Drawing.Point(334, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(118, 420);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxDeepSleepResult);
            this.groupBox3.Controls.Add(this.textBoxSleepResult);
            this.groupBox3.Location = new System.Drawing.Point(13, 379);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 54);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            // 
            // textBoxDeepSleepResult
            // 
            this.textBoxDeepSleepResult.Location = new System.Drawing.Point(164, 20);
            this.textBoxDeepSleepResult.Name = "textBoxDeepSleepResult";
            this.textBoxDeepSleepResult.Size = new System.Drawing.Size(127, 20);
            this.textBoxDeepSleepResult.TabIndex = 1;
            // 
            // textBoxSleepResult
            // 
            this.textBoxSleepResult.Location = new System.Drawing.Point(18, 20);
            this.textBoxSleepResult.Name = "textBoxSleepResult";
            this.textBoxSleepResult.Size = new System.Drawing.Size(127, 20);
            this.textBoxSleepResult.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.textBoxMPN);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.textBoxSN);
            this.groupBox4.Location = new System.Drawing.Point(13, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(314, 94);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(163, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "MPN";
            // 
            // textBoxMPN
            // 
            this.textBoxMPN.Enabled = false;
            this.textBoxMPN.Location = new System.Drawing.Point(166, 53);
            this.textBoxMPN.Name = "textBoxMPN";
            this.textBoxMPN.Size = new System.Drawing.Size(114, 20);
            this.textBoxMPN.TabIndex = 1;
            this.textBoxMPN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Serial Number";
            // 
            // textBoxSN
            // 
            this.textBoxSN.Location = new System.Drawing.Point(15, 53);
            this.textBoxSN.Name = "textBoxSN";
            this.textBoxSN.Size = new System.Drawing.Size(145, 20);
            this.textBoxSN.TabIndex = 0;
            this.textBoxSN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSN.TextChanged += new System.EventHandler(this.textBoxSerialNumber_TextChanged);
            // 
            // richTextBoxDebug
            // 
            this.richTextBoxDebug.Location = new System.Drawing.Point(470, 13);
            this.richTextBoxDebug.Name = "richTextBoxDebug";
            this.richTextBoxDebug.Size = new System.Drawing.Size(371, 420);
            this.richTextBoxDebug.TabIndex = 9;
            this.richTextBoxDebug.Text = "";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 450);
            this.Controls.Add(this.richTextBoxDebug);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormMain";
            this.Text = "TMT for 01200200 V1.1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSleep;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.Button buttonDeepSleep;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxDeepSleepResult;
        private System.Windows.Forms.TextBox textBoxSleepResult;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxMPN;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSN;
        private System.Windows.Forms.RichTextBox richTextBoxDebug;
    }
}

