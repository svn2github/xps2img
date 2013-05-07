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

        private void OutputDataReceivedWrapper(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data) || OutputDataReceived == null)
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

            OutputDataReceived(this, new ConversionProgressEventArgs(percent, pages, file));
        }

        private void ErrorDataReceivedWrapper(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            if (ErrorDataReceived != null && !_isErrorReported)
            {
                _isErrorReported = true;

                var match = ErrorMessageRegex.Match(e.Data);

                Func<string, string, string> getMatch = (g, d) => match.Success ? match.Groups[g].Value : d;

                ErrorDataReceived(this, new ConversionErrorEventArgs(getMatch("message", e.Data), getMatch("page", null)));
            }

            Stop();
        }
    }
}
