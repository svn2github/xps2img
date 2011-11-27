using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Xps2Img.Xps2Img
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
                    return new TiffBitmapEncoder { Compression = imageOptions.TiffCompression };
                case ImageType.Bmp:
                    return new BmpBitmapEncoder();
                case ImageType.Gif:
                    return new GifBitmapEncoder();
                default:
                    throw new InvalidOperationException(String.Format(Resources.Strings.Error_UnknownImageType, imageType));
            }
        }

        public static void Write(string fileName, ImageType imageType, ImageOptions imageOptions, BitmapSource bitmapSource, Action<string> writeCallback)
        {
            Write(true, fileName, imageType, imageOptions, bitmapSource, writeCallback);
        }

        private static readonly Dictionary<ImageType, string> _imageTypeExtensions = new Dictionary<ImageType, string>();

        public static string GetImageExtension(ImageType imageType)
        {
            string extension;

            if (!_imageTypeExtensions.TryGetValue(imageType, out extension))
            {
                extension = CreateEncoder(imageType, new ImageOptions()).CodecInfo.FileExtensions.Split(new[] { ',' })[0];
                _imageTypeExtensions[imageType] = extension;
            }

            return extension;
        }

        public static void Write(bool writeFile, string fileName, ImageType imageType, ImageOptions imageOptions, BitmapSource bitmapSource, Action<string> writeCallback)
        {
            var bitmapEncoder = CreateEncoder(imageType, imageOptions);
            var fullFileName = fileName + GetImageExtension(imageType);

            if (writeCallback != null)
            {
                writeCallback(fullFileName);
            }

            if (writeFile)
            {
                using (var stream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    bitmapEncoder.Save(stream);
                }
            }
        }
    }
}