﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Xps2Img.CommandLine;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Setup;
using Xps2Img.Xps2Img;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2Img
{
    internal static class Program
    {
        private static volatile bool _isCancelled;
        private static volatile bool _isUserCancelled;

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCP(int wCodePageID);

        [STAThread]
        private static int Main(string[] args)
        {
            SetupGuard.Enter();

            try
            {
                if (CommandLine.CommandLine.IsUsageDisplayed<Options>(args))
                {
                    return ReturnCode.NoArgs;
                }

                var options = CommandLine.CommandLine.Parse(args);

                if (options == null)
                {
                    return ReturnCode.InvalidArg;
                }

                if (options.ProcessorsPriorityAsEnum != ProcessPriorityClass.Normal)
                {
                    Process.GetCurrentProcess().PriorityClass = options.ProcessorsPriorityAsEnum;
                }

#if fsdfsdf
                if (options.ActualCpuAffinity != IntPtr.Zero)
                {
                    Process.GetCurrentProcess().ProcessorAffinity = options.ActualCpuAffinity;
                }
#endif

                var launchedAsInternal = !String.IsNullOrEmpty(options.CancellationObjectIds);

                if (launchedAsInternal)
                {
                    Console.OutputEncoding = Encoding.UTF8;
                    SetConsoleCP(Console.OutputEncoding.CodePage);

                    ThreadPool.QueueUserWorkItem(_ => WaitForCancellationThread(options));
                }

                Win32.SetConsoleCtrlHandler(_ => _isUserCancelled = _isCancelled = true, true);

                Convert(options, () => _isCancelled);

                return _isUserCancelled
                            ? ReturnCode.UserCancelled
                            : launchedAsInternal
                                ? ReturnCode.InternalOK
                                : ReturnCode.OK;
            }
            catch (Exception ex)
            {
                return CommandLine.CommandLine.DisplayError(ex);
            }
        }

        private static void WaitForCancellationThread(Options options)
        {
            var parentAppMutex = Mutex.OpenExisting(options.ParentAppMutexName);
            var cancelEvent = new EventWaitHandle(false, EventResetMode.AutoReset, options.CancellationEventName);
            
            try
            {
                WaitHandle.WaitAny(new WaitHandle[] { cancelEvent, parentAppMutex });
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }

            _isCancelled = true;
        }

        private static void Convert(Options options, Func<bool> cancelConversionFunc)
        {
            using (var xps2Img = Converter.Create(options.SrcFile, cancelConversionFunc))
            {
                xps2Img.OnProgress += OnProgress;

                if (!options.Pages.LessThan(xps2Img.PageCount))
                {
                    throw new ConversionException(String.Format(Resources.Strings.Error_PagesRange, xps2Img.PageCount), ReturnCode.InvalidPages);
                }

                options.Pages.SetEndValue(xps2Img.PageCount);

                xps2Img.ConverterState.SetLastAndTotalPages(options.Pages.Last().End, options.Pages.GetTotalLength());

                // Options always have default values set.
                // ReSharper disable PossibleInvalidOperationException
                options.Pages.ForEach(interval =>
                    xps2Img.Convert(
                        new Converter.Parameters
                        {
                            Silent = options.Silent,
                            Test = options.Test,
                            StartPage = interval.Begin,
                            EndPage = interval.End,
                            ImageType = options.FileType,
                            ShortenExtension = options.ShortenExtension,
                            ImageOptions = new ImageOptions(options.JpegQuality.Value, options.TiffCompression),
                            RequiredSize = options.RequiredSize,
                            Dpi = options.Dpi.Value,
                            OutputDir = options.OutDir,
                            BaseImageName = !String.IsNullOrEmpty(options.ImageName) ?
                                              options.ImageName :
                                              (options.ImageName == null ? String.Empty : null),
                            FirstPageIndex = options.FirstPageIndex.Value,
                            PrelimsPrefix = options.PrelimsPrefix,
                            Clean = options.Clean
                        }
                    )
                );
                // ReSharper restore PossibleInvalidOperationException

                xps2Img.OnProgress -= OnProgress;
            }
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
                                                converter.ConverterParameters.Clean ? Resources.Strings.Template_CleanProgress : Resources.Strings.Template_Progress,
                                                0.GetNumberFormat(args.ConverterState.LastPage, false),
                                                1.GetNumberFormat(args.ConverterState.LastPage, false));
                }

                Console.WriteLine(_progressFormatString,
                                    args.ConverterState.ActivePage,
                                    args.ConverterState.ActivePageIndex,
                                    args.ConverterState.TotalPages,
                                    args.FullFileName,
                                    (int)args.ConverterState.Percent);
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