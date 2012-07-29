using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using CommandLine;

using Windows7.Dialogs;

using Xps2Img.CommandLine;

using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

#if !DEBUG
using Xps2ImgUI.Converters;
#endif

namespace Xps2ImgUI
{
    static class Program
    {
        public const string ProductName = "xps2img";

        public const string HelpFile = ProductName + ".chm";

        public const string HelpTopicPreferences = "1000";
        public const string HelpTopicHistory     = "1100";

        public static readonly string Xps2ImgExecutable   = Path.Combine(AssemblyInfo.ApplicationFolder, ProductName + ".exe");
        public static readonly string Xps2ImgUIExecutable = Path.Combine(AssemblyInfo.ApplicationFolder, ProductName + "ui.exe");

        [STAThread]
        static void Main(string[] args)
        {
            SetupGuard.Enter();

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
                #if !DEBUG
                SystemManagement.Shutdown(PostActionToShutdownType(mainForm.Model.ShutdownType));
                #endif
            }
        }

        #if !DEBUG
        private static ShutdownType PostActionToShutdownType(string shutdownType)
        {
            switch(shutdownType)
            {
                case PostActionConverter.Exit:      return ShutdownType.Exit;
                case PostActionConverter.Hibernate: return ShutdownType.ForcedHibernate;
                case PostActionConverter.LogOff:    return ShutdownType.ForcedLogOff;
                case PostActionConverter.Reboot:    return ShutdownType.ForcedReboot;
                case PostActionConverter.Shutdown:  return ShutdownType.ForcedShutdown;
                case PostActionConverter.Sleep:     return ShutdownType.ForcedSleep;
            }
            throw new InvalidOperationException();
        }
        #endif

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private static void HandleException(Exception ex)
        {
            var exceptionMessage = ex == null
                                    ? Resources.Strings.NoExceptionDetailsPresent
                                    : ex
                                    #if DEBUG
                                    .ToString();
                                    #else
                                    .Message;
                                    #endif

            // ReSharper disable EmptyGeneralCatchClause

            using (new ModalGuard())
            {
                var form = Application.OpenForms.Cast<IWin32Window>().FirstOrDefault();

                var handle = IntPtr.Zero;
                try
                {
                    handle = GetActiveWindow();
                    if (form != null)
                    {
                        handle = form.Handle;
                    }
                }
                catch
                {
                }

                var dialogResult = TaskDialogUtils.NotSupported;
                try
                {
                    dialogResult = TaskDialogUtils.Show(handle,
                        Resources.Strings.WindowTitle,
                        Resources.Strings.UnexpectedErrorOccured,
                        (ex != null ? ex.Message : exceptionMessage).AppendDot(),
                        TaskDialogStandardIcon.Error,
                        t => 
                        {
                            MainForm.AddExceptionDetails(t, ex);
                            t.StandardButtons = TaskDialogStandardButtons.Close;
                        });
                }
                catch
                {
                }

                if(dialogResult == TaskDialogUtils.NotSupported)
                {
                    MessageBox.Show(form, exceptionMessage.AppendDot(), Resources.Strings.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
