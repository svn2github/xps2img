using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using CommandLine;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Enums;
using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Progress;
using Xps2Img.Shared.Setup;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;

using Xps2ImgLib;
using Xps2ImgLib.Utils;

using Xps2Img.Utils;

using Timer = System.Threading.Timer;
using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2Img
{
    internal static class Program
    {
        private const int ForceExitAfterSecondsPassed = 5;

        private static volatile bool _cancelled;
        private static volatile bool _cancelledByUser;

        // ReSharper disable once NotAccessedField.Local
        private static Timer _cancellationTimer;

        private static bool _outToConsole;
        private static bool _launchedAsInternal;
        private static bool _launchedAsInteractive;
        private static bool _silent;
        private static string _srcFile;

        private static int _cursorLeft;
        private static int _cursorTop;

        private static volatile Converter.ProgressEventArgs _progressEventArgs;

        private static Estimated _estimated;
        private static Timer _estimatedTimer;

        private static readonly object ProgressLock = new object();

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

                _launchedAsInteractive = !(_launchedAsInternal = options.HasInternal);

                if (_launchedAsInternal)
                {
                    Console.OutputEncoding = Encoding.UTF8;
                    Win32.SetConsoleCP(Console.OutputEncoding.CodePage);

                    ThreadPool.QueueUserWorkItem(_ => WaitForCancellationThread(options));
                }
                else
                {
                    Win32.SetConsoleCtrlHandler(_ => RequestCancellation(true), true);
                }

                _outToConsole = !(Win32.IsOutputRedirected() || _launchedAsInternal);

                if (_outToConsole)
                {
                    _cursorLeft = Console.CursorLeft;
                    _cursorTop  = Console.CursorTop;
                }

                if (_launchedAsInteractive)
                {
                    _estimated = new Estimated();
                    _estimatedTimer = new Timer(_ => DisplayProgress(true), null, TimeSpan.Zero, Estimated.Interval);
                }

                _silent = options.Silent;
                _srcFile = options.SrcFile;

                Convert(options, () => _cancelled, out conversionStarted);

                if (_estimatedTimer != null)
                {
                    _estimatedTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    _estimatedTimer.Dispose();
                }

                exitCode = ExitCode;

                if (_launchedAsInteractive && exitCode == ReturnCode.OK)
                {
                    var hasProgressEventArgs = _progressEventArgs != null;
                    var totalPages = hasProgressEventArgs ? _progressEventArgs.ConverterState.TotalPages : 0;

                    WriteProgressLine(options.Clean
                                      ? Resources.Strings.Template_Cleared
                                      : hasProgressEventArgs
                                          ? _estimated.FormatRatio(totalPages, false, Resources.Strings.Template_Done)
                                          : String.Empty,
                                      totalPages, _estimated.Elapsed);
                }
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

        private static bool RequestCancellation(bool isUserCancelled = false)
        {
            _cancelled = true;
            _cancelledByUser = isUserCancelled;
            _cancellationTimer = new Timer(_ => Environment.Exit(ExitCode), null, (long)TimeSpan.FromSeconds(ForceExitAfterSecondsPassed).TotalMilliseconds, Timeout.Infinite);
            return true;
        }

        private static int ExitCode
        {
            get
            {
                return _cancelledByUser
                    ? ReturnCode.UserCancelled
                    : _launchedAsInternal
                        ? ReturnCode.InternalOK
                        : ReturnCode.OK;
            }
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

            RequestCancellation();
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
                    throw new ConversionFailedException(String.Format(Resources.Strings.Error_PagesRange, xps2Img.PageCount), ReturnCode.InvalidPages);
                }

                conversionStarted = true;

                pages.SetEndValue(xps2Img.PageCount);

                xps2Img.ConverterState.SetLastAndTotalPages(pages.Last().End, pages.GetTotalLength());

                try
                {
                    pages.ForEach(interval => xps2Img.Convert(GetParameters(options, interval)));
                }
                catch (ConversionException ex)
                {
                    if (ex is ConversionCancelledException)
                    {
                        return;
                    }

                    throw;
                }
                finally
                {
                    xps2Img.OnProgress -= OnProgress;
                    xps2Img.OnError -= OnError;
                }
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
                PageCrop = options.PageCrop,
                PageCropMargin = options.PageCropMargin,
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
            if (_progressFormatString != null)
            {
                return;
            }

            _progressFormatString = String.Format(
                                        converter.ConverterParameters.Clean
                                            ? Resources.Strings.Template_CleanProgress
                                            : Resources.Strings.Template_Progress,
                                        0.GetNumberFormat(args.ConverterState.LastPage, false),
                                        1.GetNumberFormat(args.ConverterState.TotalPages, false));
        }

        private static void OnProgress(object sender, Converter.ProgressEventArgs args)
        {
            var converter = (Converter) sender;

            if (_silent)
            {
                return;
            }

            _progressEventArgs = args;

            InitProgressFormatString(args, converter);
            DisplayProgress();
        }

        private static void DisplayProgress(bool fromTimer = false)
        {
            if (_silent)
            {
                return;
            }

            if (_launchedAsInternal)
            {
                DisplayProgressInternal(fromTimer);
                return;
            }
            
            lock (ProgressLock)
            {
                DisplayProgressInternal(fromTimer);
            }
        }

        private static void DisplayProgressInternal(bool fromTimer)
        {
            var args = _progressEventArgs;

            if (args == null)
            {
                return;
            }

            var converterState = args.ConverterState;

            TimeSpan timeLeft = TimeSpan.Zero, timeElapsed = TimeSpan.Zero;

            if (_launchedAsInteractive)
            {
                _estimated.Caclulate(converterState.Percent, fromTimer);

                timeLeft = _estimated.Left;
                timeElapsed = _estimated.Elapsed;
            }

            var percent = (int)converterState.Percent;

            WriteProgressLine(_progressFormatString,
                converterState.ActivePage,
                converterState.ActivePageIndex,
                converterState.TotalPages,
                args.FullFileName,
                percent,
                timeLeft,
                timeElapsed);

            if (_launchedAsInteractive)
            {
                Console.Title = String.Format(Resources.Strings.Template_ProgressTitle,
                    percent,
                    converterState.ActivePageIndex,
                    converterState.TotalPages,
                    Path.GetFileName(args.FullFileName),
                    Path.GetFileNameWithoutExtension(_srcFile),
                    timeLeft,
                    timeElapsed);
            }
        }

        private static void OnError(object sender, Converter.ErrorEventArgs args)
        {
            DisplayError(args.Exception);
        }

        private static int _lastProgressLineLength;

        private static void WriteProgressLine(string format, params object[] args)
        {
            if (_silent)
            {
                return;
            }

            if (!_outToConsole)
            {
                Console.WriteLine(format, args);
                return;
            }

            var progressLine = String.Format(format, args);
            
            var strLength = progressLine.Length;
            var spacesCount = _lastProgressLineLength - strLength;
            _lastProgressLineLength = strLength;
            Console.SetCursorPosition(_cursorLeft, _cursorTop);

            Console.WriteLine(spacesCount > 0 ? progressLine + new String('\x20', spacesCount) : progressLine);
        }
    }
}