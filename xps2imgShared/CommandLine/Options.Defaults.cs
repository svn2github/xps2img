using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        internal static class Defaults
        {
            public const string FileType        = "png";
            public const string JpegQuality     = "85";
            public const string TiffCompression = "zip";
            public const string Dpi             = "120";
            public const string FirstPageIndex  = "1";
            public const string PrelimsPrefix   = "$";
            public const int    Processors      = ProcessorsNumberTypeConverter.AutoValue;
            public const string ProcessPriority = Validation.AutoValue;
        }
    }
}
