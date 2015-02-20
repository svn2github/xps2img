﻿using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

using Xps2Img.Shared.Utils;

namespace Xps2ImgUI.Localization
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

        private static CultureInfo SetUICulture(CultureInfo cultureInfo)
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
            Action<FieldInfo, bool> setCultureFor = (f, s) => { if (s) f.SetValue(null, culture); };

            setCultureFor(UserDefaultCultureFieldInfo, setCulture);
            setCultureFor(UserDefaultUICultureFieldInfo, setUICulture);
        }
    }
}
