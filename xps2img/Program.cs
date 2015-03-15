using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using CommandLine;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Setup;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;
using Xps2Img.Utils;

using Xps2ImgLib;
using Xps2ImgLib.Utils;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2Img
{
    internal static class Program
    {
        private static volatile bool _isCancelled;
        private static volatile bool _isUserCancelled;

        private static bool _launchedAsInternal;

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCP(int codePageId);

        [STAThread]
        private static int Main(string[] args)
        {
            SetupGuard.Enter();

            Parser.RegisterStringsSource<Shared.Resources.Strings>();

            var conversionStarted = false;

            Options options = null;
            int exitCode;

            try
            {
                if (CommandLine.CommandLine.IsUsageDisplayed<Options>(args))
                {
                    return ReturnCode.NoArgs;
                }

                options = CommandLine.CommandLine.Parse(args);

                if (options == null)
                {
                    return ReturnCode.InvalidArg;
                }

                LocalizationManager.SetUICulture(options.InternalCulture);

                if (options.ProcessPriority != ProcessPriorityClassTypeConverter.Auto)
                {
                    Process.GetCurrentProcess().PriorityClass = options.ProcessPriority;
                }

                if (options.CpuAffinity.HasValue && options.CpuAffinity != IntPtr.Zero)
                {
                    Process.GetCurrentProcess().ProcessorAffinity = options.CpuAffinity.Value;
                }

                _launchedAsInternal = options.HasInternal;

                if (_launchedAsInternal)
                {
                    Console.OutputEncoding = Encoding.UTF8;
                    SetConsoleCP(Console.OutputEncoding.CodePage);

                    ThreadPool.QueueUserWorkItem(_ => WaitForCancellationThread(options));
                }

                Win32.SetConsoleCtrlHandler(_ => _isUserCancelled = _isCancelled = true, true);

                Convert(options, () => _isCancelled, out conversionStarted);

                exitCode = _isUserCancelled
                                ? ReturnCode.UserCancelled
                                : _launchedAsInternal
                                    ? ReturnCode.InternalOK
                                    : ReturnCode.OK;
            }
            catch (Exception ex)
            {
                exitCode = DisplayError(ex);
            }

            if (conversionStarted && options.PostAction != PostAction.Exit)
            {
                SystemManagementUtils.Shutdown(options.PostAction);
            }

            return exitCode;
        }

        private static int DisplayError(Exception ex)
        {
            return CommandLine.CommandLine.DisplayError(ex, _launchedAsInternal);
        }

        private static void WaitForCancellationThread(Options options)
        {
            var parentAppMutex = Mutex.OpenExisting(options.InternalParentAppMutexName);
            var cancelEvent = new EventWaitHandle(false, EventResetMode.AutoReset, options.InternalCancelEventName);

            // ReSharper disable once EmptyGeneralCatchClause
            try
            {
                WaitHandle.WaitAny(new WaitHandle[] { cancelEvent, parentAppMutex });
            }
            catch
            {
            }

            _isCancelled = true;
        }

        private static void Convert(Options options, Func<bool> cancelConversionFunc, out bool conversionStarted)
        {
            conversionStarted = false;

            using (var xps2Img = Converter.Create(options.SrcFile, cancelConversionFunc))
            {
                xps2Img.OnProgress += OnProgress;
                xps2Img.OnError += OnError;

                var pages = options.SafePages;

                if (!pages.LessThan(xps2Img.PageCount))
                {
                    throw new ConvertException(String.Format(Resources.Strings.Error_PagesRange, xps2Img.PageCount), ReturnCode.InvalidPages);
                }

                conversionStarted = true;

                pages.SetEndValue(xps2Img.PageCount);

                xps2Img.ConverterState.SetLastAndTotalPages(pages.Last().End, pages.GetTotalLength());

                pages.ForEach(interval => xps2Img.Convert(GetParameters(options, interval)));

                xps2Img.OnProgress -= OnProgress;
                xps2Img.OnError -= OnError;
            }
        }

        private static Converter.Parameters GetParameters(Options options, Interval interval)
        {
            // Options always have default values set.
            // ReSharper disable PossibleInvalidOperationException
            return new Converter.Parameters
            {
                OutputDir = options.OutDir,
                StartPage = interval.Begin,
                EndPage = interval.End,
                ImageType = options.FileType,
                ShortenExtension = options.ShortenExtension,
                ImageOptions = new ImageOptions(options.JpegQuality.Value, options.TiffCompression),
                RequiredSize = options.RequiredSize,
                Dpi = options.Dpi.Value,
                BaseImageName = !String.IsNullOrEmpty(options.ImageName) ?
                                    options.ImageName :
                                    (options.ImageName == null ? String.Empty : null),
                FirstPageIndex = options.FirstPageIndex.Value,
                PrelimsPrefix = options.PrelimsPrefix,
                IgnoreExisting = options.IgnoreExisting,
                IgnoreErrors = options.IgnoreErrors,
                Test = options.Test,
                Silent = options.Silent,
                Clean = options.Clean,
                OutOfMemoryStrategyEnabled = true
            };
            // ReSharper restore PossibleInvalidOperationException
        }

        private static string _progressFormatString;

        private static void InitProgressFormatString(Converter.ProgressEventArgs args, Converter converter)
        {
            if (_progressFormatString == null)
            {
                _progressFormatString = String.Format(
                                            converter.ConverterParameters.Clean
                                                ? Resources.Strings.Template_CleanProgress
                                                : Resources.Strings.Template_Progress,
                                            0.GetNumberFormat(args.ConverterState.LastPage, false),
                                            1.GetNumberFormat(args.ConverterState.TotalPages, false));
            }
        }

        private static void OnProgress(object sender, Converter.ProgressEventArgs args)
        {
            var converter = (Converter) sender;

            if (!converter.ConverterParameters.Silent)
            {
                InitProgressFormatString(args, converter);

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

        private static void OnError(object sender, Converter.ErrorEventArgs args)
        {
            DisplayError(args.Exception);
        }
    }
}