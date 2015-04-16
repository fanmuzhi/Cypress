using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CypressSemiconductor.ChinaManufacturingTest;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public partial class DUTConfig : Form
    {
        public DUTConfig()
        {
            InitializeComponent();

            DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";

            string[] sections = DeviceConfig.ReadModuleList();
            foreach (string section in sections)
            {
                comboBoxModule.Items.Add(section);
            }
        }

        private void SettingLoad()
        {
            if (!DeviceConfig.Read())
            {
                MessageBox.Show("Cannot find " + DeviceConfig.partType.ToString() + " in Production.ini", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBoxVddPS.Text = DeviceConfig.Items.VDD_OP_MAX.ToString();
            textBoxVddIO.Text = DeviceConfig.Items.VDD_OP_MIN.ToString();
            textBoxIddMax.Text = DeviceConfig.Items.IDD_MAX.ToString();
            textBoxIddMin.Text = DeviceConfig.Items.IDD_MIN.ToString();
            textBoxIddOpen.Text = DeviceConfig.Items.IDD_OPEN.ToString();
            textBoxIddShort.Text = DeviceConfig.Items.IDD_SHORT.ToString();

            textBoxI2CAddress.Text = DeviceConfig.Items.I2C_ADDRESS.ToString();

            textBoxRawCountMax.Text = DeviceConfig.Items.RAW_AVG_MAX.ToString();
            textBoxRawCountMin.Text = DeviceConfig.Items.RAW_AVG_MIN.ToString();
            textBoxRawDataReads.Text = DeviceConfig.Items.RAW_DATA_READS.ToString();
            textBoxNoiseMax.Text = DeviceConfig.Items.RAW_NOISE_MAX.ToString();
            textBoxStdDevMax.Text = DeviceConfig.Items.RAW_STD_DEV_MAX.ToString();

            textBoxFwRev.Text = DeviceConfig.Items.FW_INFO_FW_REV.ToString();
            textBoxNumCols.Text = DeviceConfig.Items.FW_INFO_NUM_COLS.ToString();
            textBoxNumRows.Text = DeviceConfig.Items.FW_INFO_NUM_ROWS.ToString();
            textBoxFwVer.Text = DeviceConfig.Items.FW_INFO_FW_Ver.ToString();

            textBoxIdacMax.Text = DeviceConfig.Items.IDAC_MAX.ToString();
            textBoxIdacMin.Text = DeviceConfig.Items.IDAC_MIN.ToString();

            if (DeviceConfig.Items.trackpad_Bootloader == DeviceConfig.TPCONFIG.TP_WITH_BOOTLOADER)
            { checkBoxBootLoader.Checked = true; }
            else
            { checkBoxBootLoader.Checked = false; }

            if (DeviceConfig.Items.trackpad_ShopFloor == DeviceConfig.TPCONFIG.TP_TEST_ONLINE)
            { checkBoxSFCS.Checked = true; }
            else
            { checkBoxSFCS.Checked = false; }

            if (DeviceConfig.Items.trackpad_Function == DeviceConfig.TPCONFIG.TP_FUNCTION_ST)
            { radioButtonModel1.Checked = true; }
            if (DeviceConfig.Items.trackpad_Function == DeviceConfig.TPCONFIG.TP_FUNCTION_MTG)
            { radioButtonModel2.Checked = true; }
            if (DeviceConfig.Items.trackpad_Function == DeviceConfig.TPCONFIG.TP_FUNCTION_APA)
            { radioButtonModel3.Checked = true; }

            if (DeviceConfig.Items.trackpad_Interface == DeviceConfig.TPCONFIG.TP_INTERFACE_I2C)
            { radioButtonInterfaceI2C.Checked = true; }
            if (DeviceConfig.Items.trackpad_Interface == DeviceConfig.TPCONFIG.TP_INTERFACE_PS2)
            { radioButtonInterfacePS2.Checked = true; }
            if (DeviceConfig.Items.trackpad_Interface == DeviceConfig.TPCONFIG.TP_INTERFACE_SMB)
            { radioButtonInterfaceSMbus.Checked = true; }

            if (DeviceConfig.Items.trackpad_TactileSwitch == DeviceConfig.TPCONFIG.TP_REMOTE_CONTROL)
            { radioButtonSwitch1.Checked = true; }
            if (DeviceConfig.Items.trackpad_TactileSwitch == DeviceConfig.TPCONFIG.TP_NORMAL_BUTTON)
            { radioButtonSwitch2.Checked = true; }
            if (DeviceConfig.Items.trackpad_TactileSwitch == DeviceConfig.TPCONFIG.TP_CLICK_PAD)
            { radioButtonSwitch3.Checked = true; }
        }

        //DUT Model
        private void radioButtonModel1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModel1.Checked)
            {
                DeviceConfig.Items.trackpad_Function = DeviceConfig.TPCONFIG.TP_FUNCTION_ST;
                radioButtonModel2.Checked = false;
                radioButtonModel3.Checked = false;
            }
        }

        private void radioButtonModel2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModel2.Checked)
            {
                DeviceConfig.Items.trackpad_Function = DeviceConfig.TPCONFIG.TP_FUNCTION_MTG;
                radioButtonModel1.Checked = false;
                radioButtonModel3.Checked = false;
            }
        }

        private void radioButtonModel3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonModel3.Checked)
            {
                DeviceConfig.Items.trackpad_Function = DeviceConfig.TPCONFIG.TP_FUNCTION_APA;
                radioButtonModel1.Checked = false;
                radioButtonModel2.Checked = false;
            }
        }

        //Shop floor system
        private void checkBoxSFCS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSFCS.Checked)
            {
                DeviceConfig.Items.trackpad_ShopFloor = DeviceConfig.TPCONFIG.TP_TEST_ONLINE;
            }
            else
            {
                DeviceConfig.Items.trackpad_ShopFloor = DeviceConfig.TPCONFIG.TP_TEST_OFFLINE;
            }
        }

        //Boot Loader
        private void checkBoxBootLoader_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxBootLoader.Checked)
            {
                DeviceConfig.Items.trackpad_Bootloader = DeviceConfig.TPCONFIG.TP_WITH_BOOTLOADER;
            }
            else
            {
                DeviceConfig.Items.trackpad_Bootloader = DeviceConfig.TPCONFIG.TP_WITHOUT_BOOTLOADER;
            }
        }


        //DUT interface model
        private void radioButtonInterfaceI2C_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonInterfaceI2C.Checked)
            {
                DeviceConfig.Items.trackpad_Interface = DeviceConfig.TPCONFIG.TP_INTERFACE_I2C;
                radioButtonInterfacePS2.Checked = false;
                radioButtonInterfaceSMbus.Checked = false;
            }
        }

        private void radioButtonInterfacePS2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonInterfacePS2.Checked)
            {
                DeviceConfig.Items.trackpad_Interface = DeviceConfig.TPCONFIG.TP_INTERFACE_PS2;
                radioButtonInterfaceI2C.Checked = false;
                radioButtonInterfaceSMbus.Checked = false;
            }
        }

        private void radioButtonInterfaceSMbus_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonInterfaceSMbus.Checked)
            {
                DeviceConfig.Items.trackpad_Interface = DeviceConfig.TPCONFIG.TP_INTERFACE_SMB;
                radioButtonInterfaceI2C.Checked = false;
                radioButtonInterfacePS2.Checked = false;
            }
        }

        //Tactile Switch Model
        private void radioButtonSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSwitch1.Checked)
            {
                DeviceConfig.Items.trackpad_TactileSwitch = DeviceConfig.TPCONFIG.TP_REMOTE_CONTROL;
                radioButtonSwitch2.Checked = false;
                radioButtonSwitch3.Checked = false;
            }
        }

        private void radioButtonSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSwitch2.Checked)
            {
                DeviceConfig.Items.trackpad_TactileSwitch = DeviceConfig.TPCONFIG.TP_NORMAL_BUTTON;
                radioButtonSwitch1.Checked = false;
                radioButtonSwitch3.Checked = false;
            }
        }

        private void radioButtonSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSwitch3.Checked)
            {
                DeviceConfig.Items.trackpad_TactileSwitch = DeviceConfig.TPCONFIG.TP_CLICK_PAD;
                radioButtonSwitch1.Checked = false;
                radioButtonSwitch2.Checked = false;
            }
        }
        
        //Save or Quit the configuration panel
        private void buttonSettingSave_Click(object sender, EventArgs e)
        {
            try
            {
                DeviceConfig.Items.FW_INFO_FW_REV = Convert.ToByte(textBoxFwRev.Text);
                DeviceConfig.Items.FW_INFO_NUM_COLS = Convert.ToByte(textBoxNumCols.Text);
                DeviceConfig.Items.FW_INFO_NUM_ROWS = Convert.ToByte(textBoxNumRows.Text);
                DeviceConfig.Items.FW_INFO_FW_Ver = Convert.ToByte(textBoxFwVer.Text);

                DeviceConfig.Items.I2C_ADDRESS = Convert.ToByte(textBoxI2CAddress.Text);

                DeviceConfig.Items.IDAC_MAX = Convert.ToByte(textBoxIdacMax.Text);
                DeviceConfig.Items.IDAC_MIN = Convert.ToByte(textBoxIdacMin.Text);

                DeviceConfig.Items.RAW_AVG_MAX = Convert.ToInt32(textBoxRawCountMax.Text);
                DeviceConfig.Items.RAW_AVG_MIN = Convert.ToInt32(textBoxRawCountMin.Text);
                DeviceConfig.Items.RAW_DATA_READS = Convert.ToInt32(textBoxRawDataReads.Text);
                DeviceConfig.Items.RAW_NOISE_MAX = Convert.ToInt32(textBoxNoiseMax.Text);
                DeviceConfig.Items.RAW_STD_DEV_MAX = Convert.ToInt32(textBoxStdDevMax.Text);

                DeviceConfig.Items.IDD_MAX = Convert.ToDouble(textBoxIddMax.Text);
                DeviceConfig.Items.IDD_MIN = Convert.ToDouble(textBoxIddMin.Text);
                DeviceConfig.Items.IDD_OPEN = Convert.ToDouble(textBoxIddOpen.Text);
                DeviceConfig.Items.IDD_SHORT = Convert.ToDouble(textBoxIddShort.Text);

                DeviceConfig.Items.VDD_OP_MAX = Convert.ToDouble(textBoxVddPS.Text);
                DeviceConfig.Items.VDD_OP_MIN = Convert.ToDouble(textBoxVddIO.Text);

                //DeviceConfig.filePath = Application.StartupPath + "\\Production.ini";
                DeviceConfig.partType = comboBoxModule.SelectedItem.ToString();
                DeviceConfig.Write();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write Production.ini, " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Close();
        }

        private void buttonSettingLoad_Click(object sender, EventArgs e)
        {
            try
            {
                DeviceConfig.partType = comboBoxModule.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            SettingLoad();

        }
    }
}
