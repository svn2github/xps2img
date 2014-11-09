namespace Xps2ImgUI.Settings
{
    public partial class Preferences
    {
        public  const string CategoryApplication   = "Application";
        public  const string CategoryConfirmations = "Confirmations";
        public  const string CategoryConversion    = "Conversion";
        public  const string CategoryUpdates       = "Updates";
        
        public  const string DefaultSelectedItem            = ApplicationLanguageDisplayName;

        private const string ApplicationLanguageDisplayName = "Application Language";
        private const string ApplicationLanguageDescription = "User interface language.";

        private const string AutoCompleteFilenamesDisplayName = "Auto Complete Filenames";
        private const string AutoCompleteFilenamesDescription = "Auto complete filenames in edit boxes.";

        private const string AutoSaveSettingsDisplayName = "Auto Save Settings";
        private const string AutoSaveSettingsDescription = "Auto save settings on exit. If not set default settings will be used next time.";

        private const string ClassicLookDisplayName = "Classic Look";
        private const string ClassicLookDescription = "Use application system look.";

        private const string ShowElapsedTimeAndStatisticsDisplayName = "Show Elapsed Time and Statistics";
        private const string ShowElapsedTimeAndStatisticsDescription = "Show time elapsed by last operation and conversion statistics in application title.";

        private const string FlashWhenCompletedDisplayName = "Flash When Completed";
        private const string FlashWhenCompletedDescription = "Flash application window when conversion is completed.";

        private const string ConfirmAfterConversionDisplayName = "Confirm After Conversion";
        private const string ConfirmAfterConversionDescription = "Ask confirmation on after conversion action.";

        private const string ConfirmDeleteDisplayName = "Confirm Delete";
        private const string ConfirmDeleteDescription = "Ask confirmation on images deletion.";

        private const string ConfirmExitDisplayName = "Confirm Exit";
        private const string ConfirmExitDescription = "Ask confirmation on application exit if conversion is in progress.";

        private const string ConfirmStopConversionDisplayName = "Confirm Stop Conversion";
        private const string ConfirmStopConversionDescription = "Ask confirmation on conversion stop.";

        private const string AlwaysResumeDisplayName = "Always Resume";
        private const string AlwaysResumeDescription = "Always resume last conversion if applicable instead of starting new one.";

        private const string SuggestResumeDisplayName = "Suggest Resume";
        private const string SuggestResumeDescription = "Suggest to resume last conversion if applicable instead of starting new one.";

        private const string ShortenImageExtensionDisplayName  = "Shorten Image Extension";
        private const string ShortensImageExtensionDescription = "Shortens image extension down to three characters.";

        private const string CheckForUpdatesDisplayName = "Check for Updates";
        private const string CheckForUpdatesDescription = "Check for updates interval.";
    }
}
