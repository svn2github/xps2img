using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

using Windows7.DesktopIntegration.Interop;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Windows7.DesktopIntegration
{
    [SuppressUnmanagedCodeSecurity]
    public static class Windows7Taskbar
    {
        #region Infrastructure

        public static ITaskbarList3 TaskbarList
        {
            get
            {
                if (_taskbarList == null)
                {
                    lock (_syncRoot)
                    {
                        if (_taskbarList == null)
                        {
                            var taskbarList = new TaskbarList() as ITaskbarList3;
                            var supported = taskbarList != null;

                            taskbarList = taskbarList ?? new TaskbarListStub();
                            taskbarList.HrInit();

                            _taskbarList = taskbarList;
                            _supported = supported;
                        }
                    }
                }
                return _taskbarList;
            }
        }

        public static uint WM_TaskbarButtonCreated
        {
            get
            {
                if (_uTBBCMsg == 0)
                {
                    _uTBBCMsg = RegisterWindowMessage("TaskbarButtonCreated");

                    try
                    {
                        ChangeWindowMessageFilter(_uTBBCMsg, MSGFLT_ADD);
                        ChangeWindowMessageFilter(WM_COMMAND, MSGFLT_ADD);
                    }
                    catch(EntryPointNotFoundException)
                    {
                    }
                }
                return _uTBBCMsg;
            }
        }

        public static volatile bool _supported;
        public static bool Supported
        {
            get { return _supported; }
        }

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern bool ChangeWindowMessageFilter(uint message, uint dwFlag);

        private const uint MSGFLT_ADD = 1;
        private const int  WM_COMMAND = 0x111;

        private static readonly object _syncRoot = new Object();
        private static volatile uint _uTBBCMsg;

        private static volatile ITaskbarList3 _taskbarList;

        #endregion

        #region Taskbar Progress Bar

        /// <summary>
        /// Represents the thumbnail progress bar state.
        /// </summary>
        public enum ThumbnailProgressState
        {
            /// <summary>
            /// No progress is displayed.
            /// </summary>
            NoProgress = 0,
            /// <summary>
            /// The progress is indeterminate (marquee).
            /// </summary>
            Indeterminate = 0x1,
            /// <summary>
            /// Normal progress is displayed.
            /// </summary>
            Normal = 0x2,
            /// <summary>
            /// An error occurred (red).
            /// </summary>
            Error = 0x4,
            /// <summary>
            /// The operation is paused (yellow).
            /// </summary>
            Paused = 0x8
        }

        public static void SetProgressState(IntPtr hwnd, ThumbnailProgressState state)
        {
            TaskbarList.SetProgressState(hwnd, (TBPFLAG)state);
        }

        public static void SetProgressState(this Form form, ThumbnailProgressState state)
        {
            if (form.IsDisposed)
            {
                return;
            }

            SetProgressState(form.Handle, state);
        }

        public static void SetProgressValue(IntPtr hwnd, int current, int maximum)
        {
            TaskbarList.SetProgressValue(hwnd, (ulong)current, (ulong)maximum);
        }

        public static void SetProgressValue(this Form form, int current, int maximum)
        {
            if (form.IsDisposed)
            {
                return;
            }

            SetProgressValue(form.Handle, current, maximum);
        }

        #endregion
    }
}
