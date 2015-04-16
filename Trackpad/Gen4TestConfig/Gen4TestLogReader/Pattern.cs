using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gen4TestLogReader
{
    public static class Gen4Pattern
    {
        public static Regex ProductionDataPattern =
            new Regex(@",\s\.header\s*,\sDATE,\s(?<TestDate>[^,]*),\sTIME,\s(?<TestTime>[^,]*)\s*,\sSW\sVERSION,\s(?<SWVersion>(?:\d{1,2}.){3}\d{1,4}),\sOPERATOR,\s(?<Operator>\d{1,5}),\sTEST\sSTATION,\s(?<TestStaion>\b\w*\b),\sTEST\sFACILITY,\s(?<TestFacility>\b\w*\b)\s*,\sCONFIG\sFILE,\s(?<ConfigFile>[^,]*),\sEXECUTION\sMODE,\s(?<TestMode>\b\w*\b),\sSENSOR\sROWS,\s(?<RowNumber>\d{1,2}),\sSENSOR\sCOLUMNS,\s(?<ColumnNumber>\d{1,2}),\s(?<TestResult>\b\w*\b)\s*,\s\.end\s*");

        public static Regex EngineeringLine1Pattern =
            new Regex(@",\s(?<SerialNumber>\w{19}),\s(?<ChipID>[^,]*),\s(?:ERRORS,\s)?(?<ErrorCode>[^:]*):\s(?<ErrorMessage>[^,]*)\s*");

        public static Regex GolbalIDACPattern =
            new Regex(@"\sSensor\sGlobal\siDAC,\s(?<Global_IDAC>\d{1,3})\s*");

        public static Regex FWVersionPattern =
            new Regex(@"\sFW\sVersion,([^,]*,){4}\sVersion,\s(?<FW_Version>\d{1,2}.\d{1,2})\s*");

        public static Regex FWRevisionPattern =
            new Regex(@"\sFW\sRevision\sControl,([^,]*,){4}\sRevision,\s(?<FW_Revision>[^,]*)\s*");

        public static Regex LocalIDACPattern =
            new Regex(@"\sLocal\siDAC,\s+ROW\d{2},(?<Local_IDAC>(\s{3,4}\d{1,3},)*(\s{3,4}\d{1,3}))\s*");

        public static Regex NoisePattern =
            new Regex(@"\sNoise,\s+ROW\d{2},(?<Noise>(\s{3,4}\d{1,3},)*(\s{3,4}\d{1,3}))\s*");

        public static Regex RawDataPattern =
            new Regex(@"\sRaw\sData,\s+ROW\d{2},(?<RawData>(\s{2,4}\-?\d{1,3},)*(\s{2,4}\-?\d{1,3}))\s*");

        public static Regex BaselinePattern =
            new Regex(@"\sBaseline,\s+ROW\d{2},(?<Baseline>(\s{2,4}\-?\d{1,3},)*(\s{2,4}\-?\d{1,3}))\s*");

        public static Regex SelfCapNoisePattern =
            new Regex(@"\sSelf-cap\sNoise,\s(COLS|ROWS),(?<SelfCapNoise>(\s{2,4}\d{1,3},)*(\s{2,4}\d{1,3}))\s*");

        public static Regex SelfCapRawDataPattern =
            new Regex(@"\sSelf-cap\sRaw\sData,\s(COLS|ROWS),(?<SelfCapRawData>(\s{2,4}\-?\d{1,3},)*(\s{2,4}\-?\d{1,3}))\s*");

        public static Regex SelfCapBaseLinePattern =
            new Regex(@"\sSelf-cap\sBaseline,\s(COLS|ROWS),(?<SelfCapBaseLine>(\s{2,4}\-?\d{1,3},)*(\s{2,4}\-?\d{1,3}))\s*");

        public static Regex SelfCapSignalPattern =
            new Regex(@"\sSelf-cap\sSignal,\s(COLS|ROWS),(?<SelfCapSignal>(\s{2,4}\-?\d{1,3},)*(\s{2,4}\-?\d{1,3}))\s*");

        public static Regex SignalPattern =
            new Regex(@"\sSignal,\s+ROW\d{2},(?<Signal>(\s{2,4}\-?\d{1,3},)*(\s{2,4}\-?\d{1,3}))\s*");

        public static Regex VCOMVoltagePattern =
            new Regex(@"\sVCOM\sVoltage,(?:[^,]*,){5}\s(?<VCOMVoltage>\d{1,2}.\d{1,2})\s*");

        public static Regex VAUXVoltagePattern =
            new Regex(@"\sVAUX\sVoltage,(?:[^,]*,){5}\s(?<VAUXVoltage>\d{1,2}.\d{1,2})\s*");

        public static Regex ICOMCurrentPattern =
            new Regex(@"\sICOM\sCurrent,(?:[^,]*,){5}\s(?<ICOMCurrent>\d{1,2}.\d{1,2})\s*");

        public static Regex IAUXCurrentPattern =
            new Regex(@"\sIAUX\sCurrent,(?:[^,]*,){5}\s(?<IAUXCurrent>\d{1,2}.\d{1,2})\s*");

        public static Regex ElapsedTimePattern =
            new Regex(@"\sELAPSED\sTIME,\s(?<ElapsedTime>\d+\.?\d?)\s*");
    }
}
