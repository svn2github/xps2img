using System;
using System.Drawing;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public partial class PropertyGridEx
    {
        // ReSharper disable InconsistentNaming
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_CONTEXTMENU   = 0x007B;
        private const int WM_KEYDOWN       = 0x0100;

        private const int VK_DELETE        = 0x2E;
        // ReSharper enable InconsistentNaming

        private static Point GetPoint(IntPtr lParam)
        {
            return new Point(GetInt(lParam));
        }

        private static int GetInt(IntPtr ptr)
        {
            return IntPtr.Size == 8 ? unchecked((int)ptr.ToInt64()) : ptr.ToInt32();
        }
    }
}
