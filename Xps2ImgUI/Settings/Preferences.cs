using System;
using System.ComponentModel;

using Xps2ImgUI.Converters;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public class Preferences : IEquatable<Preferences>
    {
        private const string CategoryApplication    = "Application";
        private const string CategoryConfirmations  = "Confirmations";
        private const string CategoryConversion     = "Conversion";

        public const string DefaultSelectedItem     = "Auto Complete Filenames";

        [DisplayName(DefaultSelectedItem)]
        [Category(CategoryApplication)]
        [Description("Auto complete filenames where appropriate.")]
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

        [DisplayName("Show Elapsed Time")]
        [Category(CategoryApplication)]
        [Description("Show time elapsed by last operation at application title.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTime { get; set; }

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

        public Preferences()
        {
            Reset();
        }

        public void Reset()
        {
            ReflectionUtils.SetDefaultValues(this);
        }

        public bool Equals(Preferences preferences)
        {
            return preferences != null && GetHashCode() == preferences.GetHashCode();
        }

        public override int GetHashCode()
        {
            var hashCode = 0;

            Action<int> addFlag = p => hashCode |= 1 << p;

            if(AutoSaveSettings)        addFlag(0);
            if(ClassicLook)             addFlag(1);
            if(ShowElapsedTime)         addFlag(2);
            if(FlashWhenCompleted)      addFlag(3);
            if(ConfirmOnDelete)         addFlag(4);
            if(ConfirmOnExit)           addFlag(5);
            if(ConfirmOnStop)           addFlag(6);
            if(AlwaysResume)            addFlag(7);
            if(SuggestResume)           addFlag(8);
            if (AutoCompleteFilenames)  addFlag(9);

            return hashCode;
        }

        public static readonly Preferences Default = new Preferences();
    }
}
