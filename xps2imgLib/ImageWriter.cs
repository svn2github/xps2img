using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

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

        public static void Write(Converter.Parameters parameters, string fileName, Func<BitmapSource> getBitmapSourceFunc, Action<string> writeCallback, Action checkIfCancelled)
        {
            var imageType = parameters.ImageType;

            var fullFileName = fileName + GetImageExtension(imageType, parameters.ShortenExtension);

            if (writeCallback != null)
            {
                writeCallback(fullFileName);
            }

            if (parameters.Test)
            {
                checkIfCancelled();
                getBitmapSourceFunc();
                return;
            }

            if (parameters.IgnoreExisting && File.Exists(fullFileName))
            {
                return;
            }

            var bitmapEncoder = CreateEncoder(imageType, parameters.ImageOptions);

            checkIfCancelled();
            var bitmapSource = getBitmapSourceFunc();
            checkIfCancelled();

            bitmapSource = Crop(parameters.PageCrop, bitmapSource, parameters.PageCropMargin);
            checkIfCancelled();

            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            checkIfCancelled();

            using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bitmapEncoder.Save(fileStream);
            }
        }

        private static BitmapSource Crop(PageCrop pageCrop, BitmapSource bitmapSource, Size pageCropMargin)
        {
            switch (pageCrop)
            {
                case PageCrop.None:
                    return bitmapSource;
                case PageCrop.Crop:
                    return bitmapSource.Crop(pageCropMargin.Width, pageCropMargin.Height);
                case PageCrop.Fit:
                    return bitmapSource;
                default:
                    throw new ArgumentOutOfRangeException("pageCrop", pageCrop, "UNEXPECTED: Unknown page crop value");
            }
        }
    }
}