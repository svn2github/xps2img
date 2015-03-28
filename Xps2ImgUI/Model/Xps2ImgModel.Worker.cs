﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using CommandLine.Utils;

using Xps2Img.Shared.CommandLine;

using Xps2ImgLib;
using Xps2ImgLib.Utils;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2ImgUI.Model
{
    public partial class Xps2ImgModel
    {
        private void LaunchInternal(ConversionType conversionType)
        {
            _conversionType = conversionType;

            IsStopPending = false;

            ThreadPool.QueueUserWorkItem(_ => WorkerThread());
        }

        private void WorkerInit()
        {
            _isRunning = true;
            _isErrorReported = false;
            _progressStarted = false;
            _userCancelled = false;

            _processExitCode = ExitOK;
            _threadsLeft = 0;
            _pagesProcessed = 0;

            _threadsCount = IsCreationMode ? OptionsObject.SafeProcessorsNumber : 1;

            _errorPages = new BitArray(1);
        }

        private void WorkerStart()
        {
            var splittedIntervals = GetDocumentSplittedIntervals();

            _appMutex = new Mutex(true, _optionsHolder.OptionsObject.InternalParentAppMutexName);

            _processLastConvertedPage = null;

            var processLastConvertedPage = new List<ProcessLastPage>();

            foreach (var t in splittedIntervals)
            {
                var processCommandLine = String.Format("{0} -" + Options.ShortOptions.Pages + " \"{1}\" {2}",
                                            FormatCommandLine(Options.ExcludedOnLaunch),
                                            IntervalUtils.ToString(t),
                                            IsCreationMode ? String.Empty : Options.Names.Clean);
                var process = StartProcess(processCommandLine);
                Interlocked.Increment(ref _threadsLeft);
                processLastConvertedPage.Add(new ProcessLastPage(process));
            }

            _processLastConvertedPage = _processedIntervals != null ? processLastConvertedPage.ToArray() : null;
        }

        private void WorkerCleanup()
        {
            _isRunning = false;

            IsStopPending = false;

            if (_appMutex != null)
            {
                _appMutex.Close();
                _appMutex = null;
            }

            if (!IsSingleProcessor)
            {
                BoostProcessPriority(false);
            }

            if (_userCancelled)
            {
                ExitCode = ReturnCode.UserCancelled;
            }
        }

        private void WorkerError(Exception ex)
        {
            Stop();

            WaitAllWorkers(false);

            var launchFailed = LaunchFailed;
            if (launchFailed == null)
            {
                throw ex;
            }

            ExitCode = ReturnCode.Failed;

            if (!_isErrorReported)
            {
                launchFailed(this, new ThreadExceptionEventArgs(ex));
            }
        }

        private void WorkerCheckPreconitions()
        {
            if (IsResumeMode && !CanResume)
            {
                throw new InvalidOperationException(Resources.Strings.UnexpectedResumeIsNotAvailable);
            }

            if (!IsResumeMode)
            {
                CanResume = false;
            }
        }

        private void WorkerThread()
        {
            var failed = false;

            try
            {
                WorkerCheckPreconitions();
                WorkerInit();
                WorkerStart();

                LaunchSucceeded.SafeInvoke(this);

                WaitAllWorkers(true);
            }
            catch (Exception ex)
            {
                failed = true;
                WorkerError(ex);
            }
            finally
            {
                CanResume = (failed || _userCancelled) && IsCreationMode;

                Completed.SafeInvoke(this);

                WorkerCleanup();
            }
        }

        private void WaitAllWorkers(bool checkExitCode)
        {
            Action throwIfProcessFailed = () =>
            {
                if (checkExitCode && ExitCode != ExitOK)
                {
                    throw new Exception(Resources.Strings.ProcessorHasTerminated);
                }
            };

            while (Interlocked.CompareExchange(ref _threadsLeft, 0, 0) != 0)
            {
                throwIfProcessFailed();
                Thread.Sleep(1000);
            }

            throwIfProcessFailed();
        }

        private int GetDocumentPageCount()
        {
            using (var converter = Converter.Create(OptionsObject.SrcFile))
            {
                return converter.PageCount;
            }
        }

        private List<Interval> GetDocumentIntervals()
        {
            var intervals = OptionsObject.SafePages.DeepClone();

            var pageCount = GetDocumentPageCount();

            if (pageCount == 0)
            {
                throw new IndexOutOfRangeException(Resources.Strings.DocumentHasNoPages);
            }

            if (!intervals.LessThan(pageCount))
            {
                throw new IndexOutOfRangeException(String.Format(Resources.Strings.PagesSpecifiedAreOutOfRangeFormat, pageCount)); 
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
                PagesTotal = totalIntervals.GetTotalLength();

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

        private void BoostProcessPriority(bool boost)
        {
            try
            {
                var process = Process.GetCurrentProcess();

                var processPriorityClass = _originalProcessPriorityClass;
                
                if (boost)
                {
                    _originalProcessPriorityClass = process.PriorityClass;

                    switch (OptionsObject.ProcessPriority)
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

        private Process StartProcess(string commandLine)
        {
            var consoleEncoding = Encoding.UTF8;

            var processStartInfo = new ProcessStartInfo(Program.Xps2ImgExecutable, commandLine)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = consoleEncoding,
                StandardErrorEncoding = consoleEncoding
            };

            var process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };

            process.OutputDataReceived += OutputDataReceivedHandler;
            process.ErrorDataReceived += ErrorDataReceivedHandler;
            process.Exited += ExitedHandler;

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }

        private void FreeProcessResources(Process process)
        {
            process.CancelOutputRead();
            process.CancelErrorRead();

            process.OutputDataReceived -= OutputDataReceivedHandler;
            process.ErrorDataReceived -= ErrorDataReceivedHandler;

            process.Close();
        }

        private bool IsSingleProcessor
        {
            get { return _threadsCount == 1; }
        }

        private ProcessLastPage[] _processLastConvertedPage;
        private BitArray _processedIntervals;

        private int _pagesProcessed;
        private int _pagesProcessedDelta;

        private int _threadsLeft;
        private int _threadsCount;

        private int _processExitCode;

        private ConversionType _conversionType;

        private volatile bool _isErrorReported;
        private volatile bool _isRunning;
        private volatile bool _progressStarted;
        private volatile bool _userCancelled;

        private EventWaitHandle _cancelEvent;
        private Mutex _appMutex;

        private ProcessPriorityClass _originalProcessPriorityClass;

        private class ProcessLastPage
        {
            public ProcessLastPage(Process process)
            {
                Process = process;
            }

            public readonly Process Process;

            public int Page { get; set; }
        }
    }
}
