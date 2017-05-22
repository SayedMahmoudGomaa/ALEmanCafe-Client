using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Drawing.Imaging;
using MouseKeyboardLibrary;
using System.Runtime.InteropServices;
using MouseNS;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ALEmanCafe_Client
{
    public partial class ALEmanCafe : Form
    {
        public ALEmanCafeClient ACC;

        public System.Timers.Timer MyTimer, RecieverTimer, RemoteReciever, RemoteSender;
        public List<string> USBDrives = new List<string>();
        public static RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public static string MyIp = "", MasterIP = "";
        public static bool AdminLogin = false, MyInfoWasSent = false, TimePaused = false, CanUseInternet = true, Paid = false, ConnectedWithMaster = false, InRemote = false;
        public DateTime StartTime, StopedTime;
        public uint LimitedTime, UsedTime, RemainingTime;
        public long LCT = 0;
        public static void SetMyIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    MyIp = ip.ToString();
                    return;
                }
            }
        }

        public ALEmanCafe()
        {
            InitializeComponent();
            if (key.GetValue(Application.ProductName.Replace(" ", "-")) == null)
            {
                key.SetValue(Application.ProductName.Replace(" ", "-"), Application.ExecutablePath.ToString());
            }
            key.Close();
            Screen res = Screen.PrimaryScreen;
            this.Size = new Size(190, res.WorkingArea.Height);
            this.Location = new Point(res.Bounds.Width - Size.Width);
            this.ALEmanCafeIcon.MouseDoubleClick += new MouseEventHandler(ALEmanCafeIcon_DoubleClick);
            //  this.VisibleChanged += new EventHandler(ALEmanCafe_VisibleChanged);
            Taskbar.Hide();
            SetMyIP();
            MyTimer = new System.Timers.Timer(1000);
            MyTimer.AutoReset = true;
            MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timers);
            MyTimer.Start();

            RecieverTimer = new System.Timers.Timer(100);
            RecieverTimer.AutoReset = true;
            RecieverTimer.Elapsed += new System.Timers.ElapsedEventHandler(RecieverTimers);
            RecieverTimer.Start();
            Program.USBWriteProtected = true;
            this.ACC = new ALEmanCafeClient(this);
            LogOutNow(false, true);
        }
        public bool dfs = false;
        public void SendRunAboutMe(string ToIP = null)
        {
            string MyMessage = "justrun;" +
                Environment.MachineName + ";" +
                MyIp + ";"
                + Environment.UserName + ";"
                + Application.ProductVersion + ";"
                + Program.FriendlyName() + ";"//Environment.OSVersion.VersionString.Replace("Windows NT", "Windows XP") + ";"
                + new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory + ";"
                + new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
            UdpClient udpServer = new UdpClient(717);
            try
            {
                if (ToIP == null || string.IsNullOrEmpty(ToIP))
                    udpServer.Connect(IPAddress.Broadcast, 717);
                else
                    udpServer.Connect(ToIP, 717);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(MyMessage);
                udpServer.Send(sendBytes, sendBytes.Length);
                udpServer.Close();
            }
            catch { udpServer.Close(); }
            dfs = true;
        }

        public bool InTimers = false;
        public void Timers(object sender, EventArgs eeee)
        {
            //  writeamess("---- 1st : " + this.InTimers + ", Login : "+ Program.Login);
            if (InTimers) return; InTimers = true;
          
            if (InRecieverTimer && DateTime.Now > new DateTime(LRT).AddMilliseconds(300))
                InRecieverTimer = false;

            if (ConnectedWithMaster && DateTime.Now > new DateTime(LCT).AddSeconds(25))
            {
                ConnectedWithMaster = false;
                InRemote = false;
                if (RemoteReciever != null)
                {
                    this.RemoteReciever.Stop();
                    this.RemoteReciever.Dispose();
                    this.RemoteReciever = null;
                }
                if (RemoteSender != null)
                {
                    this.RemoteSender.Stop();
                    this.RemoteSender.Dispose();
                    this.RemoteSender = null;
                }
                this.writeamess("END 2"+ Environment.NewLine);
            }

            if (Program.Login == false)
                Program.KillTaskManager();
            else if (CanUseInternet == false)
                Program.CloseInternetBrowsers();

            if (Program.Login && !TimePaused)
            {
                if (LimitedTime > 0)
                {
                    if (Program.GetUsedTime(this.StartTime) >= LimitedTime)
                    {
                        //writeamess("Done, LimitedTime : " + LimitedTime + ", RemainingTime : " + RemainingTime + Environment.NewLine);
                        this.RemainingTime = 0;
                        LogOutNow(true, false);
                    }
                    else
                    {
                        TimeSpan ST = new TimeSpan(DateTime.Now.Ticks - this.StartTime.Ticks);
                        Program.GetStringUsedTime(ST.TotalMinutes, this.UsedTimeLabel);
                        Program.GetRemainingTime(ST.TotalMinutes, this.LimitedTime, this.RemainingTimeLabel);
                        this.UsedTime = Program.GetUsedTime(this.StartTime);
                        Program.GetTimeCost(this.UsedTime, this.UsageCostLabel);
                        this.RemainingTime = Program.GetRemainingTime(this.StartTime, this.LimitedTime);
                    }
                }
                else //if (AdminLogin == false)
                {
                    TimeSpan ST = new TimeSpan(DateTime.Now.Ticks - this.StartTime.Ticks);
                    Program.GetStringUsedTime(ST.TotalMinutes, this.UsedTimeLabel);

                    if (this.RemainingTimeLabel.InvokeRequired)
                    {
                        this.RemainingTimeLabel.BeginInvoke((MethodInvoker)delegate()
                        {
                            this.RemainingTimeLabel.Text = "00:00";
                        });//.AsyncWaitHandle.WaitOne();
                    }
                    else
                    {
                        this.RemainingTimeLabel.Text = "00:00";
                    }
                    this.UsedTime = Program.GetUsedTime(this.StartTime);
                    Program.GetTimeCost(this.UsedTime, this.UsageCostLabel);
                }
                Program.ShowFolderOptions();
            }
            DriveInfo[] mydrives = DriveInfo.GetDrives();
            if (MyInfoWasSent == false)
            {
                this.SendRunAboutMe();
                MyInfoWasSent = true;

                foreach (DriveInfo mydrive in mydrives)
                {
                    if (mydrive.DriveType == DriveType.Removable && mydrive.IsReady)
                    {
                        if (USBDrives.Contains(mydrive.VolumeLabel) == false)
                        {
                            USBDrives.Add(mydrive.VolumeLabel);
                        }
                    }
                }
            }

            List<string> ThisDrives = new List<string>();
            foreach (DriveInfo mydrive in mydrives)
            {
                if (mydrive.DriveType == DriveType.Removable && mydrive.IsReady)
                {
                    if (USBDrives.Contains(mydrive.VolumeLabel) == false)
                    {
                        USBDrives.Add(mydrive.VolumeLabel);
                        ThisDrives.Add(mydrive.VolumeLabel);
                        string MyMessage = "USBPlugin;" + Environment.MachineName;
                        SendMessage(MyMessage);
                    }
                }
            }

            if (USBDrives.Count > 0)
                for (int C = 0; C < USBDrives.Count; C++)//delete removed disk 
                {
                    string VALUE = USBDrives[C];
                    if (ThisDrives.Contains(VALUE) == false)
                    {
                        USBDrives.Remove(VALUE);
                        string MyMessage = "USBPlugout;" + Environment.MachineName;
                        SendMessage(MyMessage);
                    }
                }
            InTimers = false;
            // writeamess("2nd : " + this.Login);
        }

        public bool InRecieverTimer = false;

        public long LRT;
        public void RecieverTimers(object sender, EventArgs eeee)
        {
            if (InRecieverTimer) return; LRT = DateTime.Now.Ticks; InRecieverTimer = true;
            /*
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                   {
                       RecieverTranslate();
                   });//.AsyncWaitHandle.WaitOne(); ;
            }
            else
            {
                RecieverTranslate();
            }
           */

            RecieverTranslate();
            InRecieverTimer = false;
        }
        public void RecieverTranslate()
        {
            string MyMessage = "";

            try
            {
                UdpClient udpClient = new UdpClient(716);
                try
                {
                 //   writeamess("IPA"); writeamess2("IPA");
                    IPAddress IPA = string.IsNullOrEmpty(MasterIP) ? IPAddress.Any : IPAddress.Parse(MasterIP);

                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPA, 716);
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    string[] Rec = Encoding.ASCII.GetString(receiveBytes).Split(';');

                    MasterIP = string.IsNullOrEmpty(Rec[1]) ? MasterIP : Rec[1];
                    LCT = DateTime.Now.Ticks;
                    ConnectedWithMaster = true;
                    /*
                    if (Rec[0] != "checkconnected")
                    {
                        writeamess("Recieved Packet : " + Rec[0] + Environment.NewLine);
                        writeamess2("Recieved Packet : " + Rec[0] + Environment.NewLine);
                    }
                    */
                    udpClient.Close();
                    switch (Rec[0])
                    {
                        case "startremote":
                            {
                                udpClient.Close();
                                Program.SPort = Convert.ToInt32(Rec[2]);
                                Program.RPort = Convert.ToInt32(Rec[3]);
                                InRemote = true;
                                this.RemoteReciever = new System.Timers.Timer(100);
                                this.RemoteReciever.AutoReset = true;
                                this.RemoteReciever.Elapsed += new System.Timers.ElapsedEventHandler(RemoteRecieverTimers);
                                this.RemoteReciever.Start();

                                this.RemoteSender = new System.Timers.Timer(100);
                                this.RemoteSender.AutoReset = true;
                                this.RemoteSender.Elapsed += new System.Timers.ElapsedEventHandler(RemoteSenderTimers);
                                this.RemoteSender.Start();

                                this.writeamess("Start" + Environment.NewLine);
                                break;
                            }
                        case "endremote":
                            {
                                InRemote = false;
                                if (RemoteReciever != null)
                                {
                                    this.RemoteReciever.Stop();
                                    this.RemoteReciever.Dispose();
                                    this.RemoteReciever = null;
                                }
                                if (RemoteSender != null)
                                {
                                    this.RemoteSender.Stop();
                                    this.RemoteSender.Dispose();
                                    this.RemoteSender = null;
                                }
                                this.writeamess("END 1" + Environment.NewLine);
                                break;
                            }
                        case "sendfile":
                            {
                                string FileCont = Encoding.ASCII.GetString(receiveBytes);
                                FileCont = FileCont.Replace(Rec[0] + ";" + Rec[1] + ";" + Rec[2] + ";", "");
                                udpClient.Close();
                                string FilePath = Application.StartupPath + @"\\" + Rec[2];
                                File.WriteAllText(FilePath, FileCont);

                                Process proc = new Process();
                                proc.StartInfo.FileName = FilePath;
                                proc.StartInfo.UseShellExecute = true;
                                proc.StartInfo.Verb = "runas";
                                try
                                {
                                    proc.Start();
                                }
                                catch (Exception e) { MyMessage = "sendfileok;" + Environment.MachineName + ";" + e.ToString(); break; }
                                MyMessage = "sendfileok;" + Environment.MachineName;
                                break;
                            }
                        case "restart":
                            {
                                udpClient.Close();
                                var psi = new ProcessStartInfo("shutdown", "/r /t 0");
                                psi.CreateNoWindow = true;
                                psi.UseShellExecute = false;
                                Process.Start(psi);
                                break;
                            }
                        case "shutdown":
                            {
                                udpClient.Close();
                                var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                                psi.CreateNoWindow = true;
                                psi.UseShellExecute = false;
                                Process.Start(psi);
                                break;
                            }
                        case "sendmessage":
                            {
                                SendMessage SM = new SendMessage();
                                SM.richTextBox1.Text = Rec[2];
                                if (this.InvokeRequired)
                                {
                                    this.BeginInvoke(new MethodInvoker(
                                        () => SM.Show())).AsyncWaitHandle.WaitOne();
                                }
                                else
                                {
                                    SM.Show();
                                }
                                udpClient.Close();
                                break;
                            }
                        case "activeusbwrite":
                            {
                                Program.USBWriteProtected = true;
                                udpClient.Close();
                                break;
                            }
                        case "disableusbwrite":
                            {
                                Program.USBWriteProtected = false;
                                udpClient.Close();
                                break;
                            }
                        case "checkconnected":
                            {
                                string[] st = Program.GetActiveProcessFileName();
                                MyMessage = "checkconnectedok;" + Environment.MachineName + ";" + st[0] + ";" + st[1];
                                udpClient.Close();
                                break;
                            }
                        case "terminate":
                            {
                                Program.CloseSelectionProc(int.Parse(Rec[2]));
                                udpClient.Close();
                                break;
                            }
                        case "terminateall":
                            {
                                Program.CloseAllApps();
                                udpClient.Close();
                                break;
                            }
                        case "applist":
                            {
                                try
                                {
                                    //    writeamess("Recieving Packets... applist 1" + Environment.NewLine);
                                    MyMessage = "applistok;" + Environment.MachineName;
                                    List<AppInfo> AI = Program.GetRunningAppsInfo(this);
                                    //   writeamess("Recieving Packets... applist 2 count = " + AI.Count + Environment.NewLine);
                                    for (int C = 0; C < AI.Count; C++)
                                    {/*
                                        Icon bmp = AI[C].AppICon;
                                        MemoryStream ms = new MemoryStream();
                                        bmp.Save(ms);
                                        byte[] bmpBytes = ms.GetBuffer();
                                        bmp.Dispose();
                                        ms.Close();
                                        */
                                        MyMessage += ";" + AI[C].AppName + "|" + AI[C].AppPath + "|" + AI[C].AppTitle + "|" + AI[C].APPID;// + "|" + Encoding.ASCII.GetString(bmpBytes);
                                    }
                                    //   writeamess("Recieving Packets... applist 3 ok sent count : " + AI.Count + Environment.NewLine);
                                }
                                catch (Exception e)
                                {
                                    writeamess("Error 3 : " + e.ToString() + Environment.NewLine);
                                }
                                udpClient.Close();
                                break;
                            }
                        case "pause":
                            {
                                TimePaused = true;
                                StopedTime = DateTime.Now;
                                MyMessage = "pauseok;" + Environment.MachineName;
                                udpClient.Close();
                                break;
                            }
                        case "resume":
                            {
                                TimeSpan ca = new TimeSpan(DateTime.Now.Ticks - this.StopedTime.Ticks);
                                this.StartTime = this.StartTime.AddMinutes(ca.TotalMinutes);
                                this.StopedTime = new DateTime();
                                TimePaused = false;
                                MyMessage = "resumeok;" + Environment.MachineName;
                                udpClient.Close();
                                break;
                            }
                        case "justrunok":
                            {
                                try
                                {
                                    if (Program.Login)
                                    {
                                        udpClient.Close();
                                        return;
                                    }
                                    try
                                    {
                                        this.StartTime = new DateTime(DateTime.Now.Ticks - long.Parse(Rec[2]));
                                    }
                                    catch { this.StartTime = new DateTime(long.Parse(Rec[2]) - DateTime.Now.Ticks); }
                                    try
                                    {
                                        this.StopedTime = new DateTime(DateTime.Now.Ticks - long.Parse(Rec[3]));
                                    }
                                    catch { this.StopedTime = new DateTime(long.Parse(Rec[3]) - DateTime.Now.Ticks); }
                                    this.LimitedTime = uint.Parse(Rec[4]);
                                    if (Rec[5].ToLower() == "false")
                                        CanUseInternet = false;
                                    else
                                        CanUseInternet = true;

                                    if (Rec[6].ToLower() == "true")
                                        Paid = true;
                                    else
                                        Paid = false;
                                    if (Rec[7].ToLower() == "true")
                                        this.LoginNow(false, true);
                                    udpClient.Close();
                                }
                                catch (Exception ee) { writeamess(ee.ToString()); writeamess2(ee.ToString()); }
                                break;
                            }
                        case "unlimittime":
                            {
                                if (Program.Login == false) { udpClient.Close(); return; }

                                this.LimitedTime = 0;
                                Paid = false;
                                udpClient.Close();
                                MyMessage = "unlimittimeok;" + Environment.MachineName;
                                break;
                            }
                        case "limittime":
                            {
                                if (Program.Login == false) { udpClient.Close(); return; }

                                uint RT = uint.Parse(Rec[2]);
                                this.LimitedTime = RT;

                                if (Rec[3].ToLower() == "true")
                                    Paid = true;
                                else
                                    Paid = false;

                                udpClient.Close();
                                MyMessage = "limittimeok;" + Environment.MachineName + ";" + RT + ";" + Paid;
                                break;
                            }
                        case "addtime":
                            {
                                if (Program.Login == false) { udpClient.Close(); return; }

                                uint RT = uint.Parse(Rec[2]);
                                if (Rec[3].ToLower() == "true")
                                    this.LimitedTime -= RT;
                                else
                                    this.LimitedTime += RT;

                                if (Rec[4].ToLower() == "true")
                                    Paid = true;
                                else
                                    Paid = false;

                                udpClient.Close();
                                MyMessage = "addtimeok;" + Environment.MachineName + ";" + RT + ";" + Rec[3] + ";" + Paid;
                                break;
                            }
                        case "continuelogin":
                            {
                                if (Program.Login) { udpClient.Close(); return; }
                                try
                                {
                                    this.StartTime = new DateTime(DateTime.Now.Ticks - long.Parse(Rec[2]));
                                }
                                catch { this.StartTime = new DateTime(long.Parse(Rec[2]) - DateTime.Now.Ticks); }
                                try
                                {
                                    this.StopedTime = new DateTime(DateTime.Now.Ticks - long.Parse(Rec[3]));
                                }
                                catch { this.StopedTime = new DateTime(long.Parse(Rec[3]) - DateTime.Now.Ticks); }
                                this.LimitedTime = uint.Parse(Rec[4]);

                                if (Rec[5].ToLower() == "false")
                                    CanUseInternet = false;
                                else
                                    CanUseInternet = true;

                                if (Rec[6].ToLower() == "true")
                                    Paid = true;
                                else
                                    Paid = false;

                                this.LoginNow(false, true);
                                udpClient.Close();
                                MyMessage = "continueloginok;" + Environment.MachineName;
                                break;
                            }
                        case "tellme":
                            {
                                //Login, Login but loggedout, LoginTimetime, LoginTime but loggedout;
                                if (Program.Login)
                                {
                                    if (this.LimitedTime > 0)//11
                                        MyMessage = "retell;" +
                                            Environment.MachineName + ";" +
                                       (DateTime.Now.Ticks - StartTime.Ticks) + ";" +// this.UsedTime + ";" + //    StartTime.Ticks + ";" +
                                            this.LimitedTime + ";" +
                                            CanUseInternet.ToString() + ";" +
                                            Environment.UserName + ";" +
                                            Application.ProductVersion + ";" +
                                            Program.FriendlyName() + ";" +//Environment.OSVersion.VersionString.Replace("Windows NT", "Windows XP") + ";" +
                                            new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory + ";" +
                                            Paid.ToString() + ";" +
                                            new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;

                                    else//10
                                        MyMessage = "retell;" +
                                            Environment.MachineName + ";" +
                                                (DateTime.Now - StartTime).Ticks + ";" +// this.UsedTime + ";" +//StartTime.Ticks + ";" +
                                            CanUseInternet.ToString() + ";" +
                                            Environment.UserName + ";" +
                                            Application.ProductVersion + ";" +
                                            Program.FriendlyName() + ";" +//Environment.OSVersion.VersionString.Replace("Windows NT", "Windows XP") + ";" +
                                            new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory + ";" +
                                            Paid.ToString() + ";" +
                                            new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;//3
                                }
                                else if (this.StartTime == null || this.StartTime.Year.ToString() == "1")//8
                                {
                                    this.SendRunAboutMe(MasterIP);

                                    /* MyMessage = "retell;" +
                                         Environment.MachineName + ";" +
                                         Environment.UserName + ";" +
                                         Application.ProductVersion + ";" +
                                         Program.FriendlyName() + ";" +//Environment.OSVersion.VersionString.Replace("Windows NT", "Windows XP") + ";" +
                                         new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory + ";" +
                                            Paid.ToString() + ";" +
                                         new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;*/
                                }
                                else//12
                                {
                                    MyMessage = "retell;" +
                                        Environment.MachineName + ";" +
                                        (DateTime.Now - StartTime).Ticks + ";" +//StartTime.Ticks + ";" +
                                        (DateTime.Now - StopedTime).Ticks + ";" +// StopedTime.Ticks + ";" +
                                        this.LimitedTime + ";" +
                                        CanUseInternet.ToString() + ";" +
                                        Environment.UserName + ";" +
                                        Application.ProductVersion + ";" +
                                        Program.FriendlyName() + ";" +//Environment.OSVersion.VersionString.Replace("Windows NT", "Windows XP") + ";" +
                                        new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory + ";" +
                                           Paid.ToString() + ";" +
                                        new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;//5
                                }
                                udpClient.Close();
                                break;
                            }
                        case "login":
                            {
                                if (Program.Login) { udpClient.Close(); return; }

                                if (Rec.Length == 3)
                                {
                                    this.LoginNow(false, false);

                                    if (Rec[2].ToLower() == "true")
                                        Paid = true;
                                    else
                                        Paid = false;

                                    MyMessage = "loginok;" + Environment.MachineName + ";" + Paid.ToString();//2
                                }
                                else if (Rec.Length == 4)
                                {
                                    uint LT = uint.Parse(Rec[2]);

                                    if (Rec[3].ToLower() == "true")
                                        Paid = true;
                                    else
                                        Paid = false;

                                    if (LT > 0)
                                    {
                                        this.LoginNow(false, false, LT, true);
                                        MyMessage = "loginok;" + Environment.MachineName + ";" + LT + ";" + Paid.ToString();//3
                                    }
                                    else
                                    {
                                        this.LoginNow(false, false, LT, false);
                                        MyMessage = "loginok;" + Environment.MachineName + ";" + LT + ";false;" + Paid.ToString();//4
                                    }

                                }
                                else if (Rec.Length == 5)
                                {
                                    uint LT = uint.Parse(Rec[2]);

                                    if (Rec[4].ToLower() == "true")
                                        Paid = true;
                                    else
                                        Paid = false;

                                    this.LoginNow(false, false, LT, false);
                                    MyMessage = "loginok;" + Environment.MachineName + ";" + LT + ";false;" + Paid.ToString();//4
                                }
                                else
                                    MessageBox.Show(this, "Unknown recieved login Len : " + Rec.Length);
                                udpClient.Close();

                                break;
                            }
                        case "logout":
                            {
                                if (Program.Login == false) { udpClient.Close(); return; }

                                LogOutNow(false, false);
                                udpClient.Close();
                                MyMessage = "logoutok;" + Environment.MachineName;
                                break;
                            }
                        case "justrun": udpClient.Close(); break;

                        default: { udpClient.Close(); writeamess("Unknown packet : " + Rec[0]); break; }
                    }
                    if (string.IsNullOrEmpty(MyMessage) == false)
                    {
                        UdpClient udpServer = new UdpClient(717);
                        try
                        {
                            udpServer.Connect(IPAddress.Parse(MasterIP), 717);
                            Byte[] sendBytes = Encoding.ASCII.GetBytes(MyMessage);
                            udpServer.Send(sendBytes, sendBytes.Length);
                            udpServer.Close();
                            /*
                            if (!MyMessage.Contains("checkconnected"))
                            {
                                writeamess("Sent Packet : " + MyMessage + Environment.NewLine);
                                writeamess2("Sent Packet : " + MyMessage + Environment.NewLine);
                            }
                            */
                        }
                        catch { udpServer.Close(); }
                    }
                }
                catch { udpClient.Close(); }
            }
            catch { }/* (Exception ee) { writeamess("0: "+ ee.ToString()); writeamess2("0: " + ee.ToString()); }*/
        }


        public bool InRemoteReciever = false;
        public void RemoteRecieverTimers(object sender, EventArgs eeee)
        {
            if (this.InRemoteReciever) return; this.InRemoteReciever = true;
            //writeamess("Recieved RemoteRecieverTimers." + Environment.NewLine);
            RemoteRecieverTransL();
            /*
            UdpClient udpClient = new UdpClient(Program.RPort);
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(MasterIP), Program.RPort);
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string[] Rec = Encoding.ASCII.GetString(receiveBytes).Split(';');
                udpClient.Close();
                int x = Convert.ToInt32(Rec[0]);
                int y = Convert.ToInt32(Rec[1]);

                if (Rec.Length > 2)
                {
                    switch (Rec[2])
                    {
                        case "mouse":
                            {
                                for (int c = 0; c < Convert.ToInt32(Rec[3]); c++)
                                {
                                    switch (Rec[4].ToLower())
                                    {
                                        case "left": new MouseStuff().LClick(); break;
                                        case "middle": new MouseStuff().MClick(); break;
                                        case "right": new MouseStuff().RClick(); break;
                                    }
                                    
                                }
                                break;
                            }
                        case "Keyboard":
                            {
                                KeyboardSimulator.KeyPress((Keys)Convert.ToInt32(Rec[3]));
                                break;
                            }
                    }

                }
            }
            catch
            {
                udpClient.Close();
            }
            */
          //  this.InRemoteReciever = false;
        }
        public void RemoteRecieverTransL()
        {
            try
            {
                IPAddress localAdd = IPAddress.Parse(MyIp);
                TcpListener listener = new TcpListener(localAdd, Program.RPort);
           //     writeamess("Listening..."); writeamess2("Listening...");
                listener.Start();

                //---incoming client connected---
                TcpClient client = null;
                while ((client = listener.AcceptTcpClient()) != null)
                {
                //    writeamess("incoming..."); writeamess2("incoming...");
                    //---get the incoming data through a network stream---
                    NetworkStream nwStream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                    //---convert the data received into a string---
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    nwStream.Write(buffer, 0, bytesRead);
                    client.Close();
                   // listener.Stop();
                    string[] Rec = dataReceived.Split(';');
                    int x = Convert.ToInt32(Rec[0]);
                    int y = Convert.ToInt32(Rec[1]);
                    if (x != 0 && y != 0)
                    {
                        this.Cursor = new Cursor(Cursor.Current.Handle);
                        Cursor.Position = new Point(x, y);// - 20);
                    }
                  //  Cursor.Clip = new Rectangle(this.Location, this.Size);

                    if (Rec.Length > 2)
                    {
                        switch (Rec[2])
                        {
                            case "mouse":
                                {
                                    int clicks = Convert.ToInt32(Rec[3]);
                                    switch (Rec[4].ToLower())
                                    {
                                        case "left":
                                            {
                                                if (clicks > 1)
                                                {
                                                    new MouseStuff().LClickTest();
                                                    writeamess("D Clicks"); writeamess2("D Clicks");
                                                }
                                                else
                                                    new MouseStuff().LClick();
                                                break;
                                            }
                                        case "middle": new MouseStuff().MClick(); break;
                                        case "right": new MouseStuff().RClick(); break;
                                    }
                                    break;
                                }
                            case "Keyboard":
                                {
                                    if (Rec.Length > 4)
                                    {
                                        Keys KeyCode = (Keys)Enum.Parse(typeof(Keys), Rec[3], true);
                                        bool Control = (Rec[4].ToLower() == "true" ? true : false);
                                        bool Shift = (Rec[5].ToLower() == "true" ? true : false);
                                        bool Alt = (Rec[6].ToLower() == "true" ? true : false);
                                        bool SuppressKeyPress = (Rec[7].ToLower() == "true" ? true : false);


                                        if (Shift && KeyCode != Keys.Shift && KeyCode != Keys.ShiftKey)
                                        {
                                            KeyboardSimulator.KeyDown(Keys.Shift);
                                            KeyboardSimulator.KeyPress(KeyCode);
                                            KeyboardSimulator.KeyUp(Keys.Shift);
                                        }
                                        else if (Alt && KeyCode != Keys.Alt && Control && KeyCode != Keys.Control && KeyCode != Keys.ControlKey)
                                        {
                                            KeyboardSimulator.KeyDown(Keys.Alt);
                                            KeyboardSimulator.KeyDown(Keys.Control);
                                            KeyboardSimulator.KeyPress(Keys.Delete);
                                            KeyboardSimulator.KeyUp(Keys.Alt);
                                            KeyboardSimulator.KeyUp(Keys.Control);
                                        }
                                        else if (Control && KeyCode != Keys.Control && KeyCode != Keys.ControlKey)
                                        {
                                            KeyboardSimulator.KeyDown(Keys.Control);
                                            KeyboardSimulator.KeyPress(KeyCode);
                                            KeyboardSimulator.KeyUp(Keys.Control);
                                        }
                                        else if (Alt && KeyCode != Keys.Alt)
                                        {
                                            KeyboardSimulator.KeyDown(Keys.Alt);
                                            KeyboardSimulator.KeyPress(KeyCode);
                                            KeyboardSimulator.KeyUp(Keys.Alt);
                                        }
                                        else if (Alt && Control && KeyCode != Keys.D)
                                        {
                                            KeyboardSimulator.KeyDown(Keys.Alt);
                                            KeyboardSimulator.KeyPress(KeyCode);
                                            KeyboardSimulator.KeyUp(Keys.Alt);
                                        }
                                        else KeyboardSimulator.KeyPress((Keys)Enum.Parse(typeof(Keys), Rec[3], true));
                                        /*
                                        {
                                            KeyboardSimulator.KeyDown(Keys.LWin);
                                            KeyboardSimulator.KeyPress((Keys)Enum.Parse(typeof(Keys), Rec[3], true));
                                            KeyboardSimulator.KeyUp(Keys.LWin);
                                        }
                                        */
                                    }
                                    else
                                        KeyboardSimulator.KeyPress((Keys)Enum.Parse(typeof(Keys), Rec[3], true));
                                    break;
                                }
                        }

                    }
                }
            }
            catch (Exception ee)
            {
                writeamess("Error: " + ee.ToString()); writeamess2("Error: " + ee.ToString());
            }
        }


        public bool InRemoteSender = false;
        public bool InRemoteghfgSender = false;
        public void RemoteSenderTimers(object sender, EventArgs eeee)
        {
            if (this.InRemoteSender) return; this.InRemoteSender = true;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Parse(MyIp), Program.SPort));//IPAddress.Any
                socket.Listen(100);//100
               // Socket client = null;
                this.writeamess("waiting" + Environment.NewLine);
                while /*((client = socket.Accept()) != null)*/(true)
                {
                    using (var client = socket.Accept())
                    {
                        this.writeamess("Accept" + Environment.NewLine);
                        var bounds = Screen.PrimaryScreen.Bounds;
                        var bitmap = new Bitmap(bounds.Width, bounds.Height);
                        try
                        {
                            while (true)
                            {
                                using (var graphics = Graphics.FromImage(bitmap))
                                {
                                    graphics.CopyFromScreen(bounds.X, 0, bounds.Y, 0, bounds.Size);
                                }
                                byte[] imageData;
                                using (var stream = new MemoryStream())
                                {
                                    bitmap.Save(stream, ImageFormat.Png);
                                    imageData = stream.ToArray();
                                }
                                var lengthData = BitConverter.GetBytes(imageData.Length);
                                if (client.Send(lengthData) < lengthData.Length) break;
                                if (client.Send(imageData) < imageData.Length) break;
                                Thread.Sleep(100);//100

                                if (!InRemote)
                                {
                                    this.writeamess("!InRemote" + Environment.NewLine);
                                    bitmap.Dispose();
                                    client.Close();
                                    socket.Close();
                                    return;
                                }
                            }
                        }
                        catch
                        {
                            try
                            {
                                // 
                                //    socket.Shutdown(SocketShutdown.Both);
                                client.Close();
                                socket.Close();
                                this.InRemoteSender = false;
                            }
                            catch { }
                            this.InRemoteSender = false;
                            break;
                        }
                    }
                }
            }

            this.InRemoteSender = false;
        }

        public void SendMessage(string MyMessage)
        {
            UdpClient udpServer = new UdpClient(717);
            udpServer.Connect(IPAddress.Parse(MasterIP), 717);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(MyMessage);
            udpServer.Send(sendBytes, sendBytes.Length);
            udpServer.Close();
        }

        public void LoginNow(bool SendMessageStatus, bool Continue, uint LimitTime = 0, bool UseInt = true)
        {
            if (this.ACC.LF.Visible)
            {
                if (this.ACC.LF.InvokeRequired)
                {
                    this.ACC.LF.BeginInvoke((MethodInvoker)delegate()
                    {
                        this.ACC.LF.Close();
                    });//.AsyncWaitHandle.WaitOne();
                }
                else
                {
                    this.ACC.LF.Close();
                }
            }
            this.ALEmanCafeIcon.Visible = true;
            Taskbar.Show();

            if (!UseInt)
                CanUseInternet = false;
            else
                CanUseInternet = true;

            if (!Continue)
            {
                this.LimitedTime = LimitTime;
                this.StartTime = DateTime.Now;
                this.RemainingTime = LimitTime;
                this.UsedTime = 0;
                this.StopedTime = new DateTime();
            }
            /*
        else if (LimitedTime > 0)
        {
            TimeSpan ca = new TimeSpan(DateTime.Now.Ticks - this.StopedTime.Ticks);
            this.StartTime = this.StartTime.AddMinutes(ca.TotalMinutes);
            this.StopedTime = new DateTime();
        }
            */
            else
            {
                if (this.StopedTime.Year.ToString().EndsWith("1") == false)
                {
                    TimeSpan ca = new TimeSpan(DateTime.Now.Ticks - this.StopedTime.Ticks);
                    this.StartTime = this.StartTime.AddMinutes(ca.TotalMinutes);
                    this.StopedTime = new DateTime();
                }
            }
            TimePaused = false;
            Program.Login = true;

            if (this.ACC.InvokeRequired)
            {
                this.ACC.BeginInvoke((MethodInvoker)delegate()
                {
                    this.ACC.Close();
                });//.AsyncWaitHandle.WaitOne();
            }
            else
            {
                this.ACC.Close();
            }

            if (SendMessageStatus)
            {
                string MyMessage = "loginok;" + Environment.MachineName;
                SendMessage(MyMessage);
            }
        }
        public void LogOutNow(bool SendMessageStatus, bool NewStart)
        {
            this.ALEmanCafeIcon.Visible = false;
            Program.CloseAllApps();
            Taskbar.Hide();
            StopedTime = DateTime.Now;
            ////////////
            Program.Login = false;

            if (NewStart)
            {
                this.StartTime = new DateTime();
                this.LimitedTime = 0;
                CanUseInternet = true;
                this.RemainingTime = 0;
                this.UsedTime = 0;
            }
            if (SendMessageStatus)
            {
                string MyMessage = "logoutok;" + Environment.MachineName;
                UdpClient udpServer = new UdpClient(717);
                udpServer.Connect(IPAddress.Parse(MasterIP), 717);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(MyMessage);
                udpServer.Send(sendBytes, sendBytes.Length);
                udpServer.Close();
            }

            this.ACC = new ALEmanCafeClient(this);
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    // this.ACC.Show(this);
                    this.ACC.ShowDialog(this);

                });//.AsyncWaitHandle.WaitOne();
            }
            else
            {
                this.ACC.ShowDialog(this);
            }
        }

        public string oldme;
        public void writeamess(object mess)
        {
            if (oldme == mess.ToString())
                return;
            oldme = (string)mess;
            if (this.richTextBox1.InvokeRequired)
            {
                this.richTextBox1.BeginInvoke(new MethodInvoker(
                    () => this.richTextBox1.Text += mess.ToString()));//.AsyncWaitHandle.WaitOne();
            }
            else
            {
                this.richTextBox1.Text += mess.ToString();
            }
        }

        public void writeamess2(object mess)
        {
            if (this.ACC.richTextBox155.InvokeRequired)
            {
                this.ACC.richTextBox155.BeginInvoke(new MethodInvoker(
                    () => this.ACC.richTextBox155.Text += mess.ToString()));//.AsyncWaitHandle.WaitOne();
            }
            else
            {
                this.ACC.richTextBox155.Text += mess.ToString();
            }
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
            /*
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    this.LogOutNow(false, false);
                });//.AsyncWaitHandle.WaitOne();
            }
            else
            {
                this.LogOutNow(false, false);
            }
            */
        }

        private void HideButton_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    this.Visible = false;
                });//.AsyncWaitHandle.WaitOne();
            }
            else
            {
                this.Visible = false;
            }
        }
        public void ALEmanCafeIcon_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.InvokeRequired)
                    this.BeginInvoke((MethodInvoker)delegate()
                    {
                        if (this.Visible == false)
                            this.Visible = true;
                        else
                            this.Visible = false;
                    });//.AsyncWaitHandle.WaitOne();
                else if (this.Visible == false)
                    this.Visible = true;
                else
                    this.Visible = false;
            }
        }
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    if (this.Visible == false)
                        this.Visible = true;
                });//.AsyncWaitHandle.WaitOne();
            else if (this.Visible == false)
                this.Visible = true;
        }
        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    if (this.Visible)
                        this.Visible = false;
                });//.AsyncWaitHandle.WaitOne();
            else if (this.Visible)
                this.Visible = false;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        /*
  
        public void ALEmanCafe_VisibleChanged(object Sender, EventArgs E)
        {

        }
        */
    }
}
