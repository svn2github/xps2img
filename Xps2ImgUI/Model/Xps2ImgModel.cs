using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using Xps2Img.CommandLine;
using Xps2ImgUI.Attributes.OptionsHolder;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Model
{
    public class Xps2ImgModel
    {
        public static readonly string ApplicationFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static readonly string Xps2ImgExecutable = Path.Combine(ApplicationFolder, "xps2img.exe");

        public void Init()
        {
            Reset();
        }

        public void Reset()
        {
            if (_optionsHolder != null)
            {
                _optionsHolder.OptionsObjectChanged -= OptionsHolderOptionsObjectChanged;
            }

            _optionsHolder = new OptionsHolder<Options>();
            _optionsHolder.OptionsObjectChanged += OptionsHolderOptionsObjectChanged;

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
            Stop();

            new Thread(Xps2ImgTreadStart).Start();
        }

        public void Stop()
        {
            if (_process == null)
            {
                return;
            }

            try
            {
                FreeProcessResources();
                _process.Kill();
            }
            catch(InvalidOperationException)
            {
            }
        }

        public string FormatCommandLine()
        {
            return _optionsHolder.FormatCommandLine();
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
            get
            {
                try
                {
                    return _process != null && !_process.HasExited;
                }
                catch(InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;
        public event EventHandler Completed;

        public event EventHandler OptionsObjectChanged;

        public event ThreadExceptionEventHandler LaunchFailed;

        private void FreeProcessResources()
        {
            _process.CancelOutputRead();
            _process.CancelErrorRead();

            _process.OutputDataReceived -= OutputDataReceivedWrapper;
            _process.ErrorDataReceived -= ErrorDataReceivedWrapper;
            _process.Exited -= ExitedWrapper;
        }

        private void Xps2ImgTreadStart(object context)
        {
            try
            {
                var consoleEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.GetConsoleFallbackUICulture().TextInfo.OEMCodePage);

                var processStartInfo =  new ProcessStartInfo(Xps2ImgExecutable, FormatCommandLine())
                                        {
                                            CreateNoWindow = true,
                                            UseShellExecute = false,
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true,
                                            StandardOutputEncoding = consoleEncoding,
                                            StandardErrorEncoding = consoleEncoding
                                        };

                using (_process = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true })
                {
                    _process.OutputDataReceived += OutputDataReceivedWrapper;
                    _process.ErrorDataReceived += ErrorDataReceivedWrapper;
                    _process.Exited += ExitedWrapper;

                    _process.Start();

                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();

                    _process.WaitForExit();

                    FreeProcessResources();
                }
            }
            catch (Exception ex)
            {
                if (LaunchFailed == null)
                {
                    throw;
                }

                LaunchFailed(this, new ThreadExceptionEventArgs(ex));
            }
        }

        private void OutputDataReceivedWrapper(object sender, DataReceivedEventArgs e)
        {
            if (OutputDataReceived != null)
            {
                OutputDataReceived(sender, e);
            }
        }

        private void ErrorDataReceivedWrapper(object sender, DataReceivedEventArgs e)
        {
            if (ErrorDataReceived != null)
            {
                ErrorDataReceived(sender, e);
            }
        }

        private void ExitedWrapper(object sender, EventArgs e)
        {
            if (Completed != null)
            {
                Completed(sender, e);
            }
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
        private Process _process;
    }
}
