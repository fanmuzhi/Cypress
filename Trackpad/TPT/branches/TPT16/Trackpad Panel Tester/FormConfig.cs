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

namespace CypressSemiconductor.ChinaManufacturingTest.TPT
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
           
           

            TextBoxU2651AAddr.Text = HardwareConfig.Option.U2651AtxtAddr;
            TextBoxU2722AAddr.Text = HardwareConfig.Option.U2722AtxtAddr;
            TextBoxU2751AAddr1.Text = HardwareConfig.Option.U2751AtxtAddr1;
            TextBoxU2751AAddr2.Text = HardwareConfig.Option.U2751AtxtAddr2;
		}

		// OK
		private void ButtonOK_Click(object sender, EventArgs e)
		{
            try
            {
                HardwareConfig.Port.MPQPort = ComboBoxMPQPort.Text;
        //        HardwareConfig.Port.ScannerPort = ComboBoxScannerPort.Text;
                HardwareConfig.Option.U2651AtxtAddr = TextBoxU2651AAddr.Text;
                HardwareConfig.Option.U2722AtxtAddr = TextBoxU2722AAddr.Text;
                HardwareConfig.Option.U2751AtxtAddr1 = TextBoxU2751AAddr1.Text;
                HardwareConfig.Option.U2751AtxtAddr2 = TextBoxU2751AAddr2.Text;
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