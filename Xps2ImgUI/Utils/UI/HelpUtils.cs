using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Xps2Img.Shared.CommandLine;

using Xps2ImgUI.Settings;

namespace Xps2ImgUI.Utils.UI
{
    public static class HelpUtils
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

        private static readonly Dictionary<Func<string>, string> CategoryToTopicMap = new Dictionary<Func<string>, string>
        {
            // Options.
            { () => Options.CategoryParameters,         HelpTopicXpsFile },
            { () => Options.CategoryOptions,            HelpTopicPageNumbers },

            // Preferences.
            { () => Preferences.CategoryGeneral,        HelpTopicGeneral },
            { () => Preferences.CategoryInterface,      HelpTopicInterface },
            { () => Preferences.CategoryConfirmations,  HelpTopicConfirmations },
            { () => Preferences.CategoryConversion,     HelpTopicConversion },
            { () => Preferences.CategoryUpdates,        HelpTopicUpdates }
        };

        private static readonly Dictionary<string, string> PropertyToTopicMap = new Dictionary<string, string>
        {
            // Options.
            { Options.Properties.SrcFile,            HelpTopicXpsFile },
            { Options.Properties.OutDir,             HelpTopicOutputFolder },
            { Options.Properties.PostAction,         HelpTopicPostConversionAction },
            { Options.Properties.Pages,              HelpTopicPageNumbers },
            { Options.Properties.FileType,           HelpTopicImageType },
            { Options.Properties.JpegQuality,        HelpTopicJpegQuality },
            { Options.Properties.TiffCompression,    HelpTopicTiffCompression },
            { Options.Properties.RequiredSize,       HelpTopicImageSize },
            { Options.Properties.Dpi,                HelpTopicImageDpi },
            { Options.Properties.ImageName,          HelpTopicImagePrefix },
            { Options.Properties.FirstPageIndex,     HelpTopicFirstPageIndex },
            { Options.Properties.PrelimsPrefix,      HelpTopicPreliminariesPrefix },
            { Options.Properties.ProcessorsNumber,   HelpTopicProcessors },
            { Options.Properties.ProcessPriority,    HelpTopicProcessPriority },
            { Options.Properties.CpuAffinity,        HelpTopicProcessAffinity },
            { Options.Properties.IgnoreExisting,     HelpTopicIgnoreExistingDisplayName },
            { Options.Properties.IgnoreErrors,       HelpTopicIgnoreErrorsDisplayName },
            { Options.Properties.Test,               HelpTopicTestMode },

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

        private static Form _helpForm;

        private static Form HelpForm
        {
            get { return _helpForm ?? (_helpForm = new Form()); }
        }

        public static void ShowHelpTableOfContents()
        {
            Help.ShowHelp(HelpForm, HelpFile, HelpNavigator.TableOfContents);
        }

        public static void ShowHelpTopicId(string topicId)
        {
            Help.ShowHelp(HelpForm, HelpFile, HelpNavigator.TopicId, topicId);
        }

        private static bool ShowPropertyHelp(string text, string fallbackTopicId = null)
        {
            string topicId;

            if (String.IsNullOrEmpty(text))
            {
                return false;
            }

            if (!PropertyToTopicMap.TryGetValue(text, out topicId))
            {
                topicId = CategoryToTopicMap.FirstOrDefault(kvp => kvp.Key() == text).Value ?? fallbackTopicId;

                if (topicId == null)
                {
                    return false;
                }
            }

            ShowHelpTopicId(topicId);

            return true;
        }

        public static bool ShowPropertyHelp(PropertyGrid propertyGrid, string fallbackTopicId = null)
        {
            if (!propertyGrid.Focused)
            {
                return false;
            }

            var gridItem = propertyGrid.SelectedGridItem;
            return gridItem != null && ShowPropertyHelp(gridItem.IsCategory() ? propertyGrid.GetCategoryName(gridItem) : gridItem.PropertyDescriptor.Name, fallbackTopicId);
        }
    }
}
