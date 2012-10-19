﻿using System;
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
            if (File.Exists(pathOrFile) || Directory.Exists(pathOrFile))
            {
                Execute("select", pathOrFile);
            }
            else
            {
                try
                {
                    Browse(Path.GetDirectoryName(pathOrFile));
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }
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

        private static void Execute(string command, string pathOrFile)
        {
            Process.Start("explorer.exe", String.Format("/{0},{1}", command, pathOrFile));
        }
    }
}
