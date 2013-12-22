using System;
using System.Windows.Forms;

using Windows7.Dialogs;

using Microsoft.WindowsAPICodePack.Dialogs;

using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private void ShowErrorMessageBox(string text, Exception exception = null, bool error = true, string taskInstruction = null, string taskText = null, string closeText = null)
        {
            var message = text.AppendDot();

            if (Model.IsBatchMode)
            {
                if (Win32Utils.AttachConsole())
                {
                    Console.WriteLine(message);
                    Win32Utils.FreeConsole();
                }
                return;
            }

            if (Model.ShutdownRequested)
            {
                return;
            }

            var dialogResult = TaskDialogUtils.Show(
                                    Handle,
                                    Resources.Strings.WindowTitle,
                                    taskInstruction,
                                    taskText.AppendDot(),
                                    error ? TaskDialogStandardIcon.Error : TaskDialogStandardIcon.Warning,
                                    t => AddExceptionDetails(t, exception),
                                    new TaskDialogCommandInfo(TaskDialogResult.Close, closeText));

            if(dialogResult == TaskDialogUtils.NotSupported)
            {
                ShowMessageBox(message, MessageBoxButtons.OK, error ? MessageBoxIcon.Error : MessageBoxIcon.Warning);
            }
        }

        private DialogResult ShowMessageBox(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button1)
        {
            if (IsDisposed)
            {
                return DialogResult.Cancel;
            }

            using (new ModalGuard())
            {
                return MessageBox.Show(this, text, Resources.Strings.WindowTitle, buttons, icon, messageBoxDefaultButton);
            }
        }

        private bool ShowConfirmationMessageBox(string text, string messageToAppend = null, MessageBoxDefaultButton messageBoxDefaultButton = MessageBoxDefaultButton.Button2, MessageBoxIcon messageBoxIcon = MessageBoxIcon.Exclamation, Exception exception = null)
        {
            string taskInstruction;
            string taskText;
            TaskDialogStandardIcon taskDialogStandardIcon;
            bool footerCheckBoxChecked;
            Action noConfirmation;
            string okCommand;
            string footerText;

            GetTaskDialogMessageParams(text, out taskInstruction, out taskText, out taskDialogStandardIcon, out footerCheckBoxChecked, out noConfirmation, out footerText, out okCommand);

            if (!String.IsNullOrEmpty(messageToAppend))
            {
                text += messageToAppend;
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
                                    t =>
                                    {
                                        AddDetails(t, messageToAppend);
                                        AddExceptionDetails(t, exception);
                                        TaskDialogUtils.AddFooterText(t, footerText);
                                    },
                                    new TaskDialogCommandInfo(TaskDialogResult.Ok,      okCommand),
                                    new TaskDialogCommandInfo(TaskDialogResult.Cancel,  Resources.Strings.NoBackToApplication));

            if (!text.EndsWith("\n"))
            {
                text += StringUtils.SpaceString;
            }

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

        private void GetTaskDialogMessageParams(string message, out string taskInstruction, out string taskText, out TaskDialogStandardIcon taskDialogStandardIcon, out bool footerCheckBoxChecked, out Action noConfirmation, out string footerText, out string okCommand)
        {
            footerText = null;

            taskDialogStandardIcon = TaskDialogStandardIcon.Warning;

            noConfirmation = null;

            footerCheckBoxChecked = false;

            taskText = null;
            taskInstruction = null;
            okCommand = null;

            if (message == Resources.Strings.DeleteConvertedImagesConfirmation)
            {
                taskInstruction = Resources.Strings.DeleteImagesConfirmation;
                taskText = Resources.Strings.WouldYouLikeToDeleteImages;
                okCommand = Resources.Strings.YesDeleteImages;
                noConfirmation = () => _preferences.ConfirmOnDelete = false;
            }
            else if (message == Resources.Strings.ConversionStopConfirmation)
            {
                taskInstruction = Resources.Strings.StopConversionConfirmation;
                taskText = Resources.Strings.WouldYouLikeToStopConversion;
                okCommand = Resources.Strings.YesStopConversion;
                noConfirmation = () => _preferences.ConfirmOnStop = false;
            }
            else if (message == Resources.Strings.ClosingConfirmation)
            {
                taskInstruction = Resources.Strings.CloseApplicationConfirmation;
                taskText = Resources.Strings.WouldYouLikeToAbortConversionAndCloseApplication;
                okCommand = Resources.Strings.YesAbortAndClose;
                noConfirmation = () => _preferences.ConfirmOnExit = false;
            }
            else if (message == Resources.Strings.UpdatesCheckFailedWarning)
            {
                taskInstruction = Resources.Strings.UpdatesCheckFailed;
                taskText = Resources.Strings.WouldYouLikeToCheckForUpdatesManually;
                okCommand = Resources.Strings.YesCheckForUpdatesManually;
            }
            else if (message == Resources.Strings.DownloadFailedWarning)
            {
                taskInstruction = Resources.Strings.DownloadFailed;
                taskText = Resources.Strings.WouldYouLikeToDownloadUpdateManually;
                okCommand = Resources.Strings.YesDownloadManually;
            }
            else if (message == Resources.Strings.UpdateFailedWarning)
            {
                taskInstruction = Resources.Strings.UpdateFailed;
                taskText = Resources.Strings.WouldYouLikeToDownloadAndUpdateManually;
                okCommand = Resources.Strings.YesDownloadAndUpdateManually;
            }
            else if (message == Resources.Strings.NewUpdateIsAvailable)
            {
                taskInstruction = Resources.Strings.NewUpdateAvailable;
                taskText = Resources.Strings.WouldYouLikeToDownloadItAndUpdateApplication;
                footerText = Resources.Strings.VisitDownloadPage;
                okCommand = Resources.Strings.YesDownloadAndUpdate;
                taskDialogStandardIcon = TaskDialogStandardIcon.Information;
            }
            else if (message == Resources.Strings.ConversionFailedForTheFollowingPages)
            {
                taskInstruction = Resources.Strings.CompletedWithErrors;
                taskText = Resources.Strings.WouldYouLikeToCopyPageNumbersToClipboard;
                okCommand = Resources.Strings.YesCopyToClipboard;
            }
        }

        private static void AddDetails(TaskDialog taskDialog, string message)
        {
            TaskDialogUtils.AddDetails(taskDialog, message, Resources.Strings.HideDetails, Resources.Strings.ShowDetails);
        }

        public static void AddExceptionDetails(TaskDialog taskDialog, Exception ex)
        {
            TaskDialogUtils.AddExceptionDetails(taskDialog, ex, Resources.Strings.HideDetails, Resources.Strings.ShowDetails);
        }
    }
}
