using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
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

        public string XpsFileName { get; private set; }

        public ConverterState ConverterState { get; private set; }
        public Parameters ConverterParameters { get; private set; }

        private readonly Func<bool> _cancelConversionFunc;

        private bool IsCancelled
        {
            get { return _cancelConversionFunc != null && _cancelConversionFunc(); }
        }

        private Converter(string xpsFileName, Func<bool> cancelConversionFunc)
        {
            XpsFileName = xpsFileName;
            _cancelConversionFunc = cancelConversionFunc;

            _xpsDocument = new XpsDocument(xpsFileName, FileAccess.Read);
            // ReSharper disable PossibleNullReferenceException
            _documentPaginator = _xpsDocument.GetFixedDocumentSequence().DocumentPaginator;
            // ReSharper restore PossibleNullReferenceException

            ConverterState = new ConverterState();
        }

        public int PageCount { get { return _documentPaginator.PageCount; } }

        public static Converter Create(string xpsFileName, Func<bool> cancelConversionFunc)
        {
            return new Converter(xpsFileName, cancelConversionFunc);
        }

        public static Converter Create(string xpsFileName)
        {
            return Create(xpsFileName, null);
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

        public delegate void ProgressDelegate(object sender, ProgressEventArgs args);

        public event ProgressDelegate OnProgress;

        public class Parameters
        {
            public bool Silent { get; set; }
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

            public bool Clean { get; set; }
            
            public Parameters()
            {
                FirstPageIndex = 1;
                PrelimsPrefix = "$";
            }
        }

        public void Convert(Parameters parameters)
        {
            if (IsCancelled)
            {
                return;
            }

            ConverterParameters = parameters;

            var numberFormat = PageCount.GetNumberFormat();

            if (parameters.BaseImageName == null)
            {
                parameters.BaseImageName = Path.GetFileNameWithoutExtension(XpsFileName) + '-';
            }

            if (String.IsNullOrEmpty(parameters.OutputDir))
            {
                // ReSharper disable AssignNullToNotNullAttribute
                parameters.OutputDir = Path.Combine(Path.GetDirectoryName(XpsFileName), Path.GetFileNameWithoutExtension(XpsFileName));
                // ReSharper restore AssignNullToNotNullAttribute
            }

            parameters.OutputDir = parameters.OutputDir
                                    .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                                    .TrimEnd(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar })
                                    + Path.DirectorySeparatorChar;

            if (!ConverterState.HasPageCount)
            {
                ConverterState.SetLastAndTotalPages(parameters.EndPage, PageCount);
            }

            var activeDir = parameters.OutputDir;
            if (!parameters.Test && !parameters.Clean)
            {
                Directory.CreateDirectory(activeDir);
            }

            if (parameters.Clean && !Directory.Exists(activeDir))
            {
                return;
            }
            
            for (var docPageNumber = parameters.StartPage; docPageNumber <= parameters.EndPage; docPageNumber++)
            {
                if (IsCancelled)
                {
                    return;
                }

                ConverterState.ActivePage = docPageNumber;

                var pageIndex = docPageNumber - parameters.FirstPageIndex + 1;

                var isContent = pageIndex <= 0;
                if (isContent)
                {
                    pageIndex = -(pageIndex + parameters.FirstPageIndex - 1);
                }

                var pageIndexFormatted = new StringBuilder(16).AppendFormat(numberFormat, pageIndex);

                if (isContent)
                {
                    pageIndexFormatted.Remove(0, 1);
                    pageIndexFormatted.Insert(0, parameters.PrelimsPrefix);
                }

                var fileName = Path.Combine(activeDir, parameters.BaseImageName + pageIndexFormatted);

                if(parameters.Clean)
                {
                    var fullFileName = fileName + ImageWriter.GetImageExtension(parameters.ImageType);

                    if (!parameters.Test)
                    {
                        File.Delete(fullFileName);
                    }

                    FireOnProgress(fullFileName);
                }
                else
                {
                    ImageWriter.Write(
                      !parameters.Test,
                      fileName,
                      parameters.ImageType,
                      parameters.ImageOptions,
                      GetPageBitmap(_documentPaginator, docPageNumber - 1, parameters),
                      FireOnProgress
                    );
                }
            }

            if (parameters.Clean && !parameters.Test)
            {
                try
                {
                    File.Delete("thumbs.db");
                    Directory.Delete(activeDir);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                {
                }
                // ReSharper restore EmptyGeneralCatchClause
            }
        }

        private void FireOnProgress(string fileName)
        {
            ConverterState.ActivePageIndex++;

            if (OnProgress != null)
            {
                OnProgress(this, new ProgressEventArgs(fileName, ConverterState));
            }
        }

        private RenderTargetBitmap GetPageBitmap(DocumentPaginator documentPaginator, int pageNumber, Parameters parameters)
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

                return RenderPageToBitmap(page, bitmap);
            }
        }

        private bool _convertionStarted;

        private RenderTargetBitmap RenderPageToBitmap(DocumentPage page, RenderTargetBitmap bitmap)
        {
            const int triesSleepInterval = 1000;
            const int triesTotal = 30;
            const int maxTries = triesTotal * 10;

            var triesCount = triesTotal;

            while (true)
            {
                try
                {
                    bitmap.Render(page.Visual);

                    // Memory leak fix.
                    // http://social.msdn.microsoft.com/Forums/en/wpf/thread/c6511918-17f6-42be-ac4c-459eeac676fd
                    ((FixedPage)page.Visual).UpdateLayout();

                    _convertionStarted = true;

                    return bitmap;
                }
                catch (OutOfMemoryException)
                {
                    if (IsCancelled)
                    {
                        return null;
                    }

                    if (--triesCount < 0 && (!_convertionStarted || -triesCount > maxTries))
                    {
                        throw;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    Thread.Sleep(triesSleepInterval);
                }
            }
        }

        public void Dispose()
        {
            _xpsDocument.Close();
            GC.SuppressFinalize(this);
        }
    }
}