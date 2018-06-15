using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Xps2ImgLib
{
    public static class ImageCropper
    {
        public static BitmapSource Crop(this BitmapSource bitmapSource, int xMargin, int yMargin)
        {
            var cropRectangle = bitmapSource.ProcessData(GetCropRectangle);

            cropRectangle.X = AdjustPosition(cropRectangle.X, xMargin);
            cropRectangle.Y = AdjustPosition(cropRectangle.Y, yMargin);
            cropRectangle.Width = AdjustSize(cropRectangle.Width, xMargin, bitmapSource.PixelWidth);
            cropRectangle.Height = AdjustSize(cropRectangle.Height, yMargin, bitmapSource.PixelHeight);

            return new CroppedBitmap(bitmapSource, cropRectangle);
        }

        public static int AdjustPosition(int original, int adjust)
        {
            var delta = original - adjust;
            return delta < 0 ? original : delta;
        }

        public static int AdjustSize(int original, int adjust, int max)
        {
            if (original <= 0)
            {
                return max;
            }

            var delta = original + adjust;
            if (delta > max)
            {
                return original;
            }

            var delta2 = delta + adjust;
            return delta2 > max ? delta : delta2;
        }

        private static unsafe Int32Rect GetCropRectangle(IntPtr data, uint stride, int width, int height)
        {
            return ImageMeasurer.GetCropRectangle(data.ToPointer(), (int) stride, width, height);
        }
    }
}