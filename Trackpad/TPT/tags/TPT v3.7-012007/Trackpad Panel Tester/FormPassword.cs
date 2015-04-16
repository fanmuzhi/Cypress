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
    public partial class FormPassword : Form
    {
        public FormPassword()
        {
            InitializeComponent();
            
        }
        private string passwordString;
        public string Password
        {
            set { passwordString = value; }
            get { return passwordString; }
        }

        private void confirmPassword_Click(object sender, EventArgs e)
        {
            passwordString = textBoxPassword.Text;
            Close();
        }

        private void cancellPassword_Click(object sender, EventArgs e)
        {
            passwordString = null;
            Close();
        }

        private void FormPassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            passwordString = textBoxPassword.Text;

        }
        
    }
}
