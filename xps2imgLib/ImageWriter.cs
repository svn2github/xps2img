using System;
using System.Collections.Generic;
using System.IO;
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
                extension = CreateEncoder(imageType, new ImageOptions()).CodecInfo.FileExtensions.Split(',')[0];
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
            var pageCrop = pageRenderer.Parameters.PageCrop;
            var pageCropMargin = pageRenderer.Parameters.PageCropMargin;
            
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

            var fitWidth = (pageRenderer.Parameters.RequiredSize ?? new Size()).Height <= 0;

            var bitmapSource = pageRenderer.GetDefaultBitmap();

            pageRenderer.ThrowIfCancelled();

            var cropRect = bitmapSource.GetCropRectangle();
            var desiredSize = pageRenderer.GetBitmapSize();

            try
            {
                pageRenderer.ThrowIfCancelled();

                var marginWidth  = pageCropMargin.Width  * 2 + 2;
                var marginHeight = pageCropMargin.Height * 2 + 2;

                var desiredSizeWidth  = desiredSize.Width;
                var desiredSizeHeight = desiredSize.Height;

                var xRatio = cropRect.Width  != 0 ? (desiredSizeWidth  > marginWidth  ? desiredSizeWidth  - marginWidth  : desiredSizeWidth)  / (double)cropRect.Width  : 1;
                var yRatio = cropRect.Height != 0 ? (desiredSizeHeight > marginHeight ? desiredSizeHeight - marginHeight : desiredSizeHeight) / (double)cropRect.Height : 1;

                var fitSize = new Size(fitWidth  ? (int)Math.Round(bitmapSource.Width  * xRatio) : 0,
                                       !fitWidth ? (int)Math.Round(bitmapSource.Height * yRatio) : 0);

                bitmapSource = pageRenderer.GetBitmap(fitSize);

                var fitCropRect = bitmapSource.GetCropRectangle(pageCropMargin.Width, pageCropMargin.Height);

                if (fitWidth)
                {
                    fitCropRect.Width  = desiredSizeWidth;
                }
                else
                {
                    fitCropRect.Height = desiredSizeHeight;
                }

                pageRenderer.ThrowIfCancelled();

                return bitmapSource.Crop(fitCropRect);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException || ex is OutOfMemoryException || ex is ArgumentException)
                {
                    return pageRenderer.GetBitmap();
                }

                throw;
            }
        }
    }
}