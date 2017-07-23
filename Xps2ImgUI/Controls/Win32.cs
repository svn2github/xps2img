using System;
using System.Drawing;

namespace Xps2ImgUI.Controls
{
    public static class Win32
    {
        // ReSharper disable InconsistentNaming
        public static class Messages
        {
            public const int WM_LBUTTONDBLCLK = 0x0203;
            public const int WM_CONTEXTMENU = 0x007B;
            public const int WM_KEYDOWN = 0x0100;
        }

        public static class Keys
        {
            public const int VK_DELETE = 0x2E;
        }
        // ReSharper enable InconsistentNaming

        public static Point GetPoint(IntPtr lParam)
        {
            return new Point(GetInt(lParam));
        }

        public static int GetInt(IntPtr ptr)
        {
            return IntPtr.Size == 8 ? unchecked((int)ptr.ToInt64()) : ptr.ToInt32();
        }
    }
}
