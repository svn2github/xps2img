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
                ShowMessageBox(text, MessageBoxButtons.OK, error ? MessageBoxIcon.Error : MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
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
            Action noConfirmation  = null;

            var footerCheckBoxChecked = false;

            string taskText = null;
            string taskIntruction = null;
            string okCommand = null;

            if(text == Resources.Strings.DeleteConvertedImagesConfirmation)
            {
                taskIntruction  = Resources.Strings.DeleteImagesConfirmation;
                taskText        = Resources.Strings.WouldYouLikeToDeleteImages;
                okCommand       = Resources.Strings.YesDeleteImages;
                noConfirmation  = () => _preferences.ConfirmOnDelete = false;
            }
            else
            if(text == Resources.Strings.ConversionStopConfirmation)
            {
                taskIntruction  = Resources.Strings.StopConversionConfirmation;
                taskText        = Resources.Strings.WouldYouLikeToStopConversion;
                okCommand       = Resources.Strings.YesStopConversion;
                noConfirmation  = () => _preferences.ConfirmOnStop = false;
            }
            else
            if(text == Resources.Strings.ClosingConfirmation)
            {
                taskIntruction  = Resources.Strings.CloseApplicationConfirmation;
                taskText        = Resources.Strings.WouldYouLikeToAbortConversionAndCloseApplication;
                okCommand       = Resources.Strings.YesAbortAndClose;
                noConfirmation  = () => _preferences.ConfirmOnExit = false;
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
                                    taskIntruction,
                                    taskText,
                                    TaskDialogStandardIcon.Warning,
                                    Resources.Strings.AlwaysConfirmAndDoNotAskAgain,
                                    out footerCheckBoxChecked,
                                    null,
                                    new TaskDialogCommandInfo(TaskDialogResult.Ok,      okCommand),
                                    new TaskDialogCommandInfo(TaskDialogResult.Cancel,  Resources.Strings.NoBackToApplication));

            var result = DialogResult.OK == (dialogResult != TaskDialogUtils.NotSupported ? dialogResult : ShowMessageBox(text + Resources.Strings.PressToProceedMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2));

            if (result && footerCheckBoxChecked)
            {
                noConfirmation ();
            }

            return result;
        }
    }
}
