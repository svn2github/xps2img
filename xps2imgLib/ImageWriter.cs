using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using Size = System.Drawing.Size;

namespace Xps2ImgLib
{
    public static class ImageWriter
    {
        private static BitmapEncoder CreateEncoder(ImageType imageType, ImageOptions imageOptions)
        {
            imageOptions = imageOptions ?? ImageOptions.Default;

            switch (imageType)
            {
                case ImageType.Png:
                    return new PngBitmapEncoder();
                case ImageType.Jpeg:
                    return new JpegBitmapEncoder { QualityLevel = imageOptions.JpegQualityLevel };
                case ImageType.Tiff:
                    return new TiffBitmapEncoder { Compression = (System.Windows.Media.Imaging.TiffCompressOption) imageOptions.TiffCompression };
                case ImageType.Bmp:
                    return new BmpBitmapEncoder();
                case ImageType.Gif:
                    return new GifBitmapEncoder();
                default:
                    throw new InvalidOperationException(String.Format("Unknown image type '{0}'", imageType));
            }
        }

        private static readonly Dictionary<ImageType, string> ImageTypeExtensions = new Dictionary<ImageType, string>();

        public static string GetImageExtension(ImageType imageType, bool shortenExtension)
        {
            string extension;

            if (!ImageTypeExtensions.TryGetValue(imageType, out extension))
            {
                extension = CreateEncoder(imageType, new ImageOptions()).CodecInfo.FileExtensions.Split(new[] { ',' })[0];
                if (shortenExtension && extension.Length > 4)
                {
                    extension = extension.Remove(extension.Length - 2, 1);
                }
                ImageTypeExtensions[imageType] = extension;
            }

            return extension;
        }

        public static void Write(Converter.Parameters parameters, string fileName, Func<bool, Size?, BitmapSource> getBitmapSourceFunc, Func<Size> getSizeFunc, Action<string> writeCallback, Action checkIfCancelled)
        {
            var imageType = parameters.ImageType;

            var fullFileName = fileName + GetImageExtension(imageType, parameters.ShortenExtension);

            if (writeCallback != null)
            {
                writeCallback(fullFileName);
            }

            if (parameters.IgnoreExisting && File.Exists(fullFileName))
            {
                return;
            }

            checkIfCancelled();
            var bitmapSource = Crop(parameters.PageCrop, parameters.PageCropMargin, getBitmapSourceFunc, getSizeFunc, checkIfCancelled);
            checkIfCancelled();

            if (parameters.Test)
            {
                return;
            }

            var bitmapEncoder = CreateEncoder(imageType, parameters.ImageOptions);
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            checkIfCancelled();

            using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bitmapEncoder.Save(fileStream);
            }
        }

        private static BitmapSource Crop(PageCrop pageCrop, Size pageCropMargin, Func<bool, Size?, BitmapSource> getBitmapSourceFunc, Func<Size> getSizeFunc, Action checkIfCancelled)
        {
            if(pageCrop == PageCrop.None)
            {
                return getBitmapSourceFunc(false, null);
            }

            if (pageCrop == PageCrop.Crop)
            {
                var bitmapSource = getBitmapSourceFunc(false, null);
                checkIfCancelled();
                return bitmapSource.Crop(pageCropMargin.Width, pageCropMargin.Height);
            }

            if (pageCrop == PageCrop.Fit)
            {
                // TODO: REMOVE
                //pageCropMargin = new Size(3, 3);

                var bitmapSource = getBitmapSourceFunc(true, null);
                checkIfCancelled();
                // Do not use pageCropMargin here.
                var cropRectangle = bitmapSource.GetCropRectangle(pageCropMargin.Width, pageCropMargin.Height);
                var desiredSize = getSizeFunc();

                var xRatio = (double)desiredSize.Width / cropRectangle.Width;

                var fitSize = new Size((int)(bitmapSource.Width * xRatio), 0);

                try
                {
                    checkIfCancelled();
                    bitmapSource = getBitmapSourceFunc(false, fitSize);

                    var cropRectangle1 = bitmapSource.GetCropRectangle(pageCropMargin.Width, pageCropMargin.Height);
                    var fitRect = new Int32Rect((int)(cropRectangle.X * xRatio), cropRectangle1.Y, desiredSize.Width, cropRectangle1.Height);

                    checkIfCancelled();
                    return bitmapSource.Crop(fitRect);
                }
                catch (Exception ex)
                {
                    if (ex is OverflowException || ex is OutOfMemoryException)
                    {
                        return getBitmapSourceFunc(false, null);
                    }

                    throw;
                }
            }

            throw new ArgumentOutOfRangeException("pageCrop", pageCrop, "UNEXPECTED: Unknown page crop value");
        }
    }
}