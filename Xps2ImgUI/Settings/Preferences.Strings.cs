namespace Xps2ImgUI.Settings
{
    public partial class Preferences
    {
        public  const string CategoryApplication   = "Application";
        public  const string CategoryConfirmations = "Confirmations";
        public  const string CategoryConversion    = "Conversion";
        public  const string CategoryUpdates       = "Updates";
        
        public  const string DefaultSelectedItem            = ApplicationLanguageDisplayName;

        public  const string ApplicationLanguageDisplayName = "Application Language";
        private const string ApplicationLanguageDescription = "User interface language.";

        public  const string AutoCompleteFilenamesDisplayName = "Auto Complete Filenames";
        private const string AutoCompleteFilenamesDescription = "Auto complete filenames in edit boxes.";

        public  const string AutoSaveSettingsDisplayName = "Auto Save Settings";
        private const string AutoSaveSettingsDescription = "Auto save settings on exit. If not set default settings will be used next time.";

        public  const string ClassicLookDisplayName = "Classic Look";
        private const string ClassicLookDescription = "Use application system look.";

        public  const string ShowElapsedTimeAndStatisticsDisplayName = "Show Elapsed Time and Statistics";
        private const string ShowElapsedTimeAndStatisticsDescription = "Show time elapsed by last operation and conversion statistics in application title.";

        public  const string FlashWhenCompletedDisplayName = "Flash When Completed";
        private const string FlashWhenCompletedDescription = "Flash application window when conversion is completed.";

        public  const string ConfirmAfterConversionDisplayName = "Confirm After Conversion";
        private const string ConfirmAfterConversionDescription = "Ask confirmation on after conversion action.";

        public  const string ConfirmDeleteDisplayName = "Confirm Delete";
        private const string ConfirmDeleteDescription = "Ask confirmation on images deletion.";

        public  const string ConfirmExitDisplayName = "Confirm Exit";
        private const string ConfirmExitDescription = "Ask confirmation on application exit if conversion is in progress.";

        public  const string ConfirmStopConversionDisplayName = "Confirm Stop Conversion";
        private const string ConfirmStopConversionDescription = "Ask confirmation on conversion stop.";

        public  const string AlwaysResumeDisplayName = "Always Resume";
        private const string AlwaysResumeDescription = "Always resume last conversion if applicable instead of starting new one.";

        public  const string SuggestResumeDisplayName = "Suggest Resume";
        private const string SuggestResumeDescription = "Suggest to resume last conversion if applicable instead of starting new one.";

        public  const string ShortenImageExtensionDisplayName  = "Shorten Image Extension";
        private const string ShortensImageExtensionDescription = "Shortens image extension down to three characters.";

        public  const string CheckForUpdatesDisplayName = "Check for Updates";
        private const string CheckForUpdatesDescription = "Check for updates interval.";
    }
}
