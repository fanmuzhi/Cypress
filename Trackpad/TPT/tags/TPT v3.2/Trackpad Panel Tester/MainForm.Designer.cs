namespace CypressSemiconductor.ChinaManufacturingTest.TPT
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEMode = new System.Windows.Forms.ToolStripMenuItem();
            this.hardwareConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deviceConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelHardware = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSFCS = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDUT4 = new System.Windows.Forms.TextBox();
            this.textBoxDUT5 = new System.Windows.Forms.TextBox();
            this.textBoxDUT3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxDUT6 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxDUT2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDUT7 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDUT1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDUT8 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.textBoxSNA = new System.Windows.Forms.TextBox();
            this.textBoxMPNA = new System.Windows.Forms.TextBox();
            this.textBoxWellAStatus = new System.Windows.Forms.TextBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.textBoxYieldRate = new System.Windows.Forms.TextBox();
            this.textBoxTotalFailed = new System.Windows.Forms.TextBox();
            this.textBoxTotalTested = new System.Windows.Forms.TextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPMode,
            this.toolStripMenuItemEMode,
            this.hardwareConfigToolStripMenuItem,
            this.deviceConfigToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            resources.ApplyResources(this.toolToolStripMenuItem, "toolToolStripMenuItem");
            // 
            // toolStripMenuItemPMode
            // 
            this.toolStripMenuItemPMode.Name = "toolStripMenuItemPMode";
            resources.ApplyResources(this.toolStripMenuItemPMode, "toolStripMenuItemPMode");
            this.toolStripMenuItemPMode.CheckedChanged += new System.EventHandler(this.ProdTestMode_CheckedChanged);
            this.toolStripMenuItemPMode.Click += new System.EventHandler(this.toolStripMenuItemPMode_Click);
            // 
            // toolStripMenuItemEMode
            // 
            this.toolStripMenuItemEMode.Checked = true;
            this.toolStripMenuItemEMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItemEMode.Name = "toolStripMenuItemEMode";
            resources.ApplyResources(this.toolStripMenuItemEMode, "toolStripMenuItemEMode");
            this.toolStripMenuItemEMode.CheckedChanged += new System.EventHandler(this.ProdTestMode_CheckedChanged);
            this.toolStripMenuItemEMode.Click += new System.EventHandler(this.toolStripMenuItemEMode_Click);
            // 
            // hardwareConfigToolStripMenuItem
            // 
            this.hardwareConfigToolStripMenuItem.Name = "hardwareConfigToolStripMenuItem";
            resources.ApplyResources(this.hardwareConfigToolStripMenuItem, "hardwareConfigToolStripMenuItem");
            this.hardwareConfigToolStripMenuItem.Click += new System.EventHandler(this.hardwareConfigToolStripMenuItem_Click);
            // 
            // deviceConfigToolStripMenuItem
            // 
            this.deviceConfigToolStripMenuItem.Name = "deviceConfigToolStripMenuItem";
            resources.ApplyResources(this.deviceConfigToolStripMenuItem, "deviceConfigToolStripMenuItem");
            this.deviceConfigToolStripMenuItem.Click += new System.EventHandler(this.deviceConfigToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelHardware,
            this.toolStripStatusLabelSFCS,
            this.toolStripStatusProgressBar});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabelHardware
            // 
            this.toolStripStatusLabelHardware.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelHardware.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            resources.ApplyResources(this.toolStripStatusLabelHardware, "toolStripStatusLabelHardware");
            this.toolStripStatusLabelHardware.Name = "toolStripStatusLabelHardware";
            // 
            // toolStripStatusLabelSFCS
            // 
            this.toolStripStatusLabelSFCS.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelSFCS.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusLabelSFCS.Name = "toolStripStatusLabelSFCS";
            resources.ApplyResources(this.toolStripStatusLabelSFCS, "toolStripStatusLabelSFCS");
            // 
            // toolStripStatusProgressBar
            // 
            this.toolStripStatusProgressBar.Name = "toolStripStatusProgressBar";
            resources.ApplyResources(this.toolStripStatusProgressBar, "toolStripStatusProgressBar");
            // 
            // listBoxStatus
            // 
            resources.ApplyResources(this.listBoxStatus, "listBoxStatus");
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Controls.Add(this.textBoxSNA);
            this.groupBox1.Controls.Add(this.textBoxMPNA);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxDUT4);
            this.groupBox2.Controls.Add(this.textBoxDUT5);
            this.groupBox2.Controls.Add(this.textBoxDUT3);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBoxDUT6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxDUT2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxDUT7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxDUT1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxDUT8);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxDUT4
            // 
            this.textBoxDUT4.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT4, "textBoxDUT4");
            this.textBoxDUT4.Name = "textBoxDUT4";
            this.textBoxDUT4.ReadOnly = true;
            this.textBoxDUT4.TabStop = false;
            // 
            // textBoxDUT5
            // 
            this.textBoxDUT5.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT5, "textBoxDUT5");
            this.textBoxDUT5.Name = "textBoxDUT5";
            this.textBoxDUT5.ReadOnly = true;
            this.textBoxDUT5.TabStop = false;
            // 
            // textBoxDUT3
            // 
            this.textBoxDUT3.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT3, "textBoxDUT3");
            this.textBoxDUT3.Name = "textBoxDUT3";
            this.textBoxDUT3.ReadOnly = true;
            this.textBoxDUT3.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // textBoxDUT6
            // 
            this.textBoxDUT6.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT6, "textBoxDUT6");
            this.textBoxDUT6.Name = "textBoxDUT6";
            this.textBoxDUT6.ReadOnly = true;
            this.textBoxDUT6.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // textBoxDUT2
            // 
            this.textBoxDUT2.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT2, "textBoxDUT2");
            this.textBoxDUT2.Name = "textBoxDUT2";
            this.textBoxDUT2.ReadOnly = true;
            this.textBoxDUT2.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textBoxDUT7
            // 
            this.textBoxDUT7.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT7, "textBoxDUT7");
            this.textBoxDUT7.Name = "textBoxDUT7";
            this.textBoxDUT7.ReadOnly = true;
            this.textBoxDUT7.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxDUT1
            // 
            this.textBoxDUT1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT1, "textBoxDUT1");
            this.textBoxDUT1.Name = "textBoxDUT1";
            this.textBoxDUT1.ReadOnly = true;
            this.textBoxDUT1.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxDUT8
            // 
            this.textBoxDUT8.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxDUT8, "textBoxDUT8");
            this.textBoxDUT8.Name = "textBoxDUT8";
            this.textBoxDUT8.ReadOnly = true;
            this.textBoxDUT8.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // textBoxSNA
            // 
            resources.ApplyResources(this.textBoxSNA, "textBoxSNA");
            this.textBoxSNA.Name = "textBoxSNA";
            this.textBoxSNA.TextChanged += new System.EventHandler(this.textBoxSNA_TextChanged);
            // 
            // textBoxMPNA
            // 
            this.textBoxMPNA.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxMPNA, "textBoxMPNA");
            this.textBoxMPNA.Name = "textBoxMPNA";
            this.textBoxMPNA.ReadOnly = true;
            this.textBoxMPNA.TabStop = false;
            // 
            // textBoxWellAStatus
            // 
            this.textBoxWellAStatus.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxWellAStatus, "textBoxWellAStatus");
            this.textBoxWellAStatus.Name = "textBoxWellAStatus";
            this.textBoxWellAStatus.ReadOnly = true;
            this.textBoxWellAStatus.TabStop = false;
            // 
            // groupBox12
            // 
            this.groupBox12.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox12.Controls.Add(this.label40);
            this.groupBox12.Controls.Add(this.label39);
            this.groupBox12.Controls.Add(this.label38);
            this.groupBox12.Controls.Add(this.textBoxYieldRate);
            this.groupBox12.Controls.Add(this.textBoxTotalFailed);
            this.groupBox12.Controls.Add(this.textBoxTotalTested);
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // textBoxYieldRate
            // 
            this.textBoxYieldRate.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxYieldRate, "textBoxYieldRate");
            this.textBoxYieldRate.Name = "textBoxYieldRate";
            this.textBoxYieldRate.ReadOnly = true;
            // 
            // textBoxTotalFailed
            // 
            this.textBoxTotalFailed.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxTotalFailed, "textBoxTotalFailed");
            this.textBoxTotalFailed.Name = "textBoxTotalFailed";
            this.textBoxTotalFailed.ReadOnly = true;
            // 
            // textBoxTotalTested
            // 
            this.textBoxTotalTested.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxTotalTested, "textBoxTotalTested");
            this.textBoxTotalTested.Name = "textBoxTotalTested";
            this.textBoxTotalTested.ReadOnly = true;
            // 
            // groupBox14
            // 
            this.groupBox14.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox14.Controls.Add(this.listBoxStatus);
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox14);
            this.Controls.Add(this.groupBox12);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBoxWellAStatus);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardwareConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelHardware;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxDUT8;
        private System.Windows.Forms.TextBox textBoxDUT1;
        private System.Windows.Forms.TextBox textBoxDUT7;
        private System.Windows.Forms.TextBox textBoxDUT2;
        private System.Windows.Forms.TextBox textBoxDUT6;
        private System.Windows.Forms.TextBox textBoxDUT3;
        private System.Windows.Forms.TextBox textBoxDUT5;
        private System.Windows.Forms.TextBox textBoxDUT4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPMode;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEMode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxWellAStatus;
        private System.Windows.Forms.TextBox textBoxSNA;
        private System.Windows.Forms.TextBox textBoxMPNA;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox textBoxYieldRate;
        private System.Windows.Forms.TextBox textBoxTotalFailed;
        private System.Windows.Forms.TextBox textBoxTotalTested;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSFCS;
        private System.Windows.Forms.ToolStripMenuItem deviceConfigToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripProgressBar toolStripStatusProgressBar;

        
    }
}

