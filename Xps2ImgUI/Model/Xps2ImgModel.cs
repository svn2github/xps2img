﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static readonly string ApplicationFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static readonly string Xps2ImgExecutable = Path.Combine(ApplicationFolder, "xps2img.exe");

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

        public void Launch(bool convertMode)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Conversion is in progress.");  
            }

            _convertMode = convertMode;

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

        public event EventHandler<ConvertionProgressEventArgs> OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;
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

        private Process StartProcess(string commandLine, Encoding consoleEncoding)
        {
            var processStartInfo = new ProcessStartInfo(Xps2ImgExecutable, commandLine)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = consoleEncoding,
                StandardErrorEncoding = consoleEncoding
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

        private List<Interval> GetDocumentIntervals()
        {
            var intervals = Interval.Parse(OptionsObject.Pages);
            using (var xpsDocument = new XpsDocument(OptionsObject.SrcFile, FileAccess.Read))
            {
                var fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();

                if (fixedDocumentSequence == null)
                {
                    return new List<Interval>();
                }

                var pageCount = fixedDocumentSequence.DocumentPaginator.PageCount;

                if (!intervals.LessThan(pageCount))
                {
                    return new List<Interval>();
                }

                intervals.Last().SetEndValue(pageCount);

                return intervals;
            }
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
            catch { }
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
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
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
        }
        
        private void Xps2ImgLaunchThread()
        {
            try
            {               
                _isRunning = true;
                _isErrorReported = false;
                _processExitCode = 0;
                _threadsLeft = 0;
                _pagesProcessed = 0;

                var consoleEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.GetConsoleFallbackUICulture().TextInfo.OEMCodePage);

                _threadsCount = OptionsObject.ActualProcessorsNumber;

                var splittedIntervals = new List<List<Interval>> { new List<Interval>() };

                if (!IsSingleProcessor)
                {
                    BoostProcessPriority(true);

                    var intervals = GetDocumentIntervals();

                    if (intervals.Any())
                    {
                        _pagesTotal = intervals.GetTotalLength();
                        splittedIntervals = intervals.SplitBy(_threadsCount);
                    }
                    else
                    {
                        _threadsCount = 1;
                        splittedIntervals = new List<List<Interval>> { new List<Interval> { new Interval() } };
                    }
                }

                _appMutex = new Mutex(true, _optionsHolder.OptionsObject.ParentAppMutexName);

                foreach (var t in splittedIntervals)
                {
                    var process = StartProcess(
                                    IsSingleProcessor
                                        ? FormatCommandLine(Options.ExcludedUIOptions)
                                        : String.Format("{0} -p \"{1}\"", FormatCommandLine(Options.ExcludedOnLaunch), IntervalUtils.ToString(t))
                                    , consoleEncoding);
                    ThreadPool.QueueUserWorkItem(_ => Xps2ImgProcessWaitThread(process));
                    Interlocked.Increment(ref _threadsLeft);
                }

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

                // https://connect.microsoft.com/VisualStudio/feedback/details/430646/thread-handle-leak#tabs
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }

        private void OutputDataReceivedWrapper(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data) || OutputDataReceived == null)
            {
                return;
            }

            var match = (IsSingleProcessor ? OutputRegex : FileNameRegex).Match(e.Data);
            if (!match.Success)
            {
                return;
            }

            int percent;
            string pages;

            if (IsSingleProcessor)
            {
                percent = Convert.ToInt32(match.Groups["percent"].Value);
                pages = match.Groups["pages"].Value;
            }
            else
            {
                var pageIndex = Interlocked.Increment(ref _pagesProcessed);
                percent = pageIndex*100 / _pagesTotal;
                pages = String.Format("{0}/{1}", pageIndex, _pagesTotal);
            }

            var file = match.Groups["file"].Value;
                
            OutputDataReceived(this, new ConvertionProgressEventArgs(percent, pages, file));
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
                ErrorDataReceived(this, e);
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

        private const string FileNameGroup = @".+?'(?<file>.+)'";

        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\].+\(\s*(?<pages>\d+/\d+)\)" + FileNameGroup);
        private static readonly Regex FileNameRegex = new Regex(FileNameGroup);

        private OptionsHolder<Options> _optionsHolder;

        private int _pagesTotal;
        private int _pagesProcessed;

        private int _threadsLeft;
        private int _threadsCount;

        private int _processExitCode;

        private ProcessPriorityClass _originalProcessPriorityClass;

        private bool _convertMode;

        private volatile bool _isErrorReported;
        private volatile bool _isRunning;

        private EventWaitHandle _cancelEvent;
        private Mutex _appMutex;
    }
}
