using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackPad
{
     //Define the trackpad test error code (Bin code)
    public class ErrorCode
    {
        public const int ERROR_MTK_FAIL = 0x01;
        public const int ERROR_NO_TRACKPAD_IN_SLOT = 0x10;
        public const int ERROR_IDD_OPEN = 0x11;
        public const int ERROR_IDD_SHORT = 0x12;
        public const int ERROR_PROGRAM_FAILURE = 0x21;
        public const int ERROR_IDD_LOW = 0x31;
        public const int ERROR_IDD_HIGH = 0x32;
        public const int ERROR_RES_LOW = 0x33;
        public const int ERROR_RES_HIGH = 0x34; 
        public const int ERROR_ALS_LOW = 0x35;
        public const int ERROR_ALS_HIGH = 0x36;
        public const int ERROR_NO_ALS = 0x37;
        public const int ERROR_LED_LOW = 0x38;
        public const int ERROR_LED_HIGH = 0x39;
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
        public const int ERROR_No_Signal = 0x80;
        public const int ERROR_TACTILE_SWITCH = 0x81;
        public const int ERROR_LED_LIGHT = 0x82;
        public const int ERROR_SNR_LOW = 0x83;
        public const int ERROR_FINGER_MOVE = 0x84;
        public const int ERROR_SFCS_NOPERMISSION = 0x91;
        public const int ERROR_SFCS_UPLOADDATA = 0x92;
        public const int EEROR_SYSTEM_ERROR = 0xA1;


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
                case ErrorCode.ERROR_NO_ALS:
                    temp = "Error: No ALS found";
                    break;
                case ErrorCode.ERROR_LED_LOW:
                    temp = "Error: LED current Low";
                    break;
                case ErrorCode.ERROR_LED_HIGH:
                    temp = "Error: LED current high";
                    break;
                case ErrorCode.ERROR_ALS_LOW:
                    temp = "Error: ALS light value low";
                    break;
                case ErrorCode.ERROR_ALS_HIGH:
                    temp = "Error: ALS dark value high";
                    break;
                case ErrorCode.ERROR_RES_LOW:
                    temp = "Error: Thermal resistor value low";
                    break;
                case ErrorCode.ERROR_RES_HIGH:
                    temp = "Error: Thermal resistor value high";
                    break;
                case ErrorCode.ERROR_MTK_FAIL:
                    temp = "Error: MTK test fail";
                    break;
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
                case ErrorCode.ERROR_No_Signal:
                    temp = "Error: No finger find";
                    break;
                case ErrorCode.ERROR_TACTILE_SWITCH:
                    temp = "Error: SWITTH ERROR";
                    break;
                case ErrorCode.ERROR_LED_LIGHT:
                    temp = "Error: LED BROKEN";
                    break;
                case ErrorCode.ERROR_SNR_LOW:
                    temp = "Error: SNR Low";
                    break;
                case ErrorCode.ERROR_FINGER_MOVE:
                    temp = "Error: FINGER movement";
                    break;

                default:
                    temp = "Pass";
                    break;
            }
            return temp;
        }

    }
}
