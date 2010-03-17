using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using CommandLine;

using Utils;

using Xps2Img.CommandLine;

namespace Xps2Img
{
  internal static class Program
  {
    [STAThread]
    private static int Main(string[] args)
    {
      try
      {
        if(CommandLine.CommandLine.IsUsageDisplayed<Options>(args))
        {
          return (int)CommandLine.CommandLine.ReturnCode.NoArgs;
        }

        var options = CommandLine.CommandLine.Parse(args);
        
        if (options == null)
        {
          return (int)CommandLine.CommandLine.ReturnCode.InvalidArg;
        }

        Trace.WriteLine(Parser.ToCommandLine(options));
        
        // Started as child process.
        if(options.IsWorker)
        {
          return Convert(options);
        }

        silent = options.Silent;

        // Started as host process.
        Win32.SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

        options.IsWorker = true;
        
        while(true)
        {
          var retCode = StartConverterProcess(Parser.ToCommandLine(options));

          if (retCode == (int)CommandLine.CommandLine.ReturnCode.OutOfMemorySign)
          {
            #if DEBUG || TRACE
            var _msg = String.Format("\n[OUT OF MEMORY] Restarting from {0}\n", CommandLine.CommandLine.ConverterState);
            Trace.WriteLine(_msg);
            Console.Error.WriteLine(_msg);
            #endif
            
            options.WorkerCoverterState = CommandLine.CommandLine.ConverterState;
            options.Pages = options.Pages.AdjustBeginValue(CommandLine.CommandLine.ConverterState.ActivePage);
            
            continue; // Next pages.
          }
          
          if (retCode == (int)CommandLine.CommandLine.ReturnCode.ChildCompleted)
          {
            break; // All child processes are completed.
          }

          throw new Exception(Resources.Strings.Error_Fatal);
        }
      }
      catch (Exception ex)
      {
        return CommandLine.CommandLine.DisplayError(ex);
      }
      
      return (int)CommandLine.CommandLine.ReturnCode.OK;
    }
    
    private static int Convert(Options options)
    {
      using(var xps2Img = Converter.Create(options.SrcFile))
      {
        xps2Img.OnProgress += OnProgress;
        
        options.Pages.SetEndValue(xps2Img.PageCount);

        if (options.WorkerCoverterState != null)
        {
          xps2Img.ConverterState = options.WorkerCoverterState;
        }
        else
        {
          xps2Img.ConverterState.SetLastAndTotalPages(options.Pages.Last().End, options.Pages.GetTotalLength());
        }
        
        try
        {
          options.Pages.ForEach(interval =>
            xps2Img.Convert(
              new Converter.Parameters
              {
                StartPage     = interval.Begin,
                EndPage       = interval.End,
                ImageType     = options.FileType,
                ImageOptions  = new ImageOptions(options.JpegQuality, options.TiffCompression),
                RequiredSize  = options.RequiredSize,
                Dpi           = options.Dpi,
                OutputDir     = options.OutDir,
                BaseImageName = !String.IsNullOrEmpty(options.ImageName) ?
                                  options.ImageName :
                                  (options.ImageName == null ? String.Empty : null)
              }
            )
          );
        }
        catch(OutOfMemoryException)
        {
          return xps2Img.MakeOutOfMemoryRetCode();
        }
      }

      return (int)CommandLine.CommandLine.ReturnCode.ChildCompleted;
    }

    private static bool ConsoleCtrlCheck(Win32.CtrlTypes ctrlType)
    {
      // ReSharper disable EmptyGeneralCatchClause
      try
      {
        converterProcess.Kill();
      }
      catch
      {
      }
      // ReSharper restore EmptyGeneralCatchClause
      return false; // false to call standart handler.
    }
    
    private static Process converterProcess;

    private static bool silent;
    
    private static int StartConverterProcess(string commandLine)
    {
      var processStartInfo = new ProcessStartInfo
      {
        CreateNoWindow  = true,
        UseShellExecute = false,
        FileName        = Parser.EntryAssemblyLocation,
        Arguments       = commandLine,
        RedirectStandardError   = true,
        RedirectStandardOutput  = true,
        RedirectStandardInput   = true
      };
      
      using(converterProcess = Process.Start(processStartInfo))
      {
        if (converterProcess == null)
        {
          return (int) CommandLine.CommandLine.ReturnCode.Failed;
        }

        Action<string> showProcessOutput = output =>
        {
          if(String.IsNullOrEmpty(output))
          {
            return;
          }
          
          if (CommandLine.CommandLine.GetProcessExitState(output) || silent)
          {
            return;
          }
          
          var msg = output.TrimEnd(Environment.NewLine.ToCharArray());
          Trace.WriteLine(msg);
          Console.WriteLine(msg);
        };
        
        using (StreamReader stdOut = converterProcess.StandardOutput, stdErr = converterProcess.StandardError)
        {
          while (!converterProcess.HasExited)
          {
            showProcessOutput(stdOut.ReadLine());
          }
          showProcessOutput(stdOut.ReadToEnd());
          showProcessOutput(stdErr.ReadToEnd());
        }

        return converterProcess.ExitCode;
      }
    }

    #region Progress.

    private static string progreessFormatString;

    private static void OnProgress(Converter.ProgressEventArgs args)
    {
      if(progreessFormatString == null)
      {
        progreessFormatString = String.Format(
                                  Resources.Strings.Template_Progress,
                                  0.GetNumberFormat(args.ConverterState.LastPage, false),
                                  1.GetNumberFormat(args.ConverterState.LastPage, false),
                                  2.GetNumberFormat(args.ConverterState.LastPage, false));
      }
      Console.WriteLine(String.Format(progreessFormatString, args.ConverterState.ActivePage, args.ConverterState.ActivePageIndex, args.ConverterState.TotalPages, args.FullFileName, (int)args.ConverterState.Percent));
    }

    #endregion
  }
}