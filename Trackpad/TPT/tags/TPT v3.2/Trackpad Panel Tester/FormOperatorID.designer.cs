namespace CypressSemiconductor.ChinaManufacturingTest.TPT
{
    partial class FormOperatorID
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
            this.textBoxOperatorID = new System.Windows.Forms.TextBox();
            this.confirmOperatorID = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxOperatorID
            // 
            this.textBoxOperatorID.Location = new System.Drawing.Point(13, 26);
            this.textBoxOperatorID.Name = "textBoxOperatorID";
            this.textBoxOperatorID.Size = new System.Drawing.Size(188, 20);
            this.textBoxOperatorID.TabIndex = 0;
            // 
            // confirmOperatorID
            // 
            this.confirmOperatorID.Location = new System.Drawing.Point(59, 62);
            this.confirmOperatorID.Name = "confirmOperatorID";
            this.confirmOperatorID.Size = new System.Drawing.Size(84, 29);
            this.confirmOperatorID.TabIndex = 1;
            this.confirmOperatorID.Text = "OK";
            this.confirmOperatorID.UseVisualStyleBackColor = true;
            this.confirmOperatorID.Click += new System.EventHandler(this.confirmOperatorID_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "OperatorID";
            // 
            // FormOperatorID
            // 
            this.AcceptButton = this.confirmOperatorID;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 102);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.confirmOperatorID);
            this.Controls.Add(this.textBoxOperatorID);
            this.Name = "FormOperatorID";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Enter your OperatorID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOperatorID_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOperatorID;
        private System.Windows.Forms.Button confirmOperatorID;
        private System.Windows.Forms.Label label1;
    }
}