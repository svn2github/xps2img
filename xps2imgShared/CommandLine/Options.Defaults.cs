using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static class Defaults
        {
            public const string FileType        = "png";
            public const string JpegQuality     = "85";
            public const string TiffCompression = "zip";
            public const int    DpiValue        =  120;
            public const string Dpi             = "120";
            public const string FirstPageIndex  = "1";
            public const string PrelimsPrefix   = "$";
            public const int    Processors      = ProcessorsNumberTypeConverter.AutoValue;
            public const string ProcessPriority = ValidationExpressions.AutoValue;
            public const string ImageName       = null;

            public static readonly int[] DpiValues              = { 72, 96, 120, 150, 300, 600, 900, 1200, 1600, 1800, 2350 };
            public static readonly int[] JpegQualityValues      = { 10, 15, 25, 35, 45, 55, 65, 75, 85, 95, 100 };

            public static readonly string[] PrelimsPrefixValues = { "!", "#", "$", ".", "!!!", "###", "$$$", "..." };
        }
    }
}
