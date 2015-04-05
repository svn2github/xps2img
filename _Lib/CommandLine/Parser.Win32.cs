using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CommandLine
{
    [SuppressUnmanagedCodeSecurity]
    public static partial class Parser
    {
        // http://www.pinvoke.net/default.aspx/shell32/commandlinetoargvw.html

        // Here's an wrapper to CommandLineToArgvW I found useful (csells@sellsbrothers.com)
        public static string[] CommandLineToArgv()
        {
            return CommandLineToArgv(Environment.CommandLine);
        }

        public static string[] CommandLineToArgv(string cmdline)
        {
            if (String.IsNullOrEmpty(cmdline))
            {
                return new string[0];
            }

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
                    var unicodeByteCount = Encoding.Unicode.GetByteCount(arg) + Encoding.Unicode.GetByteCount(new[] { Char.MinValue });
                    argPtr = new IntPtr(argPtr.ToInt32() + unicodeByteCount);
                }
            }
            catch
            {
                return args.ToArray();
            }
            finally
            {
                LocalFree(argvPtr);
            }

            return args.ToArray();
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);
    }
}
