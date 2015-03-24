﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using Xps2Img.Shared.Internal;

namespace Xps2ImgUI.Model
{
    public partial class Xps2ImgModel
    {
        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\][^\d]+(?<page>\d+)\s+\(\s*(?<pages>\d+/\d+)\).+?'(?<file>.+)'");
        private static readonly Regex ErrorMessageRegex = new Regex(@"^(?<page>[^:]+):\s+(?<message>[\s\S]+)$");

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

            var args = new ConversionProgressEventArgs(percent, pages, file);

            _progressStarted = true;

            outputDataReceived(this, args);
        }

        private void ErrorDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var errorMessage = ProcessOutput.Decode(e.Data);

            var errorDataReceived = ErrorDataReceived;
            if (errorDataReceived != null)
            {
                var isErrorReported = _isErrorReported;
                _isErrorReported = true;

                var args = GetConversionErrorEventArgs(errorMessage);

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

        private static ConversionErrorEventArgs GetConversionErrorEventArgs(string errorMessage)
        {
            var match = ErrorMessageRegex.Match(errorMessage);

            Func<string, string, string> getMatch = (g, d) => match.Success ? match.Groups[g].Value : d;

            return new ConversionErrorEventArgs(getMatch("message", errorMessage), getMatch("page", null));
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
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref _threadsLeft);
            }
        }
    }
}
