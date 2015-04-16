using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database_query_tool
{
    class StatisticsData
    {
        //Calculate average  
        public static double Ave(int[] a)
        {
            double sum = 0;
            foreach (int d in a)
            {

                sum = sum + d;
            }
            double ave = sum / a.Length;

            return ave;
        }
        //calculate variation  
        public static double Std(int[] v)
        {

            double average = Ave(v);

            double sumStd = 0;
            foreach (int d in v)
            {
                sumStd = sumStd + (d-average)*(d-average);
            }
            double Stdev = Math.Pow(sumStd / (v.Count() - 1), 0.5);
            
            return Stdev;
        }

        // calculate CPK
        public static double Cpk(double lowerLimit, double higherLimit, double average, double StDev)
        {
            double CpL = (average - lowerLimit) / (3 * StDev);
            double CpH = (higherLimit - average) / (3 * StDev);
            if (CpH > CpL)
                return CpL;
            else
                return CpH;
        }
        public static double Cpu(double lowerLimit, double higherLimit, double average, double StDev)
        {
            
            double CpuValue = (higherLimit - average) / (3 * StDev);
            
            return CpuValue;
        }

    }
}
