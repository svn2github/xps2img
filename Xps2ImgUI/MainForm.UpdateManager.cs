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

        private void CheckForUpdates(bool periodicUpdatesCheck = false)
        {
            if (Model.IsBatchMode || (periodicUpdatesCheck && !_preferences.ShouldCheckForUpdates))
            {
                return;
            }

            CheckForUpdatesEnabled = false;

            _preferences.LastCheckedForUpdates = DateTime.UtcNow;
            _updateManager.CheckAsync(AssemblyInfo.FileVersion, periodicUpdatesCheck);
        }

        private void UpdateCheckCompleted(object sender, EventArgs e)
        {
            if (WaitIdle(UpdateCheckCompleted))
            {
                return;
            }

            using (new DisposableActions(() => CheckForUpdatesEnabled = true))
            {
                if (_updateManager.Failed)
                {
                    if (!_updateManager.Silent && ShowConfirmationMessageBox(Resources.Strings.UpdatesCheckFailedWarning, exception: _updateManager.Exception))
                    {
                        Explorer.ShellExecute(UpdateManager.ManualCheckUrl);
                    }
                    return;
                }

                if (_updateManager.HasUpdate)
                {
                    if (ShowConfirmationMessageBox(Resources.Strings.NewUpdateIsAvailable, String.Format(Resources.Strings.WhatsNewFormat, _updateManager.WhatsNew), MessageBoxDefaultButton.Button1, MessageBoxIcon.Information))
                    {
                        using (new ModalGuard())
                        {
                            using (var updateDownloadForm = new UpdateDownloadForm(_updateManager))
                            {
                                var dialogResult = updateDownloadForm.ShowDialog();
                                if (dialogResult == DialogResult.OK)
                                {
                                    SetupGuard.Leave();
                                    _updateManager.InstallAsync();
                                    return;
                                }
                            }
                        }

                        if (OpenSiteOnError(Resources.Strings.DownloadFailedWarning))
                        {
                            return;
                        }
                    }
                    return;
                }

                if (!_updateManager.Silent)
                {
                    ShowMessageBox(Resources.Strings.UpdateNoNewVersionAvailable, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void UpdateInstallationLaunched(object sender, EventArgs eventArgs)
        {
            if (WaitIdle(UpdateInstallationLaunched))
            {
                return;
            }

            if (OpenSiteOnError(Resources.Strings.UpdateFailedWarning, SetupGuard.Enter))
            {
                return;
            }

            Application.Exit();
        }

        private bool OpenSiteOnError(string message, Action beforeAction = null)
        {
            if (!_updateManager.Failed)
            {
                return false;
            }

            if (beforeAction != null)
            {
                beforeAction();
            }

            if (ShowConfirmationMessageBox(message, exception: _updateManager.Exception))
            {
                Explorer.ShellExecute(UpdateManager.ManualDownloadUrl);
            }

            return true;
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
