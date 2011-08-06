using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using Xps2Img.CommandLine;
using Xps2ImgUI.Attributes.OptionsHolder;

namespace Xps2ImgUI.Model
{
	class Xps2ImgModel
	{
		public static readonly string ApplicationFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		public static readonly string Xps2ImgExecutable = Path.Combine(ApplicationFolder, "xps2img.exe");

		public Xps2ImgModel()
		{
			Reset();
		}

		public void Reset()
		{
			_optionsHolder = OptionsHolder.Create(new Options());
		}

		public void Launch()
		{
			Stop();

			new Thread(Xps2ImgTreadStart).Start();
		}

		public void Stop()
		{
			try
			{
				_process.CancelOutputRead();
				_process.CancelErrorRead();
				_process.Kill();
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch
			// ReSharper restore EmptyGeneralCatchClause
			{
				// Ignore.
			}
		}

		public string FormatCommandLine()
		{
			return _optionsHolder.FormatCommandLine();
		}

		public object OptionsObject
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
					return !_process.HasExited;
				}
				catch
				{
					return false;
				}
			}
		}

		public event DataReceivedEventHandler OutputDataReceived;
		public event DataReceivedEventHandler ErrorDataReceived;
		public event EventHandler Completed;

		public event ThreadExceptionEventHandler LaunchFailed;

		private void Xps2ImgTreadStart(object context)
		{
			try
			{
				if (_process != null)
				{
					_process.Dispose();
				}

				var consoleEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.GetConsoleFallbackUICulture().TextInfo.OEMCodePage);

				var processStartInfo = new ProcessStartInfo(Xps2ImgExecutable, FormatCommandLine())
										   {
											   CreateNoWindow = true,
											   UseShellExecute = false,
											   RedirectStandardOutput = true,
											   RedirectStandardError = true,
											   StandardOutputEncoding = consoleEncoding,
											   StandardErrorEncoding = consoleEncoding
										   };

				_process = new Process
				{
					StartInfo = processStartInfo,
					EnableRaisingEvents = true
				};

				_process.OutputDataReceived += OutputDataReceivedWrapper;
				_process.ErrorDataReceived += ErrorDataReceivedWrapper;
				_process.Exited += ExitedWrapper;

				_process.Start();

				_process.BeginOutputReadLine();
				_process.BeginErrorReadLine();

				_process.WaitForExit();
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
			var process = (Process) sender;

			process.OutputDataReceived -= OutputDataReceivedWrapper;
			process.ErrorDataReceived -= ErrorDataReceivedWrapper;
			process.Exited -= ExitedWrapper;

			if (Completed != null)
			{
				Completed(sender, e);
			}
		}

		private OptionsHolder _optionsHolder;
		private Process _process;
	}
}
