using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest.TPT
{
    class DelayTime
    {
        // all delay is int with millisecond

        public const int MICRO_SWITCH_ON = 300; //500, 2000 for debug

        public const int PNEUMATIC_ON = 2000;

        //idac
        public const int IDAC_ERASE = 1000;

        //power 
        public const int POWER_ON_DELAY = 50;
        public const int POWER_OFF_DELAY = 20;

        //enter test mode
        public const int BootLoader_Exit = 1000;
        public const int I2C_Enter_TestMode = 500;
        public const int SMBUS_Enter_TestMode = 500;


        //agilent
        public const int AGILENT_RELAY_CLOSE = 200;
        public const int AGILENT_GPIO_DELAY = 200;
        public const int READ_MIRCO_SWITCH = 100;

        //mpq
        public const int MPQ_INIT_DELAY = 600;
        public const int MPQ_APA_COMMAND_DELAY = 600;
        public const int MPQ_MTG_COMMAND_DELAY = 300;
        public const int CHECK_PROGRAMMING_STATUS = 600;
        public const int MPQ_SWITCH_DELAY = 1000;


    }
}
