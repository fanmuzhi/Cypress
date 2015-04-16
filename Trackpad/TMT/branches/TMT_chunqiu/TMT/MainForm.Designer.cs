namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.listBoxPorts = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manufacturingInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productionModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.engineeringModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxSwitch = new System.Windows.Forms.TextBox();
            this.resolutionY = new System.Windows.Forms.TextBox();
            this.resolutionX = new System.Windows.Forms.TextBox();
            this.buttonFail = new System.Windows.Forms.Button();
            this.buttonPass = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PositionPanel = new System.Windows.Forms.Panel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelTestSite = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTestStation = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTestID = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkBoxRecal = new System.Windows.Forms.CheckBox();
            this.textBoxSN = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMPN = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxFingers = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonStart.Location = new System.Drawing.Point(468, 33);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(114, 48);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "START";
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            this.buttonStart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buttonStart_KeyDown);
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxStatus.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStatus.Location = new System.Drawing.Point(3, 256);
            this.textBoxStatus.MaximumSize = new System.Drawing.Size(638, 44);
            this.textBoxStatus.MinimumSize = new System.Drawing.Size(500, 44);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(573, 44);
            this.textBoxStatus.TabIndex = 0;
            this.textBoxStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // listBoxPorts
            // 
            this.listBoxPorts.FormattingEnabled = true;
            this.listBoxPorts.Location = new System.Drawing.Point(14, 19);
            this.listBoxPorts.Name = "listBoxPorts";
            this.listBoxPorts.Size = new System.Drawing.Size(138, 43);
            this.listBoxPorts.TabIndex = 0;
            this.listBoxPorts.SelectedIndexChanged += new System.EventHandler(this.listBoxPorts_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.listBoxPorts);
            this.groupBox2.Location = new System.Drawing.Point(7, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(171, 75);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logInToolStripMenuItem,
            this.logOutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // logInToolStripMenuItem
            // 
            this.logInToolStripMenuItem.Name = "logInToolStripMenuItem";
            this.logInToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.logInToolStripMenuItem.Text = "Log &in";
            this.logInToolStripMenuItem.Click += new System.EventHandler(this.logInToolStripMenuItem_Click);
            // 
            // logOutToolStripMenuItem
            // 
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.logOutToolStripMenuItem.Text = "Log &out";
            this.logOutToolStripMenuItem.Click += new System.EventHandler(this.logOutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.manufacturingInfoToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Enabled = false;
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // manufacturingInfoToolStripMenuItem
            // 
            this.manufacturingInfoToolStripMenuItem.Name = "manufacturingInfoToolStripMenuItem";
            this.manufacturingInfoToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.manufacturingInfoToolStripMenuItem.Text = "Manufacturing Info";
            this.manufacturingInfoToolStripMenuItem.Click += new System.EventHandler(this.manufacturingInfoToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItem2.Text = "&About";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(599, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.productionModeToolStripMenuItem,
            this.engineeringModeToolStripMenuItem,
            this.debugModeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // productionModeToolStripMenuItem
            // 
            this.productionModeToolStripMenuItem.Name = "productionModeToolStripMenuItem";
            this.productionModeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.productionModeToolStripMenuItem.Text = "Production Mode";
            this.productionModeToolStripMenuItem.Click += new System.EventHandler(this.productionModeToolStripMenuItem_Click);
            // 
            // engineeringModeToolStripMenuItem
            // 
            this.engineeringModeToolStripMenuItem.Name = "engineeringModeToolStripMenuItem";
            this.engineeringModeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.engineeringModeToolStripMenuItem.Text = "Engineering Mode";
            this.engineeringModeToolStripMenuItem.Click += new System.EventHandler(this.engineeringModeToolStripMenuItem_Click);
            // 
            // debugModeToolStripMenuItem
            // 
            this.debugModeToolStripMenuItem.Name = "debugModeToolStripMenuItem";
            this.debugModeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.debugModeToolStripMenuItem.Text = "Debug Mode";
            this.debugModeToolStripMenuItem.Click += new System.EventHandler(this.debugModeToolStripMenuItem_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.textBoxFingers);
            this.tabPage4.Controls.Add(this.textBoxSwitch);
            this.tabPage4.Controls.Add(this.resolutionY);
            this.tabPage4.Controls.Add(this.resolutionX);
            this.tabPage4.Controls.Add(this.buttonFail);
            this.tabPage4.Controls.Add(this.buttonPass);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.PositionPanel);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(565, 219);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Position Display";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(367, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 16);
            this.label10.TabIndex = 13;
            this.label10.Text = "Click:";
            // 
            // textBoxSwitch
            // 
            this.textBoxSwitch.Location = new System.Drawing.Point(410, 15);
            this.textBoxSwitch.Name = "textBoxSwitch";
            this.textBoxSwitch.Size = new System.Drawing.Size(45, 20);
            this.textBoxSwitch.TabIndex = 12;
            // 
            // resolutionY
            // 
            this.resolutionY.Location = new System.Drawing.Point(199, 15);
            this.resolutionY.Name = "resolutionY";
            this.resolutionY.Size = new System.Drawing.Size(45, 20);
            this.resolutionY.TabIndex = 4;
            this.resolutionY.Text = "680";
            this.resolutionY.TextChanged += new System.EventHandler(this.resolutionX_TextChanged);
            // 
            // resolutionX
            // 
            this.resolutionX.Location = new System.Drawing.Point(123, 15);
            this.resolutionX.Name = "resolutionX";
            this.resolutionX.Size = new System.Drawing.Size(45, 20);
            this.resolutionX.TabIndex = 2;
            this.resolutionX.Text = "1280";
            this.resolutionX.TextChanged += new System.EventHandler(this.resolutionX_TextChanged);
            // 
            // buttonFail
            // 
            this.buttonFail.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonFail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buttonFail.Location = new System.Drawing.Point(440, 76);
            this.buttonFail.Name = "buttonFail";
            this.buttonFail.Size = new System.Drawing.Size(92, 28);
            this.buttonFail.TabIndex = 10;
            this.buttonFail.Text = "Fail";
            this.buttonFail.UseVisualStyleBackColor = false;
            this.buttonFail.Click += new System.EventHandler(this.buttonFail_Click);
            // 
            // buttonPass
            // 
            this.buttonPass.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buttonPass.Location = new System.Drawing.Point(329, 76);
            this.buttonPass.Name = "buttonPass";
            this.buttonPass.Size = new System.Drawing.Size(92, 28);
            this.buttonPass.TabIndex = 10;
            this.buttonPass.Text = "Pass";
            this.buttonPass.UseVisualStyleBackColor = false;
            this.buttonPass.Click += new System.EventHandler(this.buttonPass_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(176, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Y:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(256, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 16);
            this.label6.TabIndex = 6;
            this.label6.Text = "Fingers:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Resolution:";
            // 
            // PositionPanel
            // 
            this.PositionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PositionPanel.Location = new System.Drawing.Point(3, 48);
            this.PositionPanel.Name = "PositionPanel";
            this.PositionPanel.Size = new System.Drawing.Size(320, 170);
            this.PositionPanel.TabIndex = 3;
            this.PositionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PositionPanelPaint);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.listBoxStatus);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(565, 219);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Parameter Test";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.HorizontalScrollbar = true;
            this.listBoxStatus.Location = new System.Drawing.Point(-1, 6);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(565, 212);
            this.listBoxStatus.TabIndex = 0;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Controls.Add(this.tabPage4);
            this.tabControlMain.Location = new System.Drawing.Point(3, 3);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.Padding = new System.Drawing.Point(8, 4);
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(573, 247);
            this.tabControlMain.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.tabControlMain);
            this.flowLayoutPanel1.Controls.Add(this.textBoxStatus);
            this.flowLayoutPanel1.Controls.Add(this.statusStrip1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 115);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(589, 329);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelTestSite,
            this.toolStripStatusLabelTestStation,
            this.toolStripStatusLabelTestID});
            this.statusStrip1.Location = new System.Drawing.Point(0, 303);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(232, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelTestSite
            // 
            this.toolStripStatusLabelTestSite.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusLabelTestSite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelTestSite.Name = "toolStripStatusLabelTestSite";
            this.toolStripStatusLabelTestSite.Size = new System.Drawing.Size(60, 17);
            this.toolStripStatusLabelTestSite.Text = "Test Site : ";
            this.toolStripStatusLabelTestSite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelTestStation
            // 
            this.toolStripStatusLabelTestStation.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusLabelTestStation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelTestStation.Name = "toolStripStatusLabelTestStation";
            this.toolStripStatusLabelTestStation.Size = new System.Drawing.Size(78, 17);
            this.toolStripStatusLabelTestStation.Text = "Test Station : ";
            this.toolStripStatusLabelTestStation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelTestID
            // 
            this.toolStripStatusLabelTestID.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusLabelTestID.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelTestID.Name = "toolStripStatusLabelTestID";
            this.toolStripStatusLabelTestID.Size = new System.Drawing.Size(77, 17);
            this.toolStripStatusLabelTestID.Text = "Operator ID : ";
            this.toolStripStatusLabelTestID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxRecal
            // 
            this.checkBoxRecal.AutoSize = true;
            this.checkBoxRecal.Location = new System.Drawing.Point(10, 98);
            this.checkBoxRecal.Name = "checkBoxRecal";
            this.checkBoxRecal.Size = new System.Drawing.Size(176, 17);
            this.checkBoxRecal.TabIndex = 6;
            this.checkBoxRecal.Text = "Recalibrate if Rawcount test fail";
            this.checkBoxRecal.UseVisualStyleBackColor = true;
            // 
            // textBoxSN
            // 
            this.textBoxSN.Location = new System.Drawing.Point(6, 41);
            this.textBoxSN.Name = "textBoxSN";
            this.textBoxSN.Size = new System.Drawing.Size(145, 20);
            this.textBoxSN.TabIndex = 0;
            this.textBoxSN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSN.TextChanged += new System.EventHandler(this.textBoxSN_TextChanged);
            this.textBoxSN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxSN_MouseDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Serial Number";
            // 
            // textBoxMPN
            // 
            this.textBoxMPN.Enabled = false;
            this.textBoxMPN.Location = new System.Drawing.Point(157, 41);
            this.textBoxMPN.Name = "textBoxMPN";
            this.textBoxMPN.Size = new System.Drawing.Size(114, 20);
            this.textBoxMPN.TabIndex = 1;
            this.textBoxMPN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(154, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "MPN";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBoxMPN);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBoxSN);
            this.groupBox1.Location = new System.Drawing.Point(180, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // textBoxFingers
            // 
            this.textBoxFingers.Location = new System.Drawing.Point(312, 15);
            this.textBoxFingers.Name = "textBoxFingers";
            this.textBoxFingers.Size = new System.Drawing.Size(45, 20);
            this.textBoxFingers.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 443);
            this.Controls.Add(this.checkBoxRecal);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 480);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trackpad Module Tester For Sorting v1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.ListBox listBoxPorts;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem logInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOutToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxSwitch;
        private System.Windows.Forms.TextBox resolutionY;
        private System.Windows.Forms.TextBox resolutionX;
        private System.Windows.Forms.Button buttonPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PositionPanel;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem productionModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem engineeringModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manufacturingInfoToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTestSite;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTestStation;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTestID;
        private System.Windows.Forms.CheckBox checkBoxRecal;
        private System.Windows.Forms.TextBox textBoxSN;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMPN;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonFail;
        private System.Windows.Forms.TextBox textBoxFingers;
    }
}

