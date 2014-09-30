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
using System.Xml;
using System.Windows.Input;

namespace D3marcoUI
{
    public partial class Form1 : Form
    {
        int BuffTimer = 10000;
        int Buff1Interval = 10;
        int Buff2Interval = 10;
        int Buff3Interval = 10;
        string Buff1 = "";
        string Buff2 = "";
        string Buff3 = "";
        string Buff4 = "";
        string Spam1 = "";
        string Spam2 = "";
        string Spam3 = "";
        string Spam4 = "";
        string MouseSpam = "";
        Keys BuffHotKey = Keys.CapsLock;
        bool BuffRun = true;
        bool SpamRun = true;
        bool run = false;
        bool HoldSpam = true;
        bool KeepSpam = false;
        int SpamInterval = 100;
        int SpamInterval2 = 0;
        int SpamInterval3 = 0;
        int SpamInterval4 = 0;
        bool stopSpamThread = false;
        bool stopBuffThread = false;
        bool ReadCustomKey = false;
        string CustomSpamKey = "";
        Thread spamT;
        Thread buffT;
        ManualResetEvent threadInterrupt = new ManualResetEvent(false);
        ManualResetEvent buffThreadInterrupt = new ManualResetEvent(false);

        

        ////mouse clicks simulation function
        #region mouse click
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MouseLeftClick = 0x02;
        public const int MouseLeftClickUP = 0x04;
        public const int MouseRightClick = 0x08;
        public const int MouseRightClickUP = 0x10;


        public void DoMouseClickLeft()
        {
            //Call the imported function with the cursor's current position
            int X = System.Windows.Forms.Cursor.Position.X;
            int Y = System.Windows.Forms.Cursor.Position.Y;
            mouse_event(MouseLeftClick | MouseLeftClickUP, X, Y, 0, 0);
        }
        public void DoMouseClickRight()
        {
            //Call the imported function with the cursor's current position
            int X = System.Windows.Forms.Cursor.Position.X;
            int Y = System.Windows.Forms.Cursor.Position.Y;
            mouse_event(MouseRightClick | MouseRightClickUP, X, Y, 0, 0);
        }
        #endregion

