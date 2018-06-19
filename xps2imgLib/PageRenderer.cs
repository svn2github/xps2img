using System;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Xps2ImgLib
{
    public class PageRenderer : IPageRenderer
    {
        private readonly DocumentPaginator _documentPaginator;
        private readonly int _pageNumber;

        private readonly Func<DocumentPage, double, int, int, RenderTargetBitmap> _renderToBitmapFunc;

        private readonly Action<string> _fireOnProgressAction;
        private readonly Action _checkIfCancelledAction;
        
        public PageRenderer(DocumentPaginator documentPaginator, int pageNumber, Converter.Parameters parameters, Func<DocumentPage, double, int, int, RenderTargetBitmap> renderToBitmapFunc, Action<string> fireOnProgressAction, Action checkIfCancelledAction)
        {
            _documentPaginator = documentPaginator;
            _pageNumber = pageNumber;
            _renderToBitmapFunc = renderToBitmapFunc;
            _fireOnProgressAction = fireOnProgressAction;
            _checkIfCancelledAction = checkIfCancelledAction;

            Parameters = parameters;
        }

        public Converter.Parameters Parameters { get; private set; }

        public RenderTargetBitmap GetBitmap(Size? requiredSize = null)
        {
            return GetBitmap(false, requiredSize);
        }

        public RenderTargetBitmap GetDefaultBitmap()
        {
            return GetBitmap(true);
        }

        public Size GetBitmapSize()
        {
            Func<DocumentPage, double, int, int, Size> applyPageSizeFunc = (page, dpi, width, height) => new Size(width, height);

            return CalculateAndApplyPageSize(_documentPaginator, _pageNumber, false, Parameters, applyPageSizeFunc, null);
        }

        public void FireOnProgress(string fileName)
        {
            _fireOnProgressAction(fileName);
        }

        public void ThrowIfCancelled()
        {
            _checkIfCancelledAction();
        }

        private RenderTargetBitmap GetBitmap(bool renderDefault = false, Size? requiredSize = null)
        {
            return CalculateAndApplyPageSize(_documentPaginator, _pageNumber, renderDefault, Parameters, _renderToBitmapFunc, requiredSize);
        }

        private static T CalculateAndApplyPageSize<T>(DocumentPaginator documentPaginator, int pageNumber, bool renderDefault, Converter.Parameters parameters, Func<DocumentPage, double, int, int, T> applyPageSizeFunc, Size? actualSize)
        {
            const double dpiConst = 96.0;

            var dpi = renderDefault ? dpiConst : parameters.Dpi;

            var size = actualSize ?? (!renderDefault && parameters.RequiredSize.HasValue ? parameters.RequiredSize.Value : new Size());

            Func<int, bool> isSizeDefined = requiredSize => requiredSize > 0;
            Action<int, double> calcDpi = (requiredSize, pageSize) => { if (isSizeDefined(requiredSize)) { dpi = requiredSize / pageSize * dpiConst; } };

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

                    return applyPageSizeFunc(page, dpi, (int)Math.Round(page.Size.Width * ratio), (int)Math.Round(page.Size.Height * ratio));
                }
            }
            catch (XamlParseException ex)
            {
                throw new ConversionFailedException(ex.Message, pageNumber + 1, ex);
            }
        }
    }
}
