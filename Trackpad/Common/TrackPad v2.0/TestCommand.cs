using System;
using System.Collections.Generic;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackPad
{
    public class TestCommand
    {
        public static byte Read_FW_Rev=0x00;
        public static byte Read_Postion_XY = 0x01;
        public static byte Read_Tactile_Switch = 0x02;
        public static byte Read_RawCount_X = 0x03;
        public static byte Read_RawCount_Y = 0x04;
        public static byte Read_DIFF = 0x05;
        public static byte Read_Global_IDAC = 0x07;
        public static byte Read_IDACGain = 0x08;
        public static byte Erase_IDAC = 0x09;
        public static byte Recalibrate = 0x0A;
        public static byte Read_Local_IDAC = 0x0B;       
    }
}
