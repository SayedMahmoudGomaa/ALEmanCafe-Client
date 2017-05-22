using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using MouseKeyboardLibrary;
using System.Net;
using System.Net.Sockets;

namespace ALEmanCafe_Client
{
    static class Program
    {
        public static bool Login = false;
        public static string UsernameLog = "";
        public static string PasswordLog = "";
        private static double HourPrice = 1.50;
        private static double MinimumCost = 0.50;
        public static int SPort = 720;
        public static int RPort = 720;

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [STAThread]
        static void Main()
        {
            #region CloseOtherHC
            string PN = Process.GetCurrentProcess().ProcessName;
            int PNo = Process.GetCurrentProcess().Id;
            Process op = null;
            foreach (Process p in Process.GetProcessesByName(PN))
            {
                if (p.Id != PNo)
                {
                    op = p;
                }
            }
            if (op != null)
                op.Kill();
            #endregion
            /*
            #region WFW
            UdpClient udpClient = new UdpClient(716);
            udpClient.Close();
            bool NotDone = true;
            byte nn = 0;
            while (NotDone || nn < 11)
            {
                nn++;
                byte count = 0;
                foreach (Process p in GetRunningApps(null))
                {
                    if (p.ProcessName == "rundll32" || p.MainWindowTitle == "Windows Security Alert" || Path.GetFileName(p.MainModule.FileName) == "rundll32")
                    {
                        count++;
                        SetWindowPos(p.MainWindowHandle, new IntPtr(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
                        KeyboardSimulator.KeyPress(Keys.U);
                        if (Process.GetProcessById(p.Id) == null)
                            NotDone = false;
                    }
                }
                if (count < 1)
                    NotDone = false;
            }
            #endregion
            */
            if (Environment.UserName != "ALEman-MC")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ALEmanCafe());
            }
            else
            {
                USB_disableWriteProtect();
            }
        }

        public static void KillTaskManager()
        {
            List<Process> ps = new List<Process>();
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.ToLower() == "taskmgr" || p.ProcessName.ToLower() == "taskmgr.exe")
                {
                    ps.Add(p);
                    //       MessageBox.Show(p.ProcessName);

                }
            }
            foreach (Process p in ps)
            {
                p.Kill();
            }
        }
        public static void CloseAllApps()
        {
            List<Process> Pc = new List<Process>();
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.ProcessName.ToLower() == "explorer" ||
                   p.ProcessName.ToLower() == "devenv" ||
                      p.ProcessName == Application.ProductName
                   ) continue;

                if (!string.IsNullOrEmpty(p.MainWindowTitle))
                {
                    Pc.Add(p);
                }
            }
            try
            {
                foreach (Process p in Pc)
                {
                    // MessageBox.Show(p.ProcessName + ", MainWindowTitle : " + p.MainWindowTitle + Environment.NewLine);
                    // this.ACC.richTextBox1.Text += p.ProcessName + ", MainWindowTitle : " + p.MainWindowTitle + Environment.NewLine;
                    p.Kill();
                }
            }
            catch { }
        }
        public static List<Process> GetRunningApps(ALEmanCafe AC)
        {
            List<Process> Pc = new List<Process>();
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    if (p.ProcessName.ToLower() == "explorer" ||
                       p.ProcessName.ToLower() == "devenv" ||
                          p.ProcessName == Application.ProductName
                       ) continue;

                    if (!string.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        Pc.Add(p);
                    }
                }
            }
            catch (Exception e)
            {
                if (AC != null)
                    AC.writeamess("Error 2 : " + e.ToString() + Environment.NewLine);
            }
            return Pc;
        }
        public static List<AppInfo> GetRunningAppsInfo(ALEmanCafe AC)
        {
            List<AppInfo> AppsInfo = new List<AppInfo>();
            foreach (Process p in Program.GetRunningApps(AC))
            {
                try
                {
                    AppInfo AI = new AppInfo();
                    AI.AppICon = Icon.ExtractAssociatedIcon(p.MainModule.FileName);
                    AI.AppName = p.ProcessName;
                    AI.AppPath = p.MainModule.FileName;
                    AI.AppTitle = p.MainWindowTitle;
                    AI.APPID = p.Id;
                    AppsInfo.Add(AI);
                }
                catch (Exception e)
                {
                    if (AC != null)
                        AC.writeamess("Error 1 : " + e.ToString() + Environment.NewLine);
                }
            }
            return AppsInfo;
        }
        public static void CloseInternetBrowsers()
        {
            try
            {
                List<Process> Pc = new List<Process>();
                Process[] processNames = Process.GetProcessesByName("iexplore");
                foreach (Process item in processNames)
                {
                    Pc.Add(item);
                }

                Process[] processNames2 = Process.GetProcessesByName("firefox");

                foreach (Process item in processNames2)
                {
                    Pc.Add(item);
                }
                Process[] processNames3 = Process.GetProcessesByName("chrome");

                foreach (Process item in processNames3)
                {
                    Pc.Add(item);
                }

                foreach (Process p in Pc)
                {
                    p.Kill();
                }
            }
            catch { }
        }
        public static void CloseSelectionProc(int id)
        {
            try
            {
                Process processName = Process.GetProcessById(id);
                if (processName != null)
                {
                    processName.Kill();
                }
            }
            catch { }
        }

        public static void GetTimeCost(uint Minutes, Label LB = null)
        {
            if (Minutes <= 0)
            {
                if (LB.InvokeRequired)
                {
                    LB.BeginInvoke((MethodInvoker)delegate()
                    {
                        LB.Text = "0000 جم";
                    });//.AsyncWaitHandle.WaitOne();
                }
                else
                {
                    LB.Text = "0000 جم";
                }
                //  return "0000 جم";
            }
            else
            {
                uint TM = Minutes;
                double ss = HourPrice * 10 / 60;
                double Price = 0;
                while (TM >= 10)
                {
                    Price += ss;
                    TM -= 10;
                }
                if (TM > 2)
                    Price += ss;

                if (Price < Program.MinimumCost)
                    Price = Program.MinimumCost;
                string lastp = Price + "";
                if (!lastp.Contains("."))
                    lastp = lastp + ".00";
                else if (lastp.Split('.')[1].Length < 2)
                    lastp = lastp + "0";

                if (LB.InvokeRequired)
                {
                    LB.BeginInvoke((MethodInvoker)delegate()
                    {
                        LB.Text = lastp + " جم";
                    });//.AsyncWaitHandle.WaitOne();
                }
                else
                {
                    LB.Text = lastp + " جم";
                }

                //return Usage + " جم";
            }
        }
        public static void GetStringUsedTime(double Minutes, Label LB)
        {
            string Res = "00:00";
            int Hours = 0;
            if (Minutes > 0)
            {
                while (Minutes >= 60)
                {
                    Minutes -= 60;
                    Hours++;
                }
                if (Hours < 1)
                    Res = "00:";
                else if (Hours < 10)
                    Res = "0" + Hours + ":";
                else
                    Res = Hours + ":";

                if (Minutes < 1)
                    Res += "00";
                else if (Minutes < 10)
                    Res += "0" + (Minutes.ToString().Contains(".") == false ? Minutes.ToString() : Minutes.ToString().Split('.')[0].ToString()).ToString();
                else
                    Res += Minutes.ToString().Contains(".") == false ? Minutes.ToString() : Minutes.ToString().Split('.')[0].ToString(); ;
            }
            //return Res;
            if (LB.InvokeRequired)
                LB.BeginInvoke((MethodInvoker)delegate()
                {
                    LB.Text = Res;
                });//.AsyncWaitHandle.WaitOne();
            else
                LB.Text = Res;
        }
        public static void GetRemainingTime(double UsedMinutes, uint TotalTime, Label LB)
        {
            string Res = "00:00";
            uint Remaining = TotalTime - (uint)UsedMinutes;
            uint Hours = 0;
            if (Remaining > 0)
            {
                while (Remaining > 60)
                {
                    Remaining -= 60;
                    Hours++;
                }
                if (Hours < 1)
                    Res = "00:";
                else if (Hours < 10)
                    Res = "0" + Hours + ":";
                else
                    Res = Hours + ":";

                if (Remaining < 1)
                    Res += "00";
                else if (Remaining < 10)
                    Res += "0" + Remaining;
                else
                    Res += Remaining;
            }
            if (LB.InvokeRequired)
                LB.BeginInvoke((MethodInvoker)delegate()
                {
                    LB.Text = Res;
                });//.AsyncWaitHandle.WaitOne();
            else
                LB.Text = Res;
            // return Res;
        }

        public static uint GetUsedTime(DateTime StartTime)
        {
            TimeSpan ST = new TimeSpan(DateTime.Now.Ticks - StartTime.Ticks);
            if (ST.TotalMinutes > 0)
                return (uint)ST.TotalMinutes;
            else
                return 0;
        }
        public static uint GetRemainingTime(DateTime StartTime, uint TotalTime)
        {
            TimeSpan ST = new TimeSpan(StartTime.AddMinutes(TotalTime).Ticks - DateTime.Now.Ticks);
            if (ST.TotalMinutes > 0)
                return (uint)ST.TotalMinutes;
            else
                return 0;
        }

        public static void ShowFolderOptions()
        {
            try
            {
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                regKey.SetValue("DisableTaskMgr", 0);
                regKey.Close();

                Microsoft.Win32.RegistryKey regKey2 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                regKey2.SetValue("Reg_dword", 0);
                regKey2.Close();

                Microsoft.Win32.RegistryKey regKey3 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
                regKey3.SetValue("NoFolderOptions", 0);
                regKey3.Close();
            }
            catch { }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static string[] GetActiveProcessFileName()
        {
            string[] st = new string[2];
            st[0] = "NA"; st[1] = "NA";
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);
                Process p = Process.GetProcessById((int)pid);
                st[0] = p.ProcessName;
                st[1] = p.MainWindowTitle;
                return st;
            }
            catch { }
            return st;
        }

        private static bool USBWriteWasProtected = false;
        public static bool USBWriteProtected
        {
            get
            {
                return USBWriteWasProtected;
            }
            set
            {
                USBWriteWasProtected = value;
                if (value)
                {
                    USB_enableWriteProtect();
                }
                else
                {
                    USB_disableWriteProtect();
                }
            }
        }

        private static void USB_enableWriteProtect()
        {
            RegistryKey key =
                Registry.LocalMachine.OpenSubKey
                    ("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies", true);
            if (key == null)
            {
                Registry.LocalMachine.CreateSubKey
                   ("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies",
                    RegistryKeyPermissionCheck.ReadWriteSubTree);
                key = Registry.LocalMachine.OpenSubKey
                   ("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies", true);
                key.SetValue("WriteProtect", 1, RegistryValueKind.DWord);
            }
            else if (key.GetValue("WriteProtect") != (object)(1))
            {
                key.SetValue("WriteProtect", 1, RegistryValueKind.DWord);
            }
        }
        private static void USB_disableWriteProtect()
        {
            RegistryKey key =
                Registry.LocalMachine.OpenSubKey
                   ("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies", true);
            if (key != null)
            {
                key.SetValue("WriteProtect", 0, RegistryValueKind.DWord);
            }
            key.Close();
        }

        private static string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public static string FriendlyName()
        {
            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (Directory.Exists(@"C:\Program Files (x86)") ? " x64." : " x32.");
            }
            return "";
        }
    }

    public static class Taskbar
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumThreadWindows(int threadId, EnumThreadProc pfnEnum, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int lpdwProcessId);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private const string VistaStartMenuCaption = "Start";
        private static IntPtr vistaStartMenuWnd = IntPtr.Zero;
        private delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// Show the taskbar.
        /// </summary>
        public static void Show()
        {
            SetVisibility(true);
        }

        /// <summary>
        /// Hide the taskbar.
        /// </summary>
        public static void Hide()
        {
            SetVisibility(false);
        }

        /// <summary>
        /// Sets the visibility of the taskbar.
        /// </summary>
        public static bool Visible
        {
            set { SetVisibility(value); }
        }

        /// <summary>
        /// Hide or show the Windows taskbar and startmenu.
        /// </summary>
        /// <param name="show">true to show, false to hide</param>
        private static void SetVisibility(bool show)
        {
            // get taskbar window
            IntPtr taskBarWnd = FindWindow("Shell_TrayWnd", null);

            // try it the WinXP way first...
            IntPtr startWnd = FindWindowEx(taskBarWnd, IntPtr.Zero, "Button", "Start");
            if (startWnd == IntPtr.Zero)
            {
                // ok, let's try the Vista easy way...
                startWnd = FindWindow("Button", null);

                if (startWnd == IntPtr.Zero)
                {
                    // no chance, we need to to it the hard way...
                    startWnd = GetVistaStartMenuWnd(taskBarWnd);
                }
            }

            ShowWindow(taskBarWnd, show ? SW_SHOW : SW_HIDE);
            ShowWindow(startWnd, show ? SW_SHOW : SW_HIDE);
        }

        /// <summary>
        /// Returns the window handle of the Vista start menu orb.
        /// </summary>
        /// <param name="taskBarWnd">windo handle of taskbar</param>
        /// <returns>window handle of start menu</returns>
        private static IntPtr GetVistaStartMenuWnd(IntPtr taskBarWnd)
        {
            // get process that owns the taskbar window
            int procId;
            GetWindowThreadProcessId(taskBarWnd, out procId);

            Process p = Process.GetProcessById(procId);
            if (p != null)
            {
                // enumerate all threads of that process...
                foreach (ProcessThread t in p.Threads)
                {
                    EnumThreadWindows(t.Id, MyEnumThreadWindowsProc, IntPtr.Zero);
                }
            }
            return vistaStartMenuWnd;
        }

        /// <summary>
        /// Callback method that is called from 'EnumThreadWindows' in 'GetVistaStartMenuWnd'.
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="lParam">parameter</param>
        /// <returns>true to continue enumeration, false to stop it</returns>
        private static bool MyEnumThreadWindowsProc(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder buffer = new StringBuilder(256);
            if (GetWindowText(hWnd, buffer, buffer.Capacity) > 0)
            {
                Console.WriteLine(buffer);
                if (buffer.ToString() == VistaStartMenuCaption)
                {
                    vistaStartMenuWnd = hWnd;
                    return false;
                }
            }
            return true;
        }
    }

    public class AppInfo
    {
        Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false); 
        public Icon AppICon = null;
        public int APPID;
        public string AppTitle, AppPath, AppName;
        /*
        private void USB_disableAllStorageDevices()
        {
            RegistryKey key =
                Registry.LocalMachine.OpenSubKey
                   ("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
            if (key != null)
            {
                key.SetValue("Start", 4, RegistryValueKind.DWord);
            }
            key.Close();
        }
        private void USB_enableAllStorageDevices()
        {
            RegistryKey key =
               Registry.LocalMachine.OpenSubKey
                  ("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
            if (key != null)
            {
                key.SetValue("Start", 3, RegistryValueKind.DWord);
            }
            key.Close();
        }

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        //  ExitWindowsEx(0,0);//log off
        [DllImport("user32")]
        public static extern void LockWorkStation();
        //LockWorkStation(); //lock
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
        //SetSuspendState(true, true, true); //Hibernate:
        //SetSuspendState(false, true, true); //sleep
        */
    }
    /*
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MEMORYSTATUSEX()
        {
            this.dwLength = (uint)Marshal.SizeOf(typeof(NativeMethods.MEMORYSTATUSEX));
        }

        public void Get()
        {
            ulong installedMemory;
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                installedMemory = memStatus.ullTotalPhys;
            }
        }
    }
    */

    /*
    //C://windows/system32/hnetcfg.dll and C://windows/system32/FirewallAPI.dll
    public class dfdsf
    {
        /// Allows basic access to the windows firewall API.
        /// This can be used to add an exception to the windows firewall
        /// exceptions list, so that our programs can continue to run merrily
        /// even when nasty windows firewall is running.
        ///
        /// Please note: It is not enforced here, but it might be a good idea
        /// to actually prompt the user before messing with their firewall settings,
        /// just as a matter of politeness.
        /// 

        /// 
        /// To allow the installers to authorize idiom products to work through
        /// the Windows Firewall.
        /// 
        public class FirewallHelper
        {
            #region Variables
            /// 

            /// Hooray! Singleton access.
            /// 

            private static FirewallHelper instance = null;

            /// 

            /// Interface to the firewall manager COM object
            /// 
            Type NetFwMgrType
            private INetFwMgr fwMgr = null;
            #endregion
            #region Properties
            /// 

            /// Singleton access to the firewallhelper object.
            /// Threadsafe.
            /// 

            public static FirewallHelper Instance
            {
                get
                {
                    lock (typeof(FirewallHelper))
                    {
                        if (instance == null)
                            instance = new FirewallHelper();
                        return instance;
                    }
                }
            }
            #endregion
            #region Constructivat0r
            /// 

            /// Private Constructor.  If this fails, HasFirewall will return
            /// false;
            /// 

            private FirewallHelper()
            {
                // Get the type of HNetCfg.FwMgr, or null if an error occurred
                Type fwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);

                // Assume failed.
                fwMgr = null;

                if (fwMgrType != null)
                {
                    try
                    {
                        fwMgr = (INetFwMgr)Activator.CreateInstance(fwMgrType);
                    }
                    // In all other circumnstances, fwMgr is null.
                    catch (ArgumentException) { }
                    catch (NotSupportedException) { }
                    catch (System.Reflection.TargetInvocationException) { }
                    catch (MissingMethodException) { }
                    catch (MethodAccessException) { }
                    catch (MemberAccessException) { }
                    catch (InvalidComObjectException) { }
                    catch (COMException) { }
                    catch (TypeLoadException) { }
                }
            }
            #endregion
            #region Helper Methods
            /// 

            /// Gets whether or not the firewall is installed on this computer.
            /// 

            /// 
            public bool IsFirewallInstalled
            {
                get
                {
                    if (fwMgr != null &&
                          fwMgr.LocalPolicy != null &&
                          fwMgr.LocalPolicy.CurrentProfile != null)
                        return true;
                    else
                        return false;
                }
            }

            /// 

            /// Returns whether or not the firewall is enabled.
            /// If the firewall is not installed, this returns false.
            /// 

            public bool IsFirewallEnabled
            {
                get
                {
                    if (IsFirewallInstalled && fwMgr.LocalPolicy.CurrentProfile.FirewallEnabled)
                        return true;
                    else
                        return false;
                }
            }

            /// 

            /// Returns whether or not the firewall allows Application "Exceptions".
            /// If the firewall is not installed, this returns false.
            /// 

            /// 
            /// Added to allow access to this metho
            /// 
            public bool AppAuthorizationsAllowed
            {
                get
                {
                    if (IsFirewallInstalled && !fwMgr.LocalPolicy.CurrentProfile.ExceptionsNotAllowed)
                        return true;
                    else
                        return false;
                }
            }

            /// 

            /// Adds an application to the list of authorized applications.
            /// If the application is already authorized, does nothing.
            /// 

            /// 
            ///         The full path to the application executable.  This cannot
            ///         be blank, and cannot be a relative path.
            /// 
            /// 
            ///         This is the name of the application, purely for display
            ///         puposes in the Microsoft Security Center.
            /// 
            /// 
            ///         When applicationFullPath is null OR
            ///         When appName is null.
            /// 
            /// 
            ///         When applicationFullPath is blank OR
            ///         When appName is blank OR
            ///         applicationFullPath contains invalid path characters OR
            ///         applicationFullPath is not an absolute path
            /// 
            /// 
            ///         If the firewall is not installed OR
            ///         If the firewall does not allow specific application 'exceptions' OR
            ///         Due to an exception in COM this method could not create the
            ///         necessary COM types
            /// 
            /// 
            ///         If no file exists at the given applicationFullPath
            /// 
            public void GrantAuthorization(string applicationFullPath, string appName)
            {
                #region  Parameter checking
                if (applicationFullPath == null)
                    throw new ArgumentNullException("applicationFullPath");
                if (appName == null)
                    throw new ArgumentNullException("appName");
                if (applicationFullPath.Trim().Length == 0)
                    throw new ArgumentException("applicationFullPath must not be blank");
                if (applicationFullPath.Trim().Length == 0)
                    throw new ArgumentException("appName must not be blank");
                if (applicationFullPath.IndexOfAny(Path.InvalidPathChars) >= 0)
                    throw new ArgumentException("applicationFullPath must not contain invalid path characters");
                if (!Path.IsPathRooted(applicationFullPath))
                    throw new ArgumentException("applicationFullPath is not an absolute path");
                if (!File.Exists(applicationFullPath))
                    throw new FileNotFoundException("File does not exist", applicationFullPath);
                // State checking
                if (!IsFirewallInstalled)
                    throw new FirewallHelperException("Cannot grant authorization: Firewall is not installed.");
                if (!AppAuthorizationsAllowed)
                    throw new FirewallHelperException("Application exemptions are not allowed.");
                #endregion

                if (!HasAuthorization(applicationFullPath))
                {
                    // Get the type of HNetCfg.FwMgr, or null if an error occurred
                    Type authAppType = Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication", false);

                    // Assume failed.
                    INetFwAuthorizedApplication appInfo = null;

                    if (authAppType != null)
                    {
                        try
                        {
                            appInfo = (INetFwAuthorizedApplication)Activator.CreateInstance(authAppType);
                        }
                        // In all other circumnstances, appInfo is null.
                        catch (ArgumentException) { }
                        catch (NotSupportedException) { }
                        catch (System.Reflection.TargetInvocationException) { }
                        catch (MissingMethodException) { }
                        catch (MethodAccessException) { }
                        catch (MemberAccessException) { }
                        catch (InvalidComObjectException) { }
                        catch (COMException) { }
                        catch (TypeLoadException) { }
                    }

                    if (appInfo == null)
                        throw new FirewallHelperException("Could not grant authorization: can't create INetFwAuthorizedApplication instance.");

                    appInfo.Name = appName;
                    appInfo.ProcessImageFileName = applicationFullPath;
                    // ...
                    // Use defaults for other properties of the AuthorizedApplication COM object

                    // Authorize this application
                    fwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(appInfo);
                }
                // otherwise it already has authorization so do nothing
            }
            /// 

            /// Removes an application to the list of authorized applications.
            /// Note that the specified application must exist or a FileNotFound
            /// exception will be thrown.
            /// If the specified application exists but does not current have
            /// authorization, this method will do nothing.
            /// 

            /// 
            ///         The full path to the application executable.  This cannot
            ///         be blank, and cannot be a relative path.
            /// 
            /// 
            ///         When applicationFullPath is null
            /// 
            /// 
            ///         When applicationFullPath is blank OR
            ///         applicationFullPath contains invalid path characters OR
            ///         applicationFullPath is not an absolute path
            /// 
            /// 
            ///         If the firewall is not installed.
            /// 
            /// 
            ///         If the specified application does not exist.
            /// 
            public void RemoveAuthorization(string applicationFullPath)
            {

                #region  Parameter checking
                if (applicationFullPath == null)
                    throw new ArgumentNullException("applicationFullPath");
                if (applicationFullPath.Trim().Length == 0)
                    throw new ArgumentException("applicationFullPath must not be blank");
                if (applicationFullPath.IndexOfAny(Path.InvalidPathChars) >= 0)
                    throw new ArgumentException("applicationFullPath must not contain invalid path characters");
                if (!Path.IsPathRooted(applicationFullPath))
                    throw new ArgumentException("applicationFullPath is not an absolute path");
                if (!File.Exists(applicationFullPath))
                    throw new FileNotFoundException("File does not exist", applicationFullPath);
                // State checking
                if (!IsFirewallInstalled)
                    throw new FirewallHelperException("Cannot remove authorization: Firewall is not installed.");
                #endregion

                if (HasAuthorization(applicationFullPath))
                {
                    // Remove Authorization for this application
                    fwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Remove(applicationFullPath);
                }
                // otherwise it does not have authorization so do nothing
            }
            /// 

            /// Returns whether an application is in the list of authorized applications.
            /// Note if the file does not exist, this throws a FileNotFound exception.
            /// 

            /// 
            ///         The full path to the application executable.  This cannot
            ///         be blank, and cannot be a relative path.
            /// 
            /// 
            ///         The full path to the application executable.  This cannot
            ///         be blank, and cannot be a relative path.
            /// 
            /// 
            ///         When applicationFullPath is null
            /// 
            /// 
            ///         When applicationFullPath is blank OR
            ///         applicationFullPath contains invalid path characters OR
            ///         applicationFullPath is not an absolute path
            /// 
            /// 
            ///         If the firewall is not installed.
            /// 
            /// 
            ///         If the specified application does not exist.
            /// 
            public bool HasAuthorization(string applicationFullPath)
            {
                #region  Parameter checking
                if (applicationFullPath == null)
                    throw new ArgumentNullException("applicationFullPath");
                if (applicationFullPath.Trim().Length == 0)
                    throw new ArgumentException("applicationFullPath must not be blank");
                if (applicationFullPath.IndexOfAny(Path.InvalidPathChars) >= 0)
                    throw new ArgumentException("applicationFullPath must not contain invalid path characters");
                if (!Path.IsPathRooted(applicationFullPath))
                    throw new ArgumentException("applicationFullPath is not an absolute path");
                if (!File.Exists(applicationFullPath))
                    throw new FileNotFoundException("File does not exist.", applicationFullPath);
                // State checking
                if (!IsFirewallInstalled)
                    throw new FirewallHelperException("Cannot remove authorization: Firewall is not installed.");

                #endregion

                // Locate Authorization for this application
                foreach (string appName in GetAuthorizedAppPaths())
                {
                    // Paths on windows file systems are not case sensitive.
                    if (appName.ToLower() == applicationFullPath.ToLower())
                        return true;
                }

                // Failed to locate the given app.
                return false;

            }

            /// 

            /// Retrieves a collection of paths to applications that are authorized.
            /// 

            /// 
            /// 
            ///         If the Firewall is not installed.
            ///   
            public ICollection GetAuthorizedAppPaths()
            {
                // State checking
                if (!IsFirewallInstalled)
                    throw new FirewallHelperException("Cannot remove authorization: Firewall is not installed.");

                ArrayList list = new ArrayList();
                //  Collect the paths of all authorized applications
                foreach (INetFwAuthorizedApplication app in fwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications)
                    list.Add(app.ProcessImageFileName);

                return list;
            }
            #endregion
        }

        /// 

        /// Describes a FirewallHelperException.
        /// 

        /// 
        ///
        /// 
        public class FirewallHelperException : System.Exception
        {
            /// 

            /// Construct a new FirewallHel
            /// 
            public FirewallHelperException(string message)
                : base(message)
            { }
        }

    }
    */
}