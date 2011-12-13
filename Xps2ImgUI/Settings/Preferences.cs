using System;
using System.ComponentModel;

using Xps2ImgUI.Converters;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public class Preferences
    {
        private const string CategoryConfirmations  = "Confirmations";
        private const string CategoryApplication    = "Application";

        public const string DisplayNameModernLook   = "Modern Look";

        [DisplayName(DisplayNameModernLook)]
        [Category(CategoryApplication)]
        [Description("Applies modern look to application.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ModernLook { get; set; }

        [DisplayName(@"Show Elapsed Time")]
        [Category(CategoryApplication)]
        [Description("Shows time elapsed by last operation at application title.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTime { get; set; }

        [DisplayName(@"Confirm Delete")]
        [Category(CategoryConfirmations)]
        [Description("Asks confirmation on images deletion.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName(@"Confirm Exit")]
        [Category(CategoryConfirmations)]
        [Description("Asks confirmation on application exit if convertion is in progress.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName(@"Confirm Save Settings")]
        [Category(CategoryConfirmations)]
        [Description("Asks confirmation on settings save.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnSaveSettings { get; set; }

        [DisplayName(@"Confirm Stop Convertion")]
        [Category(CategoryConfirmations)]
        [Description("Asks confirmation on convertion stop.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnStop { get; set; }

        public Preferences()
        {
            Reset();
        }

        public void Reset()
        {
            ReflectionUtils.SetDefaultValues(this);
        }

        public static readonly Preferences Default = new Preferences();
    }
}
