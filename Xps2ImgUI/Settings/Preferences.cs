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
        private const string DisplayNameAppearance      = "Appearance";

        [DisplayName(@"Confirm On Exit")]
        [Category(DisplayNameConfirmations)]
        [Description("Asks confirmation on application exit if convertion is in progress.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnExit { get; set; }

        [DisplayName(@"Confirm On Delete")]
        [Category(DisplayNameConfirmations)]
        [Description("Asks confirmation on images deletion.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ConfirmOnDelete { get; set; }

        [DisplayName(@"Modern Look")]
        [Category(DisplayNameAppearance)]
        [Description("Applies modern look to application.")]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool ModernLook { get; set; }

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
