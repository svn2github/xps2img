using System.Collections.Generic;

using Xps2Img.Shared.CommandLine;

using Xps2ImgUI.Settings;

namespace Xps2ImgUI.Utils.UI
{
    public static partial class HelpUtils
    {
        private const string HelpFile = Program.ProductName + ".chm";

        public const string HelpTopicPreferences = "1000";
        public const string HelpTopicHistory     = "1001";
        public const string HelpTopicOptions     = "1002";

        private const string HelpTopicXpsFile                   = "1200";
        private const string HelpTopicOutputFolder              = "1201";
        private const string HelpTopicPostConversionAction      = "1202";
        private const string HelpTopicPageNumbers               = "1203";
        private const string HelpTopicImageType                 = "1204";
        private const string HelpTopicJpegQuality               = "1205";
        private const string HelpTopicTiffCompression           = "1206";
        private const string HelpTopicImageSize                 = "1207";
        private const string HelpTopicImageDpi                  = "1208";
        private const string HelpTopicImagePrefix               = "1209";
        private const string HelpTopicFirstPageIndex            = "1210";
        private const string HelpTopicPreliminariesPrefix       = "1211";
        private const string HelpTopicProcessors                = "1212";
        private const string HelpTopicProcessPriority           = "1213";
        private const string HelpTopicProcessAffinity           = "1214";
        private const string HelpTopicTestMode                  = "1215";
        private const string HelpTopicGeneral                   = "1249";
        private const string HelpTopicInterface                 = "1250";
        private const string HelpTopicAutoCompleteFilenames     = "1251";
        private const string HelpTopicAutoSaveSettings          = "1252";
        private const string HelpTopicClassicLook               = "1253";
        private const string HelpTopicFlashWhenCompleted        = "1254";
        private const string HelpTopicShowElapsedTime           = "1255";
        private const string HelpTopicConfirmations             = "1256";
        private const string HelpTopicConfirmAfterConversion    = "1257";
        private const string HelpTopicConfirmDelete             = "1258";
        private const string HelpTopicConfirmExit               = "1259";
        private const string HelpTopicConfirmStopConversion     = "1260";
        private const string HelpTopicConversion                = "1261";
        private const string HelpTopicAlwaysResume              = "1262";
        private const string HelpTopicSuggestResume             = "1263";
        private const string HelpTopicShortenImageExtension     = "1264";
        private const string HelpTopicUpdates                   = "1265";
        private const string HelpTopicCheckForUpdates           = "1266";
        private const string HelpTopicIgnoreExistingDisplayName = "1267";
        private const string HelpTopicIgnoreErrorsDisplayName   = "1268";
        private const string HelpTopicApplicationLanguage       = "1269";
        private const string HelpTopicFileNameAsImagePrefix     = "1270";

        private static readonly Dictionary<string, string> CategoryToTopicMap = new Dictionary<string, string>
        {
            // Options.
            { Options.Categories.Parameters,        HelpTopicXpsFile },
            { Options.Categories.Options,           HelpTopicPageNumbers },

            // Preferences.
            { Preferences.Categories.General,       HelpTopicGeneral },
            { Preferences.Categories.Interface,     HelpTopicInterface },
            { Preferences.Categories.Confirmations, HelpTopicConfirmations },
            { Preferences.Categories.Conversion,    HelpTopicConversion },
            { Preferences.Categories.Updates,       HelpTopicUpdates }
        };

        private static readonly Dictionary<string, string> PropertyToTopicMap = new Dictionary<string, string>
        {
            // Options.
            { Options.Properties.SrcFile,                   HelpTopicXpsFile },
            { Options.Properties.OutDir,                    HelpTopicOutputFolder },
            { Options.Properties.PostAction,                HelpTopicPostConversionAction },
            { Options.Properties.Pages,                     HelpTopicPageNumbers },
            { Options.Properties.FileType,                  HelpTopicImageType },
            { Options.Properties.JpegQuality,               HelpTopicJpegQuality },
            { Options.Properties.TiffCompression,           HelpTopicTiffCompression },
            { Options.Properties.RequiredSize,              HelpTopicImageSize },
            { Options.Properties.Dpi,                       HelpTopicImageDpi },
            { Options.Properties.UseFileNameAsImageName,    HelpTopicFileNameAsImagePrefix },
            { Options.Properties.ImageName,                 HelpTopicImagePrefix },
            { Options.Properties.FirstPageIndex,            HelpTopicFirstPageIndex },
            { Options.Properties.PrelimsPrefix,             HelpTopicPreliminariesPrefix },
            { Options.Properties.ProcessorsNumber,          HelpTopicProcessors },
            { Options.Properties.ProcessPriority,           HelpTopicProcessPriority },
            { Options.Properties.CpuAffinity,               HelpTopicProcessAffinity },
            { Options.Properties.IgnoreExisting,            HelpTopicIgnoreExistingDisplayName },
            { Options.Properties.IgnoreErrors,              HelpTopicIgnoreErrorsDisplayName },
            { Options.Properties.Test,                      HelpTopicTestMode },

            // Preferences.
            { Preferences.Properties.ApplicationLanguage,            HelpTopicApplicationLanguage },
            { Preferences.Properties.AutoCompleteFilenames,          HelpTopicAutoCompleteFilenames },
            { Preferences.Properties.AutoSaveSettings,               HelpTopicAutoSaveSettings },
            { Preferences.Properties.ClassicLook,                    HelpTopicClassicLook },
            { Preferences.Properties.FlashWhenCompleted,             HelpTopicFlashWhenCompleted },
            { Preferences.Properties.ShowElapsedTimeAndStatistics,   HelpTopicShowElapsedTime },
            { Preferences.Properties.ConfirmOnAfterConversion,       HelpTopicConfirmAfterConversion },
            { Preferences.Properties.ConfirmOnDelete,                HelpTopicConfirmDelete },
            { Preferences.Properties.ConfirmOnExit,                  HelpTopicConfirmExit },
            { Preferences.Properties.ConfirmOnStop,                  HelpTopicConfirmStopConversion },
            { Preferences.Properties.AlwaysResume,                   HelpTopicAlwaysResume },
            { Preferences.Properties.SuggestResume,                  HelpTopicSuggestResume },
            { Preferences.Properties.ShortenExtension,               HelpTopicShortenImageExtension },
            { Preferences.Properties.CheckForUpdates,                HelpTopicCheckForUpdates }
        };
    }
}
