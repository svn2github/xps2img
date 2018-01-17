using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using Xps2Img.Shared.Internal;

using Xps2ImgLib.Utils;

namespace Xps2ImgUI.Model
{
    public partial class Xps2ImgModel
    {
        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\].+?\s*[^\d]+(?<page>\d+)\s+\(\s*(?<pages>\d+/\d+)\).+?'(?<file>.+)'");
        private static readonly Regex ErrorMessageRegex = new Regex(@"^(?<page>[^:]+):\s+(?<message>[\s\S]+)$");

        private void OutputDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var args = GetConversionProgressEventArgs(e.Data);
            if (args == null)
            {
                return;
            }

            RegisterLastProcessedPage(sender, args.Page);

            _progressStarted = true;

            OutputDataReceived.SafeInvoke(this, args);
        }

        private void RegisterLastProcessedPage(object sender, int page)
        {
            if (!CanResume)
            {
                return;
            }

            var lastConvertedPage = _processLastConvertedPage.First(p => ReferenceEquals(p.Process, sender));
            if (lastConvertedPage.Page != 0)
            {
                _processedIntervals.Set(lastConvertedPage.Page, false);
            }

            lastConvertedPage.Page = page;
        }

        private ConversionProgressEventArgs GetConversionProgressEventArgs(string data)
        {
            var match = OutputRegex.Match(data);
            if (!match.Success)
            {
                return null;
            }

            var pageIndex = _pagesProcessedDelta + Interlocked.Increment(ref _pagesProcessed);
            var percent = pageIndex*100.0 / PagesTotal;
            var pages = String.Format(Resources.Strings.PageOfPagesFormat, pageIndex, PagesTotal);
            var page = int.Parse(GetMatchedValue(match, "page", "0"), CultureInfo.InvariantCulture);
            var file = GetMatchedValue(match, "file", String.Empty);

            return new ConversionProgressEventArgs(percent, page, pages, file);
        }

        private void ErrorDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var errorMessage = ProcessOutput.Decode(e.Data);

            var isErrorReported = _isErrorReported;
            _isErrorReported = true;

            var args = GetConversionErrorEventArgs(errorMessage);

            if (FailedPageNumberRegistered(args))
            {
                return;
            }

            if (!isErrorReported)
            {
                ErrorDataReceived.SafeInvoke(this, args);
            }

            if (!IgnoreErrors)
            {
                Stop();
            }
        }

        private bool FailedPageNumberRegistered(ConversionErrorEventArgs args)
        {
            if (!IgnoreErrors || !args.Page.HasValue)
            {
                return false;
            }

            lock (_errorPages.SyncRoot)
            {
                _errorPages.Set(args.Page.Value, true);
            }

            return true;
        }

        private static ConversionErrorEventArgs GetConversionErrorEventArgs(string errorMessage)
        {
            var match = ErrorMessageRegex.Match(errorMessage);
            return new ConversionErrorEventArgs(GetMatchedValue(match, "message", errorMessage), GetMatchedValue(match, "page"));
        }

        private static string GetMatchedValue(Match match, string groupName, string defaultValue = null)
        {
            return match.Success ? match.Groups[groupName].Value : defaultValue;
        }

        private void ExitedHandler(object sender, EventArgs e)
        {
            var process = (Process) sender;

            process.Exited -= ExitedHandler;

            try
            {
                ExitCode = process.ExitCode;               
                FreeProcessResources(process);
            }
            // ReSharper disable once CatchAllClause
            catch
            {
                // ignored
            }
            finally
            {
                Interlocked.Decrement(ref _threadsLeft);
            }
        }
    }
}
