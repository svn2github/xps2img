using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

using Xps2ImgLib.Utils;

namespace Xps2Img.Shared.Localization
{
    public static class LocalizationManager
    {
        public static event EventHandler UICultureChanged;

        private static readonly FieldInfo UserDefaultCultureFieldInfo;
        private static readonly FieldInfo UserDefaultUICultureFieldInfo;

        static LocalizationManager()
        {
            Func<string, FieldInfo> getUserDefaultCultureFieldInfo = n => typeof(CultureInfo).GetField(n, BindingFlags.Static | BindingFlags.NonPublic);

            UserDefaultCultureFieldInfo   = getUserDefaultCultureFieldInfo("s_userDefaultCulture")   ?? getUserDefaultCultureFieldInfo("m_userDefaultCulture");
            UserDefaultUICultureFieldInfo = getUserDefaultCultureFieldInfo("s_userDefaultUICulture") ?? getUserDefaultCultureFieldInfo("m_userDefaultUICulture");
        }

        public static CultureInfo ResetUICulture()
        {
            return SetUICulture(DefaultUICulture);
        }

        public static CultureInfo SetUICulture(int lcid)
        {
            return SetUICulture(CultureInfo.GetCultureInfo(lcid));
        }

        public static CultureInfo SetUICulture(string name)
        {
            var cultureInfo = String.IsNullOrEmpty(name)
                                ? CultureInfo.InvariantCulture 
                                : CultureInfo.GetCultureInfo(name);

            return SetUICulture(cultureInfo);
        }

        public static CultureInfo SetUICulture(CultureInfo cultureInfo)
        {
            SetDefaultThreadCurrentCulture(cultureInfo, false);

            CurrentUICulture = cultureInfo;

            UICultureChanged.SafeInvoke(null);

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
            Action<FieldInfo, bool> setCultureFor = (fi, s) => { if (s && fi != null) fi.SetValue(null, culture); };

            setCultureFor(UserDefaultCultureFieldInfo, setCulture);
            setCultureFor(UserDefaultUICultureFieldInfo, setUICulture);
        }
    }
}
