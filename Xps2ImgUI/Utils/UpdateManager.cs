﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

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

        private readonly string _downloadFolder = String.Format("xps2img-update-{0}", Guid.NewGuid().ToString().Split("-".ToCharArray()).First());

        public bool HasUpdate
        {
            set { _hasUpdate = value; }
            get { return _hasUpdate; }
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
        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public void CheckAsync(string version, bool silent = false)
        {
            Silent = silent;
            ThreadPool.QueueUserWorkItem(x => Check(version));
        }

        public void DownloadAsync()
        {
            ThreadPool.QueueUserWorkItem(x => DownloadAsyncInternal());
        }

        public void InstallAsync()
        {
            ThreadPool.QueueUserWorkItem(x => Install());
        }

        private void Check(string version)
        {
            _useProxy = true;
            CheckInternal(version);

            if (Failed)
            {
                Sleep();

                _useProxy = false;
                CheckInternal(version);
            }

            if (Failed)
            {
                _useProxy = false;
            }

            if (CheckCompleted != null)
            {
                CheckCompleted(this, EventArgs.Empty);
            }
        }

        private void CheckInternal(string version)
        {
            _hasUpdate = false;
            _downloadUrl = null;
            _exception = null;

            try
            {
                var webRequest = WebRequest.Create(UpdateUrl);

                if (_useProxy)
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

        private void DownloadAsyncInternal()
        {
            var webClient = new WebClient();

            webClient.DownloadFileCompleted += WebClientDownloadFileCompleted;
            webClient.DownloadProgressChanged += WebClientDownloadProgressChanged;

            try
            {
                if (!HasUpdate || String.IsNullOrEmpty(_downloadUrl))
                {
                    throw new InvalidOperationException("DownloadUrl is not set.");
                }

                if (_useProxy)
                {
                    webClient.Proxy = GetProxy();
                }

                var downloadFolder = Path.Combine(Path.GetTempPath(), _downloadFolder);
                Directory.CreateDirectory(downloadFolder);

                // ReSharper disable AssignNullToNotNullAttribute
                _downloadedFile = Path.Combine(downloadFolder, Path.GetFileName(_downloadUrl));
                // ReSharper restore AssignNullToNotNullAttribute

                webClient.DownloadFileAsync(new Uri(_downloadUrl), _downloadedFile);
            }
            catch (Exception ex)
            {
                webClient.DownloadFileCompleted -= WebClientDownloadFileCompleted;
                webClient.DownloadProgressChanged -= WebClientDownloadProgressChanged;

                try
                {
                    File.Delete(_downloadedFile);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
                _exception = ex;
            }
        }

        private void WebClientDownloadFileCompleted(object sender, AsyncCompletedEventArgs args)
        {
            var webClient = (WebClient) sender;

            webClient.DownloadFileCompleted -= WebClientDownloadFileCompleted;
            webClient.DownloadProgressChanged -= WebClientDownloadProgressChanged;

            _exception = args.Error;
            if (DownloadFileCompleted != null)
            {
                DownloadFileCompleted(sender, args);
            }
        }

        private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(sender, args);
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
                Application.Exit();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        private static readonly char[] VersionSeparator = new[] { '.' };

        private static int CompareVersions(string version, string newVersion)
        {
            var v1 = version.Split(VersionSeparator);
            var v2 = newVersion.Split(VersionSeparator);

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

        private static void Sleep()
        {
            Thread.Sleep(1000);
        }

        private volatile bool _useProxy;
        private volatile bool _hasUpdate;

        private volatile Exception _exception;

        private volatile string _downloadUrl;
        private volatile string _downloadedFile;
    }
}
