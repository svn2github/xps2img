﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

using Xps2Img.Shared.CommandLine;

using TiffCompressOption = System.Windows.Media.Imaging.TiffCompressOption;

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
                    return new TiffBitmapEncoder { Compression = (TiffCompressOption) imageOptions.TiffCompression };
                case ImageType.Bmp:
                    return new BmpBitmapEncoder();
                case ImageType.Gif:
                    return new GifBitmapEncoder();
                default:
                    throw new InvalidOperationException(String.Format(Resources.Strings.Error_UnknownImageType, imageType));
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

        public static void Write(Func<string, bool> shouldWriteFileFunc, string fileName, ImageType imageType, bool shortenExtension, ImageOptions imageOptions, bool forceGetBitmapSourceIfNoWrite, Func<BitmapSource> getBitmapSourceFunc, Action<string> writeCallback)
        {
            var bitmapEncoder = CreateEncoder(imageType, imageOptions);
            var fullFileName = fileName + GetImageExtension(imageType, shortenExtension);

            if (writeCallback != null)
            {
                writeCallback(fullFileName);
            }

            if (!shouldWriteFileFunc(fullFileName))
            {
                if (forceGetBitmapSourceIfNoWrite)
                {
                    getBitmapSourceFunc();
                }
                return;
            }

            using (var stream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(getBitmapSourceFunc()));
                bitmapEncoder.Save(stream);
            }
        }
    }
}