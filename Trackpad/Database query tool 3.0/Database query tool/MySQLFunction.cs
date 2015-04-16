using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;


namespace Database_query_tool
{
    public class MySQLFunction
    {
        public static string mySQLServerIP = "172.23.6.227";
        public static string mySQLuser = "guest";
        public static string mySQLpwd   = "cypress";
        public static string mySQLDatabase   = "trackpad";

        public MySql.Data.MySqlClient.MySqlDataReader myData;
        public MySql.Data.MySqlClient.MySqlConnection conn;
        public MySql.Data.MySqlClient.MySqlCommand cmd;
        public DataSet DUTds = new DataSet();
        public DataSet IDACds = new DataSet();
        public DataSet RawCountds = new DataSet();
        public DataSet Noiseds = new DataSet();
        public DataSet Commonds = new DataSet();
        public DataTable dataTableDUT = new DataTable();
        public string LastError;
        //public static string Module = "";
        //public delegate void TestMessageEventHandler(object sender, TestMessageEventArgs ea);
        //public event TestMessageEventHandler TextMessageEvent;

        public bool connect()
        {
            string mySQLconnString = "server=" + mySQLServerIP + ";"
                                 + "uid=" + mySQLuser + ";"
                                 + "pwd=" + mySQLpwd + ";"
                                 + "database=" + mySQLDatabase + ";";
            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = mySQLconnString;
                conn.Open();          

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }

        }




    }
}
