using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Database_query_tool
{
    public partial class FormConfiguration : Form
    {
        public FormConfiguration()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            MySQLFunction.mySQLServerIP = textBoxIPAdress.Text;
            MySQLFunction.mySQLuser = textBoxUserName.Text;
            MySQLFunction.mySQLpwd = textBoxPassword.Text;
            MySQLFunction.mySQLDatabase = textBoxDatabase.Text;
            this.Close();
        }
    }
}
