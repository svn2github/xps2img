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

        public static void Write(string fileName, IPageRenderer pageRenderer)
        {
            var parameters = pageRenderer.Parameters;

            var imageType = parameters.ImageType;

            var fullFileName = fileName + GetImageExtension(imageType, parameters.ShortenExtension);

            pageRenderer.FireOnProgress(fullFileName);

            if (parameters.IgnoreExisting && File.Exists(fullFileName))
            {
                return;
            }

            pageRenderer.ThrowIfCancelled();

            var bitmapSource = Crop(pageRenderer);

            pageRenderer.ThrowIfCancelled();

            if (parameters.Test)
            {
                return;
            }

            var bitmapEncoder = CreateEncoder(imageType, parameters.ImageOptions);
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            pageRenderer.ThrowIfCancelled();

            using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bitmapEncoder.Save(fileStream);
            }
        }

        private static BitmapSource Crop(IPageRenderer pageRenderer)
        {
            var parameters = pageRenderer.Parameters;

            var pageCrop = parameters.PageCrop;
            var pageCropMargin = parameters.PageCropMargin;
            
            if (pageCrop == PageCrop.None)
            {
                return pageRenderer.GetBitmap();
            }

            if (pageCrop == PageCrop.Crop)
            {
                var bitmapSource = pageRenderer.GetBitmap();

                pageRenderer.ThrowIfCancelled();

                return bitmapSource.Crop(pageCropMargin.Width, pageCropMargin.Height);
            }

            if (pageCrop == PageCrop.Fit)
            {
                return CropToFit(pageRenderer);
            }

            throw new ArgumentOutOfRangeException("pageRenderer", pageCrop, "UNEXPECTED: Unknown page crop value");
        }

        private static BitmapSource CropToFit(IPageRenderer pageRenderer)
        {
            var pageCropMargin = pageRenderer.Parameters.PageCropMargin;

            // TODO: REMOVE
            //pageCropMargin = new Size(3, 3);

            var bitmapSource = pageRenderer.GetDefaultBitmap();
            pageRenderer.ThrowIfCancelled();

            // Do not use pageCropMargin here.
            var cropRect = bitmapSource.GetCropRectangle(pageCropMargin.Width, pageCropMargin.Height);
            var desiredSize = pageRenderer.GetBitmapSize();

            try
            {
                pageRenderer.ThrowIfCancelled();

                var xRatio = (double)desiredSize.Width / cropRect.Width;
                var fitSize = new Size((int)(bitmapSource.Width * xRatio), 0);

                bitmapSource = pageRenderer.GetBitmap(fitSize);

                var fitCropRect = bitmapSource.GetCropRectangle(pageCropMargin.Width, pageCropMargin.Height);
                var fitRect = new Int32Rect((int)(cropRect.X * xRatio), fitCropRect.Y, desiredSize.Width, fitCropRect.Height);

                pageRenderer.ThrowIfCancelled();

                return bitmapSource.Crop(fitRect);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException || ex is OutOfMemoryException)
                {
                    return pageRenderer.GetBitmap();
                }

                throw;
            }
        }
    }
}