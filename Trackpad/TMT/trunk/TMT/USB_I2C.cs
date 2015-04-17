using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using PP_COM_Wrapper;
//using PSOCPROGRAMMERCOMLib;
using PSoCProgrammerCOMLib;

namespace CypressSemiconductor.ChinaManufacturingTest.TrackpadModuleTester
{
    public class USB_I2C
    {
        PSoCProgrammerCOM_Object pp;
        
        private string lastError="";
        public string LastError
        {
            get { return lastError; }
        }

        private byte deviceAddress = 0x04;
        public byte DeviceAddress
        {
            set { deviceAddress = value; }
            get { return deviceAddress; }
        }
        
        public USB_I2C()
        {
            pp = new PSoCProgrammerCOM_Object();
        }
        
        public string[] GetPorts()
        {
            object p;
            string[] ports;
            pp.GetPorts(out p, out lastError);
            ports = p as string[];
            pp.ClosePort(out lastError);
            return ports;
        }

        public int OpenPort(string progID)
        {
            int ok = 1;
            ok = pp.OpenPort(progID, out lastError);
            return ok;
        }

        public int ClosePort()
        {
            int ok = 1;
            ok = pp.ClosePort(out lastError);
            return ok;
        }

        public int SetPower(string power)
        {
            int ok = 1;
            ok = pp.SetPowerVoltage(power, out lastError);
            return ok;
        }

        public int PowerOn()
        {
            int ok = 1;
            ok = pp.PowerOn(out lastError);
            return ok;
        }

        public int PowerOff()
        {
            int ok = 1;
            ok = pp.PowerOff(out lastError);
            return ok;
        }

        public int SetI2CSpeed(byte speed)
        {
            int ok = 1;
            switch (speed)
            {
                case 0:
                    ok = pp.I2C_SetSpeed(enumI2Cspeed.CLK_400K, out lastError);
                    break;
                case 1:
                    ok = pp.I2C_SetSpeed(enumI2Cspeed.CLK_100K, out lastError);
                    break;
                case 2:
                    ok = pp.I2C_SetSpeed(enumI2Cspeed.CLK_50K, out lastError);
                    break;
            }
            ok = pp.I2C_SetTimeout(1000, out lastError);
            return ok;
        }

        public int GetDeviceAddress()
        {
            int ok = 1;
            object DeviceList;
            ok = pp.I2C_GetDeviceList(out DeviceList, out lastError);
            if (ok == 0)
            {
                byte[] devices = DeviceList as byte[];
                if (devices.Length <= 0)
                {
                    lastError = "No Device Found";
                    ok = 1;
                }
                else
                    deviceAddress = devices[0];
            }
            return ok;   
        }
       
        //command
        //Write to I2C
        public void Write(byte command)
        {
            pp.I2C_SendData(deviceAddress, new byte[] { 0x00, command }, out lastError);
        }

        public void Write(byte[] commands)
        {
            pp.I2C_SendData(deviceAddress, commands, out lastError);
        }

        //Read and Write I2C
        public byte[] ReadWrite(byte command, byte nToRead, int delay_time_ms)
        {
            byte repeat = 0;
            bool success = false;
            byte[] data;
            object dataIn;
            do
            {
                pp.I2C_SendData(deviceAddress, new byte[] { 0x00, command }, out lastError);
                System.Threading.Thread.Sleep(delay_time_ms * (repeat + 1));
                pp.I2C_ReadData(deviceAddress, nToRead, out dataIn, out lastError);
                data = dataIn as byte[];
                if (data[0] == command)
                { success = true; }

                repeat++;

            } while (!success && repeat < 5);

            if (!success)
            {
                Log.error("Fail to excute command: " + command.ToString());
            }
            
            return data;
        }

        public byte[] ReadWrite2(byte command, byte nToRead, int delay_time_ms)
        {
            byte repeat = 0;
            bool success = false;
            byte[] data;
            object dataIn;
            do
            {
                pp.I2C_SendData(deviceAddress, new byte[] { command }, out lastError);
                System.Threading.Thread.Sleep(delay_time_ms * (repeat + 1));
                pp.I2C_ReadData(deviceAddress, nToRead, out dataIn, out lastError);
                data = dataIn as byte[];
                if (data[0] == command)
                { success = true; }

                repeat++;

            } while (!success && repeat < 5);

            if (!success)
            {
                Log.error("Fail to excute command: " + command.ToString());
            }

            return data;
        }

        public byte[] ReadWrite(byte command, byte parameter, byte nToRead, int delay_time_ms)
        {
            byte repeat = 0;
            bool success=false;
            byte[] data;
            object dataIn;
            do
            {
                pp.I2C_SendData(deviceAddress, new byte[] { 0x00, command, parameter }, out lastError);
                System.Threading.Thread.Sleep(delay_time_ms * (repeat + 1));
                pp.I2C_ReadData(deviceAddress, nToRead, out dataIn, out lastError);
                data = dataIn as byte[];
                if (data[0] == command)
                { success = true; }

                repeat++;

            } while (!success && repeat < 5);

            if (!success)
            {
                Log.error("Fail to excute command: " + command.ToString());
            }
            return data;
        }

        public byte[] ReadAfterWrite(byte[] command, byte nToRead, int delay_time_ms)
        {              
            byte[] data;
            object dataIn;

            pp.I2C_SendData(deviceAddress, command, out lastError);
            System.Threading.Thread.Sleep(delay_time_ms);
            pp.I2C_ReadData(deviceAddress, nToRead, out dataIn, out lastError);
            data = dataIn as byte[];               

            return data;
        }

    }
}
