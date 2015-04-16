using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Gen4TestLogReader
{

    [XmlRootAttribute("TestItem", IsNullable = false)]
    public class CONFIGFILE
    {
        public bool Selected;
        public string Test;
        public double Min;
        public double Max;
        public int Samples;
        public bool Stop;
        public bool Log;
        public string Other;

    }
}
