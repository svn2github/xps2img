using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using CommandLine;

using Xps2Img.CommandLine;

using Xps2ImgUI.Model;
using Xps2ImgUI.Resources;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    static class Program
    {
        public const string ProductName = "xps2img";

        public const string HelpFile = ProductName + ".chm";

        public const string HelpTopicPreferences = "1000";

        public static readonly string ApplicationFolder   = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static readonly string Xps2ImgExecutable   = Path.Combine(ApplicationFolder, ProductName + ".exe");
        public static readonly string Xps2ImgUIExecutable = Path.Combine(ApplicationFolder, ProductName + "ui.exe");

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleException(e.ExceptionObject as Exception);
            Application.ThreadException += (sender, e) => HandleException(e.Exception);

            int workerThreads, completionPortThreads;

            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMinThreads(workerThreads*10/4, completionPortThreads);

            var options = Parser.IsUsageRequiested(args) ? null : Parser.Parse<Options>(args, true);

            var mainForm =  new MainForm { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };

            SettingsManager.DeserializeSettings(mainForm);

            if (options != null)
            {
                mainForm.Model = SettingsManager.IsSettingFile(options.SrcFile)
                                    ? SettingsManager.LoadSettings(options.SrcFile)
                                    : new Xps2ImgModel(options);
            }

            Application.Run(mainForm);

            SettingsManager.SerializeSettings(mainForm);

            if (mainForm.Model.ShutdownRequested)
            {
                SystemManagement.Shutdown(ShutdownType.ForcedShutdown);
            }
        }

        private static void HandleException(Exception ex)
        {
            var exceptionMessage = (ex == null) ? Strings.NoExceptionMessage : ex
                                                                                         #if DEBUG
                                                                                         .ToString();
                                                                                         #else
                                                                                         .Message;
                                                                                         #endif

            using (new ModalGuard())
            {
                MessageBox.Show(Application.OpenForms.Cast<IWin32Window>().FirstOrDefault(), exceptionMessage, Strings.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
