using System;
using System.ComponentModel;

using Xps2ImgUI.Converters;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public class Preferences
    {
        private const string DisplayNameConfirmations   = "Confirmations";
        private const string DisplayNameApplication     = "Application";

        [DisplayName(@"Confirm Exit")]
        [Category(DisplayNameConfirmations)]
        [Description("Asks confirmation on application exit if convertion is in progress.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName(@"Confirm Delete")]
        [Category(DisplayNameConfirmations)]
        [Description("Asks confirmation on images deletion.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName(@"Modern Look")]
        [Category(DisplayNameApplication)]
        [Description("Applies modern look to application.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ModernLook { get; set; }

        [DisplayName(@"Save Settings On Exit")]
        [Category(DisplayNameApplication)]
        [Description("Saves application settings on exit.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool AutoSaveSettings { get; set; }

        [DisplayName(@"Show Elapsed Time")]
        [Category(DisplayNameApplication)]
        [Description("Shows time elapsed by last operation.")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ShowElapsedTime { get; set; }

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
