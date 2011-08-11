﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// ReSharper disable InconsistentNaming

namespace Xps2ImgUI.Utils.UI
{
    // http://pietschsoft.com/post/2009/01/26/CSharp-Flash-Window-in-Taskbar-via-Win32-FlashWindowEx.aspx
    public static class Win32Utils
    {
        public static void Restore(this Form form)
        {
            if (!form.IsDisposed)
            {
                ShowWindow(form.Handle, SW_RESTORE);
            }
        }

        public static bool Flash(this Form form)
        {
            return Flash(form, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue);
        }

        public static bool Flash(this Form form, uint count)
        {
            return Flash(form, FLASHW_ALL, count);
        }

        public static bool Flash(this Form form, uint flags, uint timeout)
        {
            if (form.IsDisposed)
            {
                return false;
            }
            var fi = Create_FLASHWINFO(form.Handle, flags, timeout, 0);
            return FlashWindowEx(ref fi);
        }

        public static bool StartFlashing(this Form form)
        {
            return Flash(form, uint.MaxValue);
        }

        public static bool StopFlashing(this Form form)
        {
            return Flash(form, FLASHW_STOP, uint.MaxValue);
        }

        /// <summary>
        /// Stop flashing. The system restores the window to its original state.
        /// </summary>
        public const uint FLASHW_STOP = 0;

        /// <summary>
        /// Flash the window caption.
        /// </summary>
        public const uint FLASHW_CAPTION = 0x00000001;

        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        public const uint FLASHW_TRAY = 0x00000002;

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        public const uint FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY;

        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        public const uint FLASHW_TIMER = 0x00000004;

        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        public const uint FLASHW_TIMERNOFG = 0x0000000C;

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint uCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;
        }

        private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            var fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private const uint SW_RESTORE = 0x09;

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint msg);
    }
}