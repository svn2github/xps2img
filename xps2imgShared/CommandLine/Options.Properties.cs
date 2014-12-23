// ReSharper disable ConvertToConstant.Global

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static class Properties
        {
            public static class Consts
            {
                // ReSharper disable MemberHidesStaticFromOuterClass
                public const string ProcessPriority = "ProcessPriority";
                public const string SrcFile         = "SrcFile";
                public const string CpuAffinity     = "CpuAffinity";
                public const string IgnoreExisting  = "IgnoreExisting";
                // ReSharper restore MemberHidesStaticFromOuterClass
            }

            public static readonly string SrcFile           = Consts.SrcFile;
            public static readonly string OutDir            = "OutDir";
            public static readonly string PostAction        = "PostAction";
            public static readonly string Pages             = "Pages";
            public static readonly string FileType          = "FileType";
            public static readonly string JpegQuality       = "JpegQuality";
            public static readonly string TiffCompression   = "TiffCompression";
            public static readonly string RequiredSize      = "RequiredSize";
            public static readonly string Dpi               = "Dpi";
            public static readonly string ImageName         = "ImageName";
            public static readonly string FirstPageIndex    = "FirstPageIndex";
            public static readonly string PrelimsPrefix     = "PrelimsPrefix";
            public static readonly string ProcessorsNumber  = "ProcessorsNumber";
            public static readonly string ProcessPriority   = Consts.ProcessPriority;
            public static readonly string CpuAffinity       = Consts.CpuAffinity;
            public static readonly string IgnoreExisting    = Consts.IgnoreExisting;
            public static readonly string IgnoreErrors      = "IgnoreErrors";
            public static readonly string Test              = "Test";
        }
    }
}
