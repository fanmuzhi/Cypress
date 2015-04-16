namespace CypressSemiconductor.ChinaManufacturingTest.TPT
{
    partial class FormPassword
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
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.confirmPassword = new System.Windows.Forms.Button();
            this.cancellPassword = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(13, 26);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(188, 20);
            this.textBoxPassword.TabIndex = 0;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // confirmPassword
            // 
            this.confirmPassword.Location = new System.Drawing.Point(9, 62);
            this.confirmPassword.Name = "confirmPassword";
            this.confirmPassword.Size = new System.Drawing.Size(84, 29);
            this.confirmPassword.TabIndex = 1;
            this.confirmPassword.Text = "OK";
            this.confirmPassword.UseVisualStyleBackColor = true;
            this.confirmPassword.Click += new System.EventHandler(this.confirmPassword_Click);
            // 
            // cancellPassword
            // 
            this.cancellPassword.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancellPassword.Location = new System.Drawing.Point(117, 62);
            this.cancellPassword.Name = "cancellPassword";
            this.cancellPassword.Size = new System.Drawing.Size(84, 29);
            this.cancellPassword.TabIndex = 1;
            this.cancellPassword.Text = "Cancel";
            this.cancellPassword.UseVisualStyleBackColor = true;
            this.cancellPassword.Click += new System.EventHandler(this.cancellPassword_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Password";
            // 
            // FormPassword
            // 
            this.AcceptButton = this.confirmPassword;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancellPassword;
            this.ClientSize = new System.Drawing.Size(218, 102);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.confirmPassword);
            this.Controls.Add(this.cancellPassword);
            this.Controls.Add(this.textBoxPassword);
            this.Name = "FormPassword";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Enter your Password";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPassword_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button confirmPassword;
        private System.Windows.Forms.Button cancellPassword;
        private System.Windows.Forms.Label label1;
    }
}