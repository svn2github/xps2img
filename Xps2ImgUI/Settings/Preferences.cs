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

        private const string DisplayNameAutoSaveSettings    = "Auto Save Settings";
        public const string DefaultSelectionItem            = DisplayNameAutoSaveSettings;

        [DisplayName(DisplayNameAutoSaveSettings)]
        [Category(CategoryApplication)]
        [Description("Save settings on application exit.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool SaveSettingsOnExit { get; set; }

        [DisplayName(@"Modern Look")]
        [Category(CategoryApplication)]
        [Description("Apply application modern look.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ModernLook { get; set; }

        [DisplayName(@"Show Elapsed Time")]
        [Category(CategoryApplication)]
        [Description("Show time elapsed by last operation at application title.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTime { get; set; }

        [DisplayName(@"Confirm Delete")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on images deletion.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName(@"Confirm Exit")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on application exit if convertion is in progress.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName(@"Confirm Save Settings")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on settings save.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnSaveSettings { get; set; }

        [DisplayName(@"Confirm Stop Convertion")]
        [Category(CategoryConfirmations)]
        [Description("Ask confirmation on convertion stop.")]
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
