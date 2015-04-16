using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public partial class FormNewProjectName : Form
    {
        private bool validName = false;
        public bool VailidName
        {
            get { return validName; }
        }

        private string m_projectName;
        public string ProjectName
        {
            get { return m_projectName; }
        } 
      
        public FormNewProjectName()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (validName)
            { Close(); }
            else
            {
                MessageBox.Show("You must enter a valid project name.",
                                 Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxProjectName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxProjectName.Text.Length == 8)
            {
                m_projectName = textBoxProjectName.Text;
                validName = true;
            }
            else
            {
                validName = false;
            }
        }


    }
}
