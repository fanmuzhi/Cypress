namespace CypressSemiconductor.ChinaManufacturingTest.Logitech_RemoteControl
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.PositionPanel = new System.Windows.Forms.Panel();
            this.buttonReadPosition = new System.Windows.Forms.Button();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // PositionPanel
            // 
            this.PositionPanel.BackColor = System.Drawing.Color.White;
            this.PositionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PositionPanel.Location = new System.Drawing.Point(12, 13);
            this.PositionPanel.Name = "PositionPanel";
            this.PositionPanel.Size = new System.Drawing.Size(600, 325);
            this.PositionPanel.TabIndex = 0;
            this.PositionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PositionPanel_Paint);
            // 
            // buttonReadPosition
            // 
            this.buttonReadPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReadPosition.Location = new System.Drawing.Point(12, 351);
            this.buttonReadPosition.Name = "buttonReadPosition";
            this.buttonReadPosition.Size = new System.Drawing.Size(135, 40);
            this.buttonReadPosition.TabIndex = 1;
            this.buttonReadPosition.Text = "START";
            this.buttonReadPosition.UseVisualStyleBackColor = true;
            this.buttonReadPosition.Click += new System.EventHandler(this.buttonReadPosition_Click);
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.Location = new System.Drawing.Point(12, 406);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(596, 106);
            this.listBoxStatus.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 529);
            this.Controls.Add(this.listBoxStatus);
            this.Controls.Add(this.buttonReadPosition);
            this.Controls.Add(this.PositionPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Finger Position V1.02";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PositionPanel;
        private System.Windows.Forms.Button buttonReadPosition;
        private System.Windows.Forms.ListBox listBoxStatus;
    }
}

