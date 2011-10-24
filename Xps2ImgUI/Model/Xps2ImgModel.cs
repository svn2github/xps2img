using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
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

        public void Launch()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Conversion is in progress.");
            }

            var xps2ImgLaunchThread = new Thread(Xps2ImgLaunchThread);

            xps2ImgLaunchThread.SetApartmentState(ApartmentState.STA);
            xps2ImgLaunchThread.Start();
        }

        private EventWaitHandle _cancelEvent;

        private EventWaitHandle CancelEvent
        {
            get { return _cancelEvent ?? (_cancelEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _optionsHolder.OptionsObject.CancellationObjectId)); }
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

        private volatile bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
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

            var process = new Process {StartInfo = processStartInfo, EnableRaisingEvents = true };

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

        private List<Interval> GetPages()
        {
            var intervals = Interval.Parse(OptionsObject.Pages);
            if (intervals.Last().HasMaxValue)
            {
                using (var xpsDocument = new XpsDocument(OptionsObject.SrcFile, FileAccess.Read, CompressionOption.NotCompressed))
                {
                    var fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();
                    if (fixedDocumentSequence == null)
                    {
                        intervals.Clear();
                    }
                    else
                    {
                        intervals.Last().SetEndValue(fixedDocumentSequence.DocumentPaginator.PageCount);
                    }
                }
            }
            return intervals;
        }

        private void Xps2ImgProcessWaitThread(Process process)
        {
            try
            {
                process.WaitForExit();
                FreeProcessResources(process);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
        }

        private int _pagesTotal;
        private int _pagesProcessed;

        private int _threadsCount;

        private bool IsSingleProcessor
        {
            get { return _threadsCount == 1; }
        }

        private ProcessPriorityClass _originalProcessPriorityClass;

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

        private void Xps2ImgLaunchThread()
        {
            //_threadsCount = OptionsObject.ActualProcessorsNumber;

            //var pps = new[] { "-100", "101-200" };
            var pps = new[] { "-50", "51-100", "101-150", "151-200" };

            _threadsCount = pps.Length;

            var waitProcessThreads = new List<Thread>();

            // ReSharper disable EmptyGeneralCatchClause
            Action waitAllProcessThreads = () => waitProcessThreads.ForEach(t => { try { t.Join(); } catch {} });
            // ReSharper restore EmptyGeneralCatchClause

            try
            {
                BoostProcessPriority(true);           

                _isRunning = true;
                _isErrorReported = false;

                CancelEvent.Reset();

                var consoleEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.GetConsoleFallbackUICulture().TextInfo.OEMCodePage);

                var intervals = GetPages();

                _pagesTotal = 200; //intervals.GetTotalLength();
                _pagesProcessed = 0;

                _threadsCount = intervals.Any() ? OptionsObject.ActualProcessorsNumber : 1;

                // calculate intervals

                for (var i = 0; i < _threadsCount; i++)
                {
                    var process = StartProcess(FormatCommandLine(Options.ExcludedOnLaunch) + String.Format(" -p \"{0}\"", pps[i]), consoleEncoding);
                    var waitProcessThread = new Thread(() => Xps2ImgProcessWaitThread(process));
                    waitProcessThread.Start();
                    waitProcessThreads.Add(waitProcessThread);
                }

                if (LaunchSucceeded != null)
                {
                    LaunchSucceeded(this, EventArgs.Empty);
                }

                waitAllProcessThreads();

                if (Completed != null)
                {
                    Completed(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Stop();

                waitAllProcessThreads();

                // https://connect.microsoft.com/VisualStudio/feedback/details/430646/thread-handle-leak#tabs
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (LaunchFailed == null)
                {
                    throw;
                }

                LaunchFailed(this, new ThreadExceptionEventArgs(ex));
            }
            finally
            {
                _isRunning = false;
                BoostProcessPriority(false);
            }
        }

        private const string FileNameGroup = @".+?'(?<file>.+)'";

        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\].+\(\s*(?<pages>\d+/\d+)\)" + FileNameGroup);
        private static readonly Regex FileNameRegex = new Regex(FileNameGroup);

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

        private volatile bool _isErrorReported;

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

        private OptionsHolder<Options> _optionsHolder;
    }
}
