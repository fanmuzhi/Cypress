using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CypressSemiconductor.ChinaManufacturingTest.TPT
{
    public partial class FormOperatorID : Form
    {
        public FormOperatorID()
        {
            InitializeComponent();
            
        }
        private string operatorIDString;
        public string OperatorID
        {
            set { operatorIDString = value; }
            get { return operatorIDString; }
        }

        private void confirmOperatorID_Click(object sender, EventArgs e)
        {
            operatorIDString = textBoxOperatorID.Text;
            Close();
        }

        
        private void FormOperatorID_FormClosing(object sender, FormClosingEventArgs e)
        {
            operatorIDString = textBoxOperatorID.Text;

        }


        
    }
}
