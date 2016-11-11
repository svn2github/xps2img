using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

using Xps2ImgLib.Utils;

namespace Xps2ImgLib
{
    public partial class Converter : IDisposable
    {
        private static readonly PropertyInfo VisualTextHintingModePropertyInfo = typeof(Visual).GetProperty("VisualTextHintingMode", BindingFlags.Instance | BindingFlags.NonPublic);

        public string XpsFileName { get; private set; }

        public State ConverterState { get; private set; }
        public Parameters ConverterParameters { get; private set; }

        private readonly Func<bool> _cancelConversionFunc;

        private readonly IMediator _mediator;

        private void CheckIfCancelled()
        {
            if (_cancelConversionFunc != null && _cancelConversionFunc())
            {
                throw new ConversionCancelledException();
            }
        }

        private Converter(string xpsFileName, Func<bool> cancelConversionFunc, bool useWorkerThread)
        {
            if (String.IsNullOrEmpty(xpsFileName))
            {
                throw new ArgumentNullException("xpsFileName");
            }

            XpsFileName = xpsFileName;
            ConverterState = new State();

            _cancelConversionFunc = cancelConversionFunc;

            _mediator = useWorkerThread ? (IMediator)new MediatorThread() : new Mediator();

            try
            {
                _mediator.Init(OpenDocument, CloseDocument);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public int PageCount { get; private set; }

        private XpsDocument _xpsDocument;
        private DocumentPaginator _documentPaginator;

        public static Converter Create(string xpsFileName, Func<bool> cancelConversionFunc = null, bool useWorkerThread = true)
        {
            return new Converter(xpsFileName, cancelConversionFunc, useWorkerThread);
        }

        private void OpenDocument()
        {
            _xpsDocument = new XpsDocument(XpsFileName, FileAccess.Read);
            _documentPaginator = (_xpsDocument.GetFixedDocumentSequence() ?? new FixedDocumentSequence()).DocumentPaginator;

            PageCount = _documentPaginator.PageCount;
        }

        private void CloseDocument()
        {
            try
            {
                if (_xpsDocument != null)
                {
                    _xpsDocument.Close();
                }
            }
            catch
            {
                // ignored
            }
        }

        // ReSharper disable EventNeverSubscribedTo.Global
        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<ErrorEventArgs> OnError;
        // ReSharper restore EventNeverSubscribedTo.Global

        private void FireOnProgress(string fileName)
        {
            ConverterState.ActivePageIndex++;

            _mediator.FireOnProgress(new ProgressEventArgs(fileName, ConverterState));
        }

        public void Convert(Parameters parameters)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Document is already closed.");
            }

            _mediator.Convert(
                () => ConvertInternal(parameters),
                args => OnProgress.SafeInvoke(this, args),
                args => OnError.SafeInvoke(this, args)
            );
        }

        public static string GetOutputDirFor(string xpsFileName, string outputDir, bool create = false)
        {
            var isOutputDirEmpty = String.IsNullOrEmpty(outputDir);

            if (isOutputDirEmpty && String.IsNullOrEmpty(xpsFileName))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (isOutputDirEmpty)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                outputDir = Path.Combine(Path.GetDirectoryName(xpsFileName), Path.GetFileNameWithoutExtension(xpsFileName));
                // ReSharper restore AssignNullToNotNullAttribute
            }

            outputDir = outputDir
                            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                            + Path.DirectorySeparatorChar;

            if (create)
            {
                Directory.CreateDirectory(outputDir);
            }

            return outputDir;
        }

        private void ConvertInternal(Parameters parameters)
        {
            using (new DisposableActions(() => _mediator.RequestStop()))
            {
                ConverterParameters = parameters;

                CheckIfCancelled();

                if (parameters.BaseImageName == null)
                {
                    parameters.BaseImageName = Path.GetFileNameWithoutExtension(XpsFileName) + '-';
                }

                var outputDir = parameters.OutputDir = GetOutputDirFor(XpsFileName, parameters.OutputDir, !parameters.Test && !parameters.Clean);

                if (!ConverterState.HasPageCount)
                {
                    ConverterState.SetLastAndTotalPages(parameters.EndPage, PageCount);
                }

                if (parameters.Clean && !Directory.Exists(outputDir))
                {
                    return;
                }

                var numberFormat = PageCount.GetNumberFormat();

                for (var docPageNumber = parameters.StartPage; docPageNumber <= parameters.EndPage; docPageNumber++)
                {
                    CheckIfCancelled();

                    ConverterState.ActivePage = docPageNumber;

                    ProcessPage(parameters, outputDir, docPageNumber, numberFormat);
                }

                PostClean(parameters, outputDir);
            }
        }

