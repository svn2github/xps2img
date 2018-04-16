using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Xps2ImgUI.Utils.UI
{
    public static class Explorer
    {
        public static void Browse(string pathOrFile)
        {
            SelectInOpened(pathOrFile);
        }

        public static void ShellExecute(string command, bool useShellExecute = true, string arguments = null, string verb = null)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = command,
                UseShellExecute = useShellExecute || !String.IsNullOrEmpty(verb),
                Verb = verb,
                Arguments = arguments ?? String.Empty
            });
        }

        [DllImport("shell32.dll")]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

        [DllImport("shell32.dll")]
        private static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, out IntPtr pidl, uint sfgaoIn, out uint psfgaoOut);

        private static void SelectInOpened(string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);

            SHParseDisplayName(folderPath, IntPtr.Zero, out var folder, 0, out uint _);

            if (folder == IntPtr.Zero)
            {
                return;
            }

            SHParseDisplayName(filePath, IntPtr.Zero, out var file, 0, out _);

            if (file != IntPtr.Zero)
            {
                IntPtr[] files = { file };

                SHOpenFolderAndSelectItems(folder, (uint)files.Length, files, 0);
                Marshal.FreeCoTaskMem(file);
            }

            Marshal.FreeCoTaskMem(folder);
        }
    }
}
