using System;
using System.Collections.Generic;
using System.Text;

namespace CypressSemiconductor.ChinaManufacturingTest.Work_Loop
{
    /// <summary>
    /// Define interface of basic functions of the state machine.
    /// </summary>
    public interface IFunction
    {
        Queue<STATE.states> states_queue
        {
            get;
            set;
        }
                
        void initialize();
        void working(STATE.states state);
        void error();
        void idle();
        void exit();
    }
    
    /// <summary>
    /// Define the states of state machine
    /// </summary>
    public static class STATE
    {
        public enum states
        {
            /// <summary>
            /// TP States
            /// </summary>
            TP_PowerOn,
            TP_PowerOff,
            TP_SendSleep1Command,
            TP_SendDeepSleepCommand,
            TP_ReadFinger,

            /// <summary>
            /// MM States
            /// </summary>
            MM_ReadCurr,
            MM_CalcSleep1Curr,
            MM_CalcDeepSleepCurr,
            /// <summary>
            /// Main States
            /// </summary>
            initialize,
            working,
            error,
            idle,
            exit
        };
    }


    /// <summary>
    /// Define the everntArgs of the state machine
    /// </summary>
    public class StateMachineEventArgs : EventArgs
    {
        public StateMachineEventArgs(bool pass_or_fail, STATE.states current_state, string message)
        {
            pf = pass_or_fail;
            st = current_state;
            ms = message;
        }

        private bool pf;
        public bool PassOrFail
        {
            get { return pf; }
            set { pf = value; }
        }

        private STATE.states st;
        public STATE.states CurrentState
        {
            get { return st; }
            set { st = value; }
        }

        private string ms;
        public string Message
        {
            get { return ms; }
            set { ms = value; }
        }
    }
}
