using System.Windows;
using System.Windows.Media.Imaging;

namespace Xps2ImgLib
{
    public static class ImageCropper
    {
        public static Int32Rect GetCropRectangle(this BitmapSource bitmapSource, int pageCropThreshold, int xMargin = 0, int yMargin = 0)
        {
            var cropRectangle = bitmapSource.ProcessData(GetCropRectangle, pageCropThreshold);
            return bitmapSource.AdjustСropRectangle(cropRectangle, xMargin, yMargin);
        }

        public static BitmapSource Crop(this BitmapSource bitmapSource, int pageCropThreshold, int xMargin = 0, int yMargin = 0)
        {
            return Crop(bitmapSource, bitmapSource.GetCropRectangle(pageCropThreshold, xMargin, yMargin));
        }

        public static BitmapSource Crop(this BitmapSource bitmapSource, Int32Rect cropRectangle = default(Int32Rect))
        {
            return new CroppedBitmap(bitmapSource, bitmapSource.AdjustСropRectangle(cropRectangle, 0, 0));
        }

        private static Int32Rect AdjustСropRectangle(this BitmapSource bitmapSource, Int32Rect cropRectangle, int xMargin, int yMargin)
        {
            var x = AdjustPosition(cropRectangle.X, xMargin);
            var y = AdjustPosition(cropRectangle.Y, yMargin);

            return new Int32Rect(x, y, AdjustSize(x, cropRectangle.Width, xMargin, bitmapSource.PixelWidth), AdjustSize(y, cropRectangle.Height, yMargin, bitmapSource.PixelHeight));
        }

        private static int AdjustPosition(int original, int adjust)
        {
            var delta = original - adjust;
            return delta <= 0 ? original : delta;
        }

        private static int AdjustSize(int pos, int original, int adjust, int max)
        {
            var size = AdjustSize(original, adjust, max);
            return size + pos > max ? max - pos : size;
        }

        private static int AdjustSize(int original, int adjust, int max)
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

        private static unsafe Int32Rect GetCropRectangle(ImageProcessor.Parameters<int> parameters)
        {
            return ImageMeasurer.GetCropRectangle(parameters.Data.ToPointer(), (int)parameters.Stride, parameters.Width, parameters.Height, parameters.Parameter);
        }
    }
}