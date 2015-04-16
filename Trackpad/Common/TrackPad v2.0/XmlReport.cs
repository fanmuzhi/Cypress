using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackPad
{
    public class XmlReport
    {
        XmlTextWriter writer;
        public string LastError;
        
        public bool OpenReport(string SerailNumber, string Folder, string TestStation)
        {
            try
            {

                //string Folder=System.Windows.Forms.Application.StartupPath + @"\test results\"+DeviceConfig.partType + " " + System.DateTime.Today.ToString("yyyy-MM-dd") + @"\";

                //string filePath = Folder + SerailNumber + ".xml";

                DirectoryInfo di = new DirectoryInfo(Folder);
                if (!di.Exists)     //check if folder exists
                { di.Create(); }    //if not, create the folder


                string filePath = Folder + SerailNumber;
                FileInfo fI = new FileInfo(filePath + ".xml");
                int i = 1;
                while (fI.Exists)
                {
                    string pathTmp = filePath + "(" + i.ToString() + ")";
                    fI = new FileInfo(pathTmp + ".xml");
                    i++;
                }

                writer = new XmlTextWriter(fI.FullName, null);

                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;

                writer.WriteStartDocument();
                writer.WriteComment("XML Test Report Of Trackpad");

                writer.WriteStartElement("Trackpad");
                    
                    //writer.WriteStartElement("Serial_Number");
                    //writer.WriteString(dut.SerailNumber);
                    //writer.WriteEndElement();

                    //writer.WriteElementString("Test_Station", TestStation);
                    //writer.WriteElementString("Error_Code", string.Format("{0:X} ", dut.ErrorCode));

                    //string time = System.DateTime.Now.ToString("u", DateTimeFormatInfo.InvariantInfo);
                    //writer.WriteElementString("Test_Time", time);

                    //writer.WriteElementString("IDD_Value", dut.IDDValue.ToString());
                    //writer.WriteElementString("Firmware_Revision", dut.FwRev.ToString());

                    //writer.WriteStartElement("Raw_Count_Averages");
                    //int i = 1;
                    //foreach (int rawcount in dut.RawCount)
                    //{
                    //    writer.WriteElementString("D" + i.ToString(), rawcount.ToString());
                    //    i++;
                    //}
                    //writer.WriteEndElement();
                    
                    //i = 1;
                    //writer.WriteStartElement("Raw_Count_Noise");
                    //foreach (int noise in dut.Noise)
                    //{
                    //    writer.WriteElementString("D" + i.ToString(), noise.ToString());
                    //    i++;
                    //}
                    //writer.WriteEndElement();

                    //i = 1;
                    //writer.WriteStartElement("IDAC_Value");
                    //foreach (int idac in dut.IDAC)
                    //{
                    //    writer.WriteElementString("D" + i.ToString(), idac.ToString());
                    //    i++;
                    //}
                    //writer.WriteEndElement();

                //writer.WriteEndElement();
                //writer.Flush();
                //writer.Close();

                return true;
                
            }

            catch(Exception ex)
            {
                //Console.Write(ex.Message);
                //Console.Read();
                LastError = ex.Message;
                return false;
            }
        }

        public bool WriteSerialData(string ElementName,List<int> ElementData)
        {
            try
            {
                int i = 1;
                
                if (ElementData.Capacity > 0)
                {
                    writer.WriteStartElement(ElementName);
                    foreach (int data in ElementData)
                    {
                        writer.WriteElementString("D" + i.ToString(), data.ToString());
                        i++;
                    }
                    writer.WriteEndElement();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;    
            }
        }

        public bool WriteSerialData(string ElementName, List<Point> ElementData)
        {
            try
            {
                int i = 1;
                
                if (ElementData.Capacity > 0)
                {
                    writer.WriteStartElement(ElementName);
                    foreach (Point data in ElementData)
                    {
                        writer.WriteElementString("P" + i.ToString(), data.ToString());
                        i++;
                    }
                    writer.WriteEndElement();
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool WriteSerialData(string ElementName, List<double> ElementData)
        {
            try
            {
                int i = 1;
                
                if (ElementData.Capacity > 0)
                {
                    writer.WriteStartElement(ElementName);
                    foreach (double data in ElementData)
                    {
                        writer.WriteElementString("D" + i.ToString(), data.ToString());
                        i++;
                    }
                    writer.WriteEndElement();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool WriteSerialData(string ElementName, List<byte> ElementData)
        {
            try
            {
                int i = 1;
                
                if (ElementData.Capacity > 0)
                {
                    writer.WriteStartElement(ElementName);
                    foreach (byte data in ElementData)
                    {
                        writer.WriteElementString("D" + i.ToString(), data.ToString());
                        i++;
                    }
                    writer.WriteEndElement();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool WriteSingleData(string ElementName, string ElementData)
        {
            try
            {
                writer.WriteElementString(ElementName, ElementData);
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }   
        }

        public bool CloseReport()
        {
            try
            {
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
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
