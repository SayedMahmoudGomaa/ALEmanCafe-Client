using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ALEmanCafe_Client
{
    public partial class SendMessage : Form
    {
        public SendMessage()
        {
            InitializeComponent();
        }

        private void CancelCloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
