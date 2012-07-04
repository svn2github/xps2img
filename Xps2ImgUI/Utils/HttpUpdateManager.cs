using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils
{
    public class HttpUpdateManager
    {
        public HttpUpdateManager()
        {
        }

        private const string UpdateUrl          = "http://sourceforge.net/projects/xps2img/";
        private const string DownloadUrl        = "http://downloads.sourceforge.net/project/xps2img/Releases/";
        private const string PortableDownload   = "xps2img-{0}.7z";
        private const string SetupDownload      = "Xps2ImgSetup-{0}.exe";

        private const string VersionGroup       = "version";
        private const string VersionCheck       = @"/Releases/xps2img[^-]*-(?<version>(\.?\d+){4})";

        public void Check(Func<bool> downloadAndInstall = null, Action<string> checkFailed = null)
        {
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;

           var webRequest = WebRequest.Create(UpdateUrl);

            webRequest.Proxy = proxy;

            var responseStream = webRequest.GetResponse().GetResponseStream();
            using (var streamReader = new StreamReader(responseStream))
            {
                var page = streamReader.ReadToEnd();

                var versionMatch = (new Regex(VersionCheck, RegexOptions.IgnoreCase)).Match(page);
                if (!versionMatch.Success)
                {
                    if (checkFailed != null)
                    {
                        checkFailed(UpdateUrl);
                    }
                    return;
                }

                var version = versionMatch.Groups[VersionGroup].Value;
                var messageBoxResult = MessageBox.Show(GetDownloadUrl(version) + "\n" + GetDownloadUrl(version, false), "Files", MessageBoxButtons.YesNoCancel);
                if (messageBoxResult == DialogResult.Cancel)
                {
                    return;
                }

                var downloadUrl = GetDownloadUrl(version, messageBoxResult == DialogResult.Yes);
                using (var webClient = new WebClient())
                {
                    webClient.Proxy = proxy;
                    webClient.DownloadFile(downloadUrl, @"m:\_2sort\" + Path.GetFileName(downloadUrl));
                }
            }
        }

        private static string GetDownloadUrl(string version, bool isSetup = true)
        {
            return DownloadUrl + String.Format(isSetup ? SetupDownload : PortableDownload, version);
        }
    }
}
