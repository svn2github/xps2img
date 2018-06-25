using System.Collections.Generic;

using Xps2Img.Shared.CommandLine;

using Xps2ImgUI.Settings;

namespace Xps2ImgUI.Utils.UI
{
    public static partial class HelpUtils
    {
        private const string HelpFile = Program.ProductName + ".chm";

        public const string HelpTopicPreferences   = "1000";
        public const string HelpTopicHistory       = "1001";
        public const string HelpTopicOptions       = "1002";

        private const string HelpTopicXpsFile      = "1200";
        private const string HelpTopicOutputFolder = "1201";
        private const string HelpTopicPageNumbers  = "1203";

        private static readonly Dictionary<string, string> CategoryToTopicMap = new Dictionary<string, string>
        {
            // Options.
            { Options.Categories.Parameters,        HelpTopicXpsFile },
            { Options.Categories.Options,           HelpTopicPageNumbers },

            // Preferences.
            { Preferences.Categories.Interface,     "1250" },
            { Preferences.Categories.Confirmations, "1256" },
            { Preferences.Categories.Conversion,    "1261" },
            { Preferences.Categories.Updates,       "1265" }
        };

        private static readonly Dictionary<string, string> PropertyToTopicMap = new Dictionary<string, string>
        {
            // Options.
            { Options.Properties.SrcFile,                HelpTopicXpsFile },
            { Options.Properties.OutDir,                 HelpTopicOutputFolder },
            { Options.Properties.PostAction,             "1202" },
            { Options.Properties.Pages,                  HelpTopicPageNumbers },
            { Options.Properties.FileType,               "1204" },
            { Options.Properties.JpegQuality,            "1205" },
            { Options.Properties.TiffCompression,        "1206" },
            { Options.Properties.PreferDpiOverSize,      "1271" },           
            { Options.Properties.RequiredSize,           "1207" },
            { Options.Properties.Dpi,                    "1208" },
            { Options.Properties.PageCrop,               "1273" },
            { Options.Properties.PageCropMargin,         "1274" },
            { Options.Properties.UseFileNameAsImageName, "1270" },
            { Options.Properties.ImageName,              "1209" },
            { Options.Properties.FirstPageIndex,         "1210" },
            { Options.Properties.PrelimsPrefix,          "1211" },
            { Options.Properties.ProcessorsNumber,       "1212" },
            { Options.Properties.ProcessPriority,        "1213" },
            { Options.Properties.CpuAffinity,            "1214" },
            { Options.Properties.IgnoreExisting,         "1267" },
            { Options.Properties.IgnoreErrors,           "1268" },

            // Preferences.
            { Preferences.Properties.AutoCompleteFilenames,        "1251" },
            { Preferences.Properties.AutoSaveSettings,             "1252" },
            { Preferences.Properties.ClassicLook,                  "1253" },
            { Preferences.Properties.FlashWhenCompleted,           "1254" },
            { Preferences.Properties.ShowElapsedTimeAndStatistics, "1255" },
            { Preferences.Properties.ConfirmOnAfterConversion,     "1257" },
            { Preferences.Properties.ConfirmOnDelete,              "1258" },
            { Preferences.Properties.ConfirmOnExit,                "1259" },
            { Preferences.Properties.ConfirmOnStop,                "1260" },
            { Preferences.Properties.AlwaysResume,                 "1262" },
            { Preferences.Properties.SuggestResume,                "1263" },
            { Preferences.Properties.ShortenExtension,             "1264" },
            { Preferences.Properties.CheckForUpdates,              "1266" },
            { Preferences.Properties.UseFullExePath,               "1272" }
        };
    }
}
