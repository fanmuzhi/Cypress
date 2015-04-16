namespace Database_query_tool
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testDatabaseConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSelectedRawCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSelectedRawCountToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSelectedRawCountToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.searchAndSaveIDDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.passYieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noiseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iDACToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btSEARCH = new System.Windows.Forms.Button();
            this.btCLEAR = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.comboBoxSearchType = new System.Windows.Forms.ComboBox();
            this.cmbTestStation = new System.Windows.Forms.ComboBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.cmbIDDValue = new System.Windows.Forms.ComboBox();
            this.cmbErrorCode = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbFWVersion = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbIDDValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbErrorCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewTrackpad = new System.Windows.Forms.DataGridView();
            this.tbResultCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonRawCount = new System.Windows.Forms.Button();
            this.buttonNoise = new System.Windows.Forms.Button();
            this.buttonIDAC = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.buttonIDD = new System.Windows.Forms.Button();
            this.tPTIDDAnalyzeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTrackpad)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.saveFilesToolStripMenuItem,
            this.analyzeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1094, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem1,
            this.testDatabaseConnectionToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // configurationToolStripMenuItem1
            // 
            this.configurationToolStripMenuItem1.Name = "configurationToolStripMenuItem1";
            this.configurationToolStripMenuItem1.Size = new System.Drawing.Size(212, 22);
            this.configurationToolStripMenuItem1.Text = "&Configuration";
            this.configurationToolStripMenuItem1.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // testDatabaseConnectionToolStripMenuItem
            // 
            this.testDatabaseConnectionToolStripMenuItem.Name = "testDatabaseConnectionToolStripMenuItem";
            this.testDatabaseConnectionToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.testDatabaseConnectionToolStripMenuItem.Text = "&Test Database Connection";
            this.testDatabaseConnectionToolStripMenuItem.Click += new System.EventHandler(this.testDatabaseToolStripMenuItem_Click);
            // 
            // saveFilesToolStripMenuItem
            // 
            this.saveFilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveSelectedRawCountToolStripMenuItem,
            this.saveSelectedRawCountToolStripMenuItem1,
            this.saveSelectedRawCountToolStripMenuItem2,
            this.searchAndSaveIDDToolStripMenuItem});
            this.saveFilesToolStripMenuItem.Name = "saveFilesToolStripMenuItem";
            this.saveFilesToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.saveFilesToolStripMenuItem.Text = "Save Files";
            // 
            // saveSelectedRawCountToolStripMenuItem
            // 
            this.saveSelectedRawCountToolStripMenuItem.Name = "saveSelectedRawCountToolStripMenuItem";
            this.saveSelectedRawCountToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.saveSelectedRawCountToolStripMenuItem.Text = "Search and Save RawCount";
            this.saveSelectedRawCountToolStripMenuItem.Click += new System.EventHandler(this.saveSelectedRawCountToolStripMenuItem_Click);
            // 
            // saveSelectedRawCountToolStripMenuItem1
            // 
            this.saveSelectedRawCountToolStripMenuItem1.Name = "saveSelectedRawCountToolStripMenuItem1";
            this.saveSelectedRawCountToolStripMenuItem1.Size = new System.Drawing.Size(217, 22);
            this.saveSelectedRawCountToolStripMenuItem1.Text = "Search and Save Noise";
            this.saveSelectedRawCountToolStripMenuItem1.Click += new System.EventHandler(this.saveSelectedRawCountNoiseToolStripMenuItem_Click);
            // 
            // saveSelectedRawCountToolStripMenuItem2
            // 
            this.saveSelectedRawCountToolStripMenuItem2.Name = "saveSelectedRawCountToolStripMenuItem2";
            this.saveSelectedRawCountToolStripMenuItem2.Size = new System.Drawing.Size(217, 22);
            this.saveSelectedRawCountToolStripMenuItem2.Text = "Search and Save IDAC";
            this.saveSelectedRawCountToolStripMenuItem2.Click += new System.EventHandler(this.saveSelectedIDACValueToolStripMenuItem_Click);
            // 
            // searchAndSaveIDDToolStripMenuItem
            // 
            this.searchAndSaveIDDToolStripMenuItem.Name = "searchAndSaveIDDToolStripMenuItem";
            this.searchAndSaveIDDToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.searchAndSaveIDDToolStripMenuItem.Text = "Search and Save IDD";
            this.searchAndSaveIDDToolStripMenuItem.Click += new System.EventHandler(this.saveSelectedIDDToolStripMenuItem_Click);
            // 
            // analyzeToolStripMenuItem
            // 
            this.analyzeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.passYieldToolStripMenuItem,
            this.rawCountToolStripMenuItem,
            this.noiseToolStripMenuItem,
            this.iDACToolStripMenuItem,
            this.tPTIDDAnalyzeToolStripMenuItem});
            this.analyzeToolStripMenuItem.Name = "analyzeToolStripMenuItem";
            this.analyzeToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.analyzeToolStripMenuItem.Text = "Analyze";
            // 
            // passYieldToolStripMenuItem
            // 
            this.passYieldToolStripMenuItem.Name = "passYieldToolStripMenuItem";
            this.passYieldToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.passYieldToolStripMenuItem.Text = "First Pass Yield";
            this.passYieldToolStripMenuItem.Click += new System.EventHandler(this.passYieldToolStripMenuItem_Click);
            // 
            // rawCountToolStripMenuItem
            // 
            this.rawCountToolStripMenuItem.Name = "rawCountToolStripMenuItem";
            this.rawCountToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.rawCountToolStripMenuItem.Text = "RawCountAnalyze";
            this.rawCountToolStripMenuItem.Click += new System.EventHandler(this.rawCountToolStripMenuItem_Click);
            // 
            // noiseToolStripMenuItem
            // 
            this.noiseToolStripMenuItem.Name = "noiseToolStripMenuItem";
            this.noiseToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.noiseToolStripMenuItem.Text = "NoiseAnalyze";
            this.noiseToolStripMenuItem.Click += new System.EventHandler(this.noiseToolStripMenuItem_Click);
            // 
            // iDACToolStripMenuItem
            // 
            this.iDACToolStripMenuItem.Name = "iDACToolStripMenuItem";
            this.iDACToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.iDACToolStripMenuItem.Text = "IDACAnalyze";
            this.iDACToolStripMenuItem.Click += new System.EventHandler(this.iDACToolStripMenuItem_Click);
            // 
            // btSEARCH
            // 
            this.btSEARCH.Location = new System.Drawing.Point(9, 150);
            this.btSEARCH.Name = "btSEARCH";
            this.btSEARCH.Size = new System.Drawing.Size(75, 25);
            this.btSEARCH.TabIndex = 14;
            this.btSEARCH.Text = "Search";
            this.btSEARCH.UseVisualStyleBackColor = true;
            this.btSEARCH.Click += new System.EventHandler(this.btSEARCH_Click);
            // 
            // btCLEAR
            // 
            this.btCLEAR.Location = new System.Drawing.Point(88, 150);
            this.btCLEAR.Name = "btCLEAR";
            this.btCLEAR.Size = new System.Drawing.Size(75, 25);
            this.btCLEAR.TabIndex = 12;
            this.btCLEAR.Text = "Clear";
            this.btCLEAR.UseVisualStyleBackColor = true;
            this.btCLEAR.Click += new System.EventHandler(this.btCLEAR_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Controls.Add(this.comboBoxSearchType);
            this.groupBox1.Controls.Add(this.cmbTestStation);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.cmbModel);
            this.groupBox1.Controls.Add(this.cmbIDDValue);
            this.groupBox1.Controls.Add(this.cmbErrorCode);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tbFWVersion);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbIDDValue);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbErrorCode);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbSN);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(9, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1073, 65);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(787, 31);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(103, 20);
            this.dateTimePicker2.TabIndex = 20;
            // 
            // comboBoxSearchType
            // 
            this.comboBoxSearchType.FormattingEnabled = true;
            this.comboBoxSearchType.Items.AddRange(new object[] {
            "AllLogs",
            "FailedUnits",
            "UnitsWithFailLogs",
            "Output"});
            this.comboBoxSearchType.Location = new System.Drawing.Point(911, 30);
            this.comboBoxSearchType.Name = "comboBoxSearchType";
            this.comboBoxSearchType.Size = new System.Drawing.Size(100, 21);
            this.comboBoxSearchType.TabIndex = 16;
            this.comboBoxSearchType.Text = "AllLogs";
            // 
            // cmbTestStation
            // 
            this.cmbTestStation.FormattingEnabled = true;
            this.cmbTestStation.Items.AddRange(new object[] {
            "TMT",
            "TPT",
            "IDD"});
            this.cmbTestStation.Location = new System.Drawing.Point(117, 32);
            this.cmbTestStation.Name = "cmbTestStation";
            this.cmbTestStation.Size = new System.Drawing.Size(100, 21);
            this.cmbTestStation.TabIndex = 16;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(672, 31);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(103, 20);
            this.dateTimePicker1.TabIndex = 20;
            this.dateTimePicker1.Value = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            // 
            // cmbModel
            // 
            this.cmbModel.FormattingEnabled = true;
            this.cmbModel.Items.AddRange(new object[] {
            "10200100",
            "10200500",
            "10200600",
            "10300300",
            "11200100",
            "01200100",
            "01200200",
            "11600100",
            "11600200",
            "11600300",
            "11600400",
            "11600500",
            "11600600",
            "11600700",
            "11600800",
            "11600900",
            "10300200",
            "10100300",
            "01200500"});
            this.cmbModel.Location = new System.Drawing.Point(230, 32);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(100, 21);
            this.cmbModel.TabIndex = 16;
            // 
            // cmbIDDValue
            // 
            this.cmbIDDValue.FormattingEnabled = true;
            this.cmbIDDValue.Items.AddRange(new object[] {
            "=",
            ">",
            "<"});
            this.cmbIDDValue.Location = new System.Drawing.Point(458, 32);
            this.cmbIDDValue.Name = "cmbIDDValue";
            this.cmbIDDValue.Size = new System.Drawing.Size(32, 21);
            this.cmbIDDValue.TabIndex = 16;
            this.cmbIDDValue.Text = "=";
            // 
            // cmbErrorCode
            // 
            this.cmbErrorCode.FormattingEnabled = true;
            this.cmbErrorCode.Items.AddRange(new object[] {
            "=",
            ">",
            "<"});
            this.cmbErrorCode.Location = new System.Drawing.Point(341, 32);
            this.cmbErrorCode.Name = "cmbErrorCode";
            this.cmbErrorCode.Size = new System.Drawing.Size(32, 21);
            this.cmbErrorCode.TabIndex = 16;
            this.cmbErrorCode.Text = "=";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(784, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Test Time End:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(675, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Test Time Start:";
            // 
            // tbFWVersion
            // 
            this.tbFWVersion.Location = new System.Drawing.Point(561, 32);
            this.tbFWVersion.Name = "tbFWVersion";
            this.tbFWVersion.Size = new System.Drawing.Size(100, 20);
            this.tbFWVersion.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(565, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "FW Ver:";
            // 
            // tbIDDValue
            // 
            this.tbIDDValue.Location = new System.Drawing.Point(491, 32);
            this.tbIDDValue.Name = "tbIDDValue";
            this.tbIDDValue.Size = new System.Drawing.Size(55, 20);
            this.tbIDDValue.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(457, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "IDD Value:";
            // 
            // tbErrorCode
            // 
            this.tbErrorCode.Location = new System.Drawing.Point(376, 32);
            this.tbErrorCode.Name = "tbErrorCode";
            this.tbErrorCode.Size = new System.Drawing.Size(63, 20);
            this.tbErrorCode.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(341, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Error Code:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(920, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Search Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(230, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Model:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Test Station:";
            // 
            // tbSN
            // 
            this.tbSN.Location = new System.Drawing.Point(6, 32);
            this.tbSN.Name = "tbSN";
            this.tbSN.Size = new System.Drawing.Size(100, 20);
            this.tbSN.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Serial Number:";
            // 
            // dataGridViewTrackpad
            // 
            this.dataGridViewTrackpad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTrackpad.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewTrackpad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTrackpad.Location = new System.Drawing.Point(-1, 181);
            this.dataGridViewTrackpad.Name = "dataGridViewTrackpad";
            this.dataGridViewTrackpad.Size = new System.Drawing.Size(1083, 293);
            this.dataGridViewTrackpad.TabIndex = 15;
            this.dataGridViewTrackpad.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridViewTrackpad_RowsRemoved);
            // 
            // tbResultCount
            // 
            this.tbResultCount.Location = new System.Drawing.Point(300, 154);
            this.tbResultCount.Name = "tbResultCount";
            this.tbResultCount.Size = new System.Drawing.Size(79, 20);
            this.tbResultCount.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(247, 158);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Searched:";
            // 
            // buttonRawCount
            // 
            this.buttonRawCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRawCount.Location = new System.Drawing.Point(732, 151);
            this.buttonRawCount.Name = "buttonRawCount";
            this.buttonRawCount.Size = new System.Drawing.Size(75, 25);
            this.buttonRawCount.TabIndex = 9;
            this.buttonRawCount.Text = "RawCount";
            this.buttonRawCount.UseVisualStyleBackColor = true;
            this.buttonRawCount.Click += new System.EventHandler(this.buttonRawCount_Click);
            // 
            // buttonNoise
            // 
            this.buttonNoise.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNoise.Location = new System.Drawing.Point(823, 151);
            this.buttonNoise.Name = "buttonNoise";
            this.buttonNoise.Size = new System.Drawing.Size(75, 25);
            this.buttonNoise.TabIndex = 9;
            this.buttonNoise.Text = "Noise";
            this.buttonNoise.UseVisualStyleBackColor = true;
            this.buttonNoise.Click += new System.EventHandler(this.buttonNoise_Click);
            // 
            // buttonIDAC
            // 
            this.buttonIDAC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonIDAC.Location = new System.Drawing.Point(913, 151);
            this.buttonIDAC.Name = "buttonIDAC";
            this.buttonIDAC.Size = new System.Drawing.Size(75, 25);
            this.buttonIDAC.TabIndex = 9;
            this.buttonIDAC.Text = "IDAC";
            this.buttonIDAC.UseVisualStyleBackColor = true;
            this.buttonIDAC.Click += new System.EventHandler(this.buttonIDAC_Click);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(167, 150);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 25);
            this.btSave.TabIndex = 12;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.Location = new System.Drawing.Point(126, 108);
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.Size = new System.Drawing.Size(956, 20);
            this.textBoxCommand.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 111);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "SQL Command:";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 477);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1094, 22);
            this.statusStrip.TabIndex = 19;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(900, 17);
            this.toolStripStatusLabel.Text = "Status";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.AutoSize = false;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // buttonIDD
            // 
            this.buttonIDD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonIDD.Location = new System.Drawing.Point(1003, 151);
            this.buttonIDD.Name = "buttonIDD";
            this.buttonIDD.Size = new System.Drawing.Size(75, 25);
            this.buttonIDD.TabIndex = 9;
            this.buttonIDD.Text = "IDD";
            this.buttonIDD.UseVisualStyleBackColor = true;
            this.buttonIDD.Click += new System.EventHandler(this.buttonIDD_Click);
            // 
            // tPTIDDAnalyzeToolStripMenuItem
            // 
            this.tPTIDDAnalyzeToolStripMenuItem.Name = "tPTIDDAnalyzeToolStripMenuItem";
            this.tPTIDDAnalyzeToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.tPTIDDAnalyzeToolStripMenuItem.Text = "TPTIDDAnalyze";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 499);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBoxCommand);
            this.Controls.Add(this.dataGridViewTrackpad);
            this.Controls.Add(this.btSEARCH);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.btCLEAR);
            this.Controls.Add(this.buttonIDD);
            this.Controls.Add(this.buttonIDAC);
            this.Controls.Add(this.buttonNoise);
            this.Controls.Add(this.buttonRawCount);
            this.Controls.Add(this.tbResultCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(948, 537);
            this.Name = "MainForm";
            this.Text = "Database Query Tool (V3.1)";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTrackpad)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btSEARCH;
        private System.Windows.Forms.Button btCLEAR;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbErrorCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbFWVersion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbIDDValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dataGridViewTrackpad;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbIDDValue;
        private System.Windows.Forms.ComboBox cmbErrorCode;
        private System.Windows.Forms.TextBox tbResultCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.ComboBox cmbTestStation;
        private System.Windows.Forms.Button buttonRawCount;
        private System.Windows.Forms.Button buttonNoise;
        private System.Windows.Forms.Button buttonIDAC;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.TextBox textBoxCommand;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem testDatabaseConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSelectedRawCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSelectedRawCountToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveSelectedRawCountToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem analyzeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem passYieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noiseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iDACToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.ComboBox comboBoxSearchType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonIDD;
        private System.Windows.Forms.ToolStripMenuItem searchAndSaveIDDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tPTIDDAnalyzeToolStripMenuItem;
    }
}

