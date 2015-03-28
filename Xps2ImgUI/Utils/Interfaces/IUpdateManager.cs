using System;
using System.ComponentModel;

namespace Xps2ImgUI.Utils.Interfaces
{
    public interface IUpdateManager
    {
        bool HasUpdate { get; }
        string WhatsNew { get; }
        bool Failed { get; }
        bool Silent { get; }

        Exception Exception { get; }

        bool IsDownloadCancelled { get; }

        event EventHandler CheckCompleted;
        event EventHandler InstallationLaunched;
        event AsyncCompletedEventHandler DownloadFileCompleted;
        event ProgressChangedEventHandler DownloadProgressChanged;

        void CheckAsync(string version, bool silent = false);
        void DownloadAsync();
        void CancelDownload();
        void InstallAsync();
    }
}
