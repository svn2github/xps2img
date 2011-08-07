using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Xps2ImgUI.Properties;

namespace Xps2ImgUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            Application.Run(new MainForm { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) });
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void HandleException(Exception ex)
        {
            var exceptionMessage = (ex != null) ? ex.Message : Resources.NoExceptionMessage;
            MessageBox.Show(Application.OpenForms.Cast<IWin32Window>().FirstOrDefault(), exceptionMessage, Resources.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