        private void ProcessPage(Parameters parameters, string activeDir, int docPageNumber, string numberFormat)
        {
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

            if (parameters.Clean)
            {
                DeleteImage(parameters, fileName, FireOnProgress);
                return;
            }

            try
            {
                // Render page.
                ImageWriter.Write(f => !parameters.Test && (!parameters.IgnoreExisting || !File.Exists(f)), fileName,
                                  parameters.ImageType, parameters.ShortenExtension, parameters.ImageOptions, parameters.Test,
                                  () => GetPageBitmap(_documentPaginator, docPageNumber - 1, parameters), FireOnProgress, CheckIfCancelled);
            }
            catch(Exception ex)
            {
                DeleteImage(parameters, fileName);
                if (!parameters.IgnoreErrors)
                {
                    throw;
                }

                _mediator.FireOnError(new ErrorEventArgs(ex));
            }
        }

        private static void DeleteImage(Parameters parameters, string fileName, Action<string> progressCallback = null)
        {
            var fullFileName = fileName + ImageWriter.GetImageExtension(parameters.ImageType, parameters.ShortenExtension);

            if (!parameters.Test)
            {
                File.Delete(fullFileName);
            }

            if (progressCallback != null)
            {
                progressCallback(fullFileName);
            }
        }

        private static void PostClean(Parameters parameters, string activeDir)
        {
            if (!parameters.Clean || parameters.Test)
            {
                return;
            }

            // ReSharper disable once EmptyGeneralCatchClause
            try
            {
                File.Delete("thumbs.db");
                Directory.Delete(activeDir);
            }
            catch
            {
            }
        }

        private RenderTargetBitmap GetPageBitmap(DocumentPaginator documentPaginator, int pageNumber, Parameters parameters)
        {
            const double dpiConst = 96.0;

            double dpi = parameters.Dpi;

            var size = parameters.RequiredSize ?? new Size();

            Func<int, bool> isSizeDefined = requiredSize => requiredSize > 0;
            Action<int, double> calcDpi = (requiredSize, pageSize) => { if (isSizeDefined(requiredSize)) { dpi = (requiredSize / pageSize) * dpiConst; } };

            try
            {
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
            catch (XamlParseException ex)
            {
                throw new ConversionFailedException(ex.Message, pageNumber+1, ex);
            }
        }

        private void SetVisualProperties(Visual visual)
        {
            if (!ConverterParameters.XpsRenderOptionsEnabled)
            {
                return;
            }

            if(ConverterParameters.XpsRenderOptions.HighQualityBitmapScalingMode)
            {
                RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);
            }

            if (ConverterParameters.XpsRenderOptions.AnimatedVisualTextHintingMode && VisualTextHintingModePropertyInfo != null)
            {
                // System.Windows.Media.TextHintingMode.Animated as of .NET 4.0+
                const int textHintingModeAnimated = 2;
                VisualTextHintingModePropertyInfo.SetValue(visual, textHintingModeAnimated, null);
            }
        }

        private void RenderPageToBitmapOnce(DocumentPage page, RenderTargetBitmap bitmap)
        {
            var visual = page.Visual;

            SetVisualProperties(visual);

            bitmap.Render(visual);

            // Memory leak fix.
            // http://social.msdn.microsoft.com/Forums/en/wpf/thread/c6511918-17f6-42be-ac4c-459eeac676fd
            ((FixedPage)visual).UpdateLayout();
        }

        private bool _convertionStarted;

        private RenderTargetBitmap RenderPageToBitmap(DocumentPage page, RenderTargetBitmap bitmap)
        {
            var triesCount = ConverterParameters.ConverterOutOfMemoryStrategy.TriesTotal;

            while (true)
            {
                try
                {
                    RenderPageToBitmapOnce(page, bitmap);

                    _convertionStarted = true;

                    return bitmap;
                }
                catch (OutOfMemoryException)
                {
                    CheckIfCancelled();

                    if (!ConverterParameters.OutOfMemoryStrategyEnabled || (--triesCount < 0 && (!_convertionStarted || -triesCount > ConverterParameters.ConverterOutOfMemoryStrategy.MaxTries)))
                    {
                        throw;
                    }

                    Thread.Sleep(ConverterParameters.ConverterOutOfMemoryStrategy.TriesSleepInterval);
                }
            }
        }

        private bool _disposed;

        private void DisposeInternal()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_mediator != null)
            {
                _mediator.Dispose();
            }
        }

        public void Dispose()
        {
            DisposeInternal();
            GC.SuppressFinalize(this);
        }

        ~Converter()
        {
            DisposeInternal();
            #if DEBUG
            System.Diagnostics.Debug.Fail("You are not supposed to be here! Use Dispose instead!");
            #endif
        }
    }
}