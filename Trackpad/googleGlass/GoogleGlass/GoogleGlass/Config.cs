using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GoogleGlass
{
    class Config
    {
        
        public static string U2722_ADDRESS = "";
        public static string COM_ADDRESS = "";
        public static byte I2C_ADDRESS = 0x13;

        public static double ALS_LIGHT = 16000;
        public static double ALS_DARK = 10;

        public static double RESISTOR_MAX = 5;
        public static double RESISTOR_MIN = 0.1;

        public static double LED_MIN = 3;
        public static double LED_MAX = 14;

        public static int MEAS_TIMES = 15;


        public static int PARTNUMBER_LENGTH = 8;
        public static int SERIAL_NUMBER_LENGTH = 19;

        public static bool ONLINE = false;

        
        public static void read()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader("config.xml");
                string temp;

                reader.MoveToContent();
                reader.ReadStartElement("CONFIG"); // <CONFIG>

                reader.ReadStartElement("U2722_ADDRESS");
                temp = reader.ReadString();
                temp.Trim();
                U2722_ADDRESS = temp;
                reader.ReadEndElement();

                reader.ReadStartElement("COM_ADDRESS");
                temp = reader.ReadString();
                temp.Trim();
                COM_ADDRESS = temp;
                reader.ReadEndElement();

                reader.ReadStartElement("I2C_ADDRESS");
                temp = reader.ReadString();
                temp.Trim();
                I2C_ADDRESS = Convert.ToByte(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("SN_LENGTH");
                temp = reader.ReadString();
                temp.Trim();
                SERIAL_NUMBER_LENGTH = Convert.ToInt32(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("ALS_DARK");
                temp = reader.ReadString();
                temp.Trim();
                ALS_DARK = Convert.ToDouble(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("ALS_LIGHT");
                temp = reader.ReadString();
                temp.Trim();
                ALS_LIGHT = Convert.ToDouble(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("RESISTOR_MIN");
                temp = reader.ReadString();
                temp.Trim();
                RESISTOR_MIN = Convert.ToDouble(temp);
                reader.ReadEndElement();


                reader.ReadStartElement("RESISTOR_MAX");
                temp = reader.ReadString();
                temp.Trim();
                RESISTOR_MAX = Convert.ToDouble(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("LED_MIN");
                temp = reader.ReadString();
                temp.Trim();
                LED_MIN = Convert.ToDouble(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("LED_MAX");
                temp = reader.ReadString();
                temp.Trim();
                LED_MAX = Convert.ToDouble(temp);
                reader.ReadEndElement();

                reader.ReadStartElement("MEAS_TIMES");
                temp = reader.ReadString();
                temp.Trim();
                MEAS_TIMES = Convert.ToInt32(temp);
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
                reader.ReadEndElement();             


                reader.ReadEndElement(); // </CONFIG>

            }
            catch
            { }

        }


    }
}
