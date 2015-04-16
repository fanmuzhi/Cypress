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
            ////Create the CONFIG XML file
            //CONFIGFILE cfg = new CONFIGFILE();
            //cfg.SOURCE_DIRECTORY = @"D:\118001test log\test log\";
            //cfg.TARGET_DIRECTORY = @"D:\118001test log\xml log\";
            //cfg.ACHIEVED_DIRECTORY = @"D:\118001test log\achieved log\";
            //cfg.GENERATE_SUMMARY = true;

            //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //FileStream fs = new FileStream(path + @"\Config.xml", FileMode.OpenOrCreate);
            //XmlSerializer xser = new XmlSerializer(typeof(CONFIGFILE));
            //xser.Serialize(fs, cfg);

            try
            {
                //Read the CONFIG XML file
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                FileStream fs = new FileStream(path + @"\Config.xml", FileMode.Open);
                XmlSerializer xser = new XmlSerializer(typeof(CONFIGFILE));
                CONFIGFILE cfg = (CONFIGFILE)xser.Deserialize(fs);

                Regex PathPattern = new Regex(@"^[CDEFGH]:\\.*\\$");
                Match matchSourceDir = PathPattern.Match(cfg.SOURCE_DIRECTORY);
                Match matchTargetDir = PathPattern.Match(cfg.TARGET_DIRECTORY);
                Match matchAchievedDir = PathPattern.Match(cfg.ACHIEVED_DIRECTORY);

                if ((!matchSourceDir.Success) | (!new DirectoryInfo(cfg.SOURCE_DIRECTORY).Exists))
                {
                    Console.WriteLine("Error: Source Directory" + cfg.SOURCE_DIRECTORY + " is not a valid path or doesn't exist.");
                    Console.ReadKey();
                    Environment.Exit(3);
                }
                else if (!matchTargetDir.Success)
                {
                    Console.WriteLine("Error: Target Directory " + cfg.TARGET_DIRECTORY + " is not a valid path.");
                    Console.ReadKey();
                    Environment.Exit(3);
                }
                else if (!matchAchievedDir.Success)
                {
                    Console.WriteLine("Error: Achieved Directory " + cfg.TARGET_DIRECTORY + " is not a valid path.");
                    Console.ReadKey();
                    Environment.Exit(3);
                }
                else if(cfg.GENERATE_SUMMARY)
                {
                    TestLog tlog = new TestLog(cfg.SOURCE_DIRECTORY, cfg.TARGET_DIRECTORY, cfg.ACHIEVED_DIRECTORY);
                    tlog.write_summary_report();
                }

                else
                {
                    TestLog tl = new TestLog(cfg.SOURCE_DIRECTORY, cfg.TARGET_DIRECTORY, cfg.ACHIEVED_DIRECTORY);
                    while (true)
                    {
                        tl.write_XML_report();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }

        }
    }
}
