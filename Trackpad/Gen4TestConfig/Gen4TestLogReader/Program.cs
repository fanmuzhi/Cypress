using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Gen4TestLogReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create the CONFIG XML file
            List<CONFIGFILE> config = new List<CONFIGFILE>();
            List<CONFIGFILE> Rconfig = new List<CONFIGFILE>();

            for (int i = 0; i < 25; i++)
            {
                CONFIGFILE cfg = new CONFIGFILE();
                cfg.Log = true;
                cfg.Max = 100;
                cfg.Min = 0;
                cfg.Other = "";
                cfg.Samples = 20;
                cfg.Selected = true;
                cfg.Stop = true;
                cfg.Test = "123";
                config.Add(cfg);
            }
            

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FileStream fs = new FileStream(path + @"\Config.xml", FileMode.OpenOrCreate);
            XmlSerializer xser = new XmlSerializer(typeof(List<CONFIGFILE>));
            xser.Serialize(fs, config);
            fs.Close();

            FileStream fs1 = new FileStream(path + @"\Config1.xml", FileMode.OpenOrCreate);
            XmlSerializer xser1 = new XmlSerializer(typeof(List<CONFIGFILE>));
            Rconfig = (List<CONFIGFILE>)xser1.Deserialize(fs1);
            fs1.Close();
        }
    }
}
