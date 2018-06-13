using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Xps2ImgLib
{
    public static class ImageCropper
    {
        public static BitmapSource Crop(this BitmapSource bitmapSource)
        {
            var cropRectangle = bitmapSource.ProcessData(GetCropRectangle);

            return new CroppedBitmap(bitmapSource, cropRectangle);
        }

        private static unsafe Int32Rect GetCropRectangle(IntPtr data, uint stride, int width, int height)
        {
            return ImageMeasurer.GetCropRectangle(data.ToPointer(), (int) stride, width, height);
        }
    }
}