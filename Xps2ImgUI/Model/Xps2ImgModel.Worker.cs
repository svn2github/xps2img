using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Xps.Packaging;

using CommandLine;
using Xps2Img.Shared.CommandLine;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2ImgUI.Model
{
    public partial class Xps2ImgModel
    {
        private void LaunchInternal(ConversionType conversionType)
        {
            _conversionType = conversionType;

            CancelEvent.Reset();

            var workerThread = new Thread(WorkerThread);

            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();
        }

        private void WorkerInit()
        {
            _isRunning = true;
            _isErrorReported = false;
            _progressStarted = false;
            _userCancelled = false;

            _processExitCode = 0;
            _threadsLeft = 0;
            _pagesProcessed = 0;

            _threadsCount = IsCreationMode ? OptionsObject.SafeProcessorsNumber : 1;
        }

        private void WorkerStart()
        {
            var splittedIntervals = GetDocumentSplittedIntervals();

            _appMutex = new Mutex(true, _optionsHolder.OptionsObject.ParentAppMutexName);

            _processLastConvertedPage = null;

            var processLastConvertedPage = new List<ProcessLastPage>();

            foreach (var t in splittedIntervals)
            {
                var processCommandLine = String.Format("{0} -" + Options.PagesShortOption + " \"{1}\" {2}",
                                            FormatCommandLine(Options.ExcludedOnLaunch),
                                            IntervalUtils.ToString(t),
                                            IsCreationMode ? String.Empty : Options.CleanOption);
                var process = StartProcess(processCommandLine);
                ThreadPool.QueueUserWorkItem(_ => WaitForProcessWorker(process));
                Interlocked.Increment(ref _threadsLeft);
                processLastConvertedPage.Add(new ProcessLastPage(process));
            }

            _processLastConvertedPage = _processedIntervals != null ? processLastConvertedPage.ToArray() : null;
        }

        private void WorkerCleanup()
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

            if (_userCancelled)
            {
                ExitCode = ReturnCode.UserCancelled;
            }
        }

        private void WorkerError(Exception ex)
        {
            Stop();

            WaitAllWorkers(false);

            if (LaunchFailed == null)
            {
                throw ex;
            }

            ExitCode = ReturnCode.Failed;

            if (!_isErrorReported)
            {
                LaunchFailed(this, new ThreadExceptionEventArgs(ex));
            }
        }

        private void WorkerWait()
        {
            if (LaunchSucceeded != null)
            {
                LaunchSucceeded(this, EventArgs.Empty);
            }

            WaitAllWorkers(true);

            if (Completed != null)
            {
                Completed(this, EventArgs.Empty);
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
            try
            {
                WorkerCheckPreconitions();
                WorkerInit();
                WorkerStart();
                WorkerWait();
            }
            catch (Exception ex)
            {
                WorkerError(ex);
            }
            finally
            {
                WorkerCleanup();
            }
        }

        private void WaitAllWorkers(bool checkExitCode)
        {
            while (Interlocked.CompareExchange(ref _threadsLeft, 0, 0) != 0)
            {
                if (checkExitCode && ExitCode != 0)
                {
                    throw new Exception(Resources.Strings.ProcessorHasTerminated);
                }
                Thread.Sleep(1000);
            }

            CanResume = IsStopPending;
        }

        private void WaitForProcessWorker(Process process)
        {
            try
            {
                process.WaitForExit();
                if (process.ExitCode >= 0)
                {
                    ExitCode = process.ExitCode;
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
                if (PagesTotal <= 0) PagesTotal = 1;

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

            process.OutputDataReceived += OutputDataReceivedWrapper;
            process.ErrorDataReceived += ErrorDataReceivedWrapper;

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }

        private void FreeProcessResources(Process process)
        {
            process.CancelOutputRead();
            process.CancelErrorRead();

            process.OutputDataReceived -= OutputDataReceivedWrapper;
            process.ErrorDataReceived -= ErrorDataReceivedWrapper;

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
