//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.Windows.Forms;
//using CypressSemiconductor.ChinaManufacturingTest.Work_Loop;

//namespace CypressSemiconductor.ChinaManufacturingTest
//{
//    public class MainFunction:IFunction
//    {

//        MutliMeterFunction mmFunction;
//        StatesThread Multi_Meter_Thread;


//        private Stack<STATE.states> stack = new Stack<STATE.states>();
//        public Stack<STATE.states> states_stack
//        {
//          get
//          {
//              return stack;
//          }

//          set
//          {
//              stack = value;
//          }
//        }

//        public void initialize()
//        {
//            Trace.WriteLine("==> In Function initialize.");
//        }

//        public void working()
//        {
//            Trace.WriteLine("==> In Function working.");
//        }

//        public void stop()
//        {
//            Trace.WriteLine("==> In Function stop.");
//        }


//        public void idle()
//        {
//            Trace.WriteLine("==> In Function idle.");
//        }

//        public void error()
//        {
//            Trace.WriteLine("==> In Function error.");
//        }

//        public void exit()
//        {
//            Trace.WriteLine("==> In Function exit.");
//        }

//    }
//}
