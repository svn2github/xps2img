using System;
using System.Windows.Forms;

using Xps2Img.Shared.Setup;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.System;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private bool CheckForUpdatesEnabled
        {
            get { return _updatesToolStripButtonItem.ToolStripItem.Enabled; }
            set { _updatesToolStripButtonItem.ToolStripItem.Enabled = value; }
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
                if (Model.ShutdownRequested)
                {
                    return;
                }

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
                        if (BeginUpdateInstallation())
                        {
                            return;
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

        private bool BeginUpdateInstallation()
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
                        return true;
                    }
                }
            }
            return false;
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

        private void RegisterIdleHandler(EventHandler eventHandler, bool unregisterFirst = false)
        {
            if (unregisterFirst)
            {
                UnregisterIdleHandler(eventHandler);
            }
            this.InvokeIfNeeded(() => Application.Idle += eventHandler);
        }

        private void UnregisterIdleHandler(EventHandler eventHandler)
        {
            this.InvokeIfNeeded(() => Application.Idle -= eventHandler);
        }
    }
}
