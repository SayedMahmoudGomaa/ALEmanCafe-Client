using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseNS
{
    public class MouseStuff
    {

        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const int MOUSEEVENTF_LEFTDOWN = 2;
        public const int MOUSEEVENTF_LEFTUP = 4;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_MOVE = 1;
        public const int MOUSEEVENTF_RIGHTDOWN = 8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern long GetMessageExtraInfo();
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetCursorPos(int x, int y);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public long x;
            public long y;
        }
        public void LClick()
        {
            MouseStuff.mouse_event(6L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
            MouseStuff.mouse_event(4L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void LClickTest()
        {
            MouseStuff.mouse_event(6L, 2, 2, 2, MouseStuff.GetMessageExtraInfo());
            MouseStuff.mouse_event(4L, 2, 2, 2, MouseStuff.GetMessageExtraInfo());
        }

        public void LDown()
        {
            MouseStuff.mouse_event(6L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void LUp()
        {
            MouseStuff.mouse_event(4L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void MClick()
        {
            MouseStuff.mouse_event(0x60L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
            MouseStuff.mouse_event(0x40L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void MDown()
        {
            MouseStuff.mouse_event(0x60L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public bool Move(int X, int Y)
        {
            Point pt = new Point(1, 1);
            Rectangle bounds = Screen.GetBounds(pt);
            if ((X <= bounds.Width) & (Y <= bounds.Height))
            {
                MouseStuff.SetCursorPos(X, Y);
                return true;
            }
            return false;
        }

        public void MUp()
        {
            MouseStuff.mouse_event(0x40L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void RClick()
        {
            MouseStuff.mouse_event(0x18L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
            MouseStuff.mouse_event(0x10L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void RDown()
        {
            MouseStuff.mouse_event(0x18L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        public void RUp()
        {
            MouseStuff.mouse_event(0x10L, 0L, 0L, 0L, MouseStuff.GetMessageExtraInfo());
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
        }
    }
}

