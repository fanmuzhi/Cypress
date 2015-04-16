using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CypressSemiconductor.ChinaManufacturingTest.Work_Loop
{
    public class StatesThread
    {

        private IFunction mf;
        private bool exit_thread = false;
        private STATE.states m_state;

        public StatesThread(IFunction interfaceFuntion)
        {
            mf = interfaceFuntion;
        
        }

        public void run()
        {
            try
            {
                Thread t1 = new Thread(thread_ruuning);
                t1.IsBackground = false;
                t1.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
            }
        }

        public void exit()
        {
            //exit_thread = true;

            mf.states_queue.Clear();
            mf.states_queue.TrimExcess();
            mf.states_queue.Enqueue(STATE.states.exit);
            
        }

        public void en_queue(List<STATE.states> states)
        {
            lock (mf.states_queue)
            {
                foreach(STATE.states st in states)
                mf.states_queue.Enqueue(st);
            }
        }

        public void en_queue(STATE.states state)
        {
            lock (mf.states_queue)
            {
                mf.states_queue.Enqueue(state);
            }
        }

        private void thread_ruuning()
        {

            mf.states_queue.Enqueue(STATE.states.initialize);
            mf.states_queue.Enqueue(STATE.states.idle);

            while (!exit_thread)
            {
                lock (mf.states_queue)
                {
                    m_state = mf.states_queue.Dequeue();
                    mf.states_queue.TrimExcess();
                    switch (m_state)
                    {
                        case STATE.states.initialize:

                            //Trace.WriteLine("In State: initalize.");
                            mf.initialize();
                            break;


                        case STATE.states.error:

                            //Trace.WriteLine("In State: error.");
                            mf.error();
                            break;

                        case STATE.states.exit:

                            //Trace.WriteLine("In State: exit.");
                            mf.exit();
                            exit_thread = true;

                            break;

                        case STATE.states.idle:
                            
                            //Trace.WriteLine("In State: idle.");
                            mf.states_queue.Enqueue(STATE.states.idle);
                            mf.idle();
                            break;
                        
                        case STATE.states.working:
                        default:
                            //Trace.WriteLine("In State: working.");
                            mf.working(m_state);
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }
    
    }//End of Class
}//End of NS

