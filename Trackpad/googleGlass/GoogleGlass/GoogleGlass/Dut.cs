using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleGlass
{
    class DUT
    {

        // Serial Number
        private string m_SerialNumber;
        public string SerailNumber
        {
            set { m_SerialNumber = value; }
            get { return m_SerialNumber; }
        }

        //Error Code
        private int m_ErrorCode;
        public int ErrorCode
        {
            set { m_ErrorCode = value; }
            get { return m_ErrorCode; }
        }

        // ALS value
        private int m_ALS_dark;
        public int ALS_dark
        {
            set { m_ALS_dark = value; }
            get { return m_ALS_dark; }
        }

        private int m_ALS_light;
        public int ALS_light
        {
            set { m_ALS_light = value; }
            get { return m_ALS_light; }
        }
        // RES value
        private double m_Resistor;
        public double Resistor
        {
            set { m_Resistor = value; }
            get { return m_Resistor; }
        }

        // LED value
        private double m_LED;
        public double LED
        {
            set { m_LED = value; }
            get { return m_LED; }
        }

    }
}
