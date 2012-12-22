using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommandLine;

using Xps2Img.Shared.TypeConverters;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public partial class Preferences : IEquatable<Preferences>
    {
        [DisplayName(DefaultSelectedItem)]
        [Category(CategoryApplication)]
        [Description(AutoCompleteFilenamesDescription)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoCompleteFilenames { get; set; }

        [DisplayName(AutoSaveSettingsDisplayName)]
        [Category(CategoryApplication)]
        [Description(AutoSaveSettingsDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoSaveSettings { get; set; }

        [DisplayName(ClassicLookDisplayName)]
        [Category(CategoryApplication)]
        [Description(ClassicLookDescription)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ClassicLook { get; set; }

        [DisplayName(ShowElapsedTimeAndStatisticsDisplayName)]
        [Category(CategoryApplication)]
        [Description(ShowElapsedTimeAndStatisticsDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTimeAndStatistics { get; set; }

        [DisplayName(FlashWhenCompletedDisplayName)]
        [Category(CategoryApplication)]
        [Description(FlashWhenCompletedDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool FlashWhenCompleted { get; set; }

        [DisplayName(ConfirmAfterConversionDisplayName)]
        [Category(CategoryConfirmations)]
        [Description(ConfirmAfterConversionDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnAfterConversion { get; set; }

        [DisplayName(ConfirmDeleteDisplayName)]
        [Category(CategoryConfirmations)]
        [Description(ConfirmDeleteDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName(ConfirmExitDisplayName)]
        [Category(CategoryConfirmations)]
        [Description(ConfirmExitDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName(ConfirmStopConversionDisplayName)]
        [Category(CategoryConfirmations)]
        [Description(ConfirmStopConversionDescription)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnStop { get; set; }

        [DisplayName(AlwaysResumeDisplayName)]
        [Category(CategoryConversion)]
        [Description(AlwaysResumeDescription)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AlwaysResume { get; set; }

        [DisplayName(SuggestResumeDisplayName)]
        [Category(CategoryConversion)]
        [Description(SuggestResumeDescription)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool SuggestResume { get; set; }

        [ReadOnly(false)]
        [DisplayName(ShortenImageExtensionDisplayName)]
        [Category(CategoryConversion)]
        [Description(ShortensImageExtensionDescription)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShortenExtension { get; set; }

        public enum CheckInterval
        {
            Never,
            Weekly,
            Monthly
        }

        [DisplayName(CheckForUpdatesDisplayName)]
        [Category(CategoryUpdates)]
        [Description(CheckForUpdatesDescription)]
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

        [Browsable(false)]
        public int WaitForShutdownSeconds { get { return 45; } }

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
                yield return ConfirmOnAfterConversion;
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
