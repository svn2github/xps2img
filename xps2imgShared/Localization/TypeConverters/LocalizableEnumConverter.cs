using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

using CommandLine.Strings;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.Localization.TypeConverters
{
    public class LocalizableEnumConverter : EnumConverter
    {
        private readonly StringsSource _stringsSource;
        private readonly ILocalizablePropertyDescriptorStrategy _localizablePropertyDescriptorStrategy;

        private readonly Dictionary<CultureInfo, Dictionary<string, object>> _cultureInfoToEnumValue = new Dictionary<CultureInfo, Dictionary<string, object>>();

        public LocalizableEnumConverter(Type type, Type stringsSourceType, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy)
            : base(type)
        {
            _stringsSource = new StringsSource(stringsSourceType);
            _localizablePropertyDescriptorStrategy = localizablePropertyDescriptorStrategy;
        }

        private static CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        private static bool IsInvariant(CultureInfo culture)
        {
            return String.IsNullOrEmpty(culture.Name);
        }

        private void InitCultureEnumValues()
        {
            if (_cultureInfoToEnumValue.ContainsKey(CurrentUICulture))
            {
                return;
            }

            _cultureInfoToEnumValue[CurrentUICulture] = Enum.GetNames(EnumType).ToDictionary(GetLocalizedString, ParseEnum, StringComparer.OrdinalIgnoreCase);
        }

        private string GetLocalizedString(object value)
        {
            return _stringsSource.GetString(_localizablePropertyDescriptorStrategy.GetEnumValueId(EnumType, value)) ?? value.ToString();
        }

        private object ParseEnum(string value)
        {
            return Enum.Parse(EnumType, value, true);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            InitCultureEnumValues();

            return IsInvariant(culture) ? value.ToString() : GetLocalizedString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            InitCultureEnumValues();

            var str = (string)value;
            return IsInvariant(culture) ? ParseEnum(str.RemoveSpaces()) : _cultureInfoToEnumValue[CurrentUICulture][str];
        }
    }
}
