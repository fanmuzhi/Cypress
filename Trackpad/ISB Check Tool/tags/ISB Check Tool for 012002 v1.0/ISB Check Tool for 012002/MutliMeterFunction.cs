using System;
using System.Collections.Generic;
using System.Text;
using CypressSemiconductor.ChinaManufacturingTest.Work_Loop;
using System.Diagnostics;

namespace CypressSemiconductor.ChinaManufacturingTest
{

    //class containing event data
    public class MultiMeterEventArgs : EventArgs
    {
        public MultiMeterEventArgs(double curr)
        {
            current = curr; 
        }

        private double current;
        public double Current
        {
            get { return current; }
            set { current = value; }
        }
    }

    //class containing event data
    public class MMResultEventArgs : EventArgs
    {
        public MMResultEventArgs(int status, bool result, double average)
        {
            st = status;
            re = result;
            aver = average;
        }

        private int st;
        public int Status
        {
            get { return st; }
            set { st = value; }
        }

        private bool re;
        public bool Result
        {
            get { return re; }
            set { re = value; }
        }

        private double aver;
        public double Average
        {
            get { return aver; }
            set { aver = value; }
        }
    }

    class MutliMeterFunction:IFunction
    {

        //****************************************//
        //    Define the Current Value Event      //
        //****************************************//
        public delegate void MultiMeterEventHandler(object sender, MultiMeterEventArgs ea);
        public event MultiMeterEventHandler CurrentMeasumentEvent;
        protected virtual void OnMeasurement(MultiMeterEventArgs ea)
        {
            MultiMeterEventHandler handler = CurrentMeasumentEvent;
            if (handler != null)
            {
                handler(this, ea);
            }
        }

        //****************************************//
        //    Define the Result Sending Event     //
        //****************************************//
        public delegate void MMResultEventHandler(object sender, MMResultEventArgs ea);
        public event MMResultEventHandler ResultEvent;
        protected virtual void DisplayResult(MMResultEventArgs ea)
        {
            MMResultEventHandler handler = ResultEvent;
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

        private MultiMeter mm;

        private Queue<STATE.states> queue = new Queue<STATE.states>();
        public Queue<STATE.states> states_queue
        {
            get { return queue; }
            set { queue = value; }
        }


        private List<double> current;



        //##################################################################################################//


        public void initialize()
        {
            Trace.WriteLine("mm ==> In Function initialize.");
            try
            {
                mm = new MultiMeter("U3606A");

                current = new List<double>();

                StateStatus(new StateMachineEventArgs(true, STATE.states.initialize, "mm ==> init succeed"));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("tp ==> Error: " + ex.Message);

                //not try to fix the error in this class, directly go to the erorr handler.
                queue.Clear();
                queue.TrimExcess();
                queue.Enqueue(STATE.states.error);
                queue.Enqueue(STATE.states.initialize);

                StateStatus(new StateMachineEventArgs(false, STATE.states.initialize, "mm ==> init failed: " + ex.Message));
            }
        }
        
        public void working(STATE.states state)
        {
            try
            {
                if (state == STATE.states.MM_ReadCurr)
                {
                    Trace.WriteLine("mm ==> In Function Read Current.");

                    double curr = mm.MeasureChannelCurrent().average;
                    current.Add(curr);
                    OnMeasurement(new MultiMeterEventArgs(curr * 1000000)); //convert to uA

                }

                if (state == STATE.states.MM_CalcSleep1Curr)
                {
                    double sum = 0;
                    double aver = 0;
                    foreach (double temp in current)
                    {
                        sum += temp;
                    }

                    aver = Math.Round((sum / Config.MEAS_TIMES) * 1000000, 2); //convert to uA

                    if ((aver < Config.SLEEP1_MAX) && (aver > Config.SLEEP1_MIN))
                    {
                        DisplayResult(new MMResultEventArgs(1, true, aver));
                    }
                    else
                    {
                        DisplayResult(new MMResultEventArgs(1, false, aver));
                    }

                    current.Clear();
                }

                if (state == STATE.states.MM_CalcDeepSleepCurr)
                {
                    double sum = 0;
                    double aver = 0;
                    foreach (double temp in current)
                    {
                        sum += temp;
                    }

                    aver = Math.Round((sum / Config.MEAS_TIMES) * 1000000, 2);

                    if ((aver < Config.DEEP_SLEEP_MAX)&&(aver>Config.DEEP_SLEEP_MIN))
                    {
                        DisplayResult(new MMResultEventArgs(2, true, aver));
                    }
                    else
                    {
                        DisplayResult(new MMResultEventArgs(2, false, aver));
                    }

                    current.Clear();
                }
            }
            catch(Exception ex)
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

        public void error()
        {
            Trace.WriteLine("mm ==> In Function error.");

            //get the failure state
            STATE.states state = queue.Dequeue();

            //if it is not fatal error, then try to fix it.
            //TO-DO

            //if it is fatal error
            queue.Enqueue(STATE.states.exit);

            try
            {
                mm = null;
            }
            catch
            { }
        }
        
        public void idle()
        {
            //Trace.WriteLine("==> In Function idle.");
            //do nothing
        }

        public void exit()
        {
            Trace.WriteLine("mm ==> In Function exit.");

            //generate event that this thread is dead.
            StateStatus(new StateMachineEventArgs(true, STATE.states.exit, "mm ==> exit"));
        }

    }
}
