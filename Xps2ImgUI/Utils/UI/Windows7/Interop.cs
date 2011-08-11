using System;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Windows7.DesktopIntegration.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [Flags]
    public enum TBPFLAG
    {
        TBPF_NOPROGRESS = 0,
        TBPF_INDETERMINATE = 0x1,
        TBPF_NORMAL = 0x2,
        TBPF_ERROR = 0x4,
        TBPF_PAUSED = 0x8
    }

    [Flags]
    public enum TBATFLAG
    {
        TBATF_USEMDITHUMBNAIL = 0x1,
        TBATF_USEMDILIVEPREVIEW = 0x2
    }

    [Flags]
    public enum THBMASK
    {
        THB_BITMAP = 0x1,
        THB_ICON = 0x2,
        THB_TOOLTIP = 0x4,
        THB_FLAGS = 0x8
    }

    [Flags]
    public enum THBFLAGS
    {
        THBF_ENABLED = 0,
        THBF_DISABLED = 0x1,
        THBF_DISMISSONCLICK = 0x2,
        THBF_NOBACKGROUND = 0x4,
        THBF_HIDDEN = 0x8
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct THUMBBUTTON
    {
        [MarshalAs(UnmanagedType.U4)]
        public THBMASK dwMask;
        public uint iId;
        public uint iBitmap;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szTip;
        [MarshalAs(UnmanagedType.U4)]
        public THBFLAGS dwFlags;
    }
}