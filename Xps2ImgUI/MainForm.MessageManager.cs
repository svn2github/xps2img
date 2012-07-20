using System;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using Xps2ImgUI.Utils.UI;

using Windows7.Dialogs;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private void ShowErrorMessageBox(string text, bool error = true, string taskInstruction = null, string taskText = null, string closeText = null)
        {
            var dialogResult = TaskDialogUtils.Show(
                                    Handle,
                                    Resources.Strings.WindowTitle,
                                    taskInstruction,
                                    taskText,
                                    error ? TaskDialogStandardIcon.Error : TaskDialogStandardIcon.Warning,
                                    null,
                                    new TaskDialogCommandInfo(TaskDialogResult.Close, closeText));

            if(dialogResult == TaskDialogUtils.NotSupported)
            {
                ShowMessageBox(text, MessageBoxButtons.OK, error ? MessageBoxIcon.Error : MessageBoxIcon.Warning);
            }
        }

        private DialogResult ShowMessageBox(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button1)
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

        private bool ShowConfirmationMessageBox(string text, MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button2, MessageBoxIcon messageBoxIcon = MessageBoxIcon.Exclamation, Exception exception = null)
        {
            var taskDialogStandardIcon = TaskDialogStandardIcon.Warning;

            Action noConfirmation  = null;

            var footerCheckBoxChecked = false;

            string taskText = null;
            string taskInstruction = null;
            string okCommand = null;

            if(text == Resources.Strings.DeleteConvertedImagesConfirmation)
            {
                taskInstruction = Resources.Strings.DeleteImagesConfirmation;
                taskText        = Resources.Strings.WouldYouLikeToDeleteImages;
                okCommand       = Resources.Strings.YesDeleteImages;
                noConfirmation  = () => _preferences.ConfirmOnDelete = false;
            }
            else
            if(text == Resources.Strings.ConversionStopConfirmation)
            {
                taskInstruction = Resources.Strings.StopConversionConfirmation;
                taskText        = Resources.Strings.WouldYouLikeToStopConversion;
                okCommand       = Resources.Strings.YesStopConversion;
                noConfirmation  = () => _preferences.ConfirmOnStop = false;
            }
            else
            if(text == Resources.Strings.ClosingConfirmation)
            {
                taskInstruction = Resources.Strings.CloseApplicationConfirmation;
                taskText        = Resources.Strings.WouldYouLikeToAbortConversionAndCloseApplication;
                okCommand       = Resources.Strings.YesAbortAndClose;
                noConfirmation  = () => _preferences.ConfirmOnExit = false;
            }
            else
            if (text == Resources.Strings.UpdatesCheckFailedWarning)
            {
                taskInstruction = Resources.Strings.UpdatesCheckFailed;
                taskText        = Resources.Strings.WouldYouLikeToCheckForUpdatesManually;
                okCommand       = Resources.Strings.YesCheckForUpdatesManually;
            }
            else
            if (text == Resources.Strings.DownloadFailedWarning)
            {
                taskInstruction = Resources.Strings.DownloadFailed;
                taskText        = Resources.Strings.WouldYouLikeToDownloadUpdateManually;
                okCommand       = Resources.Strings.YesDownloadManually;
            }
            else
            if (text == Resources.Strings.NewUpdateIsAvailable)
            {
                taskInstruction = Resources.Strings.NewUpdateAvailable;
                taskText        = Resources.Strings.WouldYouLikeToDownloadAndInstall;
                okCommand       = Resources.Strings.YesDownloadAndInstall;
                taskDialogStandardIcon = TaskDialogStandardIcon.Information;
            }
            
            #if DEBUG
            if (String.IsNullOrEmpty(taskText))
            {
                throw new InvalidOperationException("Unknown confirmation: " + text);
            }
            #endif

            var dialogResult = String.IsNullOrEmpty(taskText)
                                ? TaskDialogUtils.NotSupported
                                : TaskDialogUtils.Show(
                                    Handle,
                                    Resources.Strings.WindowTitle,
                                    taskInstruction,
                                    taskText,
                                    taskDialogStandardIcon,
                                    noConfirmation == null ? null : Resources.Strings.AlwaysConfirmAndDoNotAskAgain,
                                    out footerCheckBoxChecked,
                                    t => AddExceptionDetails(t, exception),
                                    new TaskDialogCommandInfo(TaskDialogResult.Ok,      okCommand),
                                    new TaskDialogCommandInfo(TaskDialogResult.Cancel,  Resources.Strings.NoBackToApplication));

            var result = DialogResult.OK == (
                dialogResult != TaskDialogUtils.NotSupported
                    ? dialogResult
                    : ShowMessageBox(text + Resources.Strings.PressToProceedMessage, MessageBoxButtons.OKCancel, messageBoxIcon, messageBoxDefaultButton)
            );

            if (result && footerCheckBoxChecked && noConfirmation != null)
            {
                noConfirmation();
            }

            return result;
        }

        public static void AddExceptionDetails(TaskDialog taskDialog, Exception ex)
        {
            TaskDialogUtils.AddExceptionDetails(taskDialog, ex, Resources.Strings.ShowLess, Resources.Strings.ShowMore);
        }
    }
}
