using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using CommandLine;

using Windows7.Dialogs;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Diagnostics;
using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Setup;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.System;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Controls.PropertyGridEx;
using Xps2ImgUI.Localization;
using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2ImgUI
{
    static class Program
    {
        public const string ProductName = "xps2img";
        
        public static readonly string Xps2ImgExecutable   = Path.Combine(AssemblyInfo.ApplicationFolder, ProductName + ".exe");
        public static readonly string Xps2ImgUIExecutable = Path.Combine(AssemblyInfo.ApplicationFolder, ProductName + "ui.exe");

        private static bool _isBatchMode;

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                MainExec(args);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                Environment.Exit(ReturnCode.Failed);
            }
        }

        private static void MainExec(string[] args)
        {
            DebugValidation();

            SetupGuard.Enter();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleException(e.ExceptionObject as Exception);
            Application.ThreadException += (sender, e) => HandleException(e.Exception);

            int workerThreads, completionPortThreads;

            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMinThreads(workerThreads + Environment.ProcessorCount + 2, completionPortThreads);

            LocalizationManager.SetUICulture((int)Preferences.Localizations.English);
            
            PropertyGridEx.ResourceManager = Resources.Strings.ResourceManager;

            LocalizableTypeDescriptionProviderInstaller.AddProvider<Preferences>(typeof(Resources.Strings));
            LocalizableTypeDescriptionProviderInstaller.AddProvider<UIOptions>(typeof(Xps2Img.Shared.Resources.Strings));

            Parser.RegisterStringsSource<Xps2Img.Shared.Resources.Strings>();

            var options = Parser.IsUsageRequested(args) ? null : Parser.Parse<UIOptions>(args, true);

            var mainForm = new MainForm { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };

            SettingsManager.DeserializeSettings(mainForm);

            if (options != null)
            {
                mainForm.Model = SettingsManager.IsSettingFile(options.SrcFile)
                                    ? SettingsManager.LoadSettings(options.SrcFile)
                                    : new Xps2ImgModel(options);

                _isBatchMode = options.Batch;

                if (_isBatchMode)
                {
                    mainForm.WindowState = FormWindowState.Minimized;
                }
            }

            Application.Run(mainForm);

            var model = mainForm.Model;

            if (!model.IsBatchMode)
            {
                SettingsManager.SerializeSettings(mainForm);
            }

            Environment.ExitCode = model.ExitCode;

            if (model.ShutdownRequested)
            {
                SystemManagementUtils.Shutdown(model.ShutdownType);
            }
            #if DEBUG
            else
            {
                Debug.WriteLine("NO SHUTDOWN PENDING");
            }
            #endif
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private static void HandleException(Exception ex)
        {
            var exceptionMessage = ex == null
                                    ? Resources.Strings.NoExceptionDetailsPresent
                                    : ex
                                    #if USE_SHUTDOWN
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
                    if (_isBatchMode)
                    {
                        return;
                    }

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

        [Conditional("DEBUG")]
        private static void DebugValidation()
        {
            ValidateProperties.For<Preferences>(typeof(Preferences.Properties));
            ValidateProperties.For<Options>(typeof(Options.Properties));

            Debug.Assert(Enum.GetValues(typeof(Preferences.CheckInterval)).Length == 3, "Update fields for CheckInterval enum!");
            Debug.Assert(Enum.GetValues(typeof(Preferences.Localizations)).Length == 2, "Update fields for Localizations enum!");
        }
    }
}
