using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.Controls.Settings
{
    public static class Dpi
    {
        public const int DefaultValue = Options.Defaults.DpiValue;

        public const int TrackBarTickFrequency = 72;
        public const int TrackBarLargeChange   = 72;

        public static readonly int MinValue = Options.ValidationExpressions.MinDpiValue;
        public static readonly int MaxValue = Options.ValidationExpressions.MaxDpiValue;

        public static readonly int[] Values = Options.Defaults.DpiValues;
    }
}
