namespace GoogleGlass
{
    partial class GlassTest
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
            this.textBoxSN = new System.Windows.Forms.TextBox();
            this.textBoxMPN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.checkBoxCallMTK = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxSN
            // 
            this.textBoxSN.Location = new System.Drawing.Point(12, 33);
            this.textBoxSN.Name = "textBoxSN";
            this.textBoxSN.Size = new System.Drawing.Size(120, 20);
            this.textBoxSN.TabIndex = 0;
            this.textBoxSN.TextChanged += new System.EventHandler(this.textBoxSN_TextChanged);
            // 
            // textBoxMPN
            // 
            this.textBoxMPN.Location = new System.Drawing.Point(179, 33);
            this.textBoxMPN.Name = "textBoxMPN";
            this.textBoxMPN.Size = new System.Drawing.Size(100, 20);
            this.textBoxMPN.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "SerialNumber";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "MPN";
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.Location = new System.Drawing.Point(12, 71);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(269, 277);
            this.listBoxStatus.TabIndex = 6;
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new System.Drawing.Point(12, 370);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(267, 20);
            this.textBoxStatus.TabIndex = 7;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(319, 34);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(66, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // checkBoxCallMTK
            // 
            this.checkBoxCallMTK.AutoSize = true;
            this.checkBoxCallMTK.Checked = true;
            this.checkBoxCallMTK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCallMTK.Location = new System.Drawing.Point(319, 71);
            this.checkBoxCallMTK.Name = "checkBoxCallMTK";
            this.checkBoxCallMTK.Size = new System.Drawing.Size(66, 17);
            this.checkBoxCallMTK.TabIndex = 10;
            this.checkBoxCallMTK.Text = "CallMTK";
            this.checkBoxCallMTK.UseVisualStyleBackColor = true;
            // 
            // GlassTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 431);
            this.Controls.Add(this.checkBoxCallMTK);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.listBoxStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxStatus);
            this.Controls.Add(this.textBoxMPN);
            this.Controls.Add(this.textBoxSN);
            this.Name = "GlassTest";
            this.Text = "GlassTestV2.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GoogleGlass_FormClosing);
            this.Load += new System.EventHandler(this.GoogleGlass_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSN;
        private System.Windows.Forms.TextBox textBoxMPN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.CheckBox checkBoxCallMTK;
    }
}

