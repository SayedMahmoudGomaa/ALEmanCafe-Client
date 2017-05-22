using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ALEmanCafe_Client
{
    public partial class LoginForm : Form
    {
        public ALEmanCafeClient ACC;
        public ALEmanCafe AC;
        public LoginForm(ALEmanCafe AC, ALEmanCafeClient ACC)
        {
            InitializeComponent();
            this.ACC = ACC;
            this.AC = AC;
            this.PassTextBox.UseSystemPasswordChar = true;
            this.UserTextBox.KeyPress += new KeyPressEventHandler(UserTextBox_KeyPress);
            this.PassTextBox.KeyPress += new KeyPressEventHandler(PassTextBox_KeyPress);
        //    this.FormClosing += new FormClosingEventHandler(LoginForm_FormClosing);
        }

        private void UserTextBox_KeyPress(object sender, KeyPressEventArgs K)
        {
            if (K.KeyChar == (char)Keys.Enter)
            {
                PassTextBox.Select();
            }
        }

        private void PassTextBox_KeyPress(object sender, KeyPressEventArgs K)
        {
            if (K.KeyChar == (char)Keys.Enter)
            {
                LoginButton.PerformClick();// LoginButton_Click(sender, null);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (UserTextBox.Text == Program.UsernameLog && PassTextBox.Text == Program.PasswordLog)
            {
                this.AC.LoginNow(false, false);
            }
            else
                MessageBox.Show("Invaled Username or Password");
        }

        private void CancelsButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }*/
    }
}
