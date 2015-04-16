using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public class DeviceConfig
    {
        //Define the trackpad part type (i.e. 10600100)
        public static string partType;

        //Define the setting file path to read and write (i.e. C:\production.ini)
        public static string filePath;

        //Define the Items in each section
        public class Items
        {
            public static double VDD_OP_MAX;
            public static double VDD_OP_MIN;
            public static double IDD_MAX;
            public static double IDD_MIN;
            public static double IDD_SHORT;
            public static double IDD_OPEN;
            public static byte I2C_ADDRESS;
            public static byte FW_INFO_FW_REV;
            public static byte FW_INFO_FW_Ver;
            public static byte FW_INFO_NUM_COLS;
            public static byte FW_INFO_NUM_ROWS;
            public static int RAW_DATA_READS;
            public static int RAW_AVG_MAX;
            public static int RAW_AVG_MIN;
            public static int RAW_NOISE_MAX;
            public static int RAW_STD_DEV_MAX;
            public static byte IDAC_MAX;
            public static byte IDAC_MIN;

            public static int trackpad_Function = TPCONFIG.TP_FUNCTION_ST;
            public static int trackpad_Bootloader = TPCONFIG.TP_WITHOUT_BOOTLOADER;
            public static int trackpad_TactileSwitch = TPCONFIG.TP_NORMAL_BUTTON;
            public static int trackpad_Interface = TPCONFIG.TP_INTERFACE_PS2;
            public static int trackpad_ShopFloor = TPCONFIG.TP_TEST_OFFLINE;
        }

        public class TPCONFIG
        {
            public const int TP_FUNCTION_ST = 1;
            public const int TP_FUNCTION_MTG = 2;
            public const int TP_FUNCTION_APA = 3;

            public const int TP_WITHOUT_BOOTLOADER = 1;
            public const int TP_WITH_BOOTLOADER = 2;

            public const int TP_NORMAL_BUTTON = 1;
            public const int TP_REMOTE_CONTROL = 2;
            public const int TP_CLICK_PAD = 3;

            public const int TP_INTERFACE_PS2 = 1;
            public const int TP_INTERFACE_I2C = 2;
            public const int TP_INTERFACE_SMB = 3;

            public const int TP_TEST_ONLINE = 1;
            public const int TP_TEST_OFFLINE = 2;

        }

        /// <summary>
        /// Read all the sections' name in setting file
        /// </summary>
        /// <returns></returns>return the string[] of sections' name
        public static string[] ReadModuleList()
        {
            IniFile ini = new IniFile(filePath);
            string[] rtval = ini.GetSectionNames();
            return rtval;
        }

        /// <summary>
        /// Read the items of section which the name is defined part type
        /// </summary>
        /// <returns></returns>if read successfully return true, else false.
        public static bool Read()
        {
            IniFile ini = new IniFile(filePath);

            Items.VDD_OP_MAX = ini.GetDouble(partType, "VDD_OP_MAX", 0);
            Items.VDD_OP_MIN = ini.GetDouble(partType, "VDD_OP_MIN", 0);
            Items.IDD_MAX = ini.GetDouble(partType, "IDD_MAX", 0);
            Items.IDD_MIN = ini.GetDouble(partType, "IDD_MIN", 0);
            Items.IDD_SHORT = ini.GetDouble(partType, "IDD_SHORT", 0);
            Items.IDD_OPEN = ini.GetDouble(partType, "IDD_OPEN", 0);
            Items.I2C_ADDRESS = Convert.ToByte(ini.GetInt32(partType, "I2C_ADDRESS", 0));
            Items.FW_INFO_FW_REV = Convert.ToByte(ini.GetInt32(partType, "FW_INFO_FW_REV", 0));
            Items.FW_INFO_FW_Ver = Convert.ToByte(ini.GetInt32(partType, "FW_INFO_FW_VER", 0));
            Items.FW_INFO_NUM_COLS = Convert.ToByte(ini.GetInt32(partType, "FW_INFO_NUM_COLS", 0));
            Items.FW_INFO_NUM_ROWS = Convert.ToByte(ini.GetInt32(partType, "FW_INFO_NUM_ROWS", 0));
            Items.RAW_DATA_READS = ini.GetInt32(partType, "RAW_DATA_READS", 0);
            Items.RAW_AVG_MAX = ini.GetInt32(partType, "RAW_AVG_MAX", 0);
            Items.RAW_AVG_MIN = ini.GetInt32(partType, "RAW_AVG_MIN", 0);
            Items.RAW_NOISE_MAX = ini.GetInt32(partType, "RAW_NOISE_MAX", 0);
            Items.RAW_STD_DEV_MAX = ini.GetInt32(partType, "RAW_STD_DEV_MAX", 0);
            Items.IDAC_MAX = Convert.ToByte(ini.GetInt32(partType, "IDAC_MAX", 0));
            Items.IDAC_MIN = Convert.ToByte(ini.GetInt32(partType, "IDAC_MIN", 0));

            Items.trackpad_Function = ini.GetInt32(partType, "TrackpadFunction", Items.trackpad_Function);
            Items.trackpad_Bootloader = ini.GetInt32(partType, "TrackpadBootloader", Items.trackpad_Bootloader);
            Items.trackpad_TactileSwitch = ini.GetInt32(partType, "TrackpadTactileSwitch", Items.trackpad_TactileSwitch);
            Items.trackpad_Interface = ini.GetInt32(partType, "TrackpadInterface", Items.trackpad_Interface);
            Items.trackpad_ShopFloor = ini.GetInt32(partType, "TrackpadShopFloor", Items.trackpad_ShopFloor);



            if (Items.RAW_DATA_READS == 0)
            {
                return false;
                //throw new Exception("Cannot find " + partType.ToString() + " in Production.ini");
            }
            else
            { return true; }
        }

        /// <summary>
        /// Write the items of section which the name is defined part type
        /// </summary>
        public static void Write()
        {
            IniFile ini = new IniFile(filePath);

            ini.WriteValue(partType, "VDD_OP_MAX", Items.VDD_OP_MAX.ToString());
            ini.WriteValue(partType, "VDD_OP_MIN", Items.VDD_OP_MIN.ToString());
            ini.WriteValue(partType, "IDD_MAX", Items.IDD_MAX.ToString());
            ini.WriteValue(partType, "IDD_MIN", Items.IDD_MIN.ToString());
            ini.WriteValue(partType, "IDD_SHORT", Items.IDD_SHORT.ToString());
            ini.WriteValue(partType, "IDD_OPEN", Items.IDD_OPEN.ToString());
            ini.WriteValue(partType, "I2C_ADDRESS", Items.I2C_ADDRESS.ToString());
            ini.WriteValue(partType, "FW_INFO_FW_REV", Items.FW_INFO_FW_REV.ToString());
            ini.WriteValue(partType, "FW_INFO_FW_VER", Items.FW_INFO_FW_Ver.ToString());
            ini.WriteValue(partType, "FW_INFO_NUM_COLS", Items.FW_INFO_NUM_COLS.ToString());
            ini.WriteValue(partType, "FW_INFO_NUM_ROWS", Items.FW_INFO_NUM_ROWS.ToString());
            ini.WriteValue(partType, "RAW_DATA_READS", Items.RAW_DATA_READS.ToString());
            ini.WriteValue(partType, "RAW_AVG_MAX", Items.RAW_AVG_MAX.ToString());
            ini.WriteValue(partType, "RAW_AVG_MIN", Items.RAW_AVG_MIN.ToString());
            ini.WriteValue(partType, "RAW_NOISE_MAX", Items.RAW_NOISE_MAX.ToString());
            ini.WriteValue(partType, "RAW_STD_DEV_MAX", Items.RAW_STD_DEV_MAX.ToString());
            ini.WriteValue(partType, "IDAC_MAX", Items.IDAC_MAX.ToString());
            ini.WriteValue(partType, "IDAC_MIN", Items.IDAC_MIN.ToString());

            ini.WriteValue(partType, "TrackpadFunction", Items.trackpad_Function);
            ini.WriteValue(partType, "TrackpadBootloader", Items.trackpad_Bootloader);
            ini.WriteValue(partType, "TrackpadTactileSwitch", Items.trackpad_TactileSwitch);
            ini.WriteValue(partType, "TrackpadInterface", Items.trackpad_Interface);
            ini.WriteValue(partType, "TrackpadShopFloor", Items.trackpad_ShopFloor);

        }

        //Define the trackpad test error code (Bin code)
        public class ErrorCode
        {
            public const int ERROR_NO_TRACKPAD_IN_SLOT = 0x10;
            public const int ERROR_IDD_OPEN = 0x11;
            public const int ERROR_IDD_SHORT = 0x12;
            public const int ERROR_PROGRAM_FAILURE = 0x21;
            public const int ERROR_IDD_LOW = 0x31;
            public const int ERROR_IDD_HIGH = 0x32;
            public const int ERROR_FW_REV = 0x41;
            public const int ERROR_IDAC_LOW = 0x51;
            public const int ERROR_IDAC_HIGH = 0x52;
            public const int ERROR_IDAC_ERASED = 0x53;
            public const int ERROR_IDAC_GAIN_HIGH = 0x54;
            public const int ERROR_LOCAL_IDAC = 0x55;
            public const int ERROR_RAW_COUNT_LOW = 0x61;
            public const int ERROR_RAW_COUNT_HIGH = 0x62;
            public const int ERROR_RAW_COUNT_NOISE_AVG = 0x63;
            public const int ERROR_RAW_COUNT_NOISE_STDDEV = 0x64;
            public const int ERROR_RAW_COUNT_NOISE_ZERO = 0x65;
            //public const int ERROR_RAW_X_LOW = 0x61;
            //public const int ERROR_RAW_X_HIGH = 0x62;
            //public const int ERROR_RAW_X_NOISE_AVG = 0x63;
            //public const int ERROR_RAW_X_NOISE_STDDEV = 0x64;
            //public const int ERROR_RAW_X_NOISE_ZERO = 0x65;
            //public const int ERROR_RAW_Y_LOW = 0x71;
            //public const int ERROR_RAW_Y_HIGH = 0x72;
            //public const int ERROR_RAW_Y_NOISE_AVG = 0x73;
            //public const int ERROR_RAW_Y_NOISE_STDDEV = 0x74;
            //public const int ERROR_RAW_Y_NOISE_ZERO = 0x75;
            public const int ERROR_TACTILE_SWITCH = 0x81;
            public const int ERROR_SFCS_NOPERMISSION = 0x91;
            public const int ERROR_SFCS_UPLOADDATA = 0x92;
            public const int EEROR_SYSTEM_ERROR = 0xA1;
        }

        /// <summary>
        /// Translate the error code to error string
        /// </summary>
        /// <param name="errorCode"></param>error code of a single DUT
        /// <returns></returns>error string of corresponding error code
        public static string ReportErrorCodes(int errorCode)
        {
            string temp;
            switch (errorCode)
            {
                case ErrorCode.ERROR_NO_TRACKPAD_IN_SLOT:
                    temp = "Error: No trackpad in this slot";
                    break;
                case ErrorCode.ERROR_IDD_OPEN:
                    temp = "Error: VDD Open";
                    break;
                case ErrorCode.ERROR_IDD_SHORT:
                    temp = "Error: VDD Short";
                    break;
                case ErrorCode.ERROR_PROGRAM_FAILURE:
                    temp = "Error: Programming Fail";
                    break;
                case ErrorCode.ERROR_FW_REV:
                    temp = "Error: FW_REV";
                    break;
                case ErrorCode.ERROR_RAW_COUNT_HIGH:
                    temp = "Error: RawCount Average High";
                    break;
                case ErrorCode.ERROR_RAW_COUNT_LOW:
                    temp = "Error: RawCount Average Low";
                    break;
                case ErrorCode.ERROR_RAW_COUNT_NOISE_AVG:
                    temp = "Error: RawCount Average Noise High";
                    break;
                case ErrorCode.ERROR_RAW_COUNT_NOISE_STDDEV:
                    temp = "Error: Std Dev Noise High";
                    break;
                case ErrorCode.ERROR_RAW_COUNT_NOISE_ZERO:
                    temp = "Error: Zero Noise";
                    break;
                case ErrorCode.ERROR_IDAC_HIGH:
                    temp = "Error: IDAC High";
                    break;
                case ErrorCode.ERROR_IDAC_LOW:
                    temp = "Error: IDAC Low";
                    break;
                case ErrorCode.ERROR_IDAC_ERASED:
                    temp = "Error: IDAC Erase Failed";
                    break;
                case ErrorCode.ERROR_IDAC_GAIN_HIGH:
                    temp = "Error: IDAC GAIN High";
                    break;
                case ErrorCode.ERROR_LOCAL_IDAC:
                    temp = "Error: Local IDAC is out of range";
                    break;
                case ErrorCode.ERROR_IDD_LOW:
                    temp = "Error: Current Low";
                    break;
                case ErrorCode.ERROR_IDD_HIGH:
                    temp = "Error: Current High";
                    break;
                case ErrorCode.ERROR_SFCS_NOPERMISSION:
                    temp = "Error: No permission to test";
                    break;
                case ErrorCode.ERROR_SFCS_UPLOADDATA:
                    temp = "Error: Can not be recorded in database";
                    break;
                case ErrorCode.EEROR_SYSTEM_ERROR:
                    temp = "Error: Test system error.";
                    break;

                default:
                    temp = "Pass";
                    break;
            }
            return temp;
        }
    }
}
