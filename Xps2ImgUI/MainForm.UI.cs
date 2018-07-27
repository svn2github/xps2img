using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Windows7.DesktopIntegration;

using Xps2Img.Shared.Progress;

using Timer = System.Windows.Forms.Timer;

using Xps2Img.Shared.Utils;
using Xps2ImgUI.Model;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private void UpdateProgress(bool fromTimer = false)
        {
            if (_conversionProgressEventArgs == null)
            {
                return;
            }

            var percent = _conversionProgressEventArgs.Percent;
            var pages   = _conversionProgressEventArgs.Pages;
            var file    = _conversionProgressEventArgs.File;

            _estimated.Caclulate(percent, fromTimer);

            var intPercent = (int) percent;

            Text = String.Format(Resources.Strings.WindowTitleProgressFormat, Resources.Strings.WindowTitle, intPercent, pages, Path.GetFileName(file), _estimated.Left, _estimated.Elapsed, _srcFileDisplayName);
            progressBar.Value = intPercent;

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

            UpdateThumbButtons(isRunningBool);
        }

        private void UpdateThumbButtons(bool isRunning)
        {
            if (_thumbButtonManager == null)
            {
                return;
            }

            if (_thumbButton != null)
            {
                _thumbButton.Tooltip = ConvertButtonCleanText;
                _thumbButton.Icon = isRunning ? Resources.Icons.Stop : Resources.Icons.Play;
            }

            if (_thumbButtonBrowse != null)
            {
                _thumbButtonBrowse.Tooltip = BrowseImagesButtonText;
            }

            _thumbButtonManager.RefreshThumbButtons();
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                _elapsedTimer.Stop();

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
            Text = Resources.Strings.WindowTitle;

            if (reset)
            {
                _estimated.Reset();
                return;
            }

            if (!_preferences.ShowElapsedTimeAndStatistics || _estimated.IsNone)
            {
                return;
            }

            Text += _estimated.FormatRatio(Model.PagesProcessedTotal, Model.PagesTotal, Model.IsDeleteMode, Resources.Strings.ElapsedTimeTextTemplate, Resources.Strings.ElapsedTimeTextTemplateShort);
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

            UpdateRunningStatus(false);

            ShowErrorMessageBox(message, exception, true, Resources.Strings.ConversionFailed, message, Resources.Strings.BackToApplication);
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

        private volatile ConversionProgressEventArgs _conversionProgressEventArgs;

        private volatile bool _conversionFailed;

        private string _srcFileDisplayName;
        private string _uiCommandLine;

        private readonly Timer _elapsedTimer = new Timer();

        private readonly Estimated _estimated = new Estimated();
    }
}
