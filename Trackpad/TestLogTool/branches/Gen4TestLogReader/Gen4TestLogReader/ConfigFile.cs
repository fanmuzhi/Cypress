using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Gen4TestLogReader
{

    [XmlRootAttribute("CONFIGFILE", Namespace = "www.cypress.com", IsNullable = false)]
    public class CONFIGFILE
    {
        public string SOURCE_DIRECTORY;
        public string TARGET_DIRECTORY;
        public string ACHIEVED_DIRECTORY;
        public bool GENERATE_SUMMARY;
    }
}
