using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Gen4TestLogReader
{
    [XmlRootAttribute("Trackpad_Gen4", Namespace = "www.cypress.com", IsNullable = false)]
    public class DUT
    {
        public string SERIAL_NUMBER;
        public string CHIP_ID;
        public string DATE_TIME;
        public string SW_VERSION;
        public string OPERATOR;
        public string TEST_STATION;
        public string TEST_FACILITY;
        public string CONFIG_FILE;
        public string EXECUTION_MODE;
        public string SENSOR_ROWS;
        public string SENSOR_COLUMNS;
        public string TEST_RESULT;
        public string ERROR_CODE;
        public string ERROR_MESSAGE;

        public string VCOM_VOLTAGE;
        public string VAUX_VOLTAGE;
        public string ICOM_CURRENT;
        public string IAUX_CURRENT;

        public string FW_VERSION;
        public string FW_REVISION;

        public string GLOBAL_IDAC;

        public string ELAPSED_TIME;

        [XmlArray("LOCAL_IDAC")]
        [XmlArrayItem("data")]
        public List<string> LOCAL_IDAC;

        [XmlArray("NOISE")]
        [XmlArrayItem("data")]
        public List<string> NOISE;

        [XmlArray("RAW_DATA")]
        [XmlArrayItem("data")]
        public List<string> RAW_DATA;

        [XmlArray("BASELINE")]
        [XmlArrayItem("data")]
        public List<string> BASELINE;

        [XmlArray("SIGNAL")]
        [XmlArrayItem("data")]
        public List<string> SIGNAL;

        [XmlArray("SELFCAP_NOISE")]
        [XmlArrayItem("data")]
        public List<string> SELFCAP_NOISE;

        [XmlArray("SELFCAP_RAWDATE")]
        [XmlArrayItem("data")]
        public List<string> SELFCAP_RAWDATE;

        [XmlArray("SELFCAP_BASELINE")]
        [XmlArrayItem("data")]
        public List<string> SELFCAP_BASELINE;

        [XmlArray("SELFCAP_SIGNAL")]
        [XmlArrayItem("data")]
        public List<string> SELFCAP_SIGNAL;

        public DUT()
        {
            this.LOCAL_IDAC = new List<string>();
            this.NOISE = new List<string>();
            this.RAW_DATA=new List<string>();
            this.BASELINE = new List<string>();
            this.SIGNAL = new List<string>();

            this.SELFCAP_NOISE = new List<string>();
            this.SELFCAP_RAWDATE = new List<string>();
            this.SELFCAP_BASELINE = new List<string>();
            this.SELFCAP_SIGNAL = new List<string>();
        }


    }
}
