using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using Windows7.DesktopIntegration;
using Windows7.Dialogs;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Controls;
using Xps2ImgUI.Controls.PropertyGridEx;
using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2ImgUI
{
    public partial class MainForm : Form, ISettings
    {
        public MainForm()
        {
            InitializeComponent();

            Model = new Xps2ImgModel();

            _resumeToolStripMenuItemPosition = convertContextMenuStrip.Items.OfType<ToolStripMenuItem>().ToList().IndexOf(resumeToolStripMenuItem);

            _updateManager.CheckCompleted += (s, e) => this.InvokeIfNeeded(() => RegisterIdleHandler(UpdateCheckCompleted));
            _updateManager.InstallationLaunched += (s, e) => this.InvokeIfNeeded(() => RegisterIdleHandler(UpdateInstallationLaunched));

            settingsPropertyGrid.ResetGroupCallback = PropertyGridResetGroupCallback;
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

        private void ResetByCategory(string category)
        {
            settingsPropertyGrid.ResetByCategory(category);
            Model.FireOptionsObjectChanged();
        }
        
        private void OptionsObjectChanged(object sender, EventArgs e)
        {
            settingsPropertyGrid.SelectedObject = Model.OptionsObject;
            Refresh();
            UpdateCommandLine();
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = Resources.Strings.WindowTitle;

            convertButton.Text = Resources.Strings.Launch;
            convertButton.ContextMenuStrip = convertContextMenuStrip;

            var isCommandLineVisible = IsCommandLineVisible;
            IsCommandLineVisible = false;
            IsCommandLineVisible = isCommandLineVisible;

            AdjustPropertyGrid();

            ApplyPreferences();
            
            FocusFirstRequiredOption(false);

            CheckForUpdates(true);

            base.OnLoad(e);
        }

        private void ApplyPreferences()
        {
            settingsPropertyGrid.ModernLook = !_preferences.ClassicLook;
            settingsPropertyGrid.AllowAutoComplete = _preferences.AutoCompleteFilenames;
            convertContextMenuStrip.RenderMode = settingsPropertyGrid.ContextMenuStrip.RenderMode;

            if (Model.IsBatchMode)
            {
                _preferences.ShortenExtension = Model.OptionsObject.ShortenExtension;
            }
            else
            {
                Model.OptionsObject.ShortenExtension = _preferences.ShortenExtension;
            }

            UpdateCommandLine();
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
            ShowHelp();
        }

        protected override void OnShown(EventArgs e)
        {
            if (Model.IsBatchMode)
            {
                RegisterIdleHandler(OnBatchLaunch);
            }
            base.OnShown(e);
        }

        private void OnBatchLaunch(object sender, EventArgs eventArgs)
        {
            UnregisterIdleHandler(OnBatchLaunch);
            if (Model.OptionsObject.Clean)
            {
                ExecuteDeleteImages();
            }
            else
            {
                ExecuteConversion(ConversionType.Convert);
            }
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

            try
            {
                base.WndProc(ref m);
            }
            catch(NullReferenceException)
            {
                // Weird error under Vista+ when pressing F1 in editors of file open dialog.
            }
        }

        private void AdjustPropertyGrid()
        {
            settingsPropertyGrid.AutoCompleteSettings = new[]
            {
                new PropertyGridEx.EditAutoComplete(Options.SrcFileDisplayName, AutoCompleteSource.FileSystem),
                new PropertyGridEx.EditAutoComplete(Options.OutDirDisplayName, AutoCompleteSource.FileSystemDirectories)
            };

            settingsPropertyGrid.DragDrop += MainFormDragDrop;
            settingsPropertyGrid.DragEnter += MainFormDragEnter;

            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            Action<Action> modalAction = action => { using (new ModalGuard()) { action(); } };

            // Remove Property Pages button.
            settingsPropertyGrid.RemoveLastToolStripItem();

            // Preferences button.
            var preferencesToolStripSplitButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Preferences, PreferencesToolStripButtonClick);

            _shortenExtensionToolStripMenuItem = new ToolStripMenuItem(Resources.Strings.ShortenImageExtension) { CheckOnClick = true, Checked = _preferences.ShortenExtension };
            _shortenExtensionToolStripMenuItem.CheckedChanged += (s, e) =>
            {
                _preferences.ShortenExtension = _shortenExtensionToolStripMenuItem.Checked;
                Model.OptionsObject.ShortenExtension = _preferences.ShortenExtension;
                UpdateCommandLine();
            };

            var autoSaveSettingsToolStripMenuItem = new ToolStripMenuItem(Resources.Strings.AutoSaveSettings) { CheckOnClick = true, Checked = _preferences.AutoSaveSettings };
            autoSaveSettingsToolStripMenuItem.CheckedChanged += (s, e) => _preferences.AutoSaveSettings = autoSaveSettingsToolStripMenuItem.Checked;

            preferencesToolStripSplitButton.DropDownItems.Add(_shortenExtensionToolStripMenuItem);
            preferencesToolStripSplitButton.DropDownItems.Add(new ToolStripSeparator());
            preferencesToolStripSplitButton.DropDownItems.Add(autoSaveSettingsToolStripMenuItem);

            preferencesToolStripSplitButton.DropDownOpening += (s, e) =>
            {
                autoSaveSettingsToolStripMenuItem.Checked = _preferences.AutoSaveSettings;
                _shortenExtensionToolStripMenuItem.Checked = _preferences.ShortenExtension;
            };

            preferencesToolStripSplitButton.Enabled = Model.IsUserMode;

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            Func<bool, string> copyBatch = eh => String.Format(Resources.Strings.UIBatchCommandLineFormat + (eh ? Resources.Strings.UIBatchCommandLineErrorHandling : String.Empty), _uiCommandLine);

            // Show Command Line button.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ShowCommandLine, ShowCommandLineToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.CopyCommandLineToClipboard, (s, e) => CopyToClipboard(commandLineTextBox.Text)),
                new ToolStripButtonItem(Resources.Strings.CopyUICommandLineToClipboard, (s, e) => CopyToClipboard(_uiCommandLine)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyUIBatchCommandLineToClipboard, (s, e) => CopyToClipboard(() => copyBatch(true))),
                new ToolStripButtonItem(Resources.Strings.CopyUIBatchCommandLineWithoutErrorHandlingToClipboard, (s, e) => CopyToClipboard(() => copyBatch(false)))
             );

            UpdateShowCommandLineCommand();

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Load/save settings.
            _loadToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.LoadSettings, (s, e) => modalAction(() => Model = SettingsManager.LoadSettings()),
                new ToolStripButtonItem(Resources.Strings.SaveSettings, (s, e) => modalAction(() => SettingsManager.SaveSettings(Model)))
            );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Reset Settings button.
            _resetToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ResetOptions, (s, e) => ResetByCategory(Options.CategoryOptions),
                new ToolStripButtonItem(Resources.Strings.ResetParameters, (s, e) => ResetByCategory(Options.CategoryParameters)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.Reset, (s, e) => Model.Reset())
             );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Explorer browse.

            ToolStripButtonItem xpsCopyButton, xpsBrowseButton;
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.BrowseImages, BrowseConvertedImagesToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.BrowseImagesFolder, (s, e) => Explorer.Select(ConvertedImagesFolder)),
                xpsBrowseButton = new ToolStripButtonItem(Resources.Strings.BrowseXPSFile, (s, e) => Explorer.Select(Model.SrcFile)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyImagesFolderPathToClipboard, (s, e) => CopyToClipboard(ConvertedImagesFolder)),
                xpsCopyButton = new ToolStripButtonItem(Resources.Strings.CopyXPSFilePathToClipboard, (s, e) => CopyToClipboard(Model.SrcFile))
            ).DropDownOpening += (s, a) => xpsCopyButton.ToolStripItem.Enabled = xpsBrowseButton.ToolStripItem.Enabled = !String.IsNullOrEmpty(Model.SrcFile);

            //  Help.
            _updatesToolStripButtonItem = new ToolStripButtonItem(Resources.Strings.CheckForUpdates, (s, e) => CheckForUpdates());

            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Help, (s, e) => ShowHelp(), _updatesToolStripButtonItem,
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.About,
                (s, e) => modalAction(() =>
                {
                    using (var aboutForm = new AboutForm { CheckForUpdatesEnabled = CheckForUpdatesEnabled })
                    {
                        aboutForm.ShowDialog(this);
                        if (aboutForm.CheckForUpdates)
                        {
                            CheckForUpdates();
                        }
                    }
                }
            ))).Alignment = ToolStripItemAlignment.Right;

            CheckForUpdatesEnabled = Model.IsUserMode;
        }

        private static void CopyToClipboard(Func<string> messageFunc)
        {
            CopyToClipboard(messageFunc());
        }

        private static void CopyToClipboard(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return;
            }

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
                _stopwatch.Stop();
                UpdateElapsedTime();
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

            _shortenExtensionToolStripMenuItem.Enabled =
                _resetToolStripButton.Enabled =
                    _loadToolStripButton.Enabled = !isRunning;
            
            progressBar.Value = 0;

            if (!Model.IsRunning || !isRunning)
            {
                this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.NoProgress);

                if (Model.ShutdownRequested)
                {
                    RegisterIdleHandler(CloseFormHandler, true);
                    if (Model.IsBatchMode)
                    {
                        base.Activate();
                    }
                }
            }
        }

        private void CloseFormHandler(object sender, EventArgs e)
        {
            if (Model.IsRunning)
            {
                return;
            }

            UnregisterIdleHandler(CloseFormHandler);

            if (Model.IsBatchMode && Model.ExitCode == ReturnCode.UserCancelled)
            {
                ShowErrorMessageBox(Resources.Strings.ConversionWasAbortedByUser);
            }

            if (_preferences.ConfirmOnAfterConversion
                && Model.ShutdownRequested
                && Model.ShutdownType != PostAction.Exit
                && !Model.IsBatchMode
                && !IsShutdownConfirmed())
            {
                return;
            }

            _autoClosed = true;

            Close();
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

        private void UpdateElapsedTime()
        {
            if (!_preferences.ShowElapsedTimeAndStatistics)
            {
                return;
            }

            var elapsed = _stopwatch.Elapsed;

            var timeAbbrev = Resources.Strings.AbbrevSeconds;

            var pagesProcessed = Model.PagesProcessed;

            Func<double, double> par = e => (e > 0.001) ? pagesProcessed/e : pagesProcessed;

            var pp = par(elapsed.TotalSeconds);
            var ppMinute = par(elapsed.TotalMinutes);
            var ppHour = par(elapsed.TotalHours);

            if (pp < 1.0)
            {
                pp = ppMinute;
                timeAbbrev = Resources.Strings.AbbrevMinutes;
            }

            if (pp < 1.0)
            {
                pp = ppHour;
                timeAbbrev = Resources.Strings.AbbrevHours;
            }

            if (pp < 1.0)
            {
                pp = pagesProcessed;
                timeAbbrev = Resources.Strings.AbbrevDays;
            }

            var ppInt = (int)Math.Round(pp);

            Func<int, string> getPagesString = p => p == 1 ? Resources.Strings.AbbrevPage : Resources.Strings.AbbrevPages;

            Text += String.Format(Model.IsDeleteMode || pagesProcessed == 0 ? Resources.Strings.ElapsedTimeTextTemplateShort : Resources.Strings.ElapsedTimeTextTemplate,
                                  elapsed.Hours, elapsed.Minutes, elapsed.Seconds,
                                  ppInt, getPagesString(ppInt), timeAbbrev,
                                  Model.PagesProcessedTotal, Model.PagesTotal);
        }

        private void UpdateFailedStatus(string message, Exception exception = null)
        {
            _conversionFailed = true;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Error);

            FlashForm();

            ShowErrorMessageBox(message, exception, true, Resources.Strings.ConversionFailed, message, Resources.Strings.BackToApplication);

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

        private void UpdateCommandLine(bool canResume = false)
        {
            Model.CanResume = Model.CanResume && canResume;

            commandLineTextBox.Text = FormatCommandLine(false);
            _uiCommandLine = FormatCommandLine(true);
        }

        private string FormatCommandLine(bool isUi)
        {
            _srcFileDisplayName = Path.GetFileNameWithoutExtension(Model.SrcFile);
            var commandLine = Model.FormatCommandLine(isUi ? Options.ExcludedOnSave : Options.ExcludedOnView);
            var separator = String.IsNullOrEmpty(commandLine) ? String.Empty : StringUtils.SpaceString;
            return String.Format("\"{0}\"{1}{2}", isUi ? Program.Xps2ImgUIExecutable : Program.Xps2ImgExecutable, separator, commandLine);
        }

        private void UpdateShowCommandLineCommand()
        {
            _showCommandLineToolStripButton.Text = IsCommandLineVisible ? Resources.Strings.HideCommandLine : Resources.Strings.ShowCommandLine;
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
            var firstRequiredOptionLabel = Model.FirstRequiredOptionLabel;
            if (!String.IsNullOrEmpty(firstRequiredOptionLabel))
            {
                if (showMessage)
                {
                    Activate();
                    ShowErrorMessageBox(String.Format(Resources.Strings.SpecifyValueFormat, firstRequiredOptionLabel), null, false, firstRequiredOptionLabel, Resources.Strings.ParameterIsRequired, Resources.Strings.EditParameter);

                    Model.ExitCode = ReturnCode.NoArgs;

                    if (Model.IsBatchMode)
                    {
                        return true;
                    }
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
                Model.SrcFile = file;
                settingsPropertyGrid.Refresh();
                UpdateCommandLine();
            }
        }

        private void OutputDataReceived(object sender, ConversionProgressEventArgs e)
        {
            ConvertedImagesFolder = e.File;
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
            var message = e.Exception.Message.AppendDot();

            if (e.Exception is Win32Exception)
            {
                message = String.Format(Resources.Strings.Xps2ImgNotFountFormat, Environment.NewLine, message);
            }
            
            this.InvokeIfNeeded(() => { UpdateFailedStatus(message, e.Exception); EnableConvertControls(); });
        }

        private void LaunchSucceeded(object sender, EventArgs e)
        {
            if (Model.IsDeleteMode)
            {
                return;
            }

            this.InvokeIfNeeded(EnableConvertControls);
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

        private void ShowHelp()
        {
            if (!ModalGuard.IsEntered)
            {
                Help.ShowHelp(this, Program.HelpFile, HelpNavigator.TableOfContents);
            }
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

                bool footerCheckBoxChecked;

                var dialogResult = TaskDialogUtils.Show(
                                        Handle,
                                        Resources.Strings.WindowTitle,
                                        Resources.Strings.ResumeConversionConfirmation,
                                        Resources.Strings.ResumeLastConversionSuggestion,
                                        TaskDialogStandardIcon.Warning,
                                        Resources.Strings.RememberChoiceAndDoNotAskAgain,
                                        out footerCheckBoxChecked,
                                        null,
                                        new TaskDialogCommandInfo(TaskDialogResult.Yes,     Resources.Strings.YesResumeConversion),
                                        new TaskDialogCommandInfo(TaskDialogResult.No,      Resources.Strings.NoStartConversionOver),
                                        new TaskDialogCommandInfo(TaskDialogResult.Cancel,  Resources.Strings.BackToApplication));

                if (dialogResult == TaskDialogUtils.NotSupported)
                {
                    dialogResult = ShowMessageBox(Resources.Strings.ResumeLastConversionSuggestion, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                }

                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }

                _preferences.SuggestResume = !footerCheckBoxChecked;

                if (dialogResult == DialogResult.Yes)
                {
                    conversionType = ConversionType.Resume;
                    _preferences.AlwaysResume = footerCheckBoxChecked;
                }
            }

            ExecuteConversion(conversionType, (executeFlags & ExecuteFlags.DoNotClickButton) == 0);
        }

        private void ExecuteConversion(ConversionType conversionType, bool clickButton = false)
        {
            if (Model.IsRunning)
            {
                if ((!_preferences.ConfirmOnStop || ShowConfirmationMessageBox(Resources.Strings.ConversionStopConfirmation)) && Model.IsRunning)
                {
                    EnableConvertControls(ControlState.Default);
                    Model.Cancel();
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

            if (FocusFirstRequiredOption())
            {
                EnableConvertControls(ControlState.Enabled);
                return;
            }

            Text = String.Format(Resources.Strings.WindowTitleLaunchingFormat, Resources.Strings.WindowTitle);

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Indeterminate);

            _convertedImagesFolder = null;

            _stopwatch.Reset();
            _stopwatch.Start();

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
            if (e.ChangedItem.Label == Options.FileTypeDisplayName || e.ChangedItem.Label == Options.PostActionDisplayName)
            {
                settingsPropertyGrid.Refresh();
            }
        }

        private void SettingsPropertyGridSelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateCommandLine();
        }

        private void PreferencesToolStripButtonClick(object sender, EventArgs e)
        {
            using (new ModalGuard())
            {
                using (var preferencesForm = new PreferencesForm(_preferences, Model.IsRunning))
                {
                    if (preferencesForm.ShowDialog(this) == DialogResult.OK)
                    {
                        _preferences = preferencesForm.Preferences;
                        ApplyPreferences();
                    }
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
            ExecuteDeleteImages(true);
        }

        private void ExecuteDeleteImages(bool clickButton = false)
        {
            if (Model.IsBatchMode || !_preferences.ConfirmOnDelete || ShowConfirmationMessageBox(Resources.Strings.DeleteConvertedImagesConfirmation))
            {
                ExecuteConversion(ConversionType.Delete, clickButton);
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

        private bool _autoClosed;

        private volatile bool _conversionFailed;

        private string _convertedImagesFolder;

        private string ConvertedImagesFolder
        {
            set
            {
                Interlocked.CompareExchange(ref _convertedImagesFolder, Path.GetDirectoryName(value), null);
            }
            get
            {
                return Interlocked.CompareExchange(ref _convertedImagesFolder, null, null)
                        ?? (!String.IsNullOrEmpty(_model.SrcFile) ? Path.GetDirectoryName(_model.SrcFile) : null)
                        ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
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
                if (settingsSplitContainer.Panel2Collapsed == value)
                {
                    settingsSplitContainer.Panel2Collapsed = !value;
                    if (WindowState == FormWindowState.Normal)
                    {
                        Height += (commandLineTextBox.Height + settingsSplitContainer.SplitterWidth)*(value ? 1 : -1);
                    }
                }
            }
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();

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
        private ToolStripButtonItem _updatesToolStripButtonItem;
        private ToolStripSplitButton _loadToolStripButton;
        private ToolStripItem _showCommandLineToolStripButton;
        private ToolStripMenuItem _shortenExtensionToolStripMenuItem;

        private ThumbButtonManager _thumbButtonManager;
        private ThumbButton _thumbButton;

        private readonly IUpdateManager _updateManager = UpdateManager.Create();
    }
}
