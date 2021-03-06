﻿using System.Drawing;
using System.Linq;

using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static class Defaults
        {
            public const string FileType          = "png";
            public const string JpegQuality       = "85";
            public const string TiffCompression   = "zip";
            public const int    DpiValue          =  120;
            public const string Dpi               = "120";
            public const string PageCrop          = "none";
            public const string PageCropThreshold = "normal";
            public const string PageCropMargin    = "3x3";
            public const string FirstPageIndex    = "1";
            public const string PrelimsPrefix     = "$";
            public const int    Processors        = ProcessorsNumberTypeConverter.AutoValue;
            public const string ProcessPriority   = ValidationExpressions.AutoValue;
            public const string ImageName         = null;

            public static readonly int JpegQualityValue = IntToString(JpegQuality);

            public static readonly int[] DpiValues               = { 72, 96, 120, 144, 150, 192, 200, 213, 240, 288, 300, 320, 384, 480, 576, 600, 640, 672, 768, 864, 900, 960, 1056, 1152, 1200, 1248, 1344, 1440, 1536, 1600, 1632, 1728, 1800, 1824, 1920, 2000, 2016, 2112, 2208, 2304, 2350 };
            public static readonly int[] JpegQualityValues       = { 10, 15, 25, 35, 45, 55, 65, 75, 85, 95, 100 };

            private static readonly int[] ScreenSizeValues       = { 360, 480, 540, 600, 720, 768, 800, 854, 900, 960, 1024, 1050, 1080, 1136, 1200, 1280, 1334, 1366, 1440, 1536, 1600, 1700, 1800, 1824, 1920, 2000, 2048, 2160, 2304, 2560, 2880, 3840, 4096 };

            public static readonly string[] PrelimsPrefixValues  = { "!", "#", "$", ".", "!!!", "###", "$$$", "..." };
            public static readonly Size[]   RequiredSizeValues   = ScreenSizeValues.Select(s => new Size(s, 0)).Concat(ScreenSizeValues.Select(s => new Size(0, s))).ToArray();
            public static readonly Size[]   PageCropMarginValues = Enumerable.Range(0, 40+1).Select(w => new Size(w, w)).ToArray();
            public static readonly string[] ImageNameValues      = PrelimsPrefixValues;
        }
    }
}
