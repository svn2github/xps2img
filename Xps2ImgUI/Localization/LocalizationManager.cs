using System;
using System.Globalization;
using System.Threading;

namespace Xps2ImgUI.Localization
{
    public static class LocalizationManager
    {
        public static event EventHandler UICultureChanged;

        public static CultureInfo ResetUICulture()
        {
            return SetUICulture(null);
        }

        public static CultureInfo SetUICulture(string culture)
        {
            var cultureInfo = String.IsNullOrEmpty(culture)
                                ? CultureInfo.InvariantCulture 
                                : CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            var uiCultureChanged = UICultureChanged;
            if (uiCultureChanged != null)
            {
                uiCultureChanged(null, EventArgs.Empty);
            }

            return cultureInfo;
        }
    }
}
