using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public class XmlReadLogs
    {
        public string LastError;

        public DUT_Str read(string path)
        {
            string temp;
            DUT_Str dut = new DUT_Str();
            dut.RawCountX = new List<string>();
            dut.NoiseX = new List<string>();
            dut.StdDevX = new List<string>();
            dut.RawCountY = new List<string>();
            dut.NoiseY = new List<string>();
            dut.StdDevY = new List<string>();
            dut.IDAC = new List<string>();
            dut.IDACGain = new List<string>();
            dut.Global_IDAC = new List<string>();
            dut.Local_IDAC = new List<string>();
            dut.SignalX = new List<string>();
            dut.SNRX = new List<string>();

            XmlTextReader reader;
            try
            {
                reader = new XmlTextReader(path);

                reader.MoveToContent();
                reader.ReadStartElement("Trackpad");
                   
                    reader.ReadStartElement("Serial_Number");
                    temp = reader.ReadString();
                    dut.SerailNumber = temp;
                    dut.PartType = temp.Substring(0, 8);
                    //Console.WriteLine("The Serial_Number is: {0}", temp);
                    reader.ReadEndElement();

                    reader.ReadStartElement("Test_Station");
                    temp = reader.ReadString();
                    dut.TestStation = temp;
                    //Console.WriteLine("The Test_Station is: {0}", temp);
                    reader.ReadEndElement();

                    reader.ReadStartElement("Error_Code");
                    temp = reader.ReadString();
                    dut.ErrorCode = temp;
                    //Console.WriteLine("The Error_Code is: {0}", temp);
                    reader.ReadEndElement();

                    reader.ReadStartElement("Test_Time");
                    temp = reader.ReadString().Substring(0, 19);
                    dut.TestTime = temp;
                    //Console.WriteLine("The Test_Time is: {0}", temp);
                    reader.ReadEndElement();

                    reader.ReadStartElement("IDD_Value");
                    temp = reader.ReadString();
                    dut.IDDValue = temp;
                    //Console.WriteLine("The IDD_Value is: {0}", temp);
                    reader.ReadEndElement();

                    reader.ReadStartElement("Firmware_Revision");
                    temp = reader.ReadString();
                    dut.FwRev = temp;
                    //Console.WriteLine("The Firmware_Revision is: {0}", temp);
                    reader.ReadEndElement();
                    

                    //reader.ReadStartElement("IDD_Sleep1_Value");
                    //temp = reader.ReadString();
                    //dut.IDDValueSleep1 = temp;
                    ////Console.WriteLine("The IDD_Value is: {0}", temp);
                    //reader.ReadEndElement();

                    //reader.ReadStartElement("IDD_Deep_Sleep_Value");
                    //temp = reader.ReadString();
                    //dut.IDDValueDeepSleep = temp;
                    ////Console.WriteLine("The Firmware_Revision is: {0}", temp);
                    //reader.ReadEndElement();

                    reader.ReadStartElement("Raw_Count_Averages");
                    int i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.RawCountX.Add(temp);
                        //Console.WriteLine("Raw_Count_Averages" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                    reader.ReadStartElement("Raw_Count_Noise");
                    i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.NoiseX.Add(temp);
                        //Console.WriteLine("Raw_Count_Noise" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                    reader.ReadStartElement("IDAC_Value");
                    i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.IDAC.Add(temp);
                        //Console.WriteLine("IDAC_Value" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                    reader.ReadStartElement("IDAC_Gain_Value");
                    i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.IDACGain.Add(temp);
                        //Console.WriteLine("IDAC_Value" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                    reader.ReadStartElement("Local_IDAC_Value");
                    i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.Local_IDAC.Add(temp);
                        //Console.WriteLine("IDAC_Value" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                    reader.ReadStartElement("Signal_Data");
                    i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.SignalX.Add(temp);
                        //Console.WriteLine("IDAC_Value" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                    reader.ReadStartElement("SNR_Data");
                    i = 1;
                    while (reader.ReadToNextSibling("D" + i.ToString()))
                    {
                        temp = reader.ReadString();
                        dut.SNRX.Add(temp);
                        //Console.WriteLine("IDAC_Value" + i.ToString() + ": {0}", temp);
                        i++;
                    }
                    reader.ReadEndElement();

                reader.ReadEndElement();

                reader.Close();
            }
            catch
            { }


            return dut;

        }
    }
}

