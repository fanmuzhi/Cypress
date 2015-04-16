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
 
            int Found = 0;
            for (int i = 0; PortList[i] != null; ++i)
            {
                if (PortList[i] == HardwareConfig.Port.MPQPort)
                    Found = i;
            }
            ComboBoxMPQPort.SelectedIndex = Found;
           
            Found = 0;
            for (int i = 0; PortList[i] != null; ++i)
            {
                if (PortList[i] == HardwareConfig.Port.ScannerPort)
                    Found = i;
            }
            ComboBoxScannerPort.SelectedIndex = Found;

            TextBoxU2651AAddr.Text = HardwareConfig.Option.U2651AtxtAddr;
            TextBoxU2722AAddr.Text = HardwareConfig.Option.U2722AtxtAddr;
            TextBoxU2751AWellAAddr.Text = HardwareConfig.Option.U2751AWellAtxtAddr;
            TextBoxU2751AWellBAddr.Text = HardwareConfig.Option.U2751AWellBtxtAddr;
		}

		// OK
		private void ButtonOK_Click(object sender, EventArgs e)
		{
            try
            {
                HardwareConfig.Port.MPQPort = ComboBoxMPQPort.Text;
                HardwareConfig.Port.ScannerPort = ComboBoxScannerPort.Text;
                HardwareConfig.Option.U2651AtxtAddr = TextBoxU2651AAddr.Text;
                HardwareConfig.Option.U2722AtxtAddr = TextBoxU2722AAddr.Text;
                HardwareConfig.Option.U2751AWellAtxtAddr = TextBoxU2751AWellAAddr.Text;
                HardwareConfig.Option.U2751AWellBtxtAddr = TextBoxU2751AWellBAddr.Text;
                HardwareConfig.Write();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write Hardware Settings.ini, "+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
			Close();
		}

		// Cancel
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}