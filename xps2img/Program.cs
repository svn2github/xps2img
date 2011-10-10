using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using CommandLine;

using Xps2Img.CommandLine;
using Xps2Img.Xps2Img;

namespace Xps2Img
{
    internal static class Program
    {
        private static volatile bool _isCancelled;

        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                if (CommandLine.CommandLine.IsUsageDisplayed<Options>(args))
                {
                    return (int)CommandLine.CommandLine.ReturnCode.NoArgs;
                }

                var options = CommandLine.CommandLine.Parse(args);

                if (options == null)
                {
                    return (int)CommandLine.CommandLine.ReturnCode.InvalidArg;
                }

                Trace.WriteLine(Parser.ToCommandLine(options));
                
                if (!String.IsNullOrEmpty(options.CancellationObjectId))
                {
                    var cancelEvent = new EventWaitHandle(false, EventResetMode.AutoReset, options.CancellationObjectId);
                    ThreadPool.QueueUserWorkItem(_ => _isCancelled = cancelEvent.WaitOne(Timeout.Infinite));
                }

                return Convert(options, () => _isCancelled);
            }
            catch (Exception ex)
            {
                return CommandLine.CommandLine.DisplayError(ex);
            }
        }

        private static int Convert(Options options, Func<bool> cancelConvertionFunc)
        {
            using (var xps2Img = Converter.Create(options.SrcFile, cancelConvertionFunc))
            {
                xps2Img.OnProgress += OnProgress;

                options.Pages.SetEndValue(xps2Img.PageCount);

                xps2Img.ConverterState.SetLastAndTotalPages(options.Pages.Last().End, options.Pages.GetTotalLength());

                options.Pages.ForEach(interval =>
                    xps2Img.Convert(
                        new Converter.Parameters
                        {
                            Silent = options.Silent,
                            Test = options.Test,
                            StartPage = interval.Begin,
                            EndPage = interval.End,
                            ImageType = options.FileType,
                            ImageOptions = new ImageOptions(options.JpegQuality, options.TiffCompression),
                            RequiredSize = options.RequiredSize,
                            Dpi = options.Dpi,
                            OutputDir = options.OutDir,
                            BaseImageName = !String.IsNullOrEmpty(options.ImageName) ?
                                              options.ImageName :
                                              (options.ImageName == null ? String.Empty : null),
                            FirstPageIndex = options.FirstPageIndex,
                            PrelimsPrefix = options.PrelimsPrefix
                        }
                    )
                );

                xps2Img.OnProgress -= OnProgress;
            }

            return (int)CommandLine.CommandLine.ReturnCode.OK;
        }

        private static string _progressFormatString;

        private static void OnProgress(object sender, Converter.ProgressEventArgs args)
        {
            var converter = (Converter) sender;

            if (!converter.ConverterParameters.Silent)
            {
                if (_progressFormatString == null)
                {
                    _progressFormatString = String.Format(
                                                Resources.Strings.Template_Progress,
                                                0.GetNumberFormat(args.ConverterState.LastPage, false),
                                                1.GetNumberFormat(args.ConverterState.LastPage, false));
                }

                Console.WriteLine(String.Format(_progressFormatString,
                                    args.ConverterState.ActivePage,
                                    args.ConverterState.ActivePageIndex,
                                    args.ConverterState.TotalPages,
                                    args.FullFileName,
                                    (int)args.ConverterState.Percent));
            }

            Console.Title = String.Format(Resources.Strings.Template_ProgressTitle,
                                (int)args.ConverterState.Percent,
                                args.ConverterState.ActivePageIndex,
                                args.ConverterState.TotalPages,
                                Path.GetFileName(args.FullFileName),
                                Path.GetFileNameWithoutExtension(converter.XpsFileName));
        }
    }
}