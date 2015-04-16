using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    class Config
    {
        public static string MULTIMETER_ADDRESS = "U3606A";

        public static double SLEEP1_MAX = 85;
        public static double SLEEP1_MIN = 10;

        public static double DEEP_SLEEP_MAX = 5;
        public static double DEEP_SLEEP_MIN = 0.1;

        public static int MEAS_TIMES = 15;


        public static int PARTNUMBER_LENGTH = 8;
        public static int SERIAL_NUMBER_LENGTH = 19;

        public static bool ONLINE = true;

        public class Delay
        {
            public static int Measurement = 500;

            public static int BridgePowerON = 1000;

            public static int EnterSleepMode = 500;
        }


        public static void read()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader("config.xml");
                string temp;

                reader.MoveToContent();
                reader.ReadStartElement("CONFIG"); // <CONFIG>

                reader.ReadStartElement("MULTIMETER_ADDRESS");
                temp = reader.ReadString();
                temp.Trim();
                MULTIMETER_ADDRESS = temp;
                Trace.WriteLine("Config: MULTIMETER_ADDRESS is: " + MULTIMETER_ADDRESS);
                reader.ReadEndElement();

                reader.ReadStartElement("SLEEP1_MAX");
                temp = reader.ReadString();
                temp.Trim();
                SLEEP1_MAX = Convert.ToDouble(temp);
                Trace.WriteLine("Config: The SLEEP1_MAX is: " + SLEEP1_MAX.ToString());
                reader.ReadEndElement();

                reader.ReadStartElement("SLEEP1_MIN");
                temp = reader.ReadString();
                temp.Trim();
                SLEEP1_MIN = Convert.ToDouble(temp);
                Trace.WriteLine("Config: The SLEEP1_MIN is: " + SLEEP1_MIN.ToString());
                reader.ReadEndElement();

                reader.ReadStartElement("DEEP_SLEEP_MAX");
                temp = reader.ReadString();
                temp.Trim();
                DEEP_SLEEP_MAX = Convert.ToDouble(temp);
                Trace.WriteLine("Config: The DEEP_SLEEP_MAX is: " + DEEP_SLEEP_MAX.ToString());
                reader.ReadEndElement();

                reader.ReadStartElement("DEEP_SLEEP_MIN");
                temp = reader.ReadString();
                temp.Trim();
                DEEP_SLEEP_MIN = Convert.ToDouble(temp);
                Trace.WriteLine("Config: The DEEP_SLEEP_MIN is: " + DEEP_SLEEP_MIN.ToString());
                reader.ReadEndElement();

                reader.ReadStartElement("MEAS_TIMES");
                temp = reader.ReadString();
                temp.Trim();
                MEAS_TIMES = Convert.ToInt32(temp);
                Trace.WriteLine("Config: The MEAS_TIMES is: " + MEAS_TIMES.ToString());
                reader.ReadEndElement();

                reader.ReadStartElement("ONLINE");
                temp = reader.ReadString();
                temp.Trim();
                if (temp == "true")
                {
                    ONLINE = true;
                }
                else
                {
                    ONLINE = false;
                }
                Trace.WriteLine("Config: The ONLINE is: " + ONLINE);
                reader.ReadEndElement();

                reader.ReadStartElement("DELAY"); // <DELAY>

                //MEASUREMENT
                reader.ReadStartElement("MEASUREMENT");
                temp = reader.ReadString();
                temp.Trim();
                Delay.Measurement = Convert.ToInt32(temp);
                Trace.WriteLine("Delay: MEASUREMENT is: " + Delay.Measurement.ToString());
                reader.ReadEndElement();

                //POWERON
                reader.ReadStartElement("POWERON");
                temp = reader.ReadString();
                temp.Trim();
                Delay.BridgePowerON = Convert.ToInt32(temp);
                Trace.WriteLine("Delay: POWERON is: " + Delay.BridgePowerON.ToString());
                reader.ReadEndElement();

                //ENTERSLEEPMODE
                reader.ReadStartElement("ENTERSLEEPMODE");
                temp = reader.ReadString();
                temp.Trim();
                Delay.EnterSleepMode = Convert.ToInt32(temp);
                Trace.WriteLine("Delay: ENTERSLEEPMODE is: " + Delay.EnterSleepMode.ToString());
                reader.ReadEndElement();

                reader.ReadEndElement(); // </DELAY>

                reader.ReadEndElement(); // </CONFIG>

            }
            catch
            { }

        }



    }
}
