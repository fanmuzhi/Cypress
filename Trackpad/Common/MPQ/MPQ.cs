using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public class MPQ
    {
        //private static Mutex mut = new Mutex(false);
        MPQIO m_MPQIO;

        ~MPQ()
        {
            if (m_MPQIO != null)
            {
                m_MPQIO.Dispose();
            }
        }

        //0 (programming mode), 1 (I2C mode) and 2 (PS2 mode)
        private const byte PROGRMMING_MODE = 0;
        private const byte I2C_MODE = 1;
        private const byte PS2_MODE = 2;

        private const byte I2C_DELAY = 20;
        //private const byte IO_VOLTAGE = 33; //IO_VOLTAGE=33 : 3.3V, IO_VOLTAGE=50 : 5.0V , for MPQ SCL and SDA

        private byte m_Address;
        public byte Address
        {
            set { m_Address = value; }
            get { return m_Address; }
        }

        //****************************************//
        //    Define the MPQ Message Event        //
        //****************************************//
        public delegate void MPQMessageEventHandler(object sender, MPQMessageEventArgs ea);
        public event MPQMessageEventHandler MPQMessageEvent;
        protected virtual void OnMPQMessage(MPQMessageEventArgs ea)
        {
            MPQMessageEventHandler handler = MPQMessageEvent;
            if (handler != null)
            {
                handler(this, ea);
            }
        }

        //****************************************//
        //    Open RS232 Port for MPQ device      //
        //****************************************//
        public void PortInit(String ConnectString)
        {
            if (m_MPQIO == null)
            {
                m_MPQIO = new MPQIO();
                m_MPQIO.CloseComm();
                if (!m_MPQIO.OpenComm(ConnectString))
                    throw new Exception("Error in OpenComm: " + m_MPQIO.m_LastError);
                else
                    OnMPQMessage(new MPQMessageEventArgs("MPQ_OpenComm: Success"));
            }

        }

        //****************************************//
        //        Programming Fuctions            //
        //****************************************//
        public void ProgrammerInit(byte IO_VOLTAGE, byte PortEnables)
        {
            //mut.WaitOne();
            if (m_MPQIO.SelectProgrammer(m_Address))
            {
                if (!m_MPQIO.SetVoltage(m_Address, IO_VOLTAGE))
                    throw new Exception("Error in Set Voltage: " + m_MPQIO.m_LastError);
                if (!m_MPQIO.SelectMode(m_Address, PROGRMMING_MODE))
                    throw new Exception("Error in SelectMode: " + m_MPQIO.m_LastError);
                if (!m_MPQIO.EnablePorts(m_Address, PortEnables))
                    throw new Exception("Error in EnablePorts: " + m_MPQIO.m_LastError);
            }
            else
            {
                m_MPQIO.CloseComm();
                throw new Exception("Cannot accsess to MPQ: " + m_MPQIO.m_LastError);
                //OnMPQMessage(new MPQMessageEventArgs("Cannot accsess to MPQ:" + m_Address.ToString()));
            }
            //mut.ReleaseMutex();
        }

        public void Programming(byte ImagID)
        {
            //mut.WaitOne();
            if (m_MPQIO.SelectProgrammer(m_Address))
            {
                if (!m_MPQIO.Program(m_Address, ImagID))
                    throw new Exception("Error in Program: " + m_MPQIO.m_LastError);
            }
            else
            {
                m_MPQIO.CloseComm();
                throw new Exception("Cannot accsess to MPQ: " + m_MPQIO.m_LastError);
                //OnMPQMessage(new MPQMessageEventArgs("Cannot accsess to MPQ:" + m_Address.ToString()));
            }
            //mut.ReleaseMutex();
        }

        public void CheckProgrammingStatus(out byte[] PortStatusUB)
        {
            //mut.WaitOne();

            PortStatusUB = new byte[5] { 1, 1, 1, 1, 1 };
            List<byte> Status = new List<byte>();
            if (m_MPQIO.SelectProgrammer(m_Address))
            {
                if (!m_MPQIO.Status(m_Address, Status))
                { throw new Exception("Error in CheckProgramStatus: " + m_MPQIO.m_LastError); }
                else
                {
                    string str = "";
                    int i = 0;
                    foreach (byte pstatus in Status)
                    {
                        PortStatusUB[i] = pstatus;
                        str += pstatus.ToString() + ", ";
                        i++;
                    }
                    OnMPQMessage(new MPQMessageEventArgs("MPQ" + m_Address.ToString() + "_Programming_Status: " + str));
                }
            }
            else
            {
                m_MPQIO.CloseComm();
                throw new Exception("Cannot accsess to MPQ:" + m_MPQIO.m_LastError);
                //OnMPQMessage(new MPQMessageEventArgs("Cannot accsess to MPQ:" + m_Address.ToString()));

            }

            //mut.ReleaseMutex();
        }

        //****************************************//
        //            I2C Fuctions                //
        //****************************************//
        /// <summary>
        /// Initialize the I2C function of MPQ
        /// </summary>
        /// <param name="PortEnalbes"></param>Enable Port, 0xF for all enabled. 0x0 for all disabled.
        /// <param name="IO_VOLTAGE"></param>33 : 3.3V, 50 : 5.0V , for MPQ SCL and SDA
        /// <param name="Polarity"></param>Polarity=0 for ST&MTG, Polarity=1 for APA.
        public void I2CInit(byte PortEnalbes, byte Polarity)
        {
            //mut.WaitOne();
            if (m_MPQIO.SelectProgrammer(m_Address))
            {
                if (!m_MPQIO.ResetPolarity(m_Address, Polarity))
                    throw new Exception("Error in SelectMode: " + m_MPQIO.m_LastError);

                if (!m_MPQIO.SelectMode(m_Address, I2C_MODE))
                    throw new Exception("Error in SelectMode: " + m_MPQIO.m_LastError);
                if (!m_MPQIO.I2C_ClockSelect(m_Address, 7))
                    throw new Exception("Error in I2C_ClockSelect: " + m_MPQIO.m_LastError);
                if (!m_MPQIO.EnablePorts(m_Address, PortEnalbes))
                    throw new Exception("Error in EnablePorts: " + m_MPQIO.m_LastError);
            }
            else
            {
                m_MPQIO.CloseComm();
                throw new Exception("Cannot accsess to MPQ: " + m_MPQIO.m_LastError);
                //OnMPQMessage(new MPQMessageEventArgs("Cannot accsess to MPQ:" + m_Address.ToString()));
            }

            //mut.ReleaseMutex();
        }

        /// <summary>
        /// Read and Write based on I2C 
        /// </summary>
        /// <param name="I2CCommand"></param>command list, {0, command, parameter}
        /// <param name="I2C_Address"></param>I2C address of DUT
        /// <param name="BytesToRead"></param>how many bytes return
        /// <param name="RcvDataBAP"></param>bytes returned
        public void I2CRun(byte[] I2CCommand, byte I2C_Address, byte BytesToRead, out byte[,] RcvDataBAP)
        {
            //mut.WaitOne();

            RcvDataBAP = new byte[4, BytesToRead + 1];

            if (m_MPQIO.SelectProgrammer(m_Address))
            {
                List<byte> command = new List<byte>();
                //command.Add(0x00);          //offset
                //command.Add(I2CCommand);    //I2C command
                foreach (byte comm in I2CCommand)
                { command.Add(comm); }

                List<List<byte>> rcvData = new List<List<byte>>();
                for (int i = 0; i <= 3; i++)
                {
                    rcvData.Add(new List<byte>());
                }

                if (!m_MPQIO.I2C_Xaction(m_Address, command, I2C_Address, rcvData, BytesToRead, I2C_DELAY))
                {
                    throw new Exception("Error in I2C_Xaction: " + m_MPQIO.m_LastError);
                }


                int indexX = 0;
                foreach (List<byte> rVs in rcvData)
                {
                    int indexY = 0;
                    string str = "";
                    foreach (byte rV in rVs)
                    {
                        RcvDataBAP[indexX, indexY] = rV;
                        indexY++;
                        str += rV.ToString() + ", ";
                    }
                    OnMPQMessage(new MPQMessageEventArgs("MPQ" + m_Address.ToString() + "_I2CRun_Status: " + str));
                    indexX++;
                }
            }
            else
            {
                m_MPQIO.CloseComm();
                throw new Exception("Cannot accsess to MPQ: " + m_MPQIO.m_LastError);
                //OnMPQMessage(new MPQMessageEventArgs("Cannot accsess to MPQ:" + m_Address.ToString()));
            }

            //mut.ReleaseMutex();
        }

        public void I2CRun(byte[] I2CCommand, byte I2C_Address, byte BytesToRead, out byte[,] RcvDataBAP, bool writeOnly)
        {
            //mut.WaitOne();

            RcvDataBAP = new byte[4, BytesToRead + 1];

            if (m_MPQIO.SelectProgrammer(m_Address))
            {
                List<byte> command = new List<byte>();
                //command.Add(0x00);          //offset
                //command.Add(I2CCommand);    //I2C command
                //command.Add(Parameter);     //Parameter
                foreach (byte comm in I2CCommand)
                { command.Add(comm); }

                List<List<byte>> rcvData = new List<List<byte>>();
                for (int i = 0; i <= 3; i++)
                {
                    rcvData.Add(new List<byte>());
                }

                if (!m_MPQIO.I2C_Xaction(m_Address, command, I2C_Address, rcvData, BytesToRead, I2C_DELAY, writeOnly))
                {
                    throw new Exception("Error in I2C_Xaction: " + m_MPQIO.m_LastError);
                }

            }
            else
            {
                m_MPQIO.CloseComm();
                throw new Exception("Cannot accsess to MPQ: " + m_MPQIO.m_LastError);
                //OnMPQMessage(new MPQMessageEventArgs("Cannot accsess to MPQ:" + m_Address.ToString()));
            }

            //mut.ReleaseMutex();
        }
    }


    public class MPQMessageEventArgs : EventArgs
    {
        public MPQMessageEventArgs(string s)
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
}
