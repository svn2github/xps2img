using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CommandLine;

using Xps2Img.CommandLine;

using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;

namespace Xps2ImgUI
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleException(e.ExceptionObject as Exception);
            Application.ThreadException += (sender, e) => HandleException(e.Exception);

            var options = Parser.IsUsageRequiested(args) ? null : Parser.Parse<Options>(args, true);

            var mainForm =  new MainForm { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };

            SettingsManager.DeserializeSettings(mainForm);

            if (options != null)
            {
                mainForm.SetModel(SettingsManager.IsSettingFile(options.SrcFile)
                                    ? SettingsManager.LoadSettings(options.SrcFile)
                                    : new Xps2ImgModel(options));
            }

            Application.Run(mainForm);

            SettingsManager.SerializeSettings(mainForm);
        }

        private static void HandleException(Exception ex)
        {
            var exceptionMessage = (ex == null) ? Resources.Strings.NoExceptionMessage : ex
                                                                                         #if DEBUG
                                                                                         .ToString();
                                                                                         #else
                                                                                         .Message;
                                                                                         #endif

            MessageBox.Show(Application.OpenForms.Cast<IWin32Window>().FirstOrDefault(), exceptionMessage, Resources.Strings.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
