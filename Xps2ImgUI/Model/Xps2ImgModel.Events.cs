using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Xps2ImgUI.Model
{
    public partial class Xps2ImgModel
    {
        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\][^\d]+(?<page>\d+)\s+\(\s*(?<pages>\d+/\d+)\).+?'(?<file>.+)'");
        private static readonly Regex ErrorMessageRegex = new Regex(@"^(?<page>[^:]+):\s*(?<message>.+)$");

        private void OutputDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            var outputDataReceived = OutputDataReceived;
            if (String.IsNullOrEmpty(e.Data) || outputDataReceived == null)
            {
                return;
            }

            var match = OutputRegex.Match(e.Data);
            if (!match.Success)
            {
                return;
            }

            var pageIndex = _pagesProcessedDelta + Interlocked.Increment(ref _pagesProcessed);
            var percent = pageIndex * 100 / PagesTotal;
            var pages = String.Format(Resources.Strings.PageOfPagesFormat, pageIndex, PagesTotal);

            if (CanResume)
            {
                var lastConvertedPage = _processLastConvertedPage.First(p => ReferenceEquals(p.Process, sender));
                if (lastConvertedPage.Page != 0)
                {
                    _processedIntervals.Set(lastConvertedPage.Page, false);
                }

                lastConvertedPage.Page = int.Parse(match.Groups["page"].Value, CultureInfo.InvariantCulture);
            }

            var file = match.Groups["file"].Value;

            _progressStarted = true;

            outputDataReceived(this, new ConversionProgressEventArgs(percent, pages, file));
        }

        private void ErrorDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var errorDataReceived = ErrorDataReceived;
            if (errorDataReceived != null)
            {
                var isErrorReported = _isErrorReported;
                _isErrorReported = true;

                var match = ErrorMessageRegex.Match(e.Data);

                Func<string, string, string> getMatch = (g, d) => match.Success ? match.Groups[g].Value : d;
                
                var args = new ConversionErrorEventArgs(getMatch("message", e.Data), getMatch("page", null));

                if (OptionsObject.IgnoreErrors && args.Page.HasValue)
                {
                    lock (_errorPages.SyncRoot)
                    {
                        _errorPages.Set(args.Page.Value, true);
                    }
                    return;
                }

                if (!isErrorReported)
                {
                    errorDataReceived(this, args);
                }
            }

            if (!OptionsObject.IgnoreErrors)
            {
                Stop();
            }
        }
    }
}
