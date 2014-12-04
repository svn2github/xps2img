using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using CommandLine;

using Xps2Img.Shared.TypeConverters;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public partial class Preferences : IEquatable<Preferences>
    {
        public const string CategoryInterface     = "Interface";
        public const string CategoryConfirmations = "Confirmations";
        public const string CategoryConversion    = "Conversion";
        public const string CategoryUpdates       = "Updates";
        public const string CategoryGeneral       = "General";

        public enum Localizations
        {
            English     = 0x0409,
            Ukrainian   = 0x0022
        }

        public enum CheckInterval
        {
            Never,
            Weekly,
            Monthly
        }

        [ReadOnly(false)]
        [Category(CategoryGeneral)]
        [DefaultValue(Localizations.English)]
        public Localizations ApplicationLanguage { get; set; }

        [Category(CategoryInterface)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoCompleteFilenames { get; set; }

        [Category(CategoryInterface)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoSaveSettings { get; set; }

        [Category(CategoryInterface)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ClassicLook { get; set; }

        [Category(CategoryInterface)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTimeAndStatistics { get; set; }

        [Category(CategoryInterface)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool FlashWhenCompleted { get; set; }

        [Category(CategoryConfirmations)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnAfterConversion { get; set; }

        [Category(CategoryConfirmations)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [Category(CategoryConfirmations)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [Category(CategoryConfirmations)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnStop { get; set; }

        [Category(CategoryConversion)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AlwaysResume { get; set; }

        [Category(CategoryConversion)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool SuggestResume { get; set; }

        [ReadOnly(false)]
        [Category(CategoryConversion)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShortenExtension { get; set; }

        [Category(CategoryUpdates)]
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
                yield return ApplicationLanguage == Localizations.English;
                yield return ApplicationLanguage == Localizations.Ukrainian;
            }
        }

        public override int GetHashCode()
        {
            Debug.Assert(Enum.GetValues(typeof(CheckInterval)).Length == 3, "Update Fields for CheckInterval enum!");
            Debug.Assert(Enum.GetValues(typeof(Localizations)).Length == 2, "Update Localizations for CheckInterval enum!");

            var position = 0;
            return Fields.Aggregate(0, (current, field) => current | (field ? 1 : 0) << position++);
        }

        public static readonly Preferences Default = new Preferences();
    }
}
