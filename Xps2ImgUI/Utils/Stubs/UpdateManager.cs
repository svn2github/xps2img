using System;
using System.ComponentModel;
using System.IO;

namespace Xps2ImgUI.Utils.Stubs
{
    public class UpdateManager : IUpdateManager
    {
        public bool HasUpdate
        {
            get { return true; }
        }

        public bool Failed
        {
            get { return false; }
        }

        public bool Silent
        {
            get { return false; }
        }

        public Exception Exception
        {
            get { return null; }
        }

        public bool IsDownloadCancelled
        {
            get { return false; }
        }

        public event EventHandler CheckCompleted;
        public event EventHandler InstallationLaunched;
        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event ProgressChangedEventHandler DownloadProgressChanged;

        internal UpdateManager()
        {
        }

        public void CheckAsync(string version, bool silent = false)
        {
            CheckCompleted(this, EventArgs.Empty);
        }

        public void DownloadAsync()
        {
            DownloadProgressChanged(this, new ProgressChangedEventArgs(100, null));
            DownloadFileCompleted(this, new AsyncCompletedEventArgs(null, false, null));
        }

        public void CancelDownload()
        {
        }

        public void InstallAsync()
        {
            Utils.UpdateManager.CheckAccessAndInstall(Path.Combine(AssemblyInfo.ApplicationFolder, "Xps2ImgSetup.exe"));
            InstallationLaunched(this, EventArgs.Empty);
        }
    }
}
