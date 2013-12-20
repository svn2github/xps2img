//#define STUB_UPDATEMANAGER

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.System;

using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Utils
{
    public class UpdateManager : IUpdateManager
    {
        public const string ManualCheckUrl      = DownloadRootUrl;
        public const string ManualDownloadUrl   = DownloadRootUrl;

        private const string UpdateUrl          = "http://sourceforge.net/projects/xps2img/";
        private const string DownloadRootUrl    = "http://downloads.sourceforge.net/project/xps2img/Releases/";
        private const string ReadmeUrl          = UpdateUrl + "files/readme.txt/download";
        private const string SetupDownload      = "Xps2ImgSetup-{0}.exe";

        private const string VersionGroup       = "version";
        private const string VersionCheck       = @"/Releases/xps2img[^-]*-(?<version>(\.?\d+){4})";

        private static readonly string DownloadFolder = String.Format("xps2img-update-{0}", Guid.NewGuid().ToString().Split("-".ToCharArray()).First());
        private static readonly string SetupCommandLineArguments = String.Format("/dir=\"{0}\" /update /silent /nocancel {1}", AssemblyInfo.ApplicationFolder, GetPortableArguments("/portable", "/tasks=\"\""));

        private static string GetPortableArguments(params string[] arguments)
        {
            return String.Join(" ", arguments.Select(s => SettingsManager.IsPortable ? s : String.Empty).ToArray());
        }

        public bool HasUpdate
        {
            get { return _hasUpdate; }
        }

        public string WhatsNew
        {
            get { return _whatsNew; }
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

        public bool IsDownloadCancelled
        {
            get { return _isDownloadCancelled; }
            private set { _isDownloadCancelled = value; }
        }

        public event EventHandler CheckCompleted;
        public event EventHandler InstallationLaunched;
        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event ProgressChangedEventHandler DownloadProgressChanged;

        public static IUpdateManager Create()
        {
            #if STUB_UPDATEMANAGER
               #warning *** STUB USED *** Stubs.UpdateManager
               return new Stubs.UpdateManager();
            #else
                return new UpdateManager();
            #endif
        }

        private UpdateManager()
        {
        }

        public void CheckAsync(string version, bool silent = false)
        {
            Silent = silent;
            ThreadPool.QueueUserWorkItem(x => Check(version));
        }

        public void DownloadAsync()
        {
            ThreadPool.QueueUserWorkItem(x => DownloadAsyncInternal());
        }

        public void CancelDownload()
        {
            IsDownloadCancelled = true;
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
            _whatsNew = null;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(UpdateUrl + "?nocache=" + Environment.TickCount);

                AddCompressionSupport(httpWebRequest);

                httpWebRequest.Proxy = GetProxy();

                var responseStream = httpWebRequest.GetResponse().GetResponseStream();

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
                        _whatsNew = GetWhatsNew();
                    }
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        private string GetWhatsNew()
        {
            try
            {
                var webClient = new GZipWebClient { Proxy = GetProxy() };
                var readme = Encoding.UTF8.GetString(webClient.DownloadData(ReadmeUrl));

                var match = new Regex(@"([\s\S]+?)(\r?\n){2}(\d+\.){3}").Match(readme);

                if (!match.Success)
                {
                    return String.Empty;
                }

                return new Regex(@"(\S+)\s+(\d{4}/\d{2}/\d{2})").Replace(
                    new Regex(@"\[.\]\s*").Replace(match.Groups[1].Value, Resources.Strings.WhatsNewBullet),
                    m => String.Format(Resources.Strings.WhatsNewDateFormat, m.Groups[1].Value, DateTime.ParseExact(m.Groups[2].Value, "yyyy'/'MM'/'dd", null))
                );
            }
            catch
            {
                return String.Empty;
            }
        }

        private static HttpWebRequest AddCompressionSupport(HttpWebRequest httpWebRequest)
        {
            if (httpWebRequest == null)
            {
                return null;
            }

            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            return httpWebRequest;
        }

        private class GZipWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                return AddCompressionSupport((HttpWebRequest)base.GetWebRequest(address));
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

                webClient.Proxy = GetProxy();

                var downloadFolder = Path.Combine(Path.GetTempPath(), DownloadFolder);
                Directory.CreateDirectory(downloadFolder);

                // ReSharper disable AssignNullToNotNullAttribute
                _downloadedFile = Path.Combine(downloadFolder, Path.GetFileName(_downloadUrl));
                // ReSharper restore AssignNullToNotNullAttribute

                IsDownloadCancelled = false;

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

            webClient.Dispose();

            _exception = args.Error;

            var downloadFileCompleted = DownloadFileCompleted;
            if (downloadFileCompleted != null)
            {
                downloadFileCompleted(this, args);
            }
        }

        private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            if (IsDownloadCancelled)
            {
                ((WebClient)sender).CancelAsync();
                return;
            }

            var downloadProgressChanged = DownloadProgressChanged;
            if (downloadProgressChanged != null)
            {
                downloadProgressChanged(this, args);
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

                CheckAccessAndInstall(_downloadedFile);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            InstallationLaunched.SafeInvoke(this);
        }

        private static void CheckAccessAndInstall(string setup)
        {
            var requireAdmin = !IsDirectoryWritableForCurrentUser(AssemblyInfo.ApplicationFolder);
            Explorer.ShellExecute(setup, requireAdmin, SetupCommandLineArguments, requireAdmin ? "runas" : null);
        }

        private static readonly char[] VersionSeparator = { '.' };

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

        private static bool IsDirectoryWritableForCurrentUser(string path)
        {
            try
            {
                var writeAllow = false;
                var writeDeny = false;

                var accessRules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

                foreach (var rule in accessRules.Cast<FileSystemAccessRule>().Where(rule => (FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write))
                {
                    switch (rule.AccessControlType)
                    {
                        case AccessControlType.Allow:
                            writeAllow = true;
                            break;
                        case AccessControlType.Deny:
                            writeDeny = true;
                            break;
                    }
                }

                return writeAllow && !writeDeny;
            }
            catch
            {
                return false;
            }
        }

        private IWebProxy GetProxy()
        {
            if (!_useProxy)
            {
                return null;
            }

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
        private volatile bool _isDownloadCancelled;

        private volatile Exception _exception;

        private volatile string _downloadUrl;
        private volatile string _downloadedFile;

        private volatile string _whatsNew;
    }
}
