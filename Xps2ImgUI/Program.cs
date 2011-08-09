﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Xps2ImgUI.Model;

namespace Xps2ImgUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleException(e.ExceptionObject as Exception);
            Application.ThreadException += (sender1, e1) => HandleException(e1.Exception);

            Application.Run(new MainForm(new Xps2ImgModel()) { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) });
        }

        private static void HandleException(Exception ex)
        {
            var exceptionMessage = (ex != null) ? ex.Message : Resources.Strings.NoExceptionMessage;
            MessageBox.Show(Application.OpenForms.Cast<IWin32Window>().FirstOrDefault(), exceptionMessage, Resources.Strings.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
