﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

// ReSharper disable InconsistentNaming

namespace Xps2ImgUI.Utils.UI
{
    [SuppressUnmanagedCodeSecurity]
    public static class Win32Utils
    {
        public static void Restore(this Form form)
        {
            if (!form.IsDisposed)
            {
                ShowWindow(form.Handle, SW_RESTORE);
            }
        }

        public static bool IsForegroundWindow(this Form form)
        {
            if (form.IsDisposed)
            {
                return false;
            }

            return form.MdiChildren.Select(f => f.Handle)
                    .Union(form.OwnedForms.Select(f => f.Handle)
                    .Union(new[] { form.Handle }))
                    .Contains(GetForegroundWindow());
        }

        // http://pietschsoft.com/post/2009/01/26/CSharp-Flash-Window-in-Taskbar-via-Win32-FlashWindowEx.aspx
        public static bool Flash(this Form form)
        {
            return Flash(form, FLASHW_ALL | FLASHW_TIMERNOFG, UInt32.MaxValue);
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
            return Flash(form, UInt32.MaxValue);
        }

        public static bool StopFlashing(this Form form)
        {
            return Flash(form, FLASHW_STOP, UInt32.MaxValue);
        }

        public static void RemoveSystemMenuDisabledItems(this Form form)
        {
            if (form.IsDisposed)
            {
                return;
            }

            var hMenu = GetSystemMenu(form.Handle, false);
            if (hMenu == IntPtr.Zero)
            {
                return;
            }

            var menuItemInfo =  new MENUITEMINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                fMask = MIIM_STATE
            };

            for (var itemPosition = 0; itemPosition < GetMenuItemCount(hMenu);)
            {
                if (GetMenuItemInfo(hMenu, (uint)itemPosition, true, ref menuItemInfo) && (menuItemInfo.fState & MFS_DISABLED) != 0)
                {
                    RemoveMenu(hMenu, (uint)itemPosition, MF_BYPOSITION);
                    continue;
                }
                itemPosition++;
            }
        }

        public static void EnableSysClose(this Form form, bool enable)
        {
            EnableMenuItem(GetSystemMenu(form.Handle, false), SC_CLOSE, enable ? MF_ENABLED : MF_GRAYED);
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
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private const uint SW_RESTORE = 0x09;

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint msg);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        private const uint MF_BYPOSITION = 0x00000400;
        private const uint MF_ENABLED    = 0x00000000;
        private const uint MF_GRAYED     = 0x00000001;

        [DllImport("user32.dll")]
        private static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll")]
        private static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        private static extern bool GetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

        public const uint WM_APP = 0x8000U;

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, uint wEnable);

        private const uint MIIM_STATE   = 0x00000001;
        private const uint MFS_DISABLED = 0x00000003;

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [StructLayout(LayoutKind.Sequential)]
        private struct MENUITEMINFO
        {
            public uint cbSize;
            public uint fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData;
            public uint cch;
            public IntPtr hbmpItem;
        }

        private const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;

        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        public static bool AttachConsole()
        {
            return AttachConsole(ATTACH_PARENT_PROCESS);
        }

        public static void PostMessage(this IntPtr handle, uint msg, object data)
        {
            PostMessage(handle, msg, IntPtr.Zero, GCHandle.ToIntPtr(GCHandle.Alloc(data)));
        }

        public static T GetPostMessageData<T>(this Message msg)
        {
            var gcHandle = GCHandle.FromIntPtr(msg.LParam);
            var data = (T)gcHandle.Target;
            gcHandle.Free();
            return data;
        }
    }
}
