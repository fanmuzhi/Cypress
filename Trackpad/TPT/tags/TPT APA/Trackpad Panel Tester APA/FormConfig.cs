using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace CypressSemiconductor.ChinaManufacturingTest.AFT
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();

            string[] PortList = {
                                "COM0", "COM1", "COM2", "COM3",
                                "COM4", "COM5", "COM6", "COM7",
                                "COM8", "COM9", null
                                };
            int[] DelayTimeList = { 100, 200, 300, 400, 500 };               
                                
 
            int Found = 0;
            for (int i = 0; PortList[i] != null; ++i)
            {
                if (PortList[i] == HardwareConfig.Port.MPQPort)
                    Found = i;
            }
            ComboBoxMPQPort.SelectedIndex = Found;
           
            Found = 0;
            for (int i = 0; DelayTimeList[i] != 500; ++i)
            {
                if (DelayTimeList[i] == HardwareConfig.Port.MPQDelay)
                    Found = i;
            }
            ComboBoxMPQDelay.SelectedIndex = Found;

            if (HardwareConfig.PowerSupply.VDD_Default == 5.00)
            {
                radioButtonVDD5V.Checked = true;
                radioButtonVDD33V.Checked = false;
            }
            else
            {
                radioButtonVDD5V.Checked = false;
                radioButtonVDD33V.Checked = true;   
            }
            if (HardwareConfig.PowerSupply.VDD_Prog == 5.00)
            {
                radioButtonVprog5V.Checked = true;
                radioButtonVprog33V.Checked = false;
            }
            else
            {
                radioButtonVprog5V.Checked = false;
                radioButtonVprog33V.Checked = true;
            }

            TextBoxU2651AAddr.Text = HardwareConfig.Option.U2651AtxtAddr;
            TextBoxU2722AAddr.Text = HardwareConfig.Option.U2722AtxtAddr;
            TextBoxU2751AWellAAddr.Text = HardwareConfig.Option.U2751AWellAtxtAddr;
            TextBoxU2751AWellBAddr.Text = HardwareConfig.Option.U2751AWellBtxtAddr;
		}

		// OK
		private void ButtonOK_Click(object sender, EventArgs e)
		{
            HardwareConfig.Port.MPQPort = ComboBoxMPQPort.Text;
            HardwareConfig.Port.MPQDelay = Convert.ToInt32(ComboBoxMPQDelay.Text);
            HardwareConfig.Option.U2651AtxtAddr = TextBoxU2651AAddr.Text;
            HardwareConfig.Option.U2722AtxtAddr = TextBoxU2722AAddr.Text;
            HardwareConfig.Option.U2751AWellAtxtAddr = TextBoxU2751AWellAAddr.Text;
            HardwareConfig.Option.U2751AWellBtxtAddr = TextBoxU2751AWellBAddr.Text;
            HardwareConfig.Write();

			Close();
		}

		// Cancel
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButtonVDD5V_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonVDD5V.Checked)
            {
                HardwareConfig.PowerSupply.VDD_Default = 5.00;
                radioButtonVDD33V.Checked = false;
            }
        }

        private void radioButtonVDD33V_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonVDD33V.Checked)
            {
                HardwareConfig.PowerSupply.VDD_Default = 3.30;
                radioButtonVDD5V.Checked = false;
            }
        }

        private void radioButtonVprog5V_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonVprog5V.Checked)
            {
                HardwareConfig.PowerSupply.VDD_Prog = 5.00;
                radioButtonVprog33V.Checked = false;
            }
        }

        private void radioButtonVprog33V_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonVprog33V.Checked)
            {
                HardwareConfig.PowerSupply.VDD_Prog = 3.30;
                radioButtonVprog5V.Checked = false;
            }
        }


    }
}