namespace ALEmanCafe_Client
{
    partial class ALEmanCafe
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ALEmanCafe));
            this.label4 = new System.Windows.Forms.Label();
            this.Label33 = new System.Windows.Forms.Label();
            this.StatusMenu1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.UsageCostLabel = new System.Windows.Forms.Label();
            this.RemainingTimeLabel = new System.Windows.Forms.Label();
            this.HideButton = new System.Windows.Forms.Button();
            this.LogoutButton = new System.Windows.Forms.Button();
            this.UsedTimeLabel = new System.Windows.Forms.Label();
            this.ALEmanCafeIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.ALEmanCafeMenuStripIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.StatusMenu1.SuspendLayout();
            this.ALEmanCafeMenuStripIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.DarkRed;
            this.label4.Location = new System.Drawing.Point(3, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Remaining Time : ";
            // 
            // Label33
            // 
            this.Label33.AutoSize = true;
            this.Label33.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.Label33.ForeColor = System.Drawing.Color.Green;
            this.Label33.Location = new System.Drawing.Point(4, 68);
            this.Label33.Name = "Label33";
            this.Label33.Size = new System.Drawing.Size(75, 13);
            this.Label33.TabIndex = 2;
            this.Label33.Text = "Used Time : ";
            // 
            // StatusMenu1
            // 
            this.StatusMenu1.AutoSize = true;
            this.StatusMenu1.BackColor = System.Drawing.SystemColors.Window;
            this.StatusMenu1.Controls.Add(this.label5);
            this.StatusMenu1.Controls.Add(this.UsageCostLabel);
            this.StatusMenu1.Controls.Add(this.RemainingTimeLabel);
            this.StatusMenu1.Controls.Add(this.HideButton);
            this.StatusMenu1.Controls.Add(this.label4);
            this.StatusMenu1.Controls.Add(this.Label33);
            this.StatusMenu1.Controls.Add(this.LogoutButton);
            this.StatusMenu1.Controls.Add(this.UsedTimeLabel);
            this.StatusMenu1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusMenu1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.StatusMenu1.Location = new System.Drawing.Point(0, 414);
            this.StatusMenu1.Name = "StatusMenu1";
            this.StatusMenu1.Size = new System.Drawing.Size(190, 156);
            this.StatusMenu1.TabIndex = 2;
            this.StatusMenu1.TabStop = false;
            this.StatusMenu1.Text = "Status";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.DarkRed;
            this.label5.Location = new System.Drawing.Point(4, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Usage Cost : ";
            // 
            // UsageCostLabel
            // 
            this.UsageCostLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UsageCostLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.UsageCostLabel.ForeColor = System.Drawing.Color.Red;
            this.UsageCostLabel.Location = new System.Drawing.Point(84, 127);
            this.UsageCostLabel.Name = "UsageCostLabel";
            this.UsageCostLabel.Size = new System.Drawing.Size(103, 13);
            this.UsageCostLabel.TabIndex = 24;
            this.UsageCostLabel.Text = "0000 جم";
            this.UsageCostLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RemainingTimeLabel
            // 
            this.RemainingTimeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RemainingTimeLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.RemainingTimeLabel.ForeColor = System.Drawing.Color.Red;
            this.RemainingTimeLabel.Location = new System.Drawing.Point(106, 98);
            this.RemainingTimeLabel.Name = "RemainingTimeLabel";
            this.RemainingTimeLabel.Size = new System.Drawing.Size(81, 13);
            this.RemainingTimeLabel.TabIndex = 22;
            this.RemainingTimeLabel.Text = "00:00";
            this.RemainingTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // HideButton
            // 
            this.HideButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.HideButton.Location = new System.Drawing.Point(3, 39);
            this.HideButton.Name = "HideButton";
            this.HideButton.Size = new System.Drawing.Size(184, 23);
            this.HideButton.TabIndex = 0;
            this.HideButton.Text = "&Hide";
            this.HideButton.UseVisualStyleBackColor = true;
            this.HideButton.Click += new System.EventHandler(this.HideButton_Click);
            // 
            // LogoutButton
            // 
            this.LogoutButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.LogoutButton.Location = new System.Drawing.Point(3, 16);
            this.LogoutButton.Name = "LogoutButton";
            this.LogoutButton.Size = new System.Drawing.Size(184, 23);
            this.LogoutButton.TabIndex = 3;
            this.LogoutButton.Text = "&Logout";
            this.LogoutButton.UseVisualStyleBackColor = true;
            this.LogoutButton.Click += new System.EventHandler(this.Logout_Click);
            // 
            // UsedTimeLabel
            // 
            this.UsedTimeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UsedTimeLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.UsedTimeLabel.Location = new System.Drawing.Point(81, 68);
            this.UsedTimeLabel.Name = "UsedTimeLabel";
            this.UsedTimeLabel.Size = new System.Drawing.Size(106, 13);
            this.UsedTimeLabel.TabIndex = 21;
            this.UsedTimeLabel.Text = "00:00";
            this.UsedTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ALEmanCafeIcon
            // 
            this.ALEmanCafeIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ALEmanCafeIcon.BalloonTipText = "ALEman Cafe";
            this.ALEmanCafeIcon.BalloonTipTitle = "ALEman Cafe";
            this.ALEmanCafeIcon.ContextMenuStrip = this.ALEmanCafeMenuStripIcon;
            this.ALEmanCafeIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("ALEmanCafeIcon.Icon")));
            this.ALEmanCafeIcon.Text = "ALEman Cafe";
            this.ALEmanCafeIcon.Visible = true;
            // 
            // ALEmanCafeMenuStripIcon
            // 
            this.ALEmanCafeMenuStripIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.hideToolStripMenuItem});
            this.ALEmanCafeMenuStripIcon.Name = "ALEmanCafeMenuStripIcon";
            this.ALEmanCafeMenuStripIcon.Size = new System.Drawing.Size(153, 70);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showToolStripMenuItem.Text = "&Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hideToolStripMenuItem.Text = "&Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBox1.Size = new System.Drawing.Size(190, 570);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // ALEmanCafe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(190, 570);
            this.ControlBox = false;
            this.Controls.Add(this.StatusMenu1);
            this.Controls.Add(this.richTextBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ALEmanCafe";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ALEmanCafe";
            this.TopMost = true;
            this.StatusMenu1.ResumeLayout(false);
            this.StatusMenu1.PerformLayout();
            this.ALEmanCafeMenuStripIcon.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label Label33;
        private System.Windows.Forms.GroupBox StatusMenu1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button LogoutButton;
        public System.Windows.Forms.Label RemainingTimeLabel;
        public System.Windows.Forms.Label UsedTimeLabel;
        public System.Windows.Forms.Label UsageCostLabel;
        private System.Windows.Forms.ContextMenuStrip ALEmanCafeMenuStripIcon;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.Button HideButton;
        public System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.NotifyIcon ALEmanCafeIcon;

    }
}