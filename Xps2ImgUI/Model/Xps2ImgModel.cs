using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Xps.Packaging;

using Xps2Img.CommandLine;

using Xps2ImgUI.Attributes.OptionsHolder;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Model
{
    public class Xps2ImgModel
    {
        public Xps2ImgModel()
            : this(null)
        {
        }

        public Xps2ImgModel(Options options)
        {
            InitOptionsHolder();

            _optionsHolder.OptionsObject = options ?? new Options();

            if (OptionsObject.ImageName == String.Empty)
            {
                OptionsObject.ImageName = Option.Empty;
            }
        }

        public void Init()
        {
            var initOptions = _optionsHolder != null ? _optionsHolder.OptionsObject : new Options();

            InitOptionsHolder();

            Debug.Assert(_optionsHolder != null);

            _optionsHolder.OptionsObject = initOptions;
        }

        private void InitOptionsHolder()
        {
            if (_optionsHolder != null)
            {
                _optionsHolder.OptionsObjectChanged -= OptionsHolderOptionsObjectChanged;
            }

            _optionsHolder = new OptionsHolder<Options>();
            _optionsHolder.OptionsObjectChanged += OptionsHolderOptionsObjectChanged;
        }

        public void Reset()
        {
            InitOptionsHolder();
            _optionsHolder.OptionsObject = new Options();
        }

        public void ResetParameters()
        {
            ResetByCategory(Category.Parameters);
        }

        public void ResetOptions()
        {
            ResetByCategory(Category.Options);
        }

        public void Launch(ConversionType conversionType)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Conversion is in progress.");  
            }

            _conversionType = conversionType;

            CancelEvent.Reset();

            var xps2ImgLaunchThread = new Thread(Xps2ImgLaunchThread);

            xps2ImgLaunchThread.SetApartmentState(ApartmentState.STA);
            xps2ImgLaunchThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            try
            {
                CancelEvent.Set();
            }
            catch(InvalidOperationException)
            {
            }
        }

        public string FormatCommandLine(params string[] optionsToExclude)
        {
            return _optionsHolder.FormatCommandLine(false, optionsToExclude);
        }

        public Options OptionsObject
        {
            get { return _optionsHolder.OptionsObject; }
        }

        public string FirstRequiredOptionLabel
        {
            get { return _optionsHolder.FirstRequiredOptionLabel; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public bool IsConversionFailed
        {
            get { return _isErrorReported; }
        }

        public bool IsProgressStarted
        {
            get { return _progressStarted; }
        }

        public bool IsStopPending
        {
            get { return CancelEvent.WaitOne(0); }
        }

        private EventWaitHandle CancelEvent
        {
            get { return _cancelEvent ?? (_cancelEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _optionsHolder.OptionsObject.CancellationEventName)); }
        }

        private bool IsSingleProcessor
        {
            get { return _threadsCount == 1; }
        }

        public event EventHandler<ConversionProgressEventArgs> OutputDataReceived;
        public event EventHandler<ConversionErrorEventArgs> ErrorDataReceived;
        public event EventHandler Completed;

        public event EventHandler OptionsObjectChanged;

        public event ThreadExceptionEventHandler LaunchFailed;
        public event EventHandler LaunchSucceeded;

        private void FreeProcessResources(Process process)
        {
            process.CancelOutputRead();
            process.CancelErrorRead();

            process.OutputDataReceived -= OutputDataReceivedWrapper;
            process.ErrorDataReceived -= ErrorDataReceivedWrapper;

            process.Close();
        }

        private readonly Encoding _consoleEncoding = Encoding.UTF8;

        private Process StartProcess(string commandLine)
        {
            var processStartInfo = new ProcessStartInfo(Program.Xps2ImgExecutable, commandLine)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = _consoleEncoding,
                StandardErrorEncoding = _consoleEncoding
            };

            var process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };

            process.OutputDataReceived += OutputDataReceivedWrapper;
            process.ErrorDataReceived += ErrorDataReceivedWrapper;

            process.Start();

            try
            {
                process.PriorityClass = OptionsObject.ActualProcessorsPriority;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }

        private int GetDocumentPageCount()
        {
            using (var xpsDocument = new XpsDocument(OptionsObject.SrcFile, FileAccess.Read))
            {
                var fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();
                return fixedDocumentSequence == null ? 0 : fixedDocumentSequence.DocumentPaginator.PageCount;
            }
        }

        private List<Interval> GetDocumentIntervals()
        {
            var intervals = Interval.Parse(OptionsObject.Pages);

            var pageCount = GetDocumentPageCount();

            if (pageCount == 0 || !intervals.LessThan(pageCount))
            {
                return new List<Interval>();
            }

            intervals.Last().SetEndValue(pageCount);

            return intervals;
        }

        private IEnumerable<List<Interval>> GetDocumentSplittedIntervals()
        {
            _pagesProcessedDelta = 0;

            if (!IsSingleProcessor)
            {
                BoostProcessPriority(true);
            }

            var totalIntervals = GetDocumentIntervals();
            var intervals = CanResume ? IntervalUtils.FromBitArray(_processedIntervals) : totalIntervals;

            if (intervals.Any())
            {
                _pagesTotal = totalIntervals.GetTotalLength();
                if (_pagesTotal <= 0) _pagesTotal = 1;

                if (CanResume)
                {
                    _pagesProcessedDelta = totalIntervals.GetTotalLength() - intervals.GetTotalLength();
                    if (_pagesProcessedDelta < 0) _pagesProcessedDelta = 0;
                }

                _processedIntervals = intervals.ToBitArray();
                return intervals.SplitBy(_threadsCount);
            }

            _threadsCount = 1;
            _processedIntervals = null;

            return new List<List<Interval>> { new List<Interval>() };
        }

        private void Xps2ImgProcessWaitThread(Process process)
        {
            try
            {
                process.WaitForExit();
                if(process.ExitCode >= 0)
                {
                    Interlocked.CompareExchange(ref _processExitCode, 1, 0);
                }
                Interlocked.Decrement(ref _threadsLeft);
                FreeProcessResources(process);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        private void BoostProcessPriority(bool boost)
        {
            try
            {
                var process = Process.GetCurrentProcess();

                var processPriorityClass = _originalProcessPriorityClass;
                
                if (boost)
                {
                    _originalProcessPriorityClass = process.PriorityClass;

                    switch (OptionsObject.ActualProcessorsPriority)
                    {
                        case ProcessPriorityClass.Normal:
                            processPriorityClass = ProcessPriorityClass.AboveNormal;
                            break;
                        case ProcessPriorityClass.AboveNormal:
                            processPriorityClass = ProcessPriorityClass.High;
                            break;
                        default:
                            return;
                    }
                }

                process.PriorityClass = processPriorityClass;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        private void WaitAllProcessThreads(bool checkExitCode)
        {
            while (Interlocked.CompareExchange(ref _threadsLeft, 0, 0) != 0)
            {
                Thread.Sleep(500);
                if (checkExitCode && Interlocked.CompareExchange(ref _processExitCode, 0, 0) == 1)
                {
                    throw new Exception(Resources.Strings.ProcessorHasTerminated);
                }
            }

            CanResume = IsStopPending;
        }

        private void Xps2ImgLaunchThread()
        {
            try
            {
                if (IsResumeMode && !CanResume)
                {
                    throw new InvalidOperationException("Resume is not available.");
                }

                if (!IsResumeMode)
                {
                    CanResume = false;
                }

                _isRunning = true;
                _isErrorReported = false;
                _progressStarted = false;

                _processExitCode = 0;
                _threadsLeft = 0;
                _pagesProcessed = 0;

                _threadsCount = IsCreationMode ? OptionsObject.ActualProcessorsNumber : 1;
                
                var splittedIntervals = GetDocumentSplittedIntervals();

                _appMutex = new Mutex(true, _optionsHolder.OptionsObject.ParentAppMutexName);

                _processLastConvertedPage = null;

                var processLastConvertedPage = new List<ProcessLastPage>();

                foreach (var t in splittedIntervals)
                {
                    var processCommandLine = String.Format("{0} -p \"{1}\" {2}",
                                                FormatCommandLine(Options.ExcludedOnLaunch),
                                                IntervalUtils.ToString(t),
                                                IsCreationMode ? String.Empty : Options.CleanOption);
                    var process = StartProcess(processCommandLine);
                    ThreadPool.QueueUserWorkItem(_ => Xps2ImgProcessWaitThread(process));
                    Interlocked.Increment(ref _threadsLeft);
                    processLastConvertedPage.Add(new ProcessLastPage(process));
                }

                _processLastConvertedPage = _processedIntervals != null ? processLastConvertedPage.ToArray() : null;

                if (LaunchSucceeded != null)
                {
                    LaunchSucceeded(this, EventArgs.Empty);
                }

                WaitAllProcessThreads(true);

                if (Completed != null)
                {
                    Completed(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Stop();

                WaitAllProcessThreads(false);

                if (LaunchFailed == null)
                {
                    throw;
                }

                if (!_isErrorReported)
                {
                    LaunchFailed(this, new ThreadExceptionEventArgs(ex));
                }
            }
            finally
            {
                _isRunning = false;

                if (_appMutex != null)
                {
                    _appMutex.Close();
                    _appMutex = null;
                }

                if (!IsSingleProcessor)
                {
                    BoostProcessPriority(false);
                }
            }
        }

        private bool IsCreationMode
        {
            get { return IsConvertMode || IsResumeMode; }
        }

        private bool IsConvertMode
        {
            get { return _conversionType == ConversionType.Convert; }
        }
        
        private bool IsResumeMode
        {
            get { return _conversionType == ConversionType.Resume; }
        }

        public bool IsDeleteMode
        {
            get { return _conversionType == ConversionType.Delete; }
        }

        public bool CanResume
        {
            get
            {
                return 
                    _processLastConvertedPage != null &&
                    _processedIntervals != null;
            }
            set
            {
                if(!value)
                {
                    _processLastConvertedPage = null;
                    _processedIntervals = null;
                }
            }
        }

        public bool ShutdownRequested { get; set; }

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
            var percent = pageIndex * 100 / _pagesTotal;
            var pages = String.Format("{0}/{1}", pageIndex, _pagesTotal);

            if (CanResume)
            {
                var lastConvertedPage = _processLastConvertedPage.First(p => ReferenceEquals(p.Process, sender));
                if (lastConvertedPage.Page != 0)
                {
                    _processedIntervals.Set(lastConvertedPage.Page, false);
                }

                lastConvertedPage.Page = Int32.Parse(match.Groups["page"].Value);
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
                ErrorDataReceived(this, new ConversionErrorEventArgs(CleanErrorMessageRegex.Replace(e.Data, String.Empty)));
            }

            Stop();
        }

        private void OptionsHolderOptionsObjectChanged(object sender, EventArgs e)
        {
            FireOptionsObjectChanged();
        }

        private void FireOptionsObjectChanged()
        {
            if (OptionsObjectChanged != null)
            {
                OptionsObjectChanged(this, EventArgs.Empty);
            }
        }

        private void ResetByCategory(string category)
        {
            ReflectionUtils.SetDefaultValues(OptionsObject, pi => category == (pi.FirstOrDefaultAttribute<CategoryAttribute>() ?? new CategoryAttribute()).Category);
            FireOptionsObjectChanged();
        }

        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\][^\d]+(?<page>\d+)\s+\(\s*(?<pages>\d+/\d+)\).+?'(?<file>.+)'");
        private static readonly Regex CleanErrorMessageRegex = new Regex(@"^Error:\s*");

        private class ProcessLastPage
        {
            public ProcessLastPage(Process process)
            {
                Process = process;
            }

            public readonly Process Process;

            public int Page { get; set; }
        }

        private ProcessLastPage[] _processLastConvertedPage;
        private BitArray _processedIntervals;

        private OptionsHolder<Options> _optionsHolder;

        private int _pagesTotal;
        private int _pagesProcessed;
        private int _pagesProcessedDelta;

        private int _threadsLeft;
        private int _threadsCount;

        private int _processExitCode;

        private ProcessPriorityClass _originalProcessPriorityClass;

        private ConversionType _conversionType;

        private volatile bool _isErrorReported;
        private volatile bool _isRunning;
        private volatile bool _progressStarted;

        private EventWaitHandle _cancelEvent;
        private Mutex _appMutex;
    }
}
