using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace D3marcoUI
{
    public partial class Form1 : Form
    {
        int BuffTimer = 10000;
        string Buff1 = "";
        string Buff2 = "";
        string Buff3 = "";
        string Spam1 = "";
        string Spam2 = "";
        string Spam3 = "";
        bool run = true;

        public Form1()
        {
            InitializeComponent();

           //set Buff Timer
           if (comboBox6.Text == "30 seconds") BuffTimer = 30 * 1000;
           if (comboBox6.Text == "1 minute") BuffTimer = 60 * 1000;
           if (comboBox6.Text == "3 minutes") BuffTimer = 180 * 1000;
           if (comboBox6.Text == "6 minutes") BuffTimer = 360 * 1000;
           if (comboBox6.Text == "9 minutes") BuffTimer = 520 * 1000;
           if (comboBox6.Text == "10 minutes") BuffTimer = 600 * 1000;           
        }

        #region Buff
        public void Buff()
        {
            while (run)
            {
                SendKeys.SendWait(Buff1);
                System.Threading.Thread.Sleep(350);
                SendKeys.SendWait(Buff2);
                System.Threading.Thread.Sleep(400);
                SendKeys.SendWait(Buff3);
                System.Threading.Thread.Sleep(BuffTimer);
            }
        }
        #endregion 

        #region Spam
        public void Spam()
        {
            //Spam Thread
            while (run)
            {
                while (Control.ModifierKeys == Keys.Shift)
                {
                    SendKeys.SendWait(Spam1);
                    System.Threading.Thread.Sleep(100);
                    SendKeys.SendWait(Spam2);
                    System.Threading.Thread.Sleep(100);
                    SendKeys.SendWait(Spam3);
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        #endregion

        //start spam thread
        private void button1_Click(object sender, EventArgs e)
        {
            Spam1 = comboBox1.Text;
            Spam2 = comboBox2.Text;
            Spam3 = comboBox8.Text;

            Thread SpamThread = new Thread(Spam);
            SpamThread.Start();
            button1.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            comboBox8.Enabled = false;

        }

        //start buff thread
        private void button4_Click(object sender, EventArgs e)
        {
            Buff1 = comboBox4.Text;
            Buff2 = comboBox5.Text;
            Buff3 = comboBox7.Text;

            Thread BuffThread = new Thread(Buff);
            BuffThread.Start();
            button4.Enabled = false;
            comboBox4.Enabled = false;
            comboBox5.Enabled = false;
            comboBox6.Enabled = false;
            comboBox7.Enabled = false;

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            run = false;
        }

    }
}
