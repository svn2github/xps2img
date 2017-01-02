using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;

using CommandLine.Utils;

using Xps2Img.Shared.Enums;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.TypeEditors;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public partial class Preferences
    {
        [DefaultValue(LanguagesSupported.English)]
        [Browsable(false)]
        public LanguagesSupported ApplicationLanguage { get; set; }

        [Category(Categories.Interface)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool AutoCompleteFilenames { get; set; }

        [Category(Categories.Interface)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool AutoSaveSettings { get; set; }

        [Category(Categories.Interface)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ClassicLook { get; set; }

        [Category(Categories.Interface)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ShowElapsedTimeAndStatistics { get; set; }

        [Category(Categories.Interface)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool FlashWhenCompleted { get; set; }

        [Category(Categories.Confirmations)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ConfirmOnAfterConversion { get; set; }

        [Category(Categories.Confirmations)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ConfirmOnDelete { get; set; }

        [Category(Categories.Confirmations)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ConfirmOnExit { get; set; }

        [Category(Categories.Confirmations)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ConfirmOnStop { get; set; }

        [Category(Categories.Conversion)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool AlwaysResume { get; set; }

        [Category(Categories.Conversion)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool SuggestResume { get; set; }

        [ReadOnly(false)]
        [Category(Categories.Conversion)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ShortenExtension { get; set; }

        [Category(Categories.Updates)]
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

        public void Reset(Func<PropertyInfo, bool> propertyFilter = null)
        {
            var lastCheckedForUpdates = LastCheckedForUpdates;
            ReflectionUtils.SetDefaultValues(this, propertyFilter);
            LastCheckedForUpdates = lastCheckedForUpdates;
        }

        public bool Equals(Preferences preferences, bool mask)
        {
            return preferences != null &&
                   (((GetHashCode() ^ preferences.GetHashCode()) & (mask ? FieldsMask : -1)) == 0);
        }

        private const int FieldsMask = ~(-1 << 14);

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
                yield return AutoCompleteFilenames;
                yield return CheckForUpdates == CheckInterval.Never;
                yield return CheckForUpdates == CheckInterval.Weekly;
                yield return CheckForUpdates == CheckInterval.Monthly;
                yield return ShortenExtension; // 14th bit. Change FieldsMask if new fields are added before.
                yield return ApplicationLanguage == LanguagesSupported.English; // Add more languages here.
                yield return ApplicationLanguage == LanguagesSupported.Ukrainian;
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
