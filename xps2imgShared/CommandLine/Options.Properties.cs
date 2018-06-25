// ReSharper disable ConvertToConstant.Global

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static class Properties
        {
            internal static class Consts
            {
                // ReSharper disable MemberHidesStaticFromOuterClass
                internal const string SrcFile                   = "SrcFile";
                internal const string ProcessPriority           = "ProcessPriority";
                internal const string CpuAffinity               = "CpuAffinity";
                internal const string IgnoreExisting            = "IgnoreExisting";
                internal const string FileType                  = "FileType";
                internal const string PreferDpiOverSize         = "PreferDpiOverSize";
                internal const string RequiredSize              = "RequiredSize";
                internal const string PageCrop                  = "PageCrop";
                internal const string UseFileNameAsImageName    = "UseFileNameAsImageName";
                internal const string ImageName                 = "ImageName";
                // ReSharper restore MemberHidesStaticFromOuterClass
            }

            public static readonly string SrcFile               = Consts.SrcFile;
            public static readonly string OutDir                = "OutDir";
            public static readonly string PostAction            = "PostAction";
            public static readonly string Pages                 = "Pages";
            public static readonly string FileType              = Consts.FileType;
            public static readonly string JpegQuality           = "JpegQuality";
            public static readonly string TiffCompression       = "TiffCompression";
            public static readonly string PreferDpiOverSize     = "PreferDpiOverSize";
            public static readonly string RequiredSize          = Consts.RequiredSize;
            public static readonly string Dpi                   = "Dpi";
            public static readonly string PageCrop              = Consts.PageCrop;
            public static readonly string PageCropMargin        = "PageCropMargin";
            public static readonly string UseFileNameAsImageName= Consts.UseFileNameAsImageName;
            public static readonly string ImageName             = Consts.ImageName;
            public static readonly string FirstPageIndex        = "FirstPageIndex";
            public static readonly string PrelimsPrefix         = "PrelimsPrefix";
            public static readonly string ProcessorsNumber      = "ProcessorsNumber";
            public static readonly string ProcessPriority       = Consts.ProcessPriority;
            public static readonly string CpuAffinity           = Consts.CpuAffinity;
            public static readonly string IgnoreExisting        = Consts.IgnoreExisting;
            public static readonly string IgnoreErrors          = "IgnoreErrors";
            public static readonly string Test                  = "Test";
        }
    }
}
