﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

using Xps2Img.CommandLine;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Xps2Img
{
    public partial class Converter : IDisposable
    {
        private static readonly PropertyInfo VisualTextHintingModePropertyInfo = typeof(Visual).GetProperty("VisualTextHintingMode", BindingFlags.Instance | BindingFlags.NonPublic);

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

            //XpsFileName = @"P:\Projects\NET\xps2img_sf\test.xps";

            _cancelConversionFunc = cancelConversionFunc;

            ConverterState = new ConverterState();

            _currentAction = Init;

            _converterThread = new Thread(ConverterThread);
            _converterThread.SetApartmentState(ApartmentState.STA);
            _converterThread.Start();

            WaitConverter();
        }

        public int PageCount { get; private set; }

        private XpsDocument _xpsDocument;
        private DocumentPaginator _documentPaginator;

        private readonly Thread _converterThread;
        private readonly AutoResetEvent _mainEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _converterEvent = new AutoResetEvent(false);
        private Action _currentAction;

        private Exception _exception;

        private void WaitConverter()
        {
            _converterEvent.WaitOne();
            var ex = _exception;
            _exception = null;
            if (ex != null)
            {
                _currentAction = null;
                _mainEvent.Set();
                throw ex;
            }
        }

        private void Init()
        {
            _xpsDocument = new XpsDocument(XpsFileName, FileAccess.Read);
            _documentPaginator = (_xpsDocument.GetFixedDocumentSequence() ?? new FixedDocumentSequence()).DocumentPaginator;

            PageCount = _documentPaginator.PageCount;
        }
        
        private void ConverterThread()
        {
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            while (true)
            {
                try
                {
                    _exception = null;
                    if (_currentAction == null)
                    {
                        _xpsDocument.Close();
                        return;
                    }
                    _currentAction();
                    _converterEvent.Set();
                    _mainEvent.WaitOne();
                }
                catch (Exception ex)
                {
                    _exception = ex;
                    _converterEvent.Set();
                    _mainEvent.WaitOne();
                }
            }
        }

        public static Converter Create(string xpsFileName, Func<bool> cancelConversionFunc = null)
        {
            return new Converter(xpsFileName, cancelConversionFunc);
        }

        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<ExceptionEventArgs> OnError;

        public void Convert(Parameters parameters)
        {
            _exception = null;

            _currentAction = () => ConvertInternal(parameters);
            _mainEvent.Set();

            while (true)
            {
                WaitConverter();

                if (_currentAction == null)
                {
                    break;
                }

                OnProgress.SafeInvoke(this, _progressEventArgs);
                
                _mainEvent.Set();
            }
        }

        private void ConvertInternal(Parameters parameters)
        {
            //using (new DisposableActions(_currentAction = null))
            {
                ConverterParameters = parameters;

                if (IsCancelled)
                {
                    return;
                }

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

                var numberFormat = PageCount.GetNumberFormat();

                for (var docPageNumber = parameters.StartPage; docPageNumber <= parameters.EndPage; docPageNumber++)
                {
                    if (IsCancelled)
                    {
                        return;
                    }

                    ConverterState.ActivePage = docPageNumber;

                    ProcessPage(parameters, activeDir, docPageNumber, numberFormat);
                }

                PostClean(parameters, activeDir);
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
                                  () => GetPageBitmap(_documentPaginator, docPageNumber - 1, parameters), FireOnProgress);
            }
            catch(Exception ex)
            {
                DeleteImage(parameters, fileName);
                if (!parameters.IgnoreErrors)
                {
                    throw;
                }

                OnError.SafeInvoke(this, new ExceptionEventArgs(ex));
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

        private ProgressEventArgs _progressEventArgs;

        private void FireOnProgress(string fileName)
        {
            ConverterState.ActivePageIndex++;

            _progressEventArgs = new ProgressEventArgs(fileName, ConverterState);
            _converterEvent.Set();
            _mainEvent.WaitOne();
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
                throw new ConversionException(ex.Message, pageNumber+1, ex);
            }
        }

        private static void SetVisualProperties(Visual visual)
        {
            RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);

            if (VisualTextHintingModePropertyInfo != null)
            {
                // System.Windows.Media.TextHintingMode.Animated as of .NET 4.0+
                const int textHintingModeAnimated = 2;
                // Improves text rendering quality as of .NET 4.0+.
                VisualTextHintingModePropertyInfo.SetValue(visual, textHintingModeAnimated, null);
            }
        }

        private static void RenderPageToBitmapOnce(DocumentPage page, RenderTargetBitmap bitmap)
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
            const int triesSleepInterval = 1000;
            const int triesTotal = 30;
            const int maxTries = triesTotal * 10;

            var triesCount = triesTotal;

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
                    if (IsCancelled)
                    {
                        return null;
                    }

                    if (--triesCount < 0 && (!_convertionStarted || -triesCount > maxTries))
                    {
                        throw;
                    }

                    Thread.Sleep(triesSleepInterval);
                }
            }
        }

        public void Dispose()
        {
            _currentAction = null;
            _mainEvent.Set();
            _converterThread.Join();
            GC.SuppressFinalize(this);
        }
    }
}