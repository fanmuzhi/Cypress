using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CypressSemiconductor.ChinaManufacturingTest;
using System.IO;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public partial class FormTrackpadConfig : Form
    {
        string ConfigFilePath = Application.StartupPath + @"\Project Config\";

        TrackpadConfig dutConfig;
        string XML_FILENAME;

        public FormTrackpadConfig()
        {
            InitializeComponent();

            //dutConfig = new TrackpadConfig();
        }

        private void FormTrackpadConfig_Load(object sender, EventArgs e)
        {
            ReadMouleList();
        }

        private void ReadMouleList()
        {
            DirectoryInfo di = new DirectoryInfo(ConfigFilePath);
            if (!di.Exists)
            {
                di.Create();
            }

            FileInfo[] files = di.GetFiles();

            comboBoxModule.Items.Clear();

            foreach (FileInfo file in files)
            {
                if ((file.Extension == ".xml" || file.Extension == ".XML"))
                {
                    comboBoxModule.Items.Add(file.Name);
                }
            }
        }

        private void comboBoxModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                XML_FILENAME = comboBoxModule.SelectedItem.ToString();

                //load from XML file
                dutConfig = ObjectXMLSerializer<TrackpadConfig>.Load(ConfigFilePath + XML_FILENAME);

                LoadSettings();
            }
            catch
            {
                ReadMouleList();
                return; 
            }


        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //save settings to dutConfig and XML file name is not null.
            if (XML_FILENAME != null && SaveSettings())
            {
                //save dut config to XML file
                ObjectXMLSerializer<TrackpadConfig>.Save(dutConfig, ConfigFilePath + XML_FILENAME);
            }

            Close();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            try
            {
                FormNewProjectName newProject = new FormNewProjectName();
                newProject.ShowDialog();

                if (newProject.VailidName)
                {
                    XML_FILENAME = newProject.ProjectName + ".xml";
                    ObjectXMLSerializer<TrackpadConfig>.Save(new TrackpadConfig(), ConfigFilePath + XML_FILENAME);
                }

                ReadMouleList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonLoadPicture_Click(object sender, EventArgs e)
        {
            string FileName = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;

            openFileDialog.Filter = "All picture files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileName = openFileDialog.FileName;
                this.pictureBox1.Image = Image.FromFile(FileName);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            //save settings to dutConfig and XML file name is not null.
            if (XML_FILENAME != null && SaveSettings())
            {
                //save dut config to XML file
                ObjectXMLSerializer<TrackpadConfig>.Save(dutConfig, ConfigFilePath + XML_FILENAME);
            }
            else
            {
                MessageBox.Show("You must enter a valid project name, before you can save!",
                                 Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        #region Load Setttings
        /// <summary>
        /// display the dut config to text boxes.
        /// </summary>
        /// <returns></returns>
        private bool LoadSettings()
        {
            try
            {
                //basic information
                textBoxVddOpPS.Text = dutConfig.VDD_OP_PS.ToString();
                textBoxVddOpIO.Text = dutConfig.VDD_OP_IO.ToString();

                textBoxIddMax.Text = dutConfig.IDD_MAX.ToString();
                textBoxIddMin.Text = dutConfig.IDD_MIN.ToString();
                textBoxIddOpen.Text = dutConfig.IDD_OPEN.ToString();
                textBoxIddShort.Text = dutConfig.IDD_SHORT.ToString();

                textBoxI2CAddress.Text = dutConfig.I2C_ADDRESS.ToString();
                textBoxIDACMap.Text = dutConfig.IDACMap;

                textBoxSNLength.Text = dutConfig.SERIALNUMBER_LENGTH.ToString();
                textBoxResoX.Text = dutConfig.RESOLUTION_X.ToString();
                textBoxResoY.Text = dutConfig.RESOLUTION_Y.ToString();

                //checkbox of Boot loader
                checkBoxBootLoader.Checked = dutConfig.TRACKPAD_BOOTLOADER;

                //radio button of trackpad I2C speed
                if (dutConfig.I2C_SPEED == TPCONFIG.TP_I2C_400K)
                { radioButtonI2CSpeed0.Checked = true; }
                if (dutConfig.I2C_SPEED == TPCONFIG.TP_I2C_100K)
                { radioButtonI2CSpeed1.Checked = true; }
                if (dutConfig.I2C_SPEED == TPCONFIG.TP_I2C_50K)
                { radioButtonI2CSpeed2.Checked = true; }

                //radio button of trackpad platform
                if (dutConfig.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_ST)
                { radioButtonModel1.Checked = true; }
                if (dutConfig.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_MTG)
                { radioButtonModel2.Checked = true; }
                if (dutConfig.TRACKPAD_PLATFORM == TPCONFIG.TP_FUNCTION_APA)
                { radioButtonModel3.Checked = true; }

                //radio button of trackpad interface
                if (dutConfig.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_I2C)
                { radioButtonInterfaceI2C.Checked = true; }
                if (dutConfig.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_PS2)
                { radioButtonInterfacePS2.Checked = true; }
                if (dutConfig.TRACKPAD_INTERFACE == TPCONFIG.TP_INTERFACE_SMB)
                { radioButtonInterfaceSMbus.Checked = true; }

                //radio button of trackpad switch
                if (dutConfig.TRACKPAD_TACTILESWITCH == TPCONFIG.TP_REMOTE_CONTROL)
                { radioButtonSwitch1.Checked = true; }
                if (dutConfig.TRACKPAD_TACTILESWITCH == TPCONFIG.TP_NORMAL_BUTTON)
                { radioButtonSwitch2.Checked = true; }
                if (dutConfig.TRACKPAD_TACTILESWITCH == TPCONFIG.TP_CLICK_PAD)
                { radioButtonSwitch3.Checked = true; }

                //load picture
                this.pictureBox1.Image = (Image)dutConfig.Picture;
                
                //load TPT setting
                radioButtonStationTPT.Checked = true;

                testItems currentTest = dutConfig.TPT;
                loadTestItems(currentTest);
            }
            catch
            { return false; }

            return true;
        }

        private void loadTestItems(testItems temp)
        {
            //Firmware revision 
            checkBoxReadFW.Checked = temp.ReadFW;
            textBoxFwRev.Text = temp.FW_INFO_FW_REV.ToString();
            textBoxFwVer.Text = temp.FW_INFO_FW_VER.ToString();
            textBoxNumCols.Text = temp.FW_INFO_NUM_COLS.ToString();
            textBoxNumRows.Text = temp.FW_INFO_NUM_ROWS.ToString();

            //Switch Test
            checkBoxSwitchTest.Checked = temp.SwitchTest;

            //RawCount
            checkBoxReadRawCount.Checked = temp.ReadRawCount;
            textBoxRawDataReads.Text = temp.RAW_DATA_READS.ToString();
            textBoxRawCountMax.Text = temp.RAW_AVG_MAX.ToString();
            textBoxRawCountMin.Text = temp.RAW_AVG_MIN.ToString();
            textBoxNoiseMax.Text = temp.RAW_NOISE_MAX.ToString();
            textBoxStdDevMax.Text = temp.RAW_STD_DEV_MAX.ToString();

            //IDAC
            checkBoxReadIDAC.Checked = temp.ReadIDAC;
            checkBoxEraseIDAC.Checked = temp.EraseIDAC;
            textBoxIdacMax.Text = temp.IDAC_MAX.ToString();
            textBoxIdacMin.Text = temp.IDAC_MIN.ToString();
            textBoxIDACGainMAX.Text = temp.IDAC_GAIN_MAX.ToString();
            textBoxLocalIDACMax.Text = temp.LOCAL_IDAC_MAX.ToString();
            textBoxLocalIDACMin.Text = temp.LOCAL_IDAC_MIN.ToString();
        }
        #endregion

        #region Save Settings
        /// <summary>
        /// save the text boxes to dut config
        /// </summary>
        /// <returns></returns>
        private bool SaveSettings()
        {
            try
            {
                dutConfig.I2C_ADDRESS = Convert.ToByte(textBoxI2CAddress.Text);
                dutConfig.IDACMap = textBoxIDACMap.Text;
                dutConfig.TRACKPAD_BOOTLOADER = checkBoxBootLoader.Checked;
                dutConfig.RESOLUTION_X = Convert.ToInt32(textBoxResoX.Text);
                dutConfig.RESOLUTION_Y = Convert.ToInt32(textBoxResoY.Text);
                dutConfig.SERIALNUMBER_LENGTH = Convert.ToInt32(textBoxSNLength.Text);

                dutConfig.Picture = (System.Drawing.Bitmap)this.pictureBox1.Image;

                //radio button of Switch
                if (radioButtonSwitch1.Checked)
                { dutConfig.TRACKPAD_TACTILESWITCH = TPCONFIG.TP_REMOTE_CONTROL; }
                if (radioButtonSwitch2.Checked)
                { dutConfig.TRACKPAD_TACTILESWITCH = TPCONFIG.TP_NORMAL_BUTTON; }
                if (radioButtonSwitch3.Checked)
                { dutConfig.TRACKPAD_TACTILESWITCH = TPCONFIG.TP_CLICK_PAD; }

                //raido button of TP platform
                if (radioButtonModel1.Checked)
                { dutConfig.TRACKPAD_PLATFORM = TPCONFIG.TP_FUNCTION_ST; }
                if (radioButtonModel2.Checked)
                { dutConfig.TRACKPAD_PLATFORM = TPCONFIG.TP_FUNCTION_MTG; }
                if (radioButtonModel3.Checked)
                { dutConfig.TRACKPAD_PLATFORM = TPCONFIG.TP_FUNCTION_APA; }

                //radio button of I2C Speed
                if (radioButtonI2CSpeed0.Checked)
                { dutConfig.I2C_SPEED = TPCONFIG.TP_I2C_400K; }
                if (radioButtonI2CSpeed1.Checked)
                { dutConfig.I2C_SPEED = TPCONFIG.TP_I2C_100K; }
                if (radioButtonI2CSpeed2.Checked)
                { dutConfig.I2C_SPEED = TPCONFIG.TP_I2C_50K; }

                //radio button of Interface
                if (radioButtonInterfaceI2C.Checked)
                { dutConfig.TRACKPAD_INTERFACE = TPCONFIG.TP_INTERFACE_I2C; }
                if (radioButtonInterfacePS2.Checked)
                { dutConfig.TRACKPAD_INTERFACE = TPCONFIG.TP_INTERFACE_PS2; }
                if (radioButtonInterfaceSMbus.Checked)
                { dutConfig.TRACKPAD_INTERFACE = TPCONFIG.TP_INTERFACE_SMB; }


                dutConfig.VDD_OP_IO = Convert.ToDouble(textBoxVddOpIO.Text);
                dutConfig.VDD_OP_PS = Convert.ToDouble(textBoxVddOpPS.Text);

                dutConfig.IDD_MAX = Convert.ToDouble(textBoxIddMax.Text);
                dutConfig.IDD_MIN = Convert.ToDouble(textBoxIddMin.Text);
                dutConfig.IDD_OPEN = Convert.ToDouble(textBoxIddOpen.Text);
                dutConfig.IDD_SHORT = Convert.ToDouble(textBoxIddShort.Text);

                if (radioButtonStationTPT.Checked)
                {
                    dutConfig.TPT = saveTestItems();
                }
                if (radioButtonStationTMT.Checked)
                {
                    dutConfig.TMT = saveTestItems();
                }
                if (radioButtonStationOQC.Checked)
                {
                    dutConfig.OQC = saveTestItems();
                }
            }
            catch
            { return false; }

            return true;
        }

        private testItems saveTestItems()
        {
            testItems temp = new testItems();

            //Firmware revision 
            temp.ReadFW = checkBoxReadFW.Checked;
            temp.FW_INFO_FW_REV = Convert.ToByte(textBoxFwRev.Text);
            temp.FW_INFO_FW_VER = Convert.ToByte(textBoxFwVer.Text);
            temp.FW_INFO_NUM_COLS = Convert.ToByte(textBoxNumCols.Text);
            temp.FW_INFO_NUM_ROWS = Convert.ToByte(textBoxNumRows.Text);

            //Swithc Test
            temp.SwitchTest = checkBoxSwitchTest.Checked;

            //RawCount
            temp.ReadRawCount = checkBoxReadRawCount.Checked;
            temp.RAW_DATA_READS = Convert.ToInt32(textBoxRawDataReads.Text);
            temp.RAW_AVG_MAX = Convert.ToInt32(textBoxRawCountMax.Text);
            temp.RAW_AVG_MIN = Convert.ToInt32(textBoxRawCountMin.Text);
            temp.RAW_NOISE_MAX = Convert.ToInt32(textBoxNoiseMax.Text);
            temp.RAW_STD_DEV_MAX = Convert.ToInt32(textBoxStdDevMax.Text);

            //IDAC
            temp.ReadIDAC = checkBoxReadIDAC.Checked;
            temp.EraseIDAC = checkBoxEraseIDAC.Checked;
            temp.IDAC_MAX = Convert.ToByte(textBoxIdacMax.Text);
            temp.IDAC_MIN = Convert.ToByte(textBoxIdacMin.Text);
            temp.IDAC_GAIN_MAX = Convert.ToByte(textBoxIDACGainMAX.Text);
            temp.LOCAL_IDAC_MAX = Convert.ToByte(textBoxLocalIDACMax.Text);
            temp.LOCAL_IDAC_MIN = Convert.ToByte(textBoxLocalIDACMin.Text);

            return temp;
        }
        #endregion

        #region Radio Buttons of test station
        private void radioButtonStationTPT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonStationTPT.Checked)
                {
                    testItems currentTest = dutConfig.TPT;
                    loadTestItems(currentTest);
                }
            }
            catch
            { }
        }

        private void radioButtonStationTMT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonStationTMT.Checked)
                {
                    testItems currentTest = dutConfig.TMT;
                    loadTestItems(currentTest);
                }
            }
            catch
            { }
        }

        private void radioButtonStationOQC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonStationOQC.Checked)
                {
                    testItems currentTest = dutConfig.OQC;
                    loadTestItems(currentTest);
                }
            }
            catch
            { }
        }

        #endregion

        /// <summary>
        /// read particular Part Type xml file
        /// </summary>
        /// <param name="partType"></param> part type of trackpad,eg: 10200100
        public TrackpadConfig read(string partType)
        {
            TrackpadConfig TP = new TrackpadConfig();
            try
            {
                TP = ObjectXMLSerializer<TrackpadConfig>.Load(ConfigFilePath + partType + ".xml");
                return TP;
            }
            catch
            {
                TP = ObjectXMLSerializer<TrackpadConfig>.Load(ConfigFilePath + partType + ".XML");
                return TP;
            }

        }

    }

    public class TPCONFIG
    {
        public const int TP_FUNCTION_ST = 1;
        public const int TP_FUNCTION_MTG = 2;
        public const int TP_FUNCTION_APA = 3;

        public const int TP_NORMAL_BUTTON = 1;
        public const int TP_REMOTE_CONTROL = 2;
        public const int TP_CLICK_PAD = 3;

        public const int TP_INTERFACE_PS2 = 1;
        public const int TP_INTERFACE_I2C = 2;
        public const int TP_INTERFACE_SMB = 3;

        public const byte TP_I2C_400K = 0;
        public const byte TP_I2C_100K = 1;
        public const byte TP_I2C_50K = 2;
    }

    
}
