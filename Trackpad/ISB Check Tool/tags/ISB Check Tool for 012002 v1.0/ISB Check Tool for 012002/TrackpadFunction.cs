using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using CypressSemiconductor.ChinaManufacturingTest.Work_Loop;

namespace CypressSemiconductor.ChinaManufacturingTest
{

    //class containing event data
    public class TPEventArgs : EventArgs
    {
        public TPEventArgs(int status)
        {
            st = status;        
        }

        private int st;
        public int Status
        {
            get { return st; }
            set { st = value; }
        }
    }
    
    
    class TrackpadFunction : IFunction
    {

        //****************************************//
        //    Define the TP Status Event          //
        //****************************************//
        public delegate void TPEventHandler(object sender, TPEventArgs ea);
        public event TPEventHandler TPStatusEvent;
        protected virtual void OnStatusChange(TPEventArgs ea)
        {
            TPEventHandler handler = TPStatusEvent;
            if (handler != null)
            {
                handler(this, ea);
            }
        }

        //****************************************//
        //    Define the State Status Event       //
        //****************************************//
        public delegate void StateEventHandler(object sender, StateMachineEventArgs ea);
        public event StateEventHandler STEvent;
        protected virtual void StateStatus(StateMachineEventArgs ea)
        {
            StateEventHandler handler = STEvent;
            if (handler != null)
            {
                handler(this, ea);
            }
        }


        USB_I2C_Bridge bridge;


        private Queue<STATE.states> queue = new Queue<STATE.states>();
        public Queue<STATE.states> states_queue
        {
            get { return queue; }
            set { queue = value; }
        }


        //##################################################################################################//


        public void initialize()
        {
            Trace.WriteLine("tp ==> In Function initialize.");
            try
            {
                bridge = new USB_I2C_Bridge();
                BridgeGetDevice();

                StateStatus(new StateMachineEventArgs(true, STATE.states.initialize, "tp ==> init succeed"));
            }
            catch (Exception ex)
            {
                bridge = null;
                Trace.WriteLine("tp ==> Error: " + ex.Message);

                //not try to fix the error in this class, directly go to the erorr handler.
                queue.Clear();
                queue.TrimExcess();
                queue.Enqueue(STATE.states.error);
                queue.Enqueue(STATE.states.initialize);

                StateStatus(new StateMachineEventArgs(false, STATE.states.initialize, "tp ==> init failed: " + ex.Message));
            }
        }

        public void working(STATE.states state)
        {
            try
            {

                if (state == STATE.states.TP_SendSleep1Command)
                {
                    Trace.WriteLine("tp ==> In Function Sleep1.");

                    byte[] datain = bridge.ReadWrite(0x25, 0x80, 0x04);

                    OnStatusChange(new TPEventArgs(1));
                }

                if (state == STATE.states.TP_SendDeepSleepCommand)
                {
                    Trace.WriteLine("tp ==> In Function Deep Sleep.");

                    byte[] datain = bridge.ReadWrite(0x25, 0x81, 0x04);

                    OnStatusChange(new TPEventArgs(2));
                }

                if (state == STATE.states.TP_PowerOn)
                {
                    Trace.WriteLine("tp ==> In Function Power On.");
                    BridgePowerOn();
                    System.Threading.Thread.Sleep(1000);
                    BridgeGetDUTAddress();

                    OnStatusChange(new TPEventArgs(0)); 
                }

                if (state == STATE.states.TP_PowerOff)
                {
                    Trace.WriteLine("tp ==> In Function Power Off.");
                    BridgePowerOff();

                    OnStatusChange(new TPEventArgs(4));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);

                //not try to fix the error in this class, directly go to the erorr handler.
                queue.Clear();
                queue.TrimExcess();
                queue.Enqueue(STATE.states.error);

                //save the current un-do state.
                queue.Enqueue(state);
            }
        }


        public void idle()
        {
            //Trace.WriteLine("==> In Function Idle.");
            //do nothing
        }

        public void error()
        {
            Trace.WriteLine("tp ==> In Function error.");

            //get the failure state
            STATE.states state = queue.Dequeue();
 
            //if it is not fatal error, then try to fix it.
            //TO-DO

            //if it is fatal error
            queue.Enqueue(STATE.states.exit);

            try
            {
                BridgePowerOff();
             }
            catch{ }
            try
            {
                BridgeCloseDevice();
            }
            catch { }

            bridge = null;
        }

        public void exit()
        {
            Trace.WriteLine("tp ==> In Function exit.");

            try
            {
                BridgePowerOff();
            }
            catch { }
            try
            {
                BridgeCloseDevice();
            }
            catch { }

            bridge = null;


            //generate event that this thread is dead.
            StateStatus(new StateMachineEventArgs(true, STATE.states.exit, "tp ==> exit"));

        }


        //##################################################################################################//

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


    }
}
