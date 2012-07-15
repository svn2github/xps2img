using System;
using System.Windows.Forms;

using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private bool CheckForUpdatesEnabled
        {
            set { _updatesToolStripButtonItem.ToolStripItem.Enabled = value; }
            get { return _updatesToolStripButtonItem.ToolStripItem.Enabled; }
        }

        private void UpdateCheckCompleted(object sender, EventArgs e)
        {
            if (WaitIdle(UpdateCheckCompleted))
            {
                return;
            }

            var enableUpdateCheck = true;

            // ReSharper disable AccessToModifiedClosure
            using (new DisposableActions(() => CheckForUpdatesEnabled = enableUpdateCheck))
            // ReSharper restore AccessToModifiedClosure
            {
                if (_updateManager.Failed)
                {
                    if (!_updateManager.Silent && ShowConfirmationMessageBox(Resources.Strings.UpdatesCheckFailedWarning, _updateManager.Exception))
                    {
                        Explorer.ShellExecute(UpdateManager.ManualCheckUrl);
                    }
                    return;
                }

                if (_updateManager.HasUpdate)
                {
                    if (ShowConfirmationMessageBox(Resources.Strings.NewUpdateIsAvailable, null, MessageBoxIcon.Information))
                    {
                        enableUpdateCheck = false;
                        _updateManager.DownloadAsync();
                    }
                    return;
                }

                if (!_updateManager.Silent)
                {
                    ShowMessageBox(Resources.Strings.UpdateNoNewVersionAvailable, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void UpdateDownloadCompleted(object sender, EventArgs e)
        {
            if (WaitIdle(UpdateDownloadCompleted))
            {
                return;
            }

            using (new DisposableActions(() => CheckForUpdatesEnabled = true))
            {
                if (_updateManager.Failed)
                {
                    if (ShowConfirmationMessageBox(Resources.Strings.DownloadFailedWarning, _updateManager.Exception))
                    {
                        Explorer.ShellExecute(UpdateManager.ManualDownloadUrl);
                    }
                    return;
                }

                _updateManager.InstallAsync();
            }
        }

        private bool WaitIdle(EventHandler eventHandler)
        {
            if (Model.IsRunning || ModalGuard.IsEntered)
            {
                return true;
            }

            UnregisterIdleHandler(eventHandler);

            return false;
        }

        private static void RegisterIdleHandler(EventHandler eventHandler)
        {
            Application.Idle += eventHandler;
        }

        private static void UnregisterIdleHandler(EventHandler eventHandler)
        {
            Application.Idle -= eventHandler;
        }
    }
}
