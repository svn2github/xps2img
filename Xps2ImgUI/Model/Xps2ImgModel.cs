﻿using System;
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

            process.Dispose();
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

            var process = new Process {StartInfo = processStartInfo, EnableRaisingEvents = true};

            process.OutputDataReceived += OutputDataReceivedWrapper;
            process.ErrorDataReceived += ErrorDataReceivedWrapper;

            process.Start();

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

        private void Xps2ImgLaunchThread()
        {
            //var threadsCount = OptionsObject.ActualProcessorsNumber;

            var pps = new[] { "-50", "51-100", "101-150", "151-200" };

            //const int threadsCount = 2;
            //var pps = new[] { "-100", "101-200" };

            var waitProcessThreads = new List<Thread>();

            Action waitAllProcessThreads = () => waitProcessThreads.ForEach(t => t.Join());

            try
            {
                _isRunning = true;
                _isErrorReported = false;

                CancelEvent.Reset();

                var consoleEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.GetConsoleFallbackUICulture().TextInfo.OEMCodePage);

                var intervals = GetPages();

                var threadsCount = intervals.Any() ? OptionsObject.ActualProcessorsNumber : 1;

                // calculate intervals

                for (var i = 0; i < threadsCount; i++)
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

                if (LaunchFailed == null)
                {
                    throw;
                }

                LaunchFailed(this, new ThreadExceptionEventArgs(ex));
            }
            finally
            {
                _isRunning = false;
            }
        }

        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\].+\(\s*(?<pages>\d+/\d+)\).+?'(?<file>.+)'");

        private void OutputDataReceivedWrapper(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            if (OutputDataReceived != null)
            {
                var match = OutputRegex.Match(e.Data);
                if (!match.Success)
                {
                    return;
                }

                var percent = Convert.ToInt32(match.Groups["percent"].Value);
                var pages = match.Groups["pages"].Value;
                var file = match.Groups["file"].Value;
                
                OutputDataReceived(this, new ConvertionProgressEventArgs(percent, pages, file));
            }
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
