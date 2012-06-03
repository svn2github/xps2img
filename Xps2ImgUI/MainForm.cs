using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2Img.CommandLine;

using Xps2ImgUI.Controls;
using Xps2ImgUI.Controls.PropertyGridEx;
using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm : Form, ISettings
    {
        public MainForm()
        {
            InitializeComponent();

            Model = new Xps2ImgModel();

            _resumeToolStripMenuItemPosition = convertContextMenuStrip.Items.OfType<ToolStripMenuItem>().ToList().IndexOf(resumeToolStripMenuItem);

            settingsPropertyGrid.EditAutoCompletes = new[]
            {
                new PropertyGridEx.EditAutoComplete(Options.XPSFileDisplayName, AutoCompleteSource.FileSystem),
                new PropertyGridEx.EditAutoComplete(Options.OutputFolderDisplayName, AutoCompleteSource.FileSystemDirectories)
            };
        }

        private void OptionsObjectChanged(object sender, EventArgs e)
        {
            settingsPropertyGrid.SelectedObject = Model.OptionsObject;
            Refresh();
            UpdateCommandLine(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = Resources.Strings.WindowTitle;

            convertButton.Text = Resources.Strings.Launch;
            convertButton.ContextMenuStrip = convertContextMenuStrip;

            var isCommandLineVisible = IsCommandLineVisible;
            IsCommandLineVisible = false;
            MinimumSize = new Size(Size.Width, Size.Height);
            IsCommandLineVisible = isCommandLineVisible;

            AdjustPropertyGrid();

            ApplyPreferences();
            
            FocusFirstRequiredOption(null);

            base.OnLoad(e);
        }

        private void ApplyPreferences()
        {
            settingsPropertyGrid.ModernLook = !_preferences.ClassicLook;
            convertContextMenuStrip.RenderMode = _preferences.ClassicLook
                                                     ? ToolStripRenderMode.System
                                                     : ToolStripRenderMode.ManagerRenderMode;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            SizeGripStyle = WindowState == FormWindowState.Maximized ? SizeGripStyle.Hide : SizeGripStyle.Show;
            base.OnSizeChanged(e);
        }

        protected override void OnActivated(EventArgs e)
        {
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
                    Model.Stop();
                }
            }

            base.OnClosing(e);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            ShowHelp();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Windows7Taskbar.WM_TaskbarButtonCreated)
            {
                _thumbButtonManager = new ThumbButtonManager(Handle);
                _thumbButton = _thumbButtonManager.CreateThumbButton(Resources.Icons.Play, ConvertButtonCleanText, (s, e) => ExecuteConversion(ExecuteFlags.Convert | ExecuteFlags.ActivateWindow | ExecuteFlags.DoNotClickButton));
                _thumbButtonManager.AddThumbButtons(_thumbButton);
            }

            if (_thumbButtonManager != null)
            {
                _thumbButtonManager.DispatchMessage(ref m);
            }

            base.WndProc(ref m);
        }


        private void AdjustPropertyGrid()
        {
            settingsPropertyGrid.DragDrop += MainFormDragDrop;
            settingsPropertyGrid.DragEnter += MainFormDragEnter;

            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            Action<Action> modalAction = action => { using (new ModalGuard()) { action(); } };

            // Remove Property Pages button.
            settingsPropertyGrid.RemoveLastToolStripItem();

            // Preferences button.
            var preferencesToolStripSplitButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Preferences, PreferencesToolStripButtonClick);

            _shutdownWhenCompletedToolStripMenuItem = new ToolStripMenuItem(Resources.Strings.ShutdownWhenCompleted) { CheckOnClick = true };

            var autoSaveSettingsToolStripMenuItem = new ToolStripMenuItem(Resources.Strings.AutoSaveSettings) { CheckOnClick = true, Checked = _preferences.AutoSaveSettings };
            autoSaveSettingsToolStripMenuItem.CheckedChanged += (s, e) => _preferences.AutoSaveSettings = autoSaveSettingsToolStripMenuItem.Checked;

            preferencesToolStripSplitButton.DropDownItems.Add(_shutdownWhenCompletedToolStripMenuItem);
            preferencesToolStripSplitButton.DropDownItems.Add(new ToolStripSeparator());
            preferencesToolStripSplitButton.DropDownItems.Add(autoSaveSettingsToolStripMenuItem);

            preferencesToolStripSplitButton.DropDownOpening += (s, e) => autoSaveSettingsToolStripMenuItem.Checked = _preferences.AutoSaveSettings;

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Show Command Line button.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ShowCommandLine, ShowCommandLineToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.CopyCommandLineToClipboard, (s, e) => CopyToClipboard(commandLineTextBox.Text)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyUICommandLineToClipboard, (s, e) => CopyToClipboard(_uiCommandLine))
             );

            UpdateShowCommandLineCommand();

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Load/save settings.
            _loadToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.LoadSettings, (s, e) => modalAction(() => Model = SettingsManager.LoadSettings()),
                new ToolStripButtonItem(Resources.Strings.SaveSettings, (s, e) => modalAction(() => SettingsManager.SaveSettings(_model)))
            );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Reset Settings button.
            _resetToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ResetOptions, (s, e) => Model.ResetOptions(),
                new ToolStripButtonItem(Resources.Strings.ResetParameters, (s, e) => Model.ResetParameters()),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.Reset, (s, e) => Model.Reset())
             );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Explorer browse.
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.BrowseConvertedImages, BrowseConvertedImagesToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.BrowseXPSFile, (s, e) => Explorer.Select(Model.OptionsObject.SrcFile)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyConvertedImagesPathToClipboard, (s, e) => CopyToClipboard(ConvertedImagesFolder))
            );

            //  Help.
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Help, (s, e) => ShowHelp(),
                new ToolStripButtonItem(Resources.Strings.About, (s, e) => modalAction(() => new AboutForm().ShowDialog(this)))
            ).Alignment = ToolStripItemAlignment.Right;
        }

        private static void CopyToClipboard(string str)
        {
            var tries = 2;
            while(tries-- != 0)
            {
                try
                {
                    Clipboard.SetDataObject(str, true, 2, 100);
                    break;
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }
        }

        private void UpdateProgress(int percent, string pages, string file)
        {
            Text = String.Format(Resources.Strings.WindowTitleProgressFormat, Resources.Strings.WindowTitle, percent, pages, Path.GetFileName(file), _srcFileDisplayName);
            progressBar.Value = percent;

            this.SetProgressValue(progressBar.Value, progressBar.Maximum);
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                Text = Resources.Strings.WindowTitle;
                if (_stopwatch != null)
                {
                    _stopwatch.Stop();
                    if (_preferences.ShowElapsedTime)
                    {
                        var elapsed = _stopwatch.Elapsed;
                        Text += String.Format(Resources.Strings.ElapsedTimeTextTemplate, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                    }
                }
            }

            convertButton.Text = isRunning ? Resources.Strings.Stop : Resources.Strings.Launch;
            convertButton.ContextMenuStrip = isRunning ? null : convertContextMenuStrip;

            if (Windows7Taskbar.Supported)
            {
                _thumbButton.Tooltip = ConvertButtonCleanText;
                _thumbButton.Icon = isRunning ? Resources.Icons.Stop : Resources.Icons.Play;
                _thumbButtonManager.RefreshThumbButtons();
            }

            settingsPropertyGrid.ReadOnly = isRunning;
            _resetToolStripButton.Enabled = _loadToolStripButton.Enabled = !isRunning;
            
            progressBar.Value = 0;

            if (!Model.IsRunning)
            {
                this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.NoProgress);

                Model.ShutdownRequested = ShutdownWhenCompleted;
                if (Model.ShutdownRequested)
                {
                    Close();
                }
            }
        }

        private void UpdateFailedStatus(string message)
        {
            _conversionFailed = true;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Error);

            FlashForm();

            ShowMessageBox(message, MessageBoxButtons.OK, MessageBoxIcon.Error);

            UpdateRunningStatus(false);
        }

        private void EnableConvertControls()
        {
            EnableConvertControls(ControlState.EnabledAndFocused);
        }

        [Flags]
        private enum ControlState
        {
            Default = 0,
            Enabled = 1 << 0,
            Focused = 1 << 1,
            EnabledAndFocused = Enabled | Focused
        }

        private void EnableConvertControls(ControlState controlState)
        {
            var enabled = (controlState & ControlState.Enabled) != 0;
            var focused = (controlState & ControlState.Focused) != 0;

            if (_thumbButton != null)
            {
                _thumbButton.Enabled = enabled;
            }

            convertButton.Enabled = enabled;

            if (enabled && focused)
            {
                convertButton.Focus();
            }
        }

        private void UpdateCommandLine(bool canResume)
        {
            Model.CanResume = Model.CanResume && canResume;

            commandLineTextBox.Text = FormatCommandLine(false);
            _uiCommandLine = FormatCommandLine(true);
        }

        private string FormatCommandLine(bool isUI)
        {
            _srcFileDisplayName = Path.GetFileNameWithoutExtension(Model.OptionsObject.SrcFile);
            var commandLine = Model.FormatCommandLine(isUI ? Options.ExcludedOnSave : Options.ExcludedOnView);
            var separator = String.IsNullOrEmpty(commandLine) ? String.Empty : "\x20";
            return String.Format("\"{0}\"{1}{2}", isUI ? Program.Xps2ImgUIExecutable : Program.Xps2ImgExecutable, separator, commandLine);
        }

        private void UpdateShowCommandLineCommand()
        {
            _showCommandLineToolStripButton.Text = IsCommandLineVisible
                                                        ? Resources.Strings.HideCommandLine
                                                        : Resources.Strings.ShowCommandLine;
        }

        private void FlashForm()
        {
            if (_preferences.FlashWhenCompleted && !this.IsForegroundWindow())
            {
                this.Flash(4);
            }
        }

        private bool FocusFirstRequiredOption(Action<string> action)
        {
            var firstRequiredOptionLabel = Model.FirstRequiredOptionLabel;
            if (!String.IsNullOrEmpty(firstRequiredOptionLabel))
            {
                if (action != null)
                {
                    action(firstRequiredOptionLabel);
                }
                settingsPropertyGrid.SelectGridItem(firstRequiredOptionLabel, gi => (gi.Label ?? String.Empty).StartsWith(firstRequiredOptionLabel), true);
                return true;
            }
            return false;
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

        private void MainFormDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = Model.IsRunning || String.IsNullOrEmpty(GetDragFile(e.Data))
                        ? DragDropEffects.None
                        : DragDropEffects.Copy;
        }

        private void MainFormDragDrop(object sender, DragEventArgs e)
        {
            var file = GetDragFile(e.Data);

            if (!String.IsNullOrEmpty(file))
            {
                Model.OptionsObject.SrcFile = file;
                settingsPropertyGrid.Refresh();
                UpdateCommandLine(false);
            }
        }

        private void OutputDataReceived(object sender, ConversionProgressEventArgs e)
        {
            SetConvertedImagesFolder(e.File);
            this.InvokeIfNeeded(() => UpdateProgress(e.Percent, e.Pages, e.File));
        }

        private void ErrorDataReceived(object sender, ConversionErrorEventArgs e)
        {
            this.InvokeIfNeeded(() => UpdateFailedStatus(e.Message));
        }

        private void Completed(object sender, EventArgs e)
        {
            if (_conversionFailed)
            {
                return;
            }

            this.InvokeIfNeeded(() => { UpdateRunningStatus(false); EnableConvertControls(); FlashForm(); });
        }

        private void LaunchFailed(object sender, ThreadExceptionEventArgs e)
        {
            var message = e.Exception is Win32Exception
                            ? String.Format(Resources.Strings.Xps2ImgNotFount, Environment.NewLine, e.Exception.Message)
                            : e.Exception.Message;

            this.InvokeIfNeeded(() => { UpdateFailedStatus(message); EnableConvertControls(); });
        }

        private void LaunchSucceeded(object sender, EventArgs e)
        {
            this.InvokeIfNeeded(EnableConvertControls);
        }

        private new void Activate()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Restore();
            }
            else
            {
                base.Activate();
            }
        }

        private void ShowOptionIsRequiredMessage(string firstRequiredOptionLabel)
        {
            Activate();
            ShowMessageBox(String.Format(Resources.Strings.SpecifyValue, firstRequiredOptionLabel), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void ShowMessageBox(string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            ShowMessageBox(text, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        private DialogResult ShowMessageBox(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton messageBoxDefaultButton)
        {
            if ((Model.IsProgressStarted && ShutdownWhenCompleted) || IsDisposed)
            {
                return DialogResult.Cancel;
            }

            using (new ModalGuard())
            {
                return MessageBox.Show(this, text, Resources.Strings.WindowTitle, buttons, icon, messageBoxDefaultButton);
            }
        }

        private bool ShowConfirmationMessageBox(string text)
        {
            return DialogResult.OK == ShowMessageBox(text + Resources.Strings.PressToProceedMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
        }

        private void ShowHelp()
        {
            Help.ShowHelp(this, Program.HelpFile, HelpNavigator.TableOfContents);
        }

        [Flags]
        private enum ExecuteFlags
        {
            Resume          = 1 << 0,
            Convert         = 1 << 1,
            ActivateWindow  = 1 << 2,
            DoNotClickButton= 1 << 3,
            NoResume        = 1 << 4
        }

        private void ExecuteConversion(ExecuteFlags executeFlags)
        {
            var canResume = (executeFlags & ExecuteFlags.NoResume) == 0 && Model.CanResume;
            var execute = (executeFlags & ExecuteFlags.Convert) != 0 && !(canResume && _preferences.AlwaysResume);

            var conversionType = execute ? ConversionType.Convert : ConversionType.Resume;

            if (execute && canResume && _preferences.SuggestResume && !_preferences.AlwaysResume && !Model.IsRunning)
            {
                if ((executeFlags & ExecuteFlags.ActivateWindow) != 0)
                {
                    Activate();
                }

                var dialogResult = ShowMessageBox(Resources.Strings.ResumeLastConversionSuggestion, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }

                if (dialogResult == DialogResult.Yes)
                {
                    conversionType = ConversionType.Resume;
                }
            }

            ExecuteConversion(conversionType, (executeFlags & ExecuteFlags.DoNotClickButton) == 0);
        }

        private void ExecuteConversion(ConversionType conversionType, bool clickButton)
        {
            if (Model.IsRunning)
            {
                if (!_preferences.ConfirmOnStop || ShowConfirmationMessageBox(Resources.Strings.ConversionStopConfirmation))
                {
                    EnableConvertControls(ControlState.Default);
                    Model.Stop();
                }
                return;
            }

            // Force command line update if launched via shortcut.
            if (clickButton && !ModalGuard.IsEntered && !convertButton.Focused)
            {
                convertButton.Focus();
                if (settingsPropertyGrid.HasErrors)
                {
                    settingsPropertyGrid.Focus();
                }
                else
                {
                    convertButton.PerformClick();
                }
                return;
            }

            EnableConvertControls(ControlState.Default);

            if (ModalGuard.IsEntered)
            {
                Activate();
                EnableConvertControls();
                return;
            }

            _conversionFailed = false;

            if (FocusFirstRequiredOption(ShowOptionIsRequiredMessage))
            {
                EnableConvertControls(ControlState.Enabled);
                return;
            }

            Text = String.Format(Resources.Strings.WindowTitleLaunchingFormat, Resources.Strings.WindowTitle);

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Indeterminate);

            _convertedImagesFolder = null;

            if (_stopwatch != null)
            {
                _stopwatch.Stop();
                _stopwatch = null;
            }

            if (_preferences.ShowElapsedTime)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

            Model.Launch(conversionType);

            UpdateRunningStatus(true);
        }

        private void ConvertButtonClick(object sender, EventArgs e)
        {
            ExecuteConversion(ExecuteFlags.Convert | (sender is Button ? 0 : ExecuteFlags.NoResume));
        }

        private void ResumeToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExecuteConversion(ExecuteFlags.Resume);
        }

        private void SettingsPropertyGridPropertySortChanged(object sender, EventArgs e)
        {
            var propertyGrid = (PropertyGrid) sender;
            if (propertyGrid.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid.PropertySort = PropertySort.Categorized;
            }
        }

        private void SettingsPropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateCommandLine(Options.ExcludeOnResumeCheck.Contains(e.ChangedItem.Label));
            if (e.ChangedItem.Label == Options.FileTypeDisplayName)
            {
                settingsPropertyGrid.Refresh();
            }
        }

        private void SettingsPropertyGridSelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateCommandLine(false);
        }

        private void PreferencesToolStripButtonClick(object sender, EventArgs e)
        {
            using (new ModalGuard())
            {
                var preferencesForm = new PreferencesForm(_preferences);
                if (preferencesForm.ShowDialog(this) == DialogResult.OK)
                {
                    _preferences = preferencesForm.Preferences;
                    ApplyPreferences();
                }
            }
        }

        private void ShowCommandLineToolStripButtonClick(object sender, EventArgs e)
        {
            IsCommandLineVisible = !IsCommandLineVisible;
            UpdateShowCommandLineCommand();
        }

        private void BrowseConvertedImagesToolStripButtonClick(object sender, EventArgs e)
        {
            Explorer.Browse(ConvertedImagesFolder);
        }

        private void DeleteImagesToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!_preferences.ConfirmOnDelete || ShowConfirmationMessageBox(Resources.Strings.DeleteConvertedImagesConfirmation))
            {
                ExecuteConversion(ConversionType.Delete, true);
            }
        }

        private void СonvertContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            var menu = (ContextMenuStrip) sender;
            if(Model.CanResume)
            {
                menu.Items.Insert(_resumeToolStripMenuItemPosition, resumeToolStripMenuItem);
            }
            else
            {
                menu.Items.Remove(resumeToolStripMenuItem);
            }
        }

        private volatile bool _conversionFailed;

        private volatile string _convertedImagesFolder;

        private string ConvertedImagesFolder
        {
            get
            {
                return _convertedImagesFolder ?? Program.ApplicationFolder;
            }
        }

        public void SetConvertedImagesFolder(string fileName)
        {
            if (_convertedImagesFolder != null)
            {
                return;
            }

            _convertedImagesFolder = Path.GetDirectoryName(fileName);
        }

        private string ConvertButtonCleanText
        {
            get { return convertButton.Text.Replace("&", String.Empty); }
        }

        private bool ShutdownWhenCompleted
        {
            get { return _shutdownWhenCompletedToolStripMenuItem.Checked; }
        }

        private bool IsCommandLineVisible
        {
            get { return !settingsSplitContainer.Panel2Collapsed; }
            set
            {
                if (settingsSplitContainer.Panel2Collapsed == value)
                {
                    settingsSplitContainer.Panel2Collapsed = !value;
                    Height += (commandLineTextBox.Height + settingsSplitContainer.SplitterWidth) * (value ? 1 : -1);
                }
            }
        }

        private Stopwatch _stopwatch;

        private Preferences _preferences = new Preferences();

        private readonly int _resumeToolStripMenuItemPosition;

        private Xps2ImgModel _model;

        public Xps2ImgModel Model
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                if (_model != null)
                {
                    Model.OutputDataReceived -= OutputDataReceived;
                    Model.ErrorDataReceived -= ErrorDataReceived;
                    Model.Completed -= Completed;
                    Model.LaunchFailed -= LaunchFailed;
                    Model.LaunchSucceeded -= LaunchSucceeded;
                    Model.OptionsObjectChanged -= OptionsObjectChanged;
                }

                _model = value;

                Model.OutputDataReceived += OutputDataReceived;
                Model.ErrorDataReceived += ErrorDataReceived;
                Model.Completed += Completed;
                Model.LaunchFailed += LaunchFailed;
                Model.LaunchSucceeded += LaunchSucceeded;
                Model.OptionsObjectChanged += OptionsObjectChanged;

                Model.Init();
            }
            get
            {
                return _model;
            }
        }
        
        private string _srcFileDisplayName;
        private string _uiCommandLine;

        private ToolStripItem _resetToolStripButton;
        private ToolStripSplitButton _loadToolStripButton;
        private ToolStripItem _showCommandLineToolStripButton;
        private ToolStripMenuItem _shutdownWhenCompletedToolStripMenuItem;

        private ThumbButtonManager _thumbButtonManager;
        private ThumbButton _thumbButton;
    }
}
