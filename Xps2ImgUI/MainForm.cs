using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Localization;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Localization;
using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2ImgUI
{
    public partial class MainForm : Form, IFormLocalization, ISettings
    {
        public MainForm()
        {
            InitializeComponent();

            Model = new Xps2ImgModel();

            _resumeToolStripMenuItemPosition = convertContextMenuStrip.Items.OfType<ToolStripMenuItem>().ToList().IndexOf(resumeToolStripMenuItem);

            _updateManager.CheckCompleted += (s, e) => this.InvokeIfNeeded(() => RegisterIdleHandler(UpdateCheckCompleted));
            _updateManager.InstallationLaunched += (s, e) => this.InvokeIfNeeded(() => RegisterIdleHandler(UpdateInstallationLaunched));

            settingsPropertyGrid.ResetGroupCallback = PropertyGridResetGroupCallback;

            this.EnableFormLocalization();
        }

        private bool PropertyGridResetGroupCallback(string label, bool check)
        {
            if (check)
            {
                return settingsPropertyGrid.IsResetByCategoryEnabled(label);
            }

            ResetByCategory(label);

            return true;
        }

        private new void Activate()
        {
            if (Model.IsBatchMode)
            {
                return;
            }

            if (WindowState == FormWindowState.Minimized)
            {
                this.Restore();
            }
            else
            {
                base.Activate();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            _handle = Handle;

            RegisterCultureSpecificConvertButtonSize();

            convertButton.ContextMenuStrip = convertContextMenuStrip;

            var isCommandLineVisible = IsCommandLineVisible;
            IsCommandLineVisible = false;
            IsCommandLineVisible = isCommandLineVisible;

            AdjustPropertyGrid();

            ApplyPreferences();
            
            FocusFirstRequiredOption(false);

            base.OnLoad(e);

            BeginInvoke((MethodInvoker)(() =>
            {
                LocalizationManager.SetUICulture((int) _preferences.ApplicationLanguage);
                CheckForUpdates(true);
            }));
        }

        private void RegisterCultureSpecificConvertButtonSize()
        {
            var minimumConvertButtonSize = CultureSpecificConvertButtonSize;
            if (minimumConvertButtonSize.IsEmpty)
            {
                CultureSpecificConvertButtonSize = convertButton.Size;
            }
        }

        private Size CultureSpecificConvertButtonSize
        {
            get
            {
                Size size, invariantSize;
                _cultureSpecificConvertButtonSize.TryGetValue(LocalizationManager.CurrentUICulture.Name, out size);
                _cultureSpecificConvertButtonSize.TryGetValue(LocalizationManager.DefaultUICulture.Name, out invariantSize);
                return !size.IsEmpty && invariantSize.Width > size.Width ? invariantSize : size;
            }
            set
            {
                _cultureSpecificConvertButtonSize[LocalizationManager.CurrentUICulture.Name] = value;
            }
        }

        public void UICultureChanged()
        {
            convertButton.MinimumSize = Size.Empty;
            convertButton.Text = Resources.Strings.Launch;

            RegisterCultureSpecificConvertButtonSize();

            convertButton.MinimumSize = CultureSpecificConvertButtonSize;

            settingsPropertyGrid.RefreshLocalization();

            UpdateElapsedTime();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (MinimumSize.IsEmpty)
            {
                MinimumSize = Size;
            }

            SizeGripStyle = WindowState == FormWindowState.Maximized ? SizeGripStyle.Hide : SizeGripStyle.Show;

            base.OnSizeChanged(e);
        }

        private bool _minimizedChecked;

        protected override void OnActivated(EventArgs e)
        {
            if (WindowState != FormWindowState.Normal && !_minimizedChecked)
            {
                settingsPropertyGrid.UpdateLayout();
                _minimizedChecked = true;
            }
            this.StopFlashing();
            base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Model.IsRunning && !Model.IsStopPending)
            {
                Activate();

                e.Cancel = _preferences.ConfirmOnExit && !ShowConfirmationMessageBox(Resources.Strings.ClosingConfirmation);

                if (!e.Cancel)
                {
                    Model.Cancel();
                    RegisterIdleHandler(CloseFormHandler, true);
                    e.Cancel = true;
                }
            }

            if (!_autoClosed)
            {
                Model.CancelShutdownRequest();
            }

            base.OnClosing(e);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            hevent.Handled = true;
            ShowHelp();
        }

        protected override void OnShown(EventArgs e)
        {
            if (Model.IsBatchMode)
            {
                RegisterIdleHandler(BatchLaunchIdle);
            }
            base.OnShown(e);
        }

        private const uint WmProgress = Win32Utils.WM_APP + 1;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Windows7Taskbar.WM_TaskbarButtonCreated)
            {
                _thumbButtonManager = new ThumbButtonManager(Handle);
                _thumbButton = _thumbButtonManager.CreateThumbButton(Resources.Icons.Play, ConvertButtonCleanText, (s, e) => ExecuteConversion(ExecuteFlags.Convert | ExecuteFlags.ActivateWindow));
                _thumbButtonManager.AddThumbButtons(_thumbButton);
            }

            if (_thumbButtonManager != null)
            {
                _thumbButtonManager.DispatchMessage(ref m);
            }

            if (m.Msg == WmProgress)
            {
                var e = m.GetPostMessageData<ConversionProgressEventArgs>();
                UpdateProgress(e.Percent, e.Pages, e.File);
                return;
            }

            try
            {
                base.WndProc(ref m);
            }
            catch(NullReferenceException)
            {
                // Weird error under Vista+ when pressing F1 in editors of file open dialog.
            }
        }

        private bool IsShutdownConfirmed()
        {
            using (new ModalGuard())
            {
                var action = new PostActionTypeConverter().ConvertToInvariantString(Model.ShutdownType);

                using (var confirmForm = new CountdownForm(action, _preferences.WaitForShutdownSeconds)
                {
                    OKText = String.Format(Resources.Strings.DoItNowFormat, action),
                    ConfirmChecked = !_preferences.ConfirmOnAfterConversion
                })
                {
                    // ReSharper disable AccessToDisposedClosure
                    using (new DisposableActions(() => confirmForm.TopMost = true, () => confirmForm.TopMost = false))
                    // ReSharper restore AccessToDisposedClosure
                    {
                        if (confirmForm.ShowDialog(this) != DialogResult.OK)
                        {
                            Model.CancelShutdownRequest();
                            return false;
                        }

                        _preferences.ConfirmOnAfterConversion = !confirmForm.ConfirmChecked;
                    }
                }
            }

            return true;
        }

        private void FlashForm()
        {
            if (Model.IsUserMode && _preferences.FlashWhenCompleted && !this.IsForegroundWindow())
            {
                this.Flash(4);
            }
        }

        private bool FocusFirstRequiredOption(bool showMessage = true)
        {
            var firstRequiredPropertyName = Model.FirstRequiredPropertyName;

            if (String.IsNullOrEmpty(firstRequiredPropertyName))
            {
                return false;
            }

            var firstRequiredPropertyDescription = Xps2Img.Shared.Resources.Strings.ResourceManager.GetString(DefaultLocalizablePropertyDescriptorStrategy.Instance.GetDisplayNameId(typeof(Options), firstRequiredPropertyName));

            if (showMessage)
            {
                Activate();

                ShowErrorMessageBox(String.Format(Resources.Strings.SpecifyValueFormat, firstRequiredPropertyDescription), null, false, firstRequiredPropertyDescription, Resources.Strings.ParameterIsRequired, Resources.Strings.EditParameter);

                Model.ExitCode = ReturnCode.NoArgs;

                if (Model.IsBatchMode)
                {
                    return true;
                }
            }

            settingsPropertyGrid.SelectGridItem(firstRequiredPropertyName, true, focus: true);

            return true;
        }

        private static string GetDragFile(IDataObject dataObject)
        {
            if(!dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                return String.Empty;
            }

            var files =  dataObject.GetData(DataFormats.FileDrop) as string[];

            var file = files != null && files.Length == 1 ? files[0] : null;

            return file != null && ((File.GetAttributes(file) & FileAttributes.Directory) == 0) ? file : null;
        }

        private string ConvertButtonCleanText
        {
            get { return convertButton.Text.Replace("&", String.Empty); }
        }

        private bool IsCommandLineVisible
        {
            get { return !settingsSplitContainer.Panel2Collapsed; }
            set
            {
                if (settingsSplitContainer.Panel2Collapsed != value)
                {
                    return;
                }

                settingsSplitContainer.Panel2Collapsed = !value;
                if (WindowState == FormWindowState.Normal)
                {
                    Height += (commandLineTextBox.Height + settingsSplitContainer.SplitterWidth)*(value ? 1 : -1);
                }
            }
        }

        private Xps2ImgModel _model;

        public Xps2ImgModel Model
        {
            get { return _model; }
            set
            {
                if (value != null && value != _model)
                {
                    SetModel(value);
                }
            }
        }

        private void SetModel(Xps2ImgModel model)
        {
            if (_model != null)
            {
                _model.OutputDataReceived -= OutputDataReceived;
                _model.ErrorDataReceived -= ErrorDataReceived;
                _model.Completed -= Completed;
                _model.LaunchFailed -= LaunchFailed;
                _model.LaunchSucceeded -= LaunchSucceeded;
                _model.OptionsObjectChanged -= OptionsObjectChanged;
            }

            _model = model;

            _model.OutputDataReceived += OutputDataReceived;
            _model.ErrorDataReceived += ErrorDataReceived;
            _model.Completed += Completed;
            _model.LaunchFailed += LaunchFailed;
            _model.LaunchSucceeded += LaunchSucceeded;
            _model.OptionsObjectChanged += OptionsObjectChanged;

            _model.Init();
        }

        private readonly IUpdateManager _updateManager = UpdateManager.Create();
        private readonly int _resumeToolStripMenuItemPosition;

        private readonly Dictionary<string, Size> _cultureSpecificConvertButtonSize = new Dictionary<string, Size>();

        private IntPtr _handle;
    }
}
