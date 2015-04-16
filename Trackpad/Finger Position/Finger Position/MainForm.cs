using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace CypressSemiconductor.ChinaManufacturingTest.Logitech_RemoteControl
{
    public partial class MainForm : Form
    {
        Trackpad trackPad;
        Thread readPositionThread;

        //****************************************//
        //             Set List Box               //
        //****************************************//
        private delegate void SetListBoxCallback(string text);

        private void SetListBoxStatus(string text)
        {
            if (this.listBoxStatus.InvokeRequired)
            {
                SetListBoxCallback d1 = new SetListBoxCallback(SetListBoxStatus);
                this.Invoke(d1, new object[] { text });
            }
            else
            {
                listBoxStatus.Items.Add(text);
                int countNum = listBoxStatus.Items.Count;
                listBoxStatus.SetSelected(countNum - 1, true);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            
            trackPad = new Trackpad();
            trackPad.MessageChangeEvent +=new Trackpad.MessageChangeEventHandler(trackPad_MessageChangeEvent);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            trackPad.ReadPositionStop();
            Thread.Sleep(10);
            trackPad.MessageChangeEvent -= new Trackpad.MessageChangeEventHandler(trackPad_MessageChangeEvent);
            trackPad.BridgeClose();
        }

        private void buttonReadPosition_Click(object sender, EventArgs e)
        {
            if (readPositionThread == null)
            {
                try
                {
                    trackPad.BridgeOpen();

                    readPositionThread = new Thread(new ThreadStart(trackPad.ReadPosition));
                    readPositionThread.IsBackground = true;
                    readPositionThread.Start();
                    while (!readPositionThread.IsAlive)
                    {
                        Thread.Sleep(5);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                buttonReadPosition.Text = "STOP";
            }
            else
            {
                trackPad.ReadPositionStop();
                Thread.Sleep(10);
                readPositionThread.Abort();
                readPositionThread = null;
                buttonReadPosition.Text = "START";
                trackPad.BridgeClose();
            }    
        }

        private void trackPad_MessageChangeEvent(object sender, MessageChangeEventArgs e)
        {
            SetListBoxStatus(e.Message);
            PositionPanel.Invalidate();
        }

        //****************************************//
        //        Draw Trackpad Position          //
        //****************************************//
        private void PositionPanel_Paint(object sender, PaintEventArgs e)
        {
            PositionPanel.Size = new Size(600, 300);
            
            SolidBrush pointB = new SolidBrush(Color.Blue);
            SolidBrush pointB2 = new SolidBrush(Color.Black);
            SolidBrush pointB3 = new SolidBrush(Color.Brown);
            SolidBrush pointB4 = new SolidBrush(Color.HotPink);
            SolidBrush pointB5 = new SolidBrush(Color.Green);

            int finger1X = ((int)Trackpad.Register.Finger1X / 2);
            int finger1Y = ((int)Trackpad.Register.Finger1Y / 2);

            int finger2X = ((int)Trackpad.Register.Finger2X / 2);
            int finger2Y = ((int)Trackpad.Register.Finger2Y / 2);

            int finger3X = ((int)Trackpad.Register.Finger3X / 2);
            int finger3Y = ((int)Trackpad.Register.Finger3Y / 2);

            int finger4X = ((int)Trackpad.Register.Finger4X / 2);
            int finger4Y = ((int)Trackpad.Register.Finger4Y / 2);

            int finger5X = ((int)Trackpad.Register.Finger5X / 2);
            int finger5Y = ((int)Trackpad.Register.Finger5Y / 2);

            //draw Finger1 Dot
            e.Graphics.FillRectangle(pointB, finger1X, finger1Y, 1, 1);
            for (int i = 1; i <= 10; i++)
            {
                e.Graphics.FillRectangle(pointB, finger1X + i, finger1Y, 1, 1);
                e.Graphics.FillRectangle(pointB, finger1X - i, finger1Y, 1, 1);
                e.Graphics.FillRectangle(pointB, finger1X, finger1Y + i, 1, 1);
                e.Graphics.FillRectangle(pointB, finger1X, finger1Y - i, 1, 1);
            }

            //draw Finger2 Dot
            e.Graphics.FillRectangle(pointB2, finger2X, finger2Y, 1, 1);
            for (int i = 1; i <= 10; i++)
            {
                e.Graphics.FillRectangle(pointB2, finger2X + i, finger2Y, 1, 1);
                e.Graphics.FillRectangle(pointB2, finger2X - i, finger2Y, 1, 1);
                e.Graphics.FillRectangle(pointB2, finger2X, finger2Y + i, 1, 1);
                e.Graphics.FillRectangle(pointB2, finger2X, finger2Y - i, 1, 1);
            }

            //draw Finger3 Dot
            e.Graphics.FillRectangle(pointB3, finger3X, finger3Y, 1, 1);
            for (int i = 1; i <= 10; i++)
            {
                e.Graphics.FillRectangle(pointB3, finger3X + i, finger3Y, 1, 1);
                e.Graphics.FillRectangle(pointB3, finger3X - i, finger3Y, 1, 1);
                e.Graphics.FillRectangle(pointB3, finger3X, finger3Y + i, 1, 1);
                e.Graphics.FillRectangle(pointB3, finger3X, finger3Y - i, 1, 1);
            }

            //draw Finger4 Dot
            e.Graphics.FillRectangle(pointB4, finger4X, finger4Y, 1, 1);
            for (int i = 1; i <= 10; i++)
            {
                e.Graphics.FillRectangle(pointB4, finger4X + i, finger4Y, 1, 1);
                e.Graphics.FillRectangle(pointB4, finger4X - i, finger4Y, 1, 1);
                e.Graphics.FillRectangle(pointB4, finger4X, finger4Y + i, 1, 1);
                e.Graphics.FillRectangle(pointB4, finger4X, finger4Y - i, 1, 1);
            }

            //draw Finger5 Dot
            e.Graphics.FillRectangle(pointB5, finger5X, finger5Y, 1, 1);
            for (int i = 1; i <= 10; i++)
            {
                e.Graphics.FillRectangle(pointB5, finger5X + i, finger5Y, 1, 1);
                e.Graphics.FillRectangle(pointB5, finger5X - i, finger5Y, 1, 1);
                e.Graphics.FillRectangle(pointB5, finger5X, finger5Y + i, 1, 1);
                e.Graphics.FillRectangle(pointB5, finger5X, finger5Y - i, 1, 1);
            }
        }
    }
}
