using System;
using System.Diagnostics;
using System.Linq;

using CommandLine;

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
				if (CommandLine.CommandLine.IsUsageDisplayed<Options>(args))
				{
					return (int)CommandLine.CommandLine.ReturnCode.NoArgs;
				}

				var options = CommandLine.CommandLine.Parse(args);

				if (options == null)
				{
					return (int)CommandLine.CommandLine.ReturnCode.InvalidArg;
				}

				Trace.WriteLine(Parser.ToCommandLine(options));

				return Convert(options);
			}
			catch (Exception ex)
			{
				return CommandLine.CommandLine.DisplayError(ex);
			}
		}

		private static int Convert(Options options)
		{
			using (var xps2Img = Converter.Create(options.SrcFile))
			{
				xps2Img.OnProgress += OnProgress;

				options.Pages.SetEndValue(xps2Img.PageCount);

				xps2Img.ConverterState.SetLastAndTotalPages(options.Pages.Last().End, options.Pages.GetTotalLength());

				options.Pages.ForEach(interval =>
					xps2Img.Convert(
						new Converter.Parameters
						{
							Test = options.Test,
							StartPage = interval.Begin,
							EndPage = interval.End,
							ImageType = options.FileType,
							ImageOptions = new ImageOptions(options.JpegQuality, options.TiffCompression),
							RequiredSize = options.RequiredSize,
							Dpi = options.Dpi,
							OutputDir = options.OutDir,
							BaseImageName = !String.IsNullOrEmpty(options.ImageName) ?
											  options.ImageName :
											  (options.ImageName == null ? String.Empty : null)
						}
					)
				);
			}

			return (int)CommandLine.CommandLine.ReturnCode.OK;
		}

		private static string progreessFormatString;

		private static void OnProgress(Converter.ProgressEventArgs args)
		{
			if (progreessFormatString == null)
			{
				progreessFormatString = String.Format(
										  Resources.Strings.Template_Progress,
										  0.GetNumberFormat(args.ConverterState.LastPage, false),
										  1.GetNumberFormat(args.ConverterState.LastPage, false),
										  2.GetNumberFormat(args.ConverterState.LastPage, false));
			}
			Console.WriteLine(String.Format(progreessFormatString, args.ConverterState.ActivePage, args.ConverterState.ActivePageIndex, args.ConverterState.TotalPages, args.FullFileName, (int)args.ConverterState.Percent));
		}
	}
}