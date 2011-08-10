using System;
using System.Diagnostics;
using System.IO;

namespace Xps2ImgUI.Utils.UI
{
    public static class Explorer
    {
        public static void Browse(string pathOrFile)
        {
            Execute("root,select", pathOrFile);
        }

        public static void Select(string path, string file)
        {
            Select(Path.Combine(path, file));
        }

        public static void Select(string pathOrFile)
        {
            if (File.Exists(pathOrFile))
            {
                Execute("select", pathOrFile);
            }
            else
            {
                Browse(Path.GetDirectoryName(pathOrFile));
            }
        }

        private static void Execute(string command, string pathOrFile)
        {
            try
            {
                Process.Start("explorer.exe", String.Format("/{0},{1}", command, pathOrFile));
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }
}
