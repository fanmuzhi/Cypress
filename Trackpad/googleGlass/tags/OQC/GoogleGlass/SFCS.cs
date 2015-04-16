using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cypress_Link_SQL2005;

namespace GoogleGlass
{
    class SFCS
    {
        Link_SQL2005 SFCSconnection;
        public string connect_error = "";

        /// <summary>
        /// Setup connection to the SFCS traceability system.
        /// </summary>
        /// <returns></returns>true: sucessfully connected. false: failed to connect.
        public bool SFCS_Connect()
        {
            SFCSconnection = new Link_SQL2005();

            bool connected = false;

            int connectionInfo = SFCSconnection.Link_SQL();
            switch (connectionInfo)
            {
                case 0:
                    connected = true;
                    break;
                case 1:
                    connected = false;
                    connect_error = "SFCS: SFCS cannot login.";
                    break;
                case 9:
                    connected = false;
                    connect_error = "SFCS: Cannot find SFCS database.";
                    break;
                case 10:
                    connected = false;
                    connect_error = "SFCS: SFCS connection time out.";
                    break;
                case 11:
                    connected = false;
                    connect_error = "SFCS: Failed to connect SFCS.";
                    break;
                default:
                    connected = false;
                    connect_error = "SFCS: Unkown error when trying to connect SFCS.";
                    break;
            }
            return connected;
        }

        /// <summary>
        /// Check test permission of current station by searching the serial number of pre-station status.
        /// </summary>
        /// <param name="SerialNumber"></param> serial number of trackpad.
        /// <param name="Model"></param> part number of trackpad.
        /// <param name="Station"></param> current station ID.
        /// <returns></returns> true: get permission; false: no permission.
        public bool SFCS_PermissonCheck(String SerialNumber, String Model, string WorkerID, String Station)
        {
            bool permisson = false;
            int permisson_info = SFCSconnection.Select_Cypress_Result(SerialNumber, Model, WorkerID, Station);
            switch (permisson_info)
            {
                case 0:
                    permisson = true;
                    break;
                case 1:
                    permisson = false;
                    connect_error = "SFCS: " + SerialNumber.ToString() + " failed at SMT.";
                    break;
                case 2:
                    permisson = false;
                    connect_error = "SFCS: " + SerialNumber.ToString() + " failed at AOI.";
                    break;
                case 3:
                    permisson = false;
                    connect_error = "SFCS: " + SerialNumber.ToString() + " failed at TPT.";
                    break;
                case 4:
                    permisson = false;
                    connect_error = "SFCS: " + SerialNumber.ToString() + " does not exsist.";
                    break;
                case 9:
                    permisson = false;
                    connect_error = "SFCS: Cannot find SFCS database.";
                    break;
                case 10:
                    permisson = false;
                    connect_error = "SFCS: SFCS connection time out.";
                    break;
                case 11:
                    permisson = false;
                    connect_error = "SFCS: Failed to connect SFCS.";
                    break;
                default:
                    permisson = false;
                    connect_error = "SFCS: Unkown error when trying to get permission.";
                    break;
            }
            return permisson;
        }

        /// <summary>
        /// Upload the test result of current station to SFCS by serial number.
        /// </summary>
        /// <param name="SerialNumber"></param> serial number of trackpad.
        /// <param name="Model"></param> part number of trackpad.
        /// <param name="ErrorCdoe"></param> errorcode of trackpad.
        /// <param name="TestLog"></param> test records of raw data.
        /// <param name="TestResult"></param> "Pass" or "Fail".
        /// <param name="Station"></param> current station ID.
        /// <returns></returns> true: upload successfully. false: failed to upload the test result.
        public bool SFCS_UploadTestResult(String SerialNumber, String Model, string WorkerID, String ErrorCdoe, String TestLog, String TestResult, String Station)
        {
            bool resultUploaded = false;
            int upload_info = SFCSconnection.Save_Cypress_Result(SerialNumber, Model, WorkerID, ErrorCdoe, TestLog, TestResult, Station);
            switch (upload_info)
            {
                case 0:
                    resultUploaded = true;
                    break;
                case 9:
                    resultUploaded = false;
                    connect_error = "SFCS: Cannot find SFCS database.";
                    break;
                case 10:
                    resultUploaded = false;
                    connect_error = "SFCS: SFCS connection time out.";
                    break;
                case 11:
                    resultUploaded = false;
                    connect_error = "SFCS: Failed to connect SFCS.";
                    break;
                case 12:
                    resultUploaded = false;
                    connect_error = "SFCS: Not enough database space for uploading the result.";
                    break;
                case 13:
                    resultUploaded = false;
                    connect_error = "SFCS: infomation for uploading cannot meet database integrity.";
                    break;
                default:
                    resultUploaded = false;
                    connect_error = "SFCS: Unkown error when trying to upload the test result.";
                    break;
            }
            return resultUploaded;
        }
    }
}
