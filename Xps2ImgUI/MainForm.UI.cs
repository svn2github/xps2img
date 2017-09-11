using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Windows7.DesktopIntegration;

using Xps2Img.Shared.Utils;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private void UpdateProgress(int percent, string pages, string file)
        {
            Text = String.Format(Resources.Strings.WindowTitleProgressFormat, Resources.Strings.WindowTitle, percent, pages, Path.GetFileName(file), _srcFileDisplayName);
            progressBar.Value = percent;

            this.SetProgressValue(progressBar.Value, progressBar.Maximum);
        }

        private void UpdateConvertButtons(bool? isRunning = null)
        {
            var isRunningBool = isRunning ?? Model.IsRunning;

            convertButton.Text = isRunningBool
                                    ? Resources.Strings.Stop
                                    : Model.CanResume && (_activeAlwaysResume ?? _preferences.AlwaysResume)
                                        ? Resources.Strings.Resume
                                        : Resources.Strings.Launch;

            convertButton.ContextMenuStrip = isRunningBool ? null : convertContextMenuStrip;

            UpdateThumbButton(isRunningBool);
        }

        private void UpdateThumbButton(bool isRunning)
        {
            if (_thumbButton == null)
            {
                return;
            }

            _thumbButton.Tooltip = ConvertButtonCleanText;
            _thumbButton.Icon = isRunning ? Resources.Icons.Stop : Resources.Icons.Play;

            _thumbButtonManager.RefreshThumbButtons();
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                _stopwatch.Stop();
                UpdateElapsedTime();
            }

            UpdateConvertButtons(isRunning);

            settingsPropertyGrid.ReadOnly = isRunning;

            _shortenExtensionToolStripMenuItem.Enabled =
                _resetToolStripButton.Enabled =
                    _loadToolStripButton.Enabled = !isRunning;
            
            progressBar.Value = 0;

            if (Model.IsRunning && isRunning)
            {
                return;
            }

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

        private void UpdateElapsedTime(bool reset = false)
        {
            TimeSpan elapsed;

            Text = Resources.Strings.WindowTitle;

            if (reset)
            {
                _stopwatch.Reset();
                return;
            }

            if (!_preferences.ShowElapsedTimeAndStatistics || (elapsed = _stopwatch.Elapsed) == default(TimeSpan))
            {
                return;
            }

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

        private string GetShowCommandLineToolTipText()
        {
            return IsCommandLineVisible ? Resources.Strings.HideCommandLine : Resources.Strings.ShowCommandLine;
        }

        private void UpdateShowCommandLineCommand()
        {
            _showCommandLineToolStripButton.Image = IsCommandLineVisible ? Resources.Images.CommandLineHide : Resources.Images.CommandLine;
            _showCommandLineToolStripButton.ToolTipText = GetShowCommandLineToolTipText();

            settingsPropertyGrid.UpdateToolStripToolTip();
        }

        private string FormatCommandLine(bool isUi)
        {
            _srcFileDisplayName = Path.GetFileNameWithoutExtension(Model.SrcFile);
            var commandLine = Model.FormatCommandLine(isUi ? _model.OptionsObject.ExcludedOnSave : _model.OptionsObject.ExcludedOnView);
            var separator = String.IsNullOrEmpty(commandLine) ? String.Empty : StringUtils.SpaceString;

            var exe = isUi ? Program.Xps2ImgUIExecutable : Program.Xps2ImgExecutable;
            if (!(_useFullExePath ?? _preferences.UseFullExePath))
            {
                exe = Path.GetFileName(exe) ?? exe;
            }

            return String.Format((exe.Any(Char.IsWhiteSpace) ? "\"{0}\"" : "{0}") + "{1}{2}", exe, separator, commandLine);
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

        private string ConvertedImagesFolder
        {
            get
            {
                return Xps2ImgLib.Converter.GetOutputDirFor(Model.SrcFile, Model.OutDir, true);
            }
        }

        private volatile bool _conversionFailed;

        private string _srcFileDisplayName;
        private string _uiCommandLine;

        private readonly Stopwatch _stopwatch = new Stopwatch();
    }
}
