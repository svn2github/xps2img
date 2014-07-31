using System;
using System.Collections.Generic;
using System.Windows.Forms;

using CommandLine;

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
        private const string HelpTopicApplication               = "1250";
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

        private static readonly Options OptionsRef = new Options();
        private static readonly Preferences PreferencesRef = new Preferences();

        private static readonly Dictionary<string, string> PropertyToTopicMap = new Dictionary<string, string>
        {
            // Options.
            { Options.CategoryParameters,                                           HelpTopicXpsFile },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.SrcFile),            HelpTopicXpsFile },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.OutDir),             HelpTopicOutputFolder },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.PostAction),         HelpTopicPostConversionAction },
            { Options.CategoryOptions,                                              HelpTopicPageNumbers },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.Pages),              HelpTopicPageNumbers },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.FileType),           HelpTopicImageType },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.JpegQuality),        HelpTopicJpegQuality },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.TiffCompression),    HelpTopicTiffCompression },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.RequiredSize),       HelpTopicImageSize },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.Dpi),                HelpTopicImageDpi },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.ImageName),          HelpTopicImagePrefix },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.FirstPageIndex),     HelpTopicFirstPageIndex },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.PrelimsPrefix),      HelpTopicPreliminariesPrefix },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.ProcessorsNumber),   HelpTopicProcessors },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.ProcessPriority),    HelpTopicProcessPriority },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.CpuAffinity),        HelpTopicProcessAffinity },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.IgnoreExisting),     HelpTopicIgnoreExistingDisplayName },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.IgnoreErrors),       HelpTopicIgnoreErrorsDisplayName },
            { ReflectionUtils.GetPropertyName(() => OptionsRef.Test),               HelpTopicTestMode },

            // Preferences.
            { Preferences.CategoryApplication,                                                      HelpTopicApplication },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ApplicationLanguage),            HelpTopicApplicationLanguage },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.AutoCompleteFilenames),          HelpTopicAutoCompleteFilenames },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.AutoSaveSettings),               HelpTopicAutoSaveSettings },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ClassicLook),                    HelpTopicClassicLook },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.FlashWhenCompleted),             HelpTopicFlashWhenCompleted },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ShowElapsedTimeAndStatistics),   HelpTopicShowElapsedTime },
            { Preferences.CategoryConfirmations,                                                    HelpTopicConfirmations },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ConfirmOnAfterConversion),       HelpTopicConfirmAfterConversion },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ConfirmOnDelete),                HelpTopicConfirmDelete },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ConfirmOnExit),                  HelpTopicConfirmExit },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ConfirmOnStop),                  HelpTopicConfirmStopConversion },
            { Preferences.CategoryConversion,                                                       HelpTopicConversion },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.AlwaysResume),                   HelpTopicAlwaysResume },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.SuggestResume),                  HelpTopicSuggestResume },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.ShortenExtension),               HelpTopicShortenImageExtension },
            { Preferences.CategoryUpdates,                                                          HelpTopicUpdates },
            { ReflectionUtils.GetPropertyName(() => PreferencesRef.CheckForUpdates),                HelpTopicCheckForUpdates }
        };

        public static void ShowHelpTableOfContents(this Control control)
        {
            Help.ShowHelp(control, HelpFile, HelpNavigator.TableOfContents);
        }

        public static void ShowHelpTopicId(this Control control, string topicId)
        {
            Help.ShowHelp(control, HelpFile, HelpNavigator.TopicId, topicId);
        }

        public static bool ShowPropertyHelp(this Control control, string text, string fallbackTopicId = null)
        {
            string topicId;

            if (String.IsNullOrEmpty(text))
            {
                return false;
            }

            if (!PropertyToTopicMap.TryGetValue(text, out topicId))
            {
                if (fallbackTopicId == null)
                {
                    return false;
                }
                topicId = fallbackTopicId;
            }

            control.ShowHelpTopicId(topicId);

            return true;
        }

        public static bool ShowPropertyHelp(this Control control, PropertyGrid propertyGrid, string fallbackTopicId = null)
        {
            if (!propertyGrid.Focused)
            {
                return false;
            }

            var gridItem = propertyGrid.SelectedGridItem;
            return gridItem != null && control.ShowPropertyHelp(gridItem.PropertyDescriptor != null ? gridItem.PropertyDescriptor.Name : gridItem.Label, fallbackTopicId);
        }
    }
}
