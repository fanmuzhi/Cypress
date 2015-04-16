using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
{
    class DelayTime
    {
        public const int POWER_ON_MS = 500; //500, 2000 for debug
        public const int I2C_MS = 5;

        public const int IDAC_ERASE = 2000;

        public const int BootLoader_Exit = 1000;
        public const int I2C_Enter_TestMode = 1000;
        public const int SMBUS_Enter_TestMode = 500;
    }
}
