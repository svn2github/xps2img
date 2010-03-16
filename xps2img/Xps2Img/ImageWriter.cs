using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Xps2Img
{
  public static class ImageWriter
  {
    private static BitmapEncoder CreateEncoder(ImageType imageType, ImageOptions imageOptions)
    {
      imageOptions = imageOptions ?? ImageOptions.Default;

      switch(imageType)
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
          throw new InvalidCastException(String.Format(Resources.Strings.Error_UnknownImageType, imageType));
      }
    }
    
    public static void Write(string fileName, ImageType imageType, ImageOptions imageOptions, BitmapSource bitmapSource, Action<string> writeCallback)
    {
      var bitmapEncoder = CreateEncoder(imageType, imageOptions);
      var fullFileName = fileName + bitmapEncoder.CodecInfo.FileExtensions.Split(new[] { ',' })[0];
      if(writeCallback != null)
      {
        writeCallback(fullFileName);
      }
      using (var stream = File.Create(fullFileName))
      {
        bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        bitmapEncoder.Save(stream);
      }
      bitmapEncoder.Frames[0].Dispatcher.InvokeShutdown();
      bitmapEncoder.Dispatcher.InvokeShutdown();
      bitmapSource.Dispatcher.InvokeShutdown();
    }
  }
}