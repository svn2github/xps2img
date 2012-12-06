using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommandLine;

using Xps2Img.Shared.TypeConverters;

// ReSharper disable LocalizableElement

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public class Preferences : IEquatable<Preferences>
    {
        private const string CategoryApplication    = "Application";
        private const string CategoryConfirmations  = "Confirmations";
        private const string CategoryConversion     = "Conversion";
        private const string CategoryUpdates        = "Updates";

        public const string DefaultSelectedItem     = "Auto Complete Filenames";

        [DisplayName(DefaultSelectedItem)]
        [Category(CategoryApplication)]
        [Description("Auto complete filenames in edit boxes.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoCompleteFilenames { get; set; }

        [DisplayName("Auto Save Settings")]
        [Category(CategoryApplication)]
        [Description("Auto save settings on exit. If not set default settings will be used next time.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoSaveSettings { get; set; }

        [DisplayName("Classic Look")]
        [Category(CategoryApplication)]
        [Description("Use application system look.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ClassicLook { get; set; }

        [DisplayName("Show Elapsed Time and Statistics")]
        [Category(CategoryApplication)]
        [Description("Show time elapsed by last operation and conversion statistics at application title.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTimeAndStatistics { get; set; }

        [DisplayName("Flash When Completed")]
        [Category(CategoryApplication)]
        [Description("Flash application window when conversion is completed.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool FlashWhenCompleted { get; set; }

        [DisplayName("Confirm Delete")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on images deletion.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName("Confirm Exit")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on application exit if conversion is in progress.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName("Confirm Stop Conversion")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on conversion stop.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnStop { get; set; }

        [DisplayName("Always Resume")]
        [Category(CategoryConversion)]
        [Description("Always resume last conversion if applicable instead of starting new one.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AlwaysResume { get; set; }

        [DisplayName("Suggest Resume")]
        [Category(CategoryConversion)]
        [Description("Suggest to resume last conversion if applicable instead of starting new one.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool SuggestResume { get; set; }

        [ReadOnly(false)]
        [DisplayName("Shorten Image Extension")]
        [Category(CategoryConversion)]
        [Description("Shortens image extension down to three characters.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShortenExtension { get; set; }

        public enum CheckInterval
        {
            Never,
            Weekly,
            Monthly
        }

        [DisplayName("Check for Updates")]
        [Category(CategoryUpdates)]
        [Description("Check for updates interval.")]
        [DefaultValue(CheckInterval.Monthly)]
        public CheckInterval CheckForUpdates { get; set; }

        [Browsable(false)]
        [DefaultValue(null)]
        public DateTime? LastCheckedForUpdates { get; set; }

        [Browsable(false)]
        public bool ShouldCheckForUpdates
        {
            get
            {
                return (CheckForUpdates != CheckInterval.Never) &&
                       (
                           !LastCheckedForUpdates.HasValue ||
                           (
                               (
                                   CheckForUpdates == CheckInterval.Weekly
                                       ? LastCheckedForUpdates.Value.AddDays(7)
                                       : LastCheckedForUpdates.Value.AddMonths(1)
                               ) <= DateTime.UtcNow
                           )
                       );
            }
        }

        public Preferences()
        {
            Reset();
        }

        public void Reset()
        {
            var lastCheckedForUpdates = LastCheckedForUpdates;
            ReflectionUtils.SetDefaultValues(this);
            LastCheckedForUpdates = lastCheckedForUpdates;
        }

        public bool Equals(Preferences preferences)
        {
            return preferences != null && GetHashCode() == preferences.GetHashCode();
        }

        private IEnumerable<bool> Fields
        {
            get
            {
                yield return AutoSaveSettings;
                yield return ClassicLook;
                yield return ShowElapsedTimeAndStatistics;
                yield return FlashWhenCompleted;
                yield return ConfirmOnDelete;
                yield return ConfirmOnExit;
                yield return ConfirmOnStop;
                yield return AlwaysResume;
                yield return SuggestResume;
                yield return ShortenExtension;
                yield return AutoCompleteFilenames;
                yield return CheckForUpdates == CheckInterval.Never;
                yield return CheckForUpdates == CheckInterval.Weekly;
                yield return CheckForUpdates == CheckInterval.Monthly;
            }
        }

        public override int GetHashCode()
        {
            var position = 0;
            return Fields.Aggregate(0, (current, field) => current | (field ? 1 : 0) << position++);
        }

        public static readonly Preferences Default = new Preferences();
    }
}
