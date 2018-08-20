using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Utils.System;

using Xps2ImgLib.Utils;

using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.Interfaces;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Utils
{
    public class UpdateManager : IUpdateManager
    {
        public const string ManualCheckUrl      = FilesUrl;
        public const string ManualDownloadUrl   = FilesUrl;

        private const string RootUrl            = "https://sourceforge.net/projects/xps2img/";
        private const string FilesUrl           = RootUrl + "files/Releases/";
        private const string SetupDownload      = "Xps2ImgSetup-{0}.exe";

        private static readonly string DownloadFolder = String.Format("xps2img-update-{0}", Guid.NewGuid().ToString().Split("-".ToCharArray()).First());
        private static readonly string SetupCommandLineArguments = String.Format("/dir=\"{0}\" /update /silent /nocancel {1} /lang={{0}}", AssemblyInfo.ApplicationFolder, GetPortableArguments("/portable", "/tasks=\"\""));

        private static string GetPortableArguments(params string[] arguments)
        {
            return String.Join(" ", arguments.Select(s => SettingsManager.IsPortable ? s : String.Empty).ToArray()).Trim();
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
            const SecurityProtocolType tls1X = (SecurityProtocolType)(768 | 3072); // SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12
            const SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | tls1X;

            try
            {
                ServicePointManager.SecurityProtocol |= securityProtocol;
            }
            catch (NotSupportedException)
            {
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    typeof(ServicePointManager).GetField("s_SecurityProtocolType", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, ServicePointManager.SecurityProtocol | securityProtocol);
                }
                catch
                {
                    // Ignored.
                }
            }
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
                Thread.Sleep(1000);

                _useProxy = false;
                CheckInternal(version);
            }

            if (Failed)
            {
                _useProxy = false;
            }

            CheckCompleted.SafeInvoke(this);
        }

        private void CheckInternal(string version)
        {
            _hasUpdate = false;
            _downloadUrl = null;
            _exception = null;
            _whatsNew = null;

            try
            {
                using (var webClient = CreateWebClient())
                {
                    var page = Encoding.UTF8.GetString(webClient.DownloadData(FilesUrl + "?nocache=" + Environment.TickCount));

                    var versionMatch = new Regex(@"xps2img(?:|setup)-(?<version>(?:\d+\.){3}\d+)", RegexOptions.IgnoreCase).Match(page);
                    if (!versionMatch.Success)
                    {
                        throw new InvalidDataException();
                    }

                    var newVersion = versionMatch.Groups["version"].Value;
                    if (CompareVersions(version, newVersion) < 0)
                    {
                        _hasUpdate = true;
                        _downloadUrl = FilesUrl + String.Format(SetupDownload, newVersion);
                        _whatsNew = GetWhatsNew(page, version);
                    }
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        private static string GetWhatsNew(string page, string version)
        {
            try
            {
                var match = new Regex(@"(?:\d+\.){3}\d+\s+(?:\d{4}/\d{2}/\d{2})[\s\S]+?(?=(?:%VERSION%)|\<)".Replace("%VERSION%", Regex.Escape(version))).Match(page);

                if (!match.Success)
                {
                    return String.Empty;
                }

                return new Regex(@"(\S+)\s+(\d{4}/\d{2}/\d{2})")
                           .Replace(new Regex(@"\[.\]\s*").Replace(HttpUtility.HtmlDecode(match.Value), Resources.Strings.WhatsNewBullet),
                                    m => String.Format(CultureInfo.InvariantCulture, Resources.Strings.WhatsNewDateFormat, m.Groups[1].Value, DateTime.ParseExact(m.Groups[2].Value, "yyyy'/'MM'/'dd", null))
                );
            }
            catch
            {
                return String.Empty;
            }
        }

        private class UpdateWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var httpWebRequest = base.GetWebRequest(address) as HttpWebRequest;
                if (httpWebRequest == null)
                {
                    return null;
                }

                httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                return httpWebRequest;
            }
        }

        private WebClient CreateWebClient()
        {
            return new UpdateWebClient { Proxy = GetProxy() };
        }

        private void DownloadAsyncInternal()
        {
            using (var webClient = CreateWebClient())
            {
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
                    throw new InvalidOperationException("Downloaded file is not found.");
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
            Explorer.ShellExecute(setup, requireAdmin, String.Format(SetupCommandLineArguments, LocalizationManager.CurrentUICulture.Name), requireAdmin ? "runas" : null);
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

        private volatile bool _useProxy;
        private volatile bool _hasUpdate;
        private volatile bool _isDownloadCancelled;

        private volatile Exception _exception;

        private volatile string _downloadUrl;
        private volatile string _downloadedFile;

        private volatile string _whatsNew;
    }
}
