using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CypressSemiconductor.ChinaManufacturingTest
{
    public class MPQIO
    {
        SerialPort m_SerialPort = null;

        const int MPQ_MAX_XFER = 1024;

        internal string m_LastError = "";
        //byte m_I2CAddress = 0;
    
        const byte ESC = 0x1B;
        const byte SOP = 0x53;
        const byte EOP = 0x45;

        const byte CMD_FWREV = 0;
        const byte CMD_PROGRAM = 4;
        const byte CMD_STATUS = 5;
        const byte CMD_SET_PORT_DISABLE_MASK = 7;
        const byte CMD_SET_VOLTAGE = 0x0F;
        const byte CMD_RESET_POLARITY = 0x10;

        const byte XCMD_SELECT_MODE = 0x41;
        const byte XCMD_I2C_XACTION = 0x42;
        const byte XCMD_PS2_XACTION = 0x43;
        const byte XCMD_PS2_POLL = 0x44;
        const byte XCMD_SELECT_I2C_CLOCK = 0x45;
        const byte XCMD_SPI_XACTION = 0x47;


        ///// <summary>
        ///// The constructor for the MPQ class
        ///// </summary>
        ///// <param name="I2CAddress">The I2C address of the DUTs attached to the MPQ</param>
        //public MPQIO(byte I2CAddress)
        //{
        //    m_I2CAddress = I2CAddress;
        //}

        /// <summary>
        /// Closes the COM port.  Then, implements work-around for .NET exception if MTS was disconnected from USB.
        /// </summary>
        public void Dispose()
        {
            CloseComm();

            // Work-around for serial port bug in .NET 
            try
            {
                GC.ReRegisterForFinalize(m_SerialPort.BaseStream);
            }
            catch { }
        }

        /// <summary>
        /// Attempts to open and configure the serial port named in the 'port' parameter.
        /// Read and Write timeouts are set to 1 second.
        /// Baud Rate is 115200
        /// </summary>
        /// <param name="port">The name of the serial port to open (i.e. "COM1")</param>
        /// <returns>True if able to open the port.</returns>
        public bool OpenComm(string port)
        {
            m_SerialPort = new SerialPort(port);

            m_SerialPort.BaudRate = 115200;
            m_SerialPort.DtrEnable = true;
            m_SerialPort.RtsEnable = true;
            m_SerialPort.Parity = Parity.None;
            m_SerialPort.StopBits = StopBits.One;
            m_SerialPort.ReadTimeout = 1000;
            m_SerialPort.WriteTimeout = 1000;

            try
            {
                m_SerialPort.Open();

                // Work-around for serial port bug in .NET 
                GC.SuppressFinalize(m_SerialPort.BaseStream);
            }
            catch
            {
                m_SerialPort.Dispose();
                m_SerialPort = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the serial port if it is open
        /// </summary>
        public void CloseComm()
        {
            if (m_SerialPort != null)
            {
                m_SerialPort.Close();
                System.Threading.Thread.Sleep(50);
            }
               

        }

        /// <summary>
        /// Selects the active MPQ.  Future IO needs to specify this MPQ address. 
        /// </summary>
        /// <param name="mpq">The 1-based address (MPQ number) of the MPQ to be made active</param>
        /// <returns>true if the specified MPQ is now active, otherwise false</returns>
        public bool SelectProgrammer(byte mpq)
        {
            List<byte> cmd = new List<byte>();
            List<byte> data = new List<byte>();
            cmd.Add(mpq);
            cmd.Add(CMD_FWREV);

            bool rVal = false;

            if (SendCommand(cmd))
                if (ReadResponse(data, 500))
                    rVal = CheckHeader(5, mpq, CMD_FWREV, data);

            return rVal;
        }

        /// <summary>
        /// Sends the CMD_RESET_POLARITY command to the specified MPQ
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="val">Must be 0 or 1</param>
        /// <returns>True if the command was successfully sent</returns>
        public bool ResetPolarity(byte mpq, byte val)
        {
            return SendCommandWithArg(mpq, CMD_RESET_POLARITY, val, 500);
        }

        /// <summary>
        /// Sets the voltage range of the specified MPQ
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="val">The voltage in hundreds of mV.  (To set the range to 3.3V, pass 33)</param>
        /// <returns>True if the voltage range was successfully set</returns>
        public bool SetVoltage(byte mpq, byte val)
        {
            return SendCommandWithArg(mpq, CMD_SET_VOLTAGE, val, 500);
        }

        /// <summary>
        /// Sets the mode of the MPQ
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="mode">Possible values are 0 (programming mode), 1 (I2C mode) and 2 (PS2 mode)</param>
        /// <returns>True if the MPQ mode was successfully set</returns>
        public bool SelectMode(byte mpq, byte mode)
        {
            return SendCommandWithArg(mpq, XCMD_SELECT_MODE, mode, 5000);
        }

        /// <summary>
        /// Initiates a device programming sequence by the specified MPQ
        /// </summary>
        /// <param name="mpq">A 1-based index specifying which MPQ should receive the command</param>
        /// <param name="imageNum">Specifies the 1-base FW image, contained in the MPQ, that will be programmed into the devices</param>
        /// <returns>True if the CMD_PROGRAM command was successfully sent</returns>
        public bool Program(byte mpq, byte imageNum)
        {
            return SendCommandWithArg(mpq, CMD_PROGRAM, (byte)(imageNum - 1), 500);
        }

        /// <summary>
        /// Retrieves the programming status bytes from the MPQ
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="status">A list containing 5 status bytes.  The first byte is a sum of the remaining 4.
        /// Each of the last 4 status bytes reflects the programming status of the DUTs on the respective MPQ port.</param>
        /// <returns>True if able to obtain status from the MPQ</returns>
        public bool Status(byte mpq, List<byte> status)
        {
            List<byte> cmd = new List<byte>();
            List<byte> data = new List<byte>();
            cmd.Add(mpq);
            cmd.Add(CMD_STATUS);

            status.Clear();

            if (SendCommand(cmd))
                if (ReadResponse(data, 500))
                {
                    m_LastError = "";

                    if (data.Count < 7)
                        m_LastError = "Command response length";
                    else if (data[0] != mpq)
                        m_LastError = "Command response address";
                    else if (data[1] != CMD_STATUS + 0x80)
                        m_LastError = "Command response ID";
                    else if (data[2] != 0)
                        m_LastError = string.Format("Command response error: 0x{0:X2}", data[2]);

                    if (m_LastError != "")
                    {
                        status.Add(0xFF);
                        return false;
                    }
                }

            byte statSum = 0;
            for (int i = 0; i < 4; i++)
                statSum += data[i + 3];

            status.Add(statSum);
            for (int i = 0; i < 4; i++)
                status.Add(data[i + 3]);

            return true;
        }

        /// <summary>
        /// Enables the MPQ communication channels specified by the 4 bits in the low-order nibble of the mask parameter
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="mask">A bit mask that indicates which MPQ IO channels should be enabled.  (0000 1111) enables all 4 channels.</param>
        /// <returns>True if the enable command was successfully sent to the device.</returns>
        public bool EnablePorts(byte mpq, byte mask)
        {
            mask = (byte)(mask ^ 0x0F); // Invert the bits, making it a disable mask

            mask = (byte)(mask & 0x0F); // Mask-off the upper bits.

            return SendCommandWithArg(mpq, CMD_SET_PORT_DISABLE_MASK, mask, 500);
        }

        /// <summary>
        /// Sends a command to all 4 ports of the active MPQ.  Then reads 4 responses from the MPQ.
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="command">A list of bytes to send to all 4 devices.  The first byte is an offset value.
        /// If this list only contains a single (offset) value, no I2C write is performed.  But data is read from the devices,
        /// beginning at the specified offset. (Effectively turns this method into a 'Read at Offset' method.)</param>
        /// <param name="rxData">A List of Lists of bytes that will receive the data sent back from the 4 devices</param>
        /// <param name="readLen">The number of bytes to request from each port</param>
        /// <param name="msDelay">The delay, in ms, used by the MPQ between issuing the I2C write and the I2C read</param>
        /// <returns></returns>
        public bool I2C_Xaction(byte mpq, List<byte> command, byte i2cAddress, List<List<byte>> rxData, byte readLen, byte msDelay)
        {
            List<byte> cmd = new List<byte>();
            List<byte> data = new List<byte>();
            cmd.Add(mpq);
            cmd.Add(XCMD_I2C_XACTION);
            cmd.Add(i2cAddress);
            cmd.Add((byte)command.Count);
            cmd.Add(readLen);
            cmd.Add(msDelay);

            foreach (byte b in command)
                cmd.Add(b);

            foreach (List<byte> L in rxData)
                L.Clear();

            DateTime t1 = DateTime.Now;

            if (SendCommand(cmd))
                if (ReadResponses(rxData, 500))
                    for (int i = 0; i < 4; i++)
                    {
                        m_LastError = "";

                        if (rxData[i].Count < 4)
                            m_LastError = "Command response length";
                        else if (rxData[i][0] != mpq)
                            m_LastError = "Command response address";
                        else if (rxData[i][1] != XCMD_I2C_XACTION + 0x80)
                            m_LastError = "Command response ID";
                        else if (rxData[i][2] != 0)
                            m_LastError = string.Format("Command response error: 0x{0:X2}", rxData[i][2]);

                        if (m_LastError == "") // Strip off the header
                            rxData[i].RemoveRange(0, 3);
                    }
                else
                    return false;

            TimeSpan elapsed = DateTime.Now - t1;

            return true;
        }

        /// <summary>
        /// Sends a command to all 4 ports of the active MPQ.  Then reads 4 responses from the MPQ.
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="command">A list of bytes to send to all 4 devices.  The first byte is an offset value.
        /// If this list only contains a single (offset) value, no I2C write is performed.  But data is read from the devices,
        /// beginning at the specified offset. (Effectively turns this method into a 'Read at Offset' method.)</param>
        /// <param name="rxData">A List of Lists of bytes that will receive the data sent back from the 4 devices</param>
        /// <param name="readLen">The number of bytes to request from each port</param>
        /// <param name="msDelay">The delay, in ms, used by the MPQ between issuing the I2C write and the I2C read</param>
        /// <returns></returns>
        public bool I2C_Xaction(byte mpq, List<byte> command, byte i2cAddress, List<List<byte>> rxData, byte readLen, byte msDelay, bool writeOnly)
        {
            List<byte> cmd = new List<byte>();
            List<byte> data = new List<byte>();
            cmd.Add(mpq);
            cmd.Add(XCMD_I2C_XACTION);
            cmd.Add(i2cAddress);
            cmd.Add((byte)command.Count);
            cmd.Add(readLen);
            cmd.Add(msDelay);

            foreach (byte b in command)
                cmd.Add(b);

            foreach (List<byte> L in rxData)
                L.Clear();

            DateTime t1 = DateTime.Now;

            if (SendCommand(cmd))
            {
                if (!writeOnly)
                {
                    if (ReadResponses(rxData, 500))
                        for (int i = 0; i < 4; i++)
                        {
                            m_LastError = "";

                            if (rxData[i].Count < 4)
                                m_LastError = "Command response length";
                            else if (rxData[i][0] != mpq)
                                m_LastError = "Command response address";
                            else if (rxData[i][1] != XCMD_I2C_XACTION + 0x80)
                                m_LastError = "Command response ID";
                            else if (rxData[i][2] != 0)
                                m_LastError = string.Format("Command response error: 0x{0:X2}", rxData[i][2]);

                            if (m_LastError == "") // Strip off the header
                                rxData[i].RemoveRange(0, 3);
                        }
                    else
                        return false;
                }
                TimeSpan elapsed = DateTime.Now - t1;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the I2C clock speed for the MPQ
        /// </summary>
        /// <param name="mpq">1-based MPQ address.  Must be the same as the last address set with SelectProgrammer.</param>
        /// <param name="clock">Numeric code for the clock speed.  7 = 400 kHz, 29 = 100 kHz, 58 = 50 kHz</param>
        /// <returns></returns>
        public bool I2C_ClockSelect(byte mpq, byte clock)
        {
            return SendCommandWithArg(mpq, XCMD_SELECT_I2C_CLOCK, clock, 500);
        }

        /// <summary>
        /// Calls the .NET SerialPort.Write method to write data to the MPQ
        /// </summary>
        /// <param name="data">A list of bytes to send to the MPQ</param>
        /// <returns>true if the write succeeded.  False if failed (including timeout)</returns>
        bool WriteBytes(List<byte> data)
        {
            byte[] dta = data.ToArray();

            try
            {
                DateTime t1 = DateTime.Now;
                m_SerialPort.Write(dta, 0, dta.Length);
                TimeSpan elapsed = DateTime.Now - t1;

                return true;
            }
            catch
            {
                m_LastError = "MPQ_Write exception";
            }

            return false;
        }

        /// <summary>
        /// Calls the .NET SerialPort.Read method to read MPQ_MAX_XFER bytes from the MPQ.
        /// Short reads do not cause a failure.
        /// </summary>
        /// <param name="data">A list of bytes read from the MPQ</param>
        /// <returns>true if the read succeeded.  False if failed (including timeout)</returns>
        bool ReadBytes(List<byte> data)
        {
            int len = MPQ_MAX_XFER;
            byte[] dta = new byte[len];

            try
            {
                DateTime t1 = DateTime.Now;
                int bytesRead = m_SerialPort.Read(dta, 0, len);
                TimeSpan elapsed = DateTime.Now - t1;

                data.Clear();
                for (int i = 0; i < bytesRead; i++)
                    data.Add(dta[i]);

                return true;
            }
            catch
            {
                m_LastError = "MPQ_Read exception";
            }

            return false;
        }

        /// <summary>
        /// Implements the MPQ command protocol
        /// </summary>
        /// <param name="cmd">A list of bytes to send to the MPQ.</param>
        /// <returns>true if the call to MPQ_Write succeeds, otherwise, false</returns>
        bool SendCommand(List<byte> cmd)
        {
            DateTime t1 = DateTime.Now;
            m_SerialPort.DiscardInBuffer();
            m_SerialPort.DiscardOutBuffer();

            List<byte> command = new List<byte>();
            command.Add(ESC);
            command.Add(SOP);

            foreach (byte b in cmd)
            {
                if (b == ESC)
                    command.Add(ESC);

                command.Add(b);
            }

            command.Add(ESC);
            command.Add(EOP);

            bool rval = WriteBytes(command);

            //System.Threading.Thread.Sleep(500); //QIBO: for APA delay

            TimeSpan elapsed = DateTime.Now - t1;
            
            return rval;
        }

        /// <summary>
        /// Sends a command and argument to the MPQ.  Reads the MPQ Response.
        /// </summary>
        /// <param name="mpq">The MPQ address.  Must be the same as the last address set with MPQ_SelectProgrammer.</param>
        /// <param name="command">The MPQ command code to send</param>
        /// <param name="arg">An argument byte</param>
        /// <param name="msTimeout">Time limit, in ms, to wait for the response</param>
        /// <returns>true if command sent and error-free response received, otherwise false</returns>
        bool SendCommandWithArg(byte mpq, byte command, byte arg, int msTimeout)
        {
            List<byte> cmd = new List<byte>();
            List<byte> data = new List<byte>();
            cmd.Add(mpq);
            cmd.Add(command);
            cmd.Add(arg);

            if (SendCommand(cmd))
                if (ReadResponse(data, msTimeout))
                    return CheckHeader(3, mpq, command, data);

            return false;
        }

        /// <summary>
        /// Reads and parses a single MPQ response after a command has been sent
        /// </summary>
        /// <param name="response">A list of bytes returned by the MPQ</param>
        /// <param name="msTimeout">The maximum time, in ms, alloted to receive a response from the MPQ</param>
        /// <returns>true if a response is received from the MPQ within the alloted timeout, otherwise false</returns>
        bool ReadResponse(List<byte> response, int msTimeout)
        {
            DateTime t1 = DateTime.Now;
            TimeSpan elapsed = new TimeSpan();

            response.Clear();

            bool startOfPage = false;
            bool escaped = false;

            int reads = 0;

            do
            {
                List<byte> data = new List<byte>();

                if (ReadBytes(data))
                {
                    reads++;

                    foreach (byte b in data)
                        if (b == ESC)
                        {
                            escaped = !escaped;     // Toggle

                            if (!escaped)
                                response.Add(b);    // ESC ESC
                        }

                        else if (escaped)
                        {
                            if (b == EOP)
                            {
                                elapsed = DateTime.Now - t1;
                                return true;
                            }

                            else if (b == SOP)
                            {
                                startOfPage = true;
                                response.Clear();
                            }

                            else
                                response.Add(b);    // Add the escaped char

                            escaped = false;
                        }

                        else if (startOfPage)
                            response.Add(b);

                }
                else
                    return false;

                elapsed = DateTime.Now - t1;
            }
            while (elapsed.TotalMilliseconds < msTimeout);

            return false;
        }

        /// <summary>
        /// Reads and parses 4 responses (1 for each port) from the MPQ
        /// </summary>
        /// <param name="responses">A list of lists, each one of which contains the response for a single port</param>
        /// <param name="msTimeout">The maximum time, in ms, alloted to receive the responses from the MPQ</param>
        /// <returns>true if 4 responses received in the alloted time, otherwise false</returns>
        bool ReadResponses(List<List<byte>> responses, int msTimeout)
        {
            DateTime t1 = DateTime.Now;
            TimeSpan elapsed = new TimeSpan();

            foreach (List<byte> L in responses)
                L.Clear();

            bool startOfPage = false;
            bool escaped = false;

            int rx = 0;
            int reads = 0;  // A counter for debug purposes
            List<byte> data = new List<byte>();

            do
            {
                if (ReadBytes(data))
                {
                    reads++;

                    if (rx > 3) rx = 0;

                    int i = 0;  // A counter for debug purposes

                    foreach (byte b in data)
                    {
                        if (b == ESC && rx < 4)
                        {
                            escaped = !escaped;     // Toggle

                            if (!escaped)
                                responses[rx].Add(b);    // ESC ESC
                        }

                        else if (escaped)
                        {
                            if (b == EOP)// || b == 0x05)
                            {
                                startOfPage = false;
                                rx++;
                            }

                            else if (b == SOP && rx < 4)
                            {
                                startOfPage = true;
                                responses[rx].Clear();
                            }

                            else if (rx < 4)
                                responses[rx].Add(b);    // Add the escaped char

                            escaped = false;
                        }

                        else if (startOfPage && rx < 4)
                            responses[rx].Add(b);

                        i++;
                    }

                }

                elapsed = DateTime.Now - t1;

            } while (rx < 4 && elapsed.TotalMilliseconds < msTimeout);

            return rx == 4;

        }

        bool CheckHeader(int len, byte mpq, byte cmd, List<byte> data)
        {
            m_LastError = "";

            if (data.Count != len)
                m_LastError = "Command response length";

            else if (data[0] != mpq)
                m_LastError = "Command response address";

            else if (data[1] != (cmd + 0x80))
                m_LastError = "Command response ID";

            else if (data[2] != 0)
                m_LastError = string.Format("Command response error: 0x{0:X2}", data[2]);

            return m_LastError == "";
        }
    }
}