        public Form1()
        {
            InitializeComponent();

            //read config
            #region read config
            try
            {
                if (System.IO.File.Exists(@"D3marcoUI_config.xml"))
                {
                    XmlTextReader reader = new XmlTextReader(@"D3marcoUI_config.xml");
                    while (reader.Read())
                    {
                        reader.ReadToFollowing("Spam1");
                        comboBox1.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Spam2");
                        comboBox2.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Spam3");
                        comboBox8.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Spam4");
                        comboBox9.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("SpamInterval");
                        numericUpDown1.Value = Convert.ToInt32(reader.ReadElementContentAsString());
                        reader.ReadToFollowing("Buff1");
                        comboBox4.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Buff2");
                        comboBox5.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Buff3");
                        comboBox7.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Buff4");
                        comboBox10.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("BuffTimer");
                        numericUpDown4.Text = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("Buff1Interval");
                        numericUpDown2.Value = Convert.ToInt32(reader.ReadElementContentAsString());
                        reader.ReadToFollowing("Buff2Interval");
                        numericUpDown3.Value = Convert.ToInt32(reader.ReadElementContentAsString());
                        reader.ReadToFollowing("Buff3Interval");
                        numericUpDown5.Value = Convert.ToInt32(reader.ReadElementContentAsString());
                        reader.ReadToFollowing("SpamKey");
                        var temp = reader.ReadElementContentAsString();
                        if (temp != "Shift" && temp != "Ctrl" && temp != "LMouseButton" && temp != "RMouseButton")
                        {
                            comboBox3.Items.Add(temp);
                            comboBox3.Text = temp;
                        }
                        else
                        {
                            comboBox3.Text = temp;
                        }
                        reader.ReadToFollowing("BuffHotKey");
                        comboBox6.Text = reader.ReadElementContentAsString();
                        reader.Close();
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
    
        }

        #region Buff
        public void Buff()
        {
            while (BuffRun)
            {
                if (stopBuffThread)
                {
                    stopBuffThread = false;
                    break;
                }

                if (!Control.IsKeyLocked(BuffHotKey))
                {
                    run = true;
                    BuffRun = false;
                }
                if (!Control.IsKeyLocked(BuffHotKey)) break;
                SendKeys.SendWait(Buff1);
                System.Threading.Thread.Sleep(Buff1Interval);
                if (Buff2 != "")
                {
                    SendKeys.SendWait(Buff2);
                    System.Threading.Thread.Sleep(Buff2Interval);
                }
                if (Buff3 != "")
                {
                    SendKeys.SendWait(Buff3);
                    System.Threading.Thread.Sleep(Buff3Interval);
                }
                if (Buff4 != "") SendKeys.SendWait(Buff4);
                System.Threading.Thread.Sleep(BuffTimer);
                if (!Control.IsKeyLocked(BuffHotKey))
                {
                    run = true;
                    BuffRun = false;
                }
                if (!Control.IsKeyLocked(BuffHotKey)) break;
            }

            while (run)
            {
                System.Threading.Thread.Sleep(1);
                if (stopBuffThread)
                {
                    stopBuffThread = false;
                    break;
                }

                if (Control.IsKeyLocked(BuffHotKey))
                {
                    BuffRun = true;
                    run = false;
                    Buff();
                    break;
                }
            }
            
        }
        #endregion 

        #region Spam
        public void Spam()
        {
            //Spam Thread
            while (SpamRun)
            {
                System.Threading.Thread.Sleep(1);

                if (stopSpamThread)
                {
                    stopSpamThread = false;
                    break;
                }

                if (MouseSpam == "LMouseButton")
                {
                    while (Control.MouseButtons == MouseButtons.Left)
                    {

                        if (stopSpamThread)
                        {
                            stopSpamThread = false;
                            break;
                        }

                        if (Spam1 == "RClick")
                        {
                            DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam1);
                        }
                        System.Threading.Thread.Sleep(SpamInterval);                       
                        if (Spam2 == "RClick") 
                        {
                            DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam2);
                        }
                        System.Threading.Thread.Sleep(SpamInterval2);                       
                        if (Spam3 == "RClick") 
                        {
                            DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam3);
                        }
                        System.Threading.Thread.Sleep(SpamInterval3);
                        if (Spam4 == "RClick")
                        {
                            DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam4);
                        }
                        System.Threading.Thread.Sleep(SpamInterval4);
                    }

                } else if (MouseSpam == "RMouseButton")
                    {
                        while (Control.MouseButtons == MouseButtons.Right)
                        {
                            if (stopSpamThread)
                            {
                                stopSpamThread = false;
                                break;
                            }

                            if (Spam1 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam1);
                            }
                            System.Threading.Thread.Sleep(SpamInterval);
                            if (Spam2 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam2);
                            }
                            System.Threading.Thread.Sleep(SpamInterval2);
                            if (Spam3 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam3);
                            }
                            System.Threading.Thread.Sleep(SpamInterval3);
                            if (Spam4 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam4);
                            }
                            System.Threading.Thread.Sleep(SpamInterval4);
                        }
                    } else if (MouseSpam == "Shift")
                {
                    while (Control.ModifierKeys == Keys.Shift)
                    {
                        if (stopSpamThread)
                        {
                            stopSpamThread = false;
                            break;
                        }

                        if (Spam1 == "LClick" || Spam1 == "RClick")
                        {
                            if (Spam1 == "LClick") DoMouseClickLeft();
                            if (Spam1 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam1);
                        }
                        System.Threading.Thread.Sleep(SpamInterval);
                        if (Spam2 == "LClick" || Spam2 == "RClick")
                        {
                            if (Spam2 == "LClick") DoMouseClickLeft();
                            if (Spam2 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam2);
                        }
                        System.Threading.Thread.Sleep(SpamInterval2);
                        if (Spam3 == "LClick" || Spam3 == "RClick")
                        {
                            if (Spam3 == "LClick") DoMouseClickLeft();
                            if (Spam3 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam3);
                        }
                        System.Threading.Thread.Sleep(SpamInterval3);
                        if (Spam4 == "LClick" || Spam4 == "RClick")
                        {
                            if (Spam4 == "LClick") DoMouseClickLeft();
                            if (Spam4 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam4);
                        }
                        System.Threading.Thread.Sleep(SpamInterval4);
                    }
                }
                else if (MouseSpam == "Ctrl")
                {
                    while (Control.ModifierKeys == Keys.Control)
                    {
                        if (stopSpamThread)
                        {
                            stopSpamThread = false;
                            break;
                        }
                        if (Spam1 == "LClick" || Spam1 == "RClick")
                        {
                            if (Spam1 == "LClick") DoMouseClickLeft();
                            if (Spam1 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam1);
                        }
                        System.Threading.Thread.Sleep(SpamInterval);
                        if (Spam2 == "LClick" || Spam2 == "RClick")
                        {
                            if (Spam2 == "LClick") DoMouseClickLeft();
                            if (Spam2 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam2);
                        }
                        System.Threading.Thread.Sleep(SpamInterval2);
                        if (Spam3 == "LClick" || Spam3 == "RClick")
                        {
                            if (Spam3 == "LClick") DoMouseClickLeft();
                            if (Spam3 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam3);
                        }
                        System.Threading.Thread.Sleep(SpamInterval3);
                        if (Spam4 == "LClick" || Spam4 == "RClick")
                        {
                            if (Spam4 == "LClick") DoMouseClickLeft();
                            if (Spam4 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam4);
                        }
                        System.Threading.Thread.Sleep(SpamInterval4);
                    }
                }
                if (CustomSpamKey != "")
                {
                    Key key_restored = (Key)Enum.Parse(typeof(Key), CustomSpamKey);
                    while (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                    {
                        if (stopSpamThread)
                        {
                            stopSpamThread = false;
                            break;
                        }
                        if (Spam1 == "LClick" || Spam1 == "RClick")
                        {
                            if (Spam1 == "LClick") DoMouseClickLeft();
                            if (Spam1 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam1);
                        }
                        System.Threading.Thread.Sleep(SpamInterval);
                        if (Spam2 == "LClick" || Spam2 == "RClick")
                        {
                            if (Spam2 == "LClick") DoMouseClickLeft();
                            if (Spam2 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam2);
                        }
                        System.Threading.Thread.Sleep(SpamInterval2);
                        if (Spam3 == "LClick" || Spam3 == "RClick")
                        {
                            if (Spam3 == "LClick") DoMouseClickLeft();
                            if (Spam3 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam3);
                        }
                        System.Threading.Thread.Sleep(SpamInterval3);
                        if (Spam4 == "LClick" || Spam4 == "RClick")
                        {
                            if (Spam4 == "LClick") DoMouseClickLeft();
                            if (Spam4 == "RClick") DoMouseClickRight();
                        }
                        else
                        {
                            SendKeys.SendWait(Spam4);
                        }
                        System.Threading.Thread.Sleep(SpamInterval4);
                    }
                }


            }
        }
        #endregion

        #region KeepSpam
        public void SpamHold()
        {
            while (HoldSpam)
            {
                System.Threading.Thread.Sleep(1);

                if (stopSpamThread)
                {
                    stopSpamThread = false;
                    break;
                }

                if (MouseSpam == "LMouseButton")
                {
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        if (KeepSpam) KeepSpam = false;
                        if (!KeepSpam) KeepSpam = true;
                        System.Threading.Thread.Sleep(500);
                    }
                }

                if (MouseSpam == "RMouseButton")
                {
                    if (Control.MouseButtons == MouseButtons.Right)
                    {
                        if (KeepSpam) KeepSpam = false;
                        if (!KeepSpam) KeepSpam = true;
                        System.Threading.Thread.Sleep(500);
                    }
                }

                if (MouseSpam == "Shift")
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        if (KeepSpam) KeepSpam = false;
                        if (!KeepSpam) KeepSpam = true;
                        System.Threading.Thread.Sleep(500);
                    }
                }
                if (MouseSpam == "Ctrl")
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        if (KeepSpam) KeepSpam = false;
                        if (!KeepSpam) KeepSpam = true;
                        System.Threading.Thread.Sleep(500);
                    }
                }
                if (CustomSpamKey != "")
                {
                    Key key_restored = (Key)Enum.Parse(typeof(Key), CustomSpamKey);
                    if (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                    {
                        if (KeepSpam) KeepSpam = false;
                        if (!KeepSpam) KeepSpam = true;
                        System.Threading.Thread.Sleep(500);
                    }
                }

                #region KeepSpam part
                while (KeepSpam)
                {
                    System.Threading.Thread.Sleep(1);

                    if (stopSpamThread)
                    {
                        stopSpamThread = false;
                        break;
                    }

                    if (MouseSpam == "LMouseButton")
                    {
                        while (KeepSpam)
                        {
                            if (stopSpamThread)
                            {
                                stopSpamThread = false;
                                break;
                            }

                            if (Spam1 == "RClick") DoMouseClickRight();
                            else SendKeys.SendWait(Spam1);

                            if (Control.MouseButtons == MouseButtons.Left)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval);
                            if (Spam2 == "RClick") DoMouseClickRight();
                            else SendKeys.SendWait(Spam2);

                            if (Control.MouseButtons == MouseButtons.Left)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval2);
                            if (Spam3 == "RClick")
                            {
                                DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam3);
                            }
                            if (Control.MouseButtons == MouseButtons.Left)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval3);
                            if (Spam4 == "RClick")
                            {
                                DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam4);
                            }
                            if (Control.MouseButtons == MouseButtons.Left)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval4);
                        }

                    }
                    else if (MouseSpam == "RMouseButton")
                    {
                        while (KeepSpam)
                        {
                            if (stopSpamThread)
                            {
                                stopSpamThread = false;
                                break;
                            }

                            if (Spam1 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam1);
                            }
                            if (Control.MouseButtons == MouseButtons.Right)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval);
                            if (Spam2 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam2);
                            }
                            if (Control.MouseButtons == MouseButtons.Right)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval2);
                            if (Spam3 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam3);
                            }
                            if (Control.MouseButtons == MouseButtons.Right)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval3);
                            if (Spam4 == "LClick")
                            {
                                DoMouseClickLeft();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam4);
                            }
                            if (Control.MouseButtons == MouseButtons.Right)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval4);
                        }
                    }
                    else if (MouseSpam == "Shift")
                    {
                        while (KeepSpam)
                        {
                            if (stopSpamThread)
                            {
                                stopSpamThread = false;
                                break;
                            }

                            if (Spam1 == "LClick" || Spam1 == "RClick")
                            {
                                if (Spam1 == "LClick") DoMouseClickLeft();
                                if (Spam1 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam1);
                            }
                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval);
                            if (Spam2 == "LClick" || Spam2 == "RClick")
                            {
                                if (Spam2 == "LClick") DoMouseClickLeft();
                                if (Spam2 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam2);
                            }
                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval2);
                            if (Spam3 == "LClick" || Spam3 == "RClick")
                            {
                                if (Spam3 == "LClick") DoMouseClickLeft();
                                if (Spam3 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam3);
                            }
                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval3);
                            if (Spam4 == "LClick" || Spam4 == "RClick")
                            {
                                if (Spam4 == "LClick") DoMouseClickLeft();
                                if (Spam4 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam4);
                            }
                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }

                            System.Threading.Thread.Sleep(SpamInterval4);

                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }

                        }
                    }
                    else if (MouseSpam == "Ctrl")
                    {
                        while (KeepSpam)
                        {
                            if (stopSpamThread)
                            {
                                stopSpamThread = false;
                                break;
                            }

                            if (Spam1 == "LClick" || Spam1 == "RClick")
                            {
                                if (Spam1 == "LClick") DoMouseClickLeft();
                                if (Spam1 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam1);
                            }
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval);
                            if (Spam2 == "LClick" || Spam2 == "RClick")
                            {
                                if (Spam2 == "LClick") DoMouseClickLeft();
                                if (Spam2 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam2);
                            }
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval2);
                            if (Spam3 == "LClick" || Spam3 == "RClick")
                            {
                                if (Spam3 == "LClick") DoMouseClickLeft();
                                if (Spam3 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam3);
                            }
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval3);
                            if (Spam4 == "LClick" || Spam4 == "RClick")
                            {
                                if (Spam4 == "LClick") DoMouseClickLeft();
                                if (Spam4 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam4);
                            }
                            if (Control.ModifierKeys == Keys.Control)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }

                            System.Threading.Thread.Sleep(SpamInterval4);

                            if (Control.ModifierKeys == Keys.Control)
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                        }
                    }
                    if (CustomSpamKey != "")
                    {
                        Key key_restored = (Key)Enum.Parse(typeof(Key), CustomSpamKey);                            
                        while (KeepSpam)
                        {
                            if (stopSpamThread)
                            {
                                stopSpamThread = false;
                                break;
                            }

                            if (Spam1 == "LClick" || Spam1 == "RClick")
                            {
                                if (Spam1 == "LClick") DoMouseClickLeft();
                                if (Spam1 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam1);
                            }
                            if (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval);
                            if (Spam2 == "LClick" || Spam2 == "RClick")
                            {
                                if (Spam2 == "LClick") DoMouseClickLeft();
                                if (Spam2 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam2);
                            }
                            if (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval2);
                            if (Spam3 == "LClick" || Spam3 == "RClick")
                            {
                                if (Spam3 == "LClick") DoMouseClickLeft();
                                if (Spam3 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam3);
                            }
                            if (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                            System.Threading.Thread.Sleep(SpamInterval3);
                            if (Spam4 == "LClick" || Spam4 == "RClick")
                            {
                                if (Spam4 == "LClick") DoMouseClickLeft();
                                if (Spam4 == "RClick") DoMouseClickRight();
                            }
                            else
                            {
                                SendKeys.SendWait(Spam4);
                            }
                            if (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }

                            System.Threading.Thread.Sleep(SpamInterval4);

                            if (System.Windows.Input.Keyboard.IsKeyDown(key_restored))
                            {
                                KeepSpam = false;
                                System.Threading.Thread.Sleep(500);
                                break;
                            }
                        }
                    }


                }

            }
                #endregion
        }
        #endregion

        //start spam thread
        private void button1_Click(object sender, EventArgs e)
        {
            Spam1 = comboBox1.Text;
            Spam2 = comboBox2.Text;
            Spam3 = comboBox8.Text;
            Spam4 = comboBox9.Text;
            MouseSpam = comboBox3.Text;
            if (comboBox3.Text != "Shift" && comboBox3.Text != "Ctrl" && comboBox3.Text != "LMouseButton" && comboBox3.Text != "RMouseButton")
            {
                
                CustomSpamKey = comboBox3.Text;
            }
            

            SpamInterval = Convert.ToInt32(numericUpDown1.Value);
            SpamInterval2 = Convert.ToInt32(numericUpDown1.Value);
            SpamInterval3 = Convert.ToInt32(numericUpDown1.Value);
            SpamInterval4 = Convert.ToInt32(numericUpDown1.Value);

            if (Spam2 == "") SpamInterval2 = 0;
            if (Spam3 == "") SpamInterval3 = 0;
            if (Spam4 == "") SpamInterval4 = 0;

            threadInterrupt.Reset();

            if (checkBox1.Checked)
            {
                spamT = new Thread(SpamHold);
                spamT.IsBackground = true;
                spamT.SetApartmentState(ApartmentState.STA);
                spamT.Start();

            }
            else
            {
                spamT = new Thread(Spam);
                spamT.IsBackground = true;
                spamT.SetApartmentState(ApartmentState.STA);
                spamT.Start();
            }
            button1.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            comboBox8.Enabled = false;
            comboBox9.Enabled = false;
            checkBox1.Enabled = false;
            numericUpDown1.Enabled = false;
            checkBox1.Enabled = false;
            button2.Enabled = true;

        }

        //start buff thread
        private void button4_Click(object sender, EventArgs e)
        {
            Buff1 = comboBox4.Text;
            Buff2 = comboBox5.Text;
            Buff3 = comboBox7.Text;
            Buff4 = comboBox10.Text;
            if (comboBox6.Text == "CapsLock") BuffHotKey = Keys.CapsLock;
            if (comboBox6.Text == "ScrollLock") BuffHotKey = Keys.Scroll;

            Buff1Interval = Convert.ToInt32(numericUpDown2.Value);
            Buff2Interval = Convert.ToInt32(numericUpDown3.Value);
            Buff3Interval = Convert.ToInt32(numericUpDown5.Value);

            //set Buff Timer    
            BuffTimer = Convert.ToInt32(numericUpDown4.Value);

            if (Buff2 == "") Buff1Interval = 0;
            if (Buff3 == "") Buff2Interval = 0;
            if (Buff4 == "") Buff3Interval = 0;

            buffThreadInterrupt.Reset();

            buffT = new Thread(Buff);
            buffT.IsBackground = true;
            buffT.Start();
            button4.Enabled = false;
            comboBox4.Enabled = false;
            comboBox5.Enabled = false;
            numericUpDown4.Enabled = false;
            comboBox7.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
            comboBox6.Enabled = false;
            button3.Enabled = true;
            comboBox10.Enabled = false;
            numericUpDown5.Enabled = false;

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //end buff and run loops on close
            BuffRun = false;
            SpamRun = false;
            BuffTimer = 0;
            //TODO: problem disabling the caps lock
            //if (Control.IsKeyLocked(Keys.CapsLock)) SendKeys.SendWait("{CAPS LOCK}");

            //create config file
            #region create config
            try
            {
                XmlTextWriter writer = new XmlTextWriter(@"D3marcoUI_config.xml", System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Config");
                writer.WriteStartElement("Spam1");
                writer.WriteString(comboBox1.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Spam2");
                writer.WriteString(comboBox2.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Spam3");
                writer.WriteString(comboBox8.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Spam4");
                writer.WriteString(comboBox9.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("SpamInterval");
                writer.WriteString(numericUpDown1.Value.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Buff1");
                writer.WriteString(comboBox4.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Buff2");
                writer.WriteString(comboBox5.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Buff3");
                writer.WriteString(comboBox7.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Buff4");
                writer.WriteString(comboBox10.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("BuffTimer");
                writer.WriteString(numericUpDown4.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Buff1Interval");
                writer.WriteString(numericUpDown2.Value.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Buff2Interval");
                writer.WriteString(numericUpDown3.Value.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Buff3Interval");
                writer.WriteString(numericUpDown5.Value.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("SpamKey");
                if (comboBox3.Text == "Custom Button")
                {
                    writer.WriteString(CustomSpamKey);
                }
                else
                {
                    writer.WriteString(comboBox3.Text);
                }
                writer.WriteEndElement();
                writer.WriteStartElement("BuffHotKey");
                writer.WriteString(comboBox6.Text);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion

        }

        //Be safe from idiots
        private void comboBox3_TextChanged(object sender, EventArgs e)
        {

            if (comboBox3.Text == "LMouseButton" & (
                comboBox1.Text == "LClick" || comboBox2.Text == "LClick" || comboBox8.Text == "LClick" || comboBox9.Text == "LClick"
                ))
            {
                comboBox3.Text = "Shift";
                MessageBox.Show("You cannot spam Left Click while Left Mouse Button is pressed.");
            }
            if (comboBox3.Text == "RMouseButton" && (
                comboBox1.Text == "RClick" || comboBox2.Text == "RClick" || comboBox8.Text == "RClick" || comboBox9.Text == "RClick"
                ))
            {
                comboBox3.Text = "Shift";
                MessageBox.Show("You cannot spam Right Click while Right Mouse Button is pressed.");
            }
            //Pick custom Spam Button part
            #region Custom Button
            if (comboBox3.Text == "Custom Button")
            {
                pictureBox1.Visible = true;
                //disable UI
                button1.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox8.Enabled = false;
                comboBox9.Enabled = false;
                checkBox1.Enabled = false;
                numericUpDown1.Enabled = false;
                checkBox1.Enabled = false;                
                button4.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                numericUpDown4.Enabled = false;
                comboBox7.Enabled = false;
                numericUpDown2.Enabled = false;
                numericUpDown3.Enabled = false;
                comboBox6.Enabled = false;             
                comboBox10.Enabled = false;
                numericUpDown5.Enabled = false;

                //Read Custom Key
                ReadCustomKey = true;
            }
            #endregion


        }
        //stop spam thread
        private void button2_Click(object sender, EventArgs e)
        {
            
            stopSpamThread = true;

            if (!spamT.IsAlive)
            {
                spamT.Join();
                threadInterrupt.Set();
            }

            //bring back disabled UI items
            button1.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            comboBox8.Enabled = true;
            comboBox9.Enabled = true;
            checkBox1.Enabled = true;
            numericUpDown1.Enabled = true;
            checkBox1.Enabled = true;
            button2.Enabled = false;
            comboBox10.Enabled = true;
            numericUpDown5.Enabled = true;
        }
        //stop buff thread
        private void button3_Click(object sender, EventArgs e)
        {

            stopBuffThread = true;

            if (!buffT.IsAlive)
            {
                buffT.Join();
                buffThreadInterrupt.Set();
            }
            //bring back disabled UI items
            button4.Enabled = true;
            comboBox4.Enabled = true;
            comboBox5.Enabled = true;
            numericUpDown4.Enabled = true;
            comboBox7.Enabled = true;
            numericUpDown2.Enabled = true;
            numericUpDown3.Enabled = true;
            comboBox6.Enabled = true;
            button3.Enabled = false;

        }

        //Read Custom Key
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ReadCustomKey)
            {
                CustomSpamKey = keyData.ToString();
                pictureBox1.Visible = false;
                //Enable UI
                button1.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox8.Enabled = true;
                comboBox9.Enabled = true;
                checkBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                checkBox1.Enabled = true;
                button4.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                numericUpDown4.Enabled = true;
                comboBox7.Enabled = true;
                numericUpDown2.Enabled = true;
                numericUpDown3.Enabled = true;
                comboBox6.Enabled = true;
                comboBox10.Enabled = true;
                numericUpDown5.Enabled = true;
                comboBox3.Text = CustomSpamKey;

                ReadCustomKey = false;

                return ReadCustomKey;
            }
            return ReadCustomKey;
        }

    }
}
