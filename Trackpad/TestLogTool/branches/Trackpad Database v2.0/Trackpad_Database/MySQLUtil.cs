using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using CypressSemiconductor.ChinaManufacturingTest;


namespace My_Trackpad_Database
{
    public class MySQLUtil
    {
        public string LastError;

        //string mySQLserverIP    = "172.23.6.12";
        string mySQLserverIP = "localhost";
        string mySQLuser = "root";
        string mySQLpwd = "miranda";
        string mySQLDatabase = "trackpad";
        //MySql.Data.MySqlClient.MySqlDataReader myData;

        MySql.Data.MySqlClient.MySqlConnection conn;
        MySql.Data.MySqlClient.MySqlCommand cmd;

        public bool connect()
        {
            string mySQLconnString = "server=" + mySQLserverIP + ";"
                                  + "uid=" + mySQLuser + ";"
                                  + "pwd=" + mySQLpwd + ";"
                                  + "database=" + mySQLDatabase + ";";
            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = mySQLconnString;
                conn.Open();

                cmd = new MySqlCommand();
                cmd.Connection = conn;

                //cmd.CommandText = "SELECT VERSION()";
                //myData = cmd.ExecuteReader();
                //while (myData.Read())
                //{
                //    Trace.WriteLine("successfully to open " + mySQLDatabase + " @" + mySQLserverIP);
                //    Trace.WriteLine("Database Version: " + myData[0].ToString());
                //}
                //myData.Close();

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool insert_into_table(DUT_Str dut)
        {
            try
            {
                MySql.Data.MySqlClient.MySqlDataReader myData;
                string dut_id = "0";

                //get dut_id from serial_number and test_time.
                bool dataExist = false;
                cmd.CommandText = string.Format("SELECT Id FROM dut WHERE SerialNumber='{0}' AND TestTime='{1}'", dut.SerailNumber, dut.TestTime);
                myData = cmd.ExecuteReader();
                while (myData.Read())
                {
                    //Trace.WriteLine("DUT ID: ", myData[0].ToString());
                    dut_id = myData[0].ToString();
                    dataExist = true;
                }
                myData.Close();

                if (dataExist)
                {
                    LastError = "DUT already exists";
                    return false;
                }

                //foreach (string rawCount in dut.RawCountX)
                //{
                //    string SQL_insert_query = string.Format("INSERT INTO rawcountaverage (DUTID, ValueIndex, RawCountAverage) VALUES ({0},{1},{2})", dut_id, index, rawCount);
                //    index++;
                //    cmd.CommandText = SQL_insert_query;
                //    cmd.ExecuteNonQuery();
                //}

                //index = 1;
                //foreach (string noise in dut.NoiseX)
                //{
                //    string SQL_insert_query = string.Format("INSERT INTO rawcountnoise (DUTID, ValueIndex, RawCountNoise) VALUES ({0},{1},{2})", dut_id, index, noise);
                //    index++;
                //    cmd.CommandText = SQL_insert_query;
                //    cmd.ExecuteNonQuery();
                //}

                //index = 1;
                //foreach (string idac in dut.IDAC)
                //{
                //    string SQL_insert_query = string.Format("INSERT INTO idacvalue (DUTID, ValueIndex, IDACValue) VALUES ({0},{1},{2})", dut_id, index, idac);
                //    index++;
                //    cmd.CommandText = SQL_insert_query;
                //    cmd.ExecuteNonQuery();
                //}

                //insert into dut table.
                string SQL_insert_query_dut = "INSERT INTO dut (SerialNumber,TestStation,PartType,ErrorCode,IDDValue,FirmwareVersion,TestTime) ";
                SQL_insert_query_dut += "VALUES ('" + dut.SerailNumber + "'," + "'" + dut.TestStation + "'," + "'" + dut.PartType + "'," + dut.ErrorCode + "," + dut.IDDValue + "," + dut.FwRev + ",'" + dut.TestTime + "')";

                cmd.CommandText = SQL_insert_query_dut;
                cmd.ExecuteNonQuery();

                //get DUTid from current record
                cmd.CommandText = string.Format("SELECT Id FROM dut WHERE SerialNumber='{0}' AND TestTime='{1}'", dut.SerailNumber, dut.TestTime);
                myData = cmd.ExecuteReader();
                while (myData.Read())
                {
                    //Trace.WriteLine("DUT ID: ", myData[0].ToString());
                    dut_id = myData[0].ToString();
                }
                myData.Close();


                int index;
                //insert RawCount Value
                if (dut.RawCountX.Capacity > 0)
                {
                    index = 1;
                    string SQL_insert_query_rawcount = "INSERT INTO rawcountaverage (DUTID, ValueIndex, RawCountAverage) VALUES ";
                    foreach (string rawcount in dut.RawCountX)
                    {
                        SQL_insert_query_rawcount += "(" + dut_id + "," + index.ToString() + "," + rawcount + "),";
                        index++;
                    }
                    SQL_insert_query_rawcount = SQL_insert_query_rawcount.Substring(0, SQL_insert_query_rawcount.Length - 1);
                    SQL_insert_query_rawcount += ";";

                    //Trace.WriteLine(SQL_insert_query_rawcount);

                    cmd.CommandText = SQL_insert_query_rawcount;
                    cmd.ExecuteNonQuery();
                }

                //insert Noise Value
                if (dut.NoiseX.Capacity > 0)
                {
                    index = 1;
                    string SQL_insert_query_noise = "INSERT INTO rawcountnoise (DUTID, ValueIndex, RawCountNoise) VALUES ";
                    foreach (string noise in dut.NoiseX)
                    {
                        SQL_insert_query_noise += "(" + dut_id + "," + index.ToString() + "," + noise + "),";
                        index++;
                    }
                    SQL_insert_query_noise = SQL_insert_query_noise.Substring(0, SQL_insert_query_noise.Length - 1);
                    SQL_insert_query_noise += ";";

                    //Trace.WriteLine(SQL_insert_query_noise);

                    cmd.CommandText = SQL_insert_query_noise;
                    cmd.ExecuteNonQuery();
                }

                //insert IDAC Value
                if (dut.IDAC.Capacity > 0)
                {
                    index = 1;
                    string SQL_insert_query_idac = "INSERT INTO idacvalue (DUTID, ValueIndex, IDACValue) VALUES ";
                    foreach (string idac in dut.IDAC)
                    {
                        SQL_insert_query_idac += "(" + dut_id + "," + index.ToString() + "," + idac + "),";
                        index++;
                    }
                    SQL_insert_query_idac = SQL_insert_query_idac.Substring(0, SQL_insert_query_idac.Length - 1);
                    SQL_insert_query_idac += ";";

                    //Trace.WriteLine(SQL_insert_query_idac);

                    cmd.CommandText = SQL_insert_query_idac;
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public void disconnect()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }
    }
}
