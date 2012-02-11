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

        private const string DisplayNameClassicLook = "Classic Look";
        public const string DefaultSelectedItem     = DisplayNameClassicLook;

        [DisplayName(DisplayNameClassicLook)]
        [Category(CategoryApplication)]
        [Description("Use application system look.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ClassicLook { get; set; }

        [DisplayName(@"Show Elapsed Time")]
        [Category(CategoryApplication)]
        [Description("Show time elapsed by last operation at application title.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTime { get; set; }

        [DisplayName(@"Flash When Completed")]
        [Category(CategoryApplication)]
        [Description("Flash application window when conversion is completed.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool FlashWhenCompleted { get; set; }

        [DisplayName(@"Confirm Delete")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on images deletion.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName(@"Confirm Exit")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on application exit if conversion is in progress.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName(@"Confirm Stop Conversion")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on conversion stop.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnStop { get; set; }

        [DisplayName(@"Always Resume")]
        [Category(CategoryConversion)]
        [Description("Always resume last conversion if applicable instead of starting new one.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AlwaysResume { get; set; }

        [DisplayName(@"Suggest Resume")]
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

            if(ClassicLook)
            {
                hashCode |= 1 << 0;
            }

            if(ShowElapsedTime)
            {
                hashCode |= 1 << 1;
            }

            if(FlashWhenCompleted)
            {
                hashCode |= 1 << 2;
            }

            if(ConfirmOnDelete)
            {
                hashCode |= 1 << 3;
            }

            if(ConfirmOnExit)
            {
                hashCode |= 1 << 4;
            }

            if(ConfirmOnStop)
            {
                hashCode |= 1 << 5;
            }

            if(AlwaysResume)
            {
                hashCode |= 1 << 6;
            }

            if (SuggestResume)
            {
                hashCode |= 1 << 7;
            }

            return hashCode;
        }

        public static readonly Preferences Default = new Preferences();
    }
}
