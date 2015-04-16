using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CypressSemiconductor.ChinaManufacturingTest.Logitech_RemoteControl
{
    public class MessageChangeEventArgs : EventArgs
    {
        public MessageChangeEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
    
    
    class Trackpad
    {
        //****************************************//
        //     Define Trackpad Register Info      //
        //****************************************//
        public static class Register
        {
            // finger 1
            private static int _finger1X;
            public static int Finger1X
            {
                get { return _finger1X; }
                set { _finger1X = value; }
            }

            private static int _finger1Y;
            public static int Finger1Y
            {
                get { return _finger1Y; }
                set { _finger1Y = value; }
            }

            // finger 2
            private static int _finger2X;
            public static int Finger2X
            {
                get { return _finger2X; }
                set { _finger2X = value; }
            }

            private static int _finger2Y;
            public static int Finger2Y
            {
                get { return _finger2Y; }
                set { _finger2Y = value; }
            }

            // finger 3
            private static int _finger3X;
            public static int Finger3X
            {
                get { return _finger3X; }
                set { _finger3X = value; }
            }

            private static int _finger3Y;
            public static int Finger3Y
            {
                get { return _finger3Y; }
                set { _finger3Y = value; }
            }

            // finger 4
            private static int _finger4X;
            public static int Finger4X
            {
                get { return _finger4X; }
                set { _finger4X = value; }
            }

            private static int _finger4Y;
            public static int Finger4Y
            {
                get { return _finger4Y; }
                set { _finger4Y = value; }
            }

            // finger 5
            private static int _finger5X;
            public static int Finger5X
            {
                get { return _finger5X; }
                set { _finger5X = value; }
            }

            private static int _finger5Y;
            public static int Finger5Y
            {
                get { return _finger5Y; }
                set { _finger5Y = value; }
            }
        }

        USB_I2C_Bridge bridge;

        private bool trackpadConnected;                     //define if the trackpad is connected to I2C-bridge
        private volatile bool readPositionStop = false;     //define if keep reading the Position X Y
        private const byte offset = 0x07;                   //define the first 20 bytes reads is reserved for logitech

        // Publish MessageChange Event //
        public delegate void MessageChangeEventHandler(object sender, MessageChangeEventArgs ea);
        public event MessageChangeEventHandler MessageChangeEvent;
        protected virtual void OnPMessageChange(MessageChangeEventArgs ea)
        {
            MessageChangeEventHandler handler = MessageChangeEvent;
            if (handler != null)
            {
                handler(this, ea);
            }
        }

        public Trackpad()
        {
            bridge = new USB_I2C_Bridge();
            trackpadConnected = false;
            readPositionStop = false;
        }

        ~Trackpad()
        {
            bridge = null;
        }

        //****************************************//
        //    Connect/Disconn to I2C-Bridge       //
        //****************************************//
        private void BridgeGetDevice()
        {
            // Get I2C-Bridge Ports
            string[] ports = bridge.GetPorts();
            if (ports.Length < 1)
            {
                throw new Exception("No Device Found.");
            }
            else
            {
                //Open I2C-Bridge Port
                int status = bridge.OpenPort(ports[0]);
                if (status != 0)
                {
                    throw new Exception("Error in open port: " + bridge.LastError);
                }
            }
        }

        private void BridgeCloseDevice()
        {
            int status = bridge.ClosePort();
            if (status != 0)
            {
                throw new Exception("Error in close port: " + bridge.LastError);
            }
        }

        //****************************************//
        //        Set I2C-Bridge Power            //
        //****************************************//
        private void BridgePowerOff()
        {
            int Status = bridge.PowerOff();
            if (Status != 0)
            {
                throw new Exception("Error in power off: " + bridge.LastError);
            }
        }

        private void BridgePowerOn()
        {
            string powerS = "3.3";
            int Status1 = bridge.SetPower(powerS);
            int Status2 = bridge.PowerOn();
            if ((Status1 != 0) || (Status2 != 0))
            {
                throw new Exception("Error in setting power: " + bridge.LastError);
            }
            else
            {
                //Set I2C Speed to 400KHz//
                int Status3 = bridge.SetI2CSpeed(0);
                if (Status3 != 0)
                {
                    throw new Exception("Error in setting I2C speed: " + bridge.LastError);
                }
            }
        }

        //****************************************//
        //           Get DUT Address              //
        //****************************************//
        private void BridgeGetDUTAddress()
        {
            //Get DUT Device Address, normal=4//
            int Status1 = bridge.GetDeviceAddress();
            if (Status1 != 0)
            {
                throw new Exception("Error to get DUT address: " + bridge.LastError);
            }
        }

        //****************************************//
        //           Open I2C-Bridge              //
        //****************************************//
        public void BridgeOpen()
        {
            if (!trackpadConnected)
            {
                try
                {
                    BridgeGetDevice();
                    BridgePowerOn();
                    System.Threading.Thread.Sleep(1000);
                    BridgeGetDUTAddress();
                    trackpadConnected = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trackpadConnected = false;
                }
            }
        }

        //****************************************//
        //           Close I2C-Bridge             //
        //****************************************//
        public void BridgeClose()
        {
            if (trackpadConnected)
            {
                try
                {
                    BridgePowerOff();
                    BridgeCloseDevice();
                    trackpadConnected = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trackpadConnected = true;
                }
            }
        }

        //****************************************//
        //           Read X Y Position            //
        //****************************************//
        public void ReadPosition()
        {
            if (!trackpadConnected)
            {
                return;
            }
            
            readPositionStop = false;

            while (!readPositionStop)
            {
                //read X, Y position//
                byte[] datain = bridge.ReadWriteOffset(offset, 0x15);

                if (datain.Length >= 16)
                {
                    Register.Finger1X = datain[2] + (((datain[1] & 0xF0) >> 4) << 8);
                    Register.Finger1Y = datain[3] +  ((datain[1] & 0x0F) << 8);

                    Register.Finger2X = datain[6] + (((datain[5] & 0xF0) >> 4) << 8); 
                    Register.Finger2Y = datain[7] + ((datain[5] & 0x0F) << 8);

                    Register.Finger3X = datain[10] + (((datain[9] & 0xF0) >> 4) << 8);
                    Register.Finger3Y = datain[11] + ((datain[9] & 0x0F) << 8);

                    Register.Finger4X = datain[14] + (((datain[13] & 0xF0) >> 4) << 8);
                    Register.Finger4Y = datain[15] + ((datain[13] & 0x0F) << 8);

                    Register.Finger5X = datain[18] + (((datain[17] & 0xF0) >> 4) << 8);
                    Register.Finger5Y = datain[19] + ((datain[17] & 0x0F) << 8);

                    if (datain[1] != 0)
                    {
                        OnPMessageChange(new MessageChangeEventArgs(
                                                 "Finger1X: " + Register.Finger1X.ToString() + "  "
                                               + "Finger1Y: " + Register.Finger1Y.ToString() + "  "
                                               + "Finger2X: " + Register.Finger2X.ToString() + "  "
                                               + "Finger2Y: " + Register.Finger2Y.ToString()));   
                    }

                }
                else
                {
                    OnPMessageChange(new MessageChangeEventArgs("Data Array Length is: " + datain.Length.ToString()));
                }

                //int x = Position.X;
                //int y = Position.Y;
                //Position.X = (256 * datain[3]) + datain[4];
                //Position.Y = (256 * datain[5]) + datain[6];

                //if (x != Position.X || y != Position.Y)
                //{
                //   OnPMessageChange(new MessageChangeEventArgs("X Position: " + Position.X.ToString() + " Y Position: " + Position.Y.ToString()));
                //}
            }
        }

        public void ReadPositionStop()
        {
            readPositionStop = true;
        }
    }
}
