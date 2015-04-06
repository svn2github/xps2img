using System;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using Windows7.DesktopIntegration;
using Windows7.Dialogs;

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

                if (!ConfirmResume(ref conversionType))
                {
                    return;
                }
            }

            ExecuteConversion(conversionType);
        }

        private bool ConfirmResume(ref ConversionType conversionType)
        {
            bool footerCheckBoxChecked;
            DialogResult dialogResult;

            if (!ShowConfirmResumeDialog(out footerCheckBoxChecked, out dialogResult))
            {
                return false;
            }

            _preferences.SuggestResume = !footerCheckBoxChecked;

            if (dialogResult != DialogResult.Yes)
            {
                return true;
            }

            conversionType = ConversionType.Resume;
            _preferences.AlwaysResume = footerCheckBoxChecked;

            return true;
        }

        private bool ShowConfirmResumeDialog(out bool footerCheckBoxChecked, out DialogResult dialogResult)
        {
            dialogResult = TaskDialogUtils.Show(
                Handle,
                Resources.Strings.WindowTitle,
                Resources.Strings.ResumeConversionConfirmation,
                Resources.Strings.ResumeLastConversionSuggestion,
                TaskDialogStandardIcon.Warning,
                Resources.Strings.RememberChoiceAndDoNotAskAgain,
                out footerCheckBoxChecked,
                null,
                new TaskDialogCommandInfo(TaskDialogResult.Yes, Resources.Strings.YesResumeConversion),
                new TaskDialogCommandInfo(TaskDialogResult.No, Resources.Strings.NoStartConversionOver),
                new TaskDialogCommandInfo(TaskDialogResult.Cancel, Resources.Strings.BackToApplication));

            if (dialogResult == TaskDialogUtils.NotSupported)
            {
                dialogResult = ShowMessageBox(Resources.Strings.ResumeLastConversionSuggestion, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            return dialogResult != DialogResult.Cancel;
        }

        private void ExecuteConversion(ConversionType conversionType)
        {
            if (Model.IsRunning)
            {
                if ((_preferences.ConfirmOnStop && !ShowConfirmationMessageBox(Resources.Strings.ConversionStopConfirmation)) || !Model.IsRunning)
                {
                    return;
                }
                EnableConvertControls(ControlState.Default);
                Model.Cancel();
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

            if (conversionType != ConversionType.Resume)
            {
                _stopwatch.Reset();
            }

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

        private void ShowHelp()
        {
            if (ModalGuard.IsEntered)
            {
                return;
            }

            if (!HelpUtils.ShowPropertyHelp(settingsPropertyGrid, HelpUtils.HelpTopicOptions))
            {
                HelpUtils.ShowHelpTableOfContents();
            }
        }

        private void ResetByCategory(string category)
        {
            settingsPropertyGrid.ResetByCategory(category);

            Model.FireOptionsObjectChanged();

            UpdateConvertButtons();
        }

        private void ApplyPreferences(bool byUser = false)
        {
            settingsPropertyGrid.ModernLook = !_preferences.ClassicLook;
            settingsPropertyGrid.AllowAutoComplete = _preferences.AutoCompleteFilenames;

            convertContextMenuStrip.RenderMode = settingsPropertyGrid.ContextMenuStrip.RenderMode;

            var canResume = Model.ShortenExtension == _preferences.ShortenExtension;

            if (Model.IsBatchMode)
            {
                _preferences.ShortenExtension = Model.ShortenExtension;
            }
            else
            {
                Model.ShortenExtension = _preferences.ShortenExtension;
            }

            UpdateCommandLine(byUser && canResume);
        }
    }
}
