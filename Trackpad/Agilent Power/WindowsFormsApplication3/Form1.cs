using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CypressSemiconductor.ChinaManufacturingTest;
using System.Threading;

namespace CypressSemiconductor.ChinaManufacturingTest.VoltageSet
{
    public partial class FormCh1Voltage : Form
    {
        delegate void SetButtonCallback(bool buttonEnablex);

        Agilent agilentDevice;
        bool ch1Toggle = false;
        bool runing = false;

        public FormCh1Voltage()
        {
            InitializeComponent();
            agilentDevice = new Agilent();


        }
        private void SetButton(bool buttonEnable)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.buttonCh1Output.InvokeRequired)
            {
                SetButtonCallback d = new SetButtonCallback(SetButton);
                this.Invoke(d, new object[] { buttonEnable });
            }
            else
            {
                buttonCh1Output.Enabled = buttonEnable;
            }
        }



        private void buttonCh1Output_Click(object sender, EventArgs e)
        {
            if (runing == false)
            {

                Thread ss = new Thread(setOutputVoltage);
                ss.Start();

                buttonCh1Output.Text = "Generating Pulse";
                buttonCh1Output.Enabled = false;
                               
                buttonCh1Output.Text = "Power on";
                //buttonCh1Output.Enabled = true;
            }

        }

        public void setOutputVoltage()
        {
            runing = true;


            //buttonCh1Output.Text = "Power off";
            //textBoxIndicator.Text = "Power on";
            int jj = Convert.ToInt32(textBoxCycles.Text);
            jj++;
            for (int i = 0; i < Convert.ToInt32(textBoxCycles.Text); i++)
            {
                agilentDevice.ActivateChannelOutput(1);
                int delay = Convert.ToInt32(textBoxIntervalOn.Text);
                System.Threading.Thread.Sleep(delay);
                agilentDevice.De_ActivateChannelOutput(1);
                System.Threading.Thread.Sleep(delay);
            }


           
            agilentDevice.De_ActivateChannelOutput(1);

            // buttonCh1Output.Text = "Power on";
             //textBoxIndicator.Text = "Power off";
            SetButton(true);
   
            runing = false;
            // buttonCh1Output_Click(null,null);

        }

        


        private void textBoxVoltage_TextChanged(object sender, EventArgs e)
        {
            agilentDevice.SetChannelVoltage(Convert.ToDouble(textBoxVoltage.Text), 1);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                agilentDevice.InitializeU2722A(textBoxAddress.Text);
                agilentDevice.SetChannelVoltage(Convert.ToDouble(textBoxVoltage.Text), 1);
                buttonConnect.BackColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

     
    }
}
