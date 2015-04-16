using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace GoogleGlass
{
    class Log
    {
        public static string content;
        public static string sectionContent;

        public static void info(string text)
        {
            writeLog("INFO ", text);
        }

        public static void warning(string text)
        {
            writeLog("WARN ", text);
        }

        public static void error(string text)
        {
            writeLog("ERROR", text);
        }

        public static void error(string text, bool die)
        {
            writeLog("ERROR", text);
            if (die == true)
                Process.GetCurrentProcess().Kill();
        }

        private static void writeLog(string type, string error)
        {
            DateTime myTimeStamp = DateTime.Now;
            string date = myTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");

            string prefix = "[" + date + "] " + type.ToUpper() + " : ";

            content = content + prefix + error + "\r\n";
            sectionContent = sectionContent + prefix + error + "\r\n";


            StreamWriter myFile = new StreamWriter(Directory.GetCurrentDirectory() + "\\ErrorMessage.log", true);
            myFile.Write(prefix + error + "\r\n");
            myFile.Close();
        }
    }
}
