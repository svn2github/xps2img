using System.Windows.Media.Imaging;

namespace Xps2Img.Shared.CommandLine
{
    public enum ImageType
    {
        Png,
        Jpeg,
        Tiff,
        Bmp,
        Gif
    }

    public class ImageOptions
    {
        public ImageOptions() :
            this(Default.JpegQualityLevel, Default.TiffCompression)
        {
        }

        public ImageOptions(int jpegQualityLevel, TiffCompressOption tiffCompression)
        {
            JpegQualityLevel = jpegQualityLevel;
            TiffCompression = tiffCompression;
        }

        public readonly int JpegQualityLevel;
        public readonly TiffCompressOption TiffCompression;

        public static readonly ImageOptions Default = new ImageOptions(85, TiffCompressOption.Zip);
    }
}