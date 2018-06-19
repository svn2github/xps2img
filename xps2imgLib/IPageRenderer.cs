using System.Drawing;
using System.Windows.Media.Imaging;

namespace Xps2ImgLib
{
    public interface IPageRenderer
    {
        Converter.Parameters Parameters { get; }

        RenderTargetBitmap GetBitmap(Size? requiredSize = null);
        RenderTargetBitmap GetDefaultBitmap();

        Size GetBitmapSize();

        void FireOnProgress(string fileName);

        void ThrowIfCancelled();
    }
}
