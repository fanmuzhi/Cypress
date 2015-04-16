using System;
using System.Collections.Generic;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    class Config
    {
        public const string MULTIMETER_ADDRESS = "U3606A";

        public const double SLEEP1_MAX = 85;
        public const double SLEEP1_MIN = 10;

        public const double DEEP_SLEEP_MAX = 5;
        public const double DEEP_SLEEP_MIN = 0.1;

        public const int MEAS_TIMES = 15;


        public const int PARTNUMBER_LENGTH = 8;
        public const int SERIAL_NUMBER_LENGTH = 19;

    }
}
