using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Xps2ImgUI.Localization
{
    public static class LocalizationManager
    {
        public static event EventHandler UICultureChanged;

        public static CultureInfo ResetUICulture()
        {
            return SetUICulture(DefaultUICulture);
        }

        public static CultureInfo SetUICulture(int culture)
        {
            return SetUICulture(CultureInfo.GetCultureInfo(culture));
        }

        public static CultureInfo SetUICulture(string culture)
        {
            var cultureInfo = String.IsNullOrEmpty(culture)
                                ? CultureInfo.InvariantCulture 
                                : CultureInfo.GetCultureInfo(culture);

            return SetUICulture(cultureInfo);
        }

        public static CultureInfo SetUICulture(CultureInfo cultureInfo)
        {
            SetDefaultThreadCurrentCulture(cultureInfo, false);

            CurrentUICulture = cultureInfo;

            var uiCultureChanged = UICultureChanged;
            if (uiCultureChanged != null)
            {
                uiCultureChanged(null, EventArgs.Empty);
            }

            return cultureInfo;
        }

        public static CultureInfo DefaultUICulture
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public static CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            private set { Thread.CurrentThread.CurrentUICulture = value; }
        }

        private static void SetDefaultThreadCurrentCulture(CultureInfo culture, bool setCulture = true, bool setUICulture = true)
        {
            Action<bool, string> setCultureFor = (set, name) =>
            {
                if (set)
                {
                    typeof (CultureInfo).InvokeMember(name, BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static, null, culture, new object[] { culture });
                }
            };

            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                setCultureFor(setCulture, "s_userDefaultCulture");
                setCultureFor(setUICulture, "s_userDefaultUICulture");
            }
            catch
            {
            }

            try
            {
                setCultureFor(setCulture, "m_userDefaultCulture");
                setCultureFor(setUICulture, "m_userDefaultUICulture");
            }
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }
    }
}
