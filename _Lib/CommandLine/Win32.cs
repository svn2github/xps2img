using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CommandLine
{
    public static class Win32
    {
        // http://www.pinvoke.net/default.aspx/shell32/commandlinetoargvw.html

        [DllImport("shell32.dll")]
        private static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);

        // Here's an wrapper to CommandLineToArgvW I found useful (csells@sellsbrothers.com)
        public static List<string> CommandLineToExeAndArgs()
        {
            return CommandLineToExeAndArgs(Environment.CommandLine);
        }

        public static List<string> CommandLineToExeAndArgs(string cmdline)
        {
            var args = new List<string>();
            var argvPtr = IntPtr.Zero;

            try
            {
                int argc;
                argvPtr = CommandLineToArgvW(cmdline, out argc);

                // argvPtr is a pointer to a pointer; dereference it
                var argPtr = Marshal.ReadIntPtr(argvPtr);

                // CommandLineToArgvW will list the executable as argv[0]
                for (var i = 0; i < argc; i++)
                {
                    var arg = Marshal.PtrToStringUni(argPtr);
                    if (arg == null)
                    {
                        continue;
                    }

                    args.Add(arg);

                    // Increment the pointer address by the number of Unicode bytes
                    // plus one Unicode character for the string's null terminator
                    var unicodeByteCount = Encoding.Unicode.GetByteCount(arg) + Encoding.Unicode.GetByteCount(new [] { Char.MinValue });
                    argPtr = new IntPtr(argPtr.ToInt32() + unicodeByteCount);
                }
            }
            finally
            {
                LocalFree(argvPtr);
            }

            return args;
        }
    }
}
