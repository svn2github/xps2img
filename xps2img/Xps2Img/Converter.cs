using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace Xps2Img.Xps2Img
{
	public class Converter : IDisposable
	{
		private readonly XpsDocument _xpsDocument;
		private readonly DocumentPaginator _documentPaginator;

		public ConverterState ConverterState { get; set; }

		private Converter(string xpsFileName)
		{
			_xpsDocument = new XpsDocument(xpsFileName, FileAccess.Read, CompressionOption.NotCompressed);
			// ReSharper disable PossibleNullReferenceException
			_documentPaginator = _xpsDocument.GetFixedDocumentSequence().DocumentPaginator;
			// ReSharper restore PossibleNullReferenceException
			ConverterState = new ConverterState();
		}

		public int PageCount { get { return _documentPaginator.PageCount; } }

		public static Converter Create(string xpsFileName)
		{
			return new Converter(xpsFileName);
		}

		public class ProgressEventArgs : EventArgs
		{
			public ProgressEventArgs(string fullFileName, ConverterState converterState)
			{
				FullFileName = fullFileName;
				ConverterState = converterState;
			}

			public readonly string FullFileName;
			public readonly ConverterState ConverterState;
		}

		public delegate void ProgressDelegate(ProgressEventArgs args);

		public event ProgressDelegate OnProgress;

		public class Parameters
		{
			public bool Test { get; set; }

			public int StartPage { get; set; }
			public int EndPage { get; set; }

			public ImageType ImageType { get; set; }
			public ImageOptions ImageOptions { get; set; }

			public Size? RequiredSize { get; set; }
			public int Dpi { get; set; }

			public string OutputDir { get; set; }
			public string BaseImageName { get; set; }

			public int FirstPageIndex { get; set; }
			public string PrelimsPrefix { get; set; }

			public Parameters()
			{
				FirstPageIndex	= 1;
				PrelimsPrefix	= "$";
			}
		}

		public void Convert(Parameters parameters)
		{
			var xpsFileName = _xpsDocument.Uri.OriginalString;

			var numberFormat = PageCount.GetNumberFormat();

			if (parameters.BaseImageName == null)
			{
				parameters.BaseImageName = Path.GetFileNameWithoutExtension(xpsFileName) + '-';
			}

			if (String.IsNullOrEmpty(parameters.OutputDir))
			{
				// ReSharper disable AssignNullToNotNullAttribute
				parameters.OutputDir = Path.Combine(Path.GetDirectoryName(xpsFileName), Path.GetFileNameWithoutExtension(xpsFileName));
				// ReSharper restore AssignNullToNotNullAttribute
			}

			if (!ConverterState.HasPageCount)
			{
				ConverterState.SetLastAndTotalPages(parameters.EndPage, PageCount);
			}

			var activeDir = parameters.OutputDir;
			if (!parameters.Test)
			{
				Directory.CreateDirectory(activeDir);
			}

			for (var docPageNumber = parameters.StartPage; docPageNumber <= parameters.EndPage; docPageNumber++)
			{
				ConverterState.ActivePage = docPageNumber;

				var pageIndex = docPageNumber - parameters.FirstPageIndex + 1;

				var isContent = pageIndex <= 0;
				if(isContent)
				{
					pageIndex = -(pageIndex + parameters.FirstPageIndex - 1);
				}

				var pageIndexFormatted = new StringBuilder(4).AppendFormat(numberFormat, pageIndex);

				if(isContent)
				{
				    pageIndexFormatted.Remove(0, 1);
				    pageIndexFormatted.Insert(0, parameters.PrelimsPrefix);
				}

				var fileName = Path.Combine(activeDir, parameters.BaseImageName + pageIndexFormatted);

				ImageWriter.Write(
				  !parameters.Test,
				  fileName,
				  parameters.ImageType,
				  parameters.ImageOptions,
				  GetPageBitmap(_documentPaginator, docPageNumber - 1, parameters),
				  fullFileName => { if (OnProgress != null) { ConverterState.ActivePageIndex++; OnProgress(new ProgressEventArgs(fullFileName, ConverterState)); } });
			}
		}

		private static RenderTargetBitmap GetPageBitmap(DocumentPaginator documentPaginator, int pageNumber, Parameters parameters)
		{
			const double dpiConst = 96.0;

			double dpi = parameters.Dpi;

			var size = parameters.RequiredSize ?? new Size();

			Func<int, bool> isSizeDefined = requiredSize => requiredSize > 0;
			Action<int, double> calcDpi = (requiredSize, pageSize) => { if (isSizeDefined(requiredSize)) { dpi = (requiredSize / pageSize) * dpiConst; } };

			using (var page = documentPaginator.GetPage(pageNumber))
			{
				if (!size.IsEmpty)
				{
					var portrait = page.Size.Height >= page.Size.Width;

					if (portrait || !isSizeDefined(size.Width))
					{
						calcDpi(size.Height, page.Size.Height);
					}

					if (!portrait || !isSizeDefined(size.Height))
					{
						calcDpi(size.Width, page.Size.Width);
					}
				}

				var ratio = dpi / dpiConst;

				var bitmap = new RenderTargetBitmap((int)Math.Round(page.Size.Width * ratio),
													(int)Math.Round(page.Size.Height * ratio), dpi, dpi, PixelFormats.Pbgra32);
				bitmap.Render(page.Visual);

				// Memory leak fix.
				// http://social.msdn.microsoft.com/Forums/en/wpf/thread/c6511918-17f6-42be-ac4c-459eeac676fd
				((FixedPage)page.Visual).UpdateLayout();

				return bitmap;
			}
		}

		public void Dispose()
		{
			_xpsDocument.Close();
			GC.SuppressFinalize(this);
		}
	}
}