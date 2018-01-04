using System;
using System.Runtime.InteropServices;

namespace Xps2Img.Utils
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Local
    public static class Win32
    {
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate. 
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine HandlerRoutine, bool Add);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCP(int codePageId);

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT        = 0,
            CTRL_BREAK_EVENT    = 1,
            CTRL_CLOSE_EVENT    = 2,
            CTRL_LOGOFF_EVENT   = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private enum FileType
        {
            FILE_TYPE_UNKNOWN = 0x0000,
            FILE_TYPE_DISK	  = 0x0001,
            FILE_TYPE_CHAR    = 0x0002,
            FILE_TYPE_PIPE    = 0x0003,
            FILE_TYPE_REMOTE  = 0x8000
        }

        private enum StdHandle
        {
            STD_INPUT_HANDLE  = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE  = -12
        }

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle stdHandle);

        [DllImport("Kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hFile);

        public static bool IsOutputRedirected()
        {
            var hOutput = GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);
            return hOutput == IntPtr.Zero || GetFileType(hOutput) != FileType.FILE_TYPE_CHAR;
        }
    }
}
