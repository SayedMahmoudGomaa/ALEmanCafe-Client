using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace ALEmanCafe_Client
{
    public partial class ALEmanCafeClient : Form
    {
        public ALEmanCafe AC;
        public LoginForm LF;
        public ALEmanCafeClient(ALEmanCafe AC)
        {
            InitializeComponent();
            this.AC = AC;
            int PCNumber = 0;
            try
            {
                PCNumber = int.Parse(Regex.Match(Environment.UserName, @"\d+").Value);//int.Parse(Environment.UserName);
            }
            catch { }
            if (PCNumber >= 1)
                this.label1.Text = "Computer Number : " + PCNumber;
            else
                this.label1.Text = Environment.UserName + ", IP: " + ALEmanCafe.MyIp;

         
            this.FormClosing += new FormClosingEventHandler(ALEmanCafeClient_FormClosing);
            this.KeyDown += new KeyEventHandler(ALEmanCafeClient_KeyDown);
            this.MouseClick += new MouseEventHandler(ALEmanCafeClient_MouseClick);
            this.LF = new LoginForm(this.AC, this);
        }

        public void ALEmanCafeClient_FormClosing(object Sender, FormClosingEventArgs FC)
        {
            if (Program.Login == false)
            {
                FC.Cancel = true;
            }
         /*   else
            {
                this.AC.ALEmanCafeIcon.Visible = false;
                Taskbar.Show();
            }*/
        }

        public void ALEmanCafeClient_KeyDown(object Sender, KeyEventArgs KE)
        {
            if (KE.KeyCode == Keys.Apps || KE.KeyData == Keys.Apps || KE.Modifiers == Keys.Apps)
            {
                this.LF.ShowDialog(this);
            }
        }

        public void ALEmanCafeClient_MouseClick(object Sender, MouseEventArgs ME)
        {
            if (ME.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.LF.ShowDialog(this);
             //   this.LF.Show(this);
            }
        }
    }
}
