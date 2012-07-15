using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Utils
{
    public class UpdateManager
    {
        public const string ManualCheckUrl      = DownloadRootUrl;
        public const string ManualDownloadUrl   = DownloadRootUrl;

        private const string UpdateUrl          = "http://sourceforge.net/projects/xps2img/";
        private const string DownloadRootUrl    = "http://downloads.sourceforge.net/project/xps2img/Releases/";
        private const string SetupDownload      = "Xps2ImgSetup-{0}.exe";

        private const string VersionGroup       = "version";
        private const string VersionCheck       = @"/Releases/xps2img[^-]*-(?<version>(\.?\d+){4})";

        public bool HasUpdate
        {
            set { _hasUpdate = value; }
            get { return _hasUpdate && !Failed; }
        }

        public bool Failed
        {
            get { return _exception != null; }
        }

        public bool Silent { get; private set; }

        public Exception Exception
        {
            get { return _exception; }
        }

        public event EventHandler CheckCompleted;
        public event EventHandler DownloadCompleted;

        public void CheckAsync(string version, bool silent = false)
        {
            Silent = silent;
            ThreadPool.QueueUserWorkItem(x => Check(version));
        }

        public void DownloadAsync()
        {
            ThreadPool.QueueUserWorkItem(x => Download());
        }

        public void InstallAsync()
        {
            ThreadPool.QueueUserWorkItem(x => Install());
        }

        private void Check(string version)
        {
            Check(version, true);
            if (Failed)
            {
                Check(version, false);
            }

            if (CheckCompleted != null)
            {
                CheckCompleted(this, EventArgs.Empty);
            }
        }

        private void Check(string version, bool useProxy)
        {
            if (HasUpdate)
            {
                return;
            }

            _hasUpdate = false;
            _downloadUrl = null;
            _exception = null;

            try
            {
                var webRequest = WebRequest.Create(UpdateUrl);

                if (useProxy)
                {
                    webRequest.Proxy = GetProxy();
                }

                var responseStream = webRequest.GetResponse().GetResponseStream();

                // ReSharper disable AssignNullToNotNullAttribute
                using (var streamReader = new StreamReader(responseStream))
                // ReSharper restore AssignNullToNotNullAttribute
                {
                    var page = streamReader.ReadToEnd();

                    var versionMatch = (new Regex(VersionCheck, RegexOptions.IgnoreCase)).Match(page);
                    if (!versionMatch.Success)
                    {
                        return;
                    }

                    var newVersion = versionMatch.Groups[VersionGroup].Value;
                    if(CompareVersions(version, newVersion) < 0)
                    {
                        _hasUpdate = true;
                        _downloadUrl = DownloadRootUrl + String.Format(SetupDownload, newVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        private void Download()
        {
            Download(true);
            if (Failed)
            {
                Download(false);
            }
            if (DownloadCompleted != null)
            {
                DownloadCompleted(this, EventArgs.Empty);
            }
        }

        private void Download(bool useProxy)
        {
            try
            {
                if (!HasUpdate || String.IsNullOrEmpty(_downloadUrl))
                {
                    throw new InvalidOperationException("DownloadUrl is not set.");
                }

                using (var webClient = new WebClient())
                {
                    if (useProxy)
                    {
                        webClient.Proxy = GetProxy();
                    }
                    _downloadedFile = Path.GetTempFileName();
                    webClient.DownloadFile(_downloadUrl, _downloadedFile);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    File.Delete(_downloadedFile);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
                _downloadedFile = null;
                _exception = ex;
            }
        }

        private void Install()
        {
            try
            {
                if (String.IsNullOrEmpty(_downloadedFile))
                {
                    throw new InvalidOperationException("DownloadedFile is not set.");
                }

                Explorer.ShellExecute(_downloadedFile, false);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        private static int CompareVersions(string version, string newVersion)
        {
            var separator = new [] {'.'};

            var v1 = version.Split(separator);
            var v2 = newVersion.Split(separator);

            if (v1.Length != v2.Length)
            {
                throw new InvalidOperationException("Version formats differ.");
            }

            return v1.Select((t, i) => Convert.ToInt32(t) - Convert.ToInt32(v2[i])).FirstOrDefault(diff => diff != 0);
        }

        private static IWebProxy GetProxy()
        {
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            return proxy;
        }

        private volatile bool _hasUpdate;
        private volatile string _downloadUrl;
        private volatile string _downloadedFile;
        private volatile Exception _exception;
    }
}
