using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using Windows7.DesktopIntegration;
using Windows7.Dialogs;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Model;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        [Flags]
        private enum ExecuteFlags
        {
            Resume          = 1 << 0,
            Convert         = 1 << 1,
            ActivateWindow  = 1 << 2,
            NoResume        = 1 << 3
        }

        [Flags]
        private enum ControlState
        {
            Default = 0,
            Enabled = 1 << 0,
            Focused = 1 << 1,
            EnabledAndFocused = Enabled | Focused
        }

        private void ConvertButtonClick(object sender, EventArgs e)
        {
            ExecuteConversion(ExecuteFlags.Convert | (sender is Button ? 0 : ExecuteFlags.NoResume));
        }

        private void ResumeToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExecuteConversion(ExecuteFlags.Resume);
        }

        private void ExecuteConversion(ExecuteFlags executeFlags)
        {
            if (!Model.IsRunning && !settingsPropertyGrid.Validate())
            {
                return;
            }

            var canResume = (executeFlags & ExecuteFlags.NoResume) == 0 && Model.CanResume;
            var execute = (executeFlags & ExecuteFlags.Convert) != 0 && !(canResume && _preferences.AlwaysResume);

            var conversionType = execute ? ConversionType.Convert : ConversionType.Resume;

            if (execute && canResume && _preferences.SuggestResume && !_preferences.AlwaysResume && !Model.IsRunning)
            {
                if ((executeFlags & ExecuteFlags.ActivateWindow) != 0)
                {
                    Activate();
                }

                if (ModalGuard.IsEntered)
                {
                    return;
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

            ExecuteConversion(conversionType);
        }

        private void ExecuteConversion(ConversionType conversionType)
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

        private void ExecuteDeleteImages()
        {
            if (Model.IsBatchMode || !_preferences.ConfirmOnDelete || ShowConfirmationMessageBox(Resources.Strings.DeleteConvertedImagesConfirmation))
            {
                ExecuteConversion(ConversionType.Delete);
            }
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

        private void ShowHelp()
        {
            if (ModalGuard.IsEntered)
            {
                return;
            }

            if (!this.ShowPropertyHelp(settingsPropertyGrid, HelpUtils.HelpTopicOptions))
            {
                this.ShowHelpTableOfContents();
            }
        }

        private void ResetByCategory(string category)
        {
            settingsPropertyGrid.ResetByCategory(category);
            Model.FireOptionsObjectChanged();
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

        private void EnableConvertControls(ControlState controlState = ControlState.EnabledAndFocused)
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

        private void UpdateFailedStatus(string message, Exception exception = null, int? page = null)
        {
            _conversionFailed = true;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Error);

            FlashForm();

            if (page.HasValue)
            {
                message = String.Format(CultureInfo.InvariantCulture, Resources.Strings.UpdatePageFailedStatus, page, message);
            }

            ShowErrorMessageBox(message, exception, true, Resources.Strings.ConversionFailed, message, Resources.Strings.BackToApplication);

            UpdateRunningStatus(false);
        }

        private void UpdateCommandLine(bool canResume = false)
        {
            Model.CanResume = Model.CanResume && canResume;

            commandLineTextBox.Text = FormatCommandLine(false);
            _uiCommandLine = FormatCommandLine(true);
        }

        private void UpdateShowCommandLineCommand()
        {
            _showCommandLineToolStripButton.Text = IsCommandLineVisible ? Resources.Strings.HideCommandLine : Resources.Strings.ShowCommandLine;
        }

        private string FormatCommandLine(bool isUi)
        {
            _srcFileDisplayName = Path.GetFileNameWithoutExtension(Model.SrcFile);
            var commandLine = Model.FormatCommandLine(isUi ? Options.ExcludedOnSave : Options.ExcludedOnView);
            var separator = String.IsNullOrEmpty(commandLine) ? String.Empty : StringUtils.SpaceString;
            return String.Format("\"{0}\"{1}{2}", isUi ? Program.Xps2ImgUIExecutable : Program.Xps2ImgExecutable, separator, commandLine);
        }

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
                        ?? (!String.IsNullOrEmpty(Model.SrcFile) ? Path.GetDirectoryName(Model.SrcFile) : null)
                        ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        private volatile bool _conversionFailed;

        private string _srcFileDisplayName;
        private string _uiCommandLine;

        private readonly Stopwatch _stopwatch = new Stopwatch();
    }
}
