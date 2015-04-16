using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
{
    public partial class FormOperaterID : Form
    {
        public FormOperaterID()
        {
            InitializeComponent();
            
        }
        
        private string workerIDString;
        public string WorkerID
        {
            set { workerIDString = value; }
            get { return workerIDString; }
        }

        private string workerSiteString;
        public string WorkSite
        {
            set { workerSiteString = value; }
            get { return workerSiteString; }
        }

        private string workStationString;
        public string WorkStation
        {
            set { workStationString = value; }
            get { return workStationString; }
        }


        private void confirmWorkerID_Click(object sender, EventArgs e)
        {
            try
            {
                //check value of work stie valid
                if (comboBoxWorkSite.SelectedItem != null)
                {
                    if (comboBoxWorkSite.SelectedItem.ToString() != "")
                        workerSiteString = comboBoxWorkSite.SelectedItem.ToString();
                }
                else
                {
                    comboBoxWorkSite.BackColor = Color.Red;
                    return;
                }

                //check value of work station valid
                if (comboBoxWorkStation.SelectedItem != null)
                {
                    if (comboBoxWorkStation.SelectedItem.ToString() != "")
                        workStationString = comboBoxWorkStation.SelectedItem.ToString();
                }
                else
                {
                    comboBoxWorkStation.BackColor = Color.Red;
                    return;
                }

                //check work id valid
                if (textBoxWorkerID.Text != "")
                {
                    workerIDString = textBoxWorkerID.Text;
                }
                else
                {
                    textBoxWorkerID.BackColor = Color.Red;
                    return;
                }
            }
            catch
            { return; }

            Close();
        }

        private void comboBoxWorkSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxWorkSite.BackColor = Color.White;
        }

        private void comboBoxWorkStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxWorkStation.BackColor = Color.White;
        }

        private void textBoxWorkerID_TextChanged(object sender, EventArgs e)
        {
            textBoxWorkerID.BackColor = Color.White;
        }


    }
}
