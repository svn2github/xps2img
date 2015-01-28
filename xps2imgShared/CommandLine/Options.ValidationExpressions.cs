using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static class ValidationExpressions
        {
            public static class Regexps
            {
                // ReSharper disable MemberHidesStaticFromOuterClass
                public const string FirstPageIndex = @"^\s*""?\s*(?<first>[1-9](\d{1,4})?)((\s+|(\s*-\s*))(?<second>[1-9](\d{1,4})?))?\s*""?\s*$";
                // ReSharper restore MemberHidesStaticFromOuterClass
            }

            public const string AutoValue      = "Auto";
            public const string CpuAffinity    = "/" + Interval.ValidationRegex0 + "/i";
            public const string Dpi            = "16-2350";
            public const string FileName       = @"/^([^\x00-\x1F<>:""/\\|?*])*$/";
            public const string FirstPageIndex = "/" + Regexps.FirstPageIndex + "/";
            public const string ImageName      = FileName;
            public const string JpegQuality    = "10-100";
            public const string Pages          = "/" + Interval.ValidationRegex + "/";
            public const string PrelimsPrefix  = FileName;
            public const string RequiredSize   = "/" + @"^$|" + RequiredSizeTypeConverter.ValidationRegex + "/";
        }
    }
}
