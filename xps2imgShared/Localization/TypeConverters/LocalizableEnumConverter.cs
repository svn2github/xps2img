using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.Localization.TypeConverters
{
    public class LocalizableEnumConverter : EnumConverter
    {
        private readonly ResourceManager _resourceManager;
        private readonly ILocalizablePropertyDescriptorStrategy _localizablePropertyDescriptorStrategy;

        private readonly Dictionary<CultureInfo, Dictionary<string, object>> _cultureInfoToEnumValue = new Dictionary<CultureInfo, Dictionary<string, object>>();

        public LocalizableEnumConverter(Type type, ResourceManager resourceManager, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy)
            : base(type)
        {
            _resourceManager = resourceManager;
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
            return _resourceManager.GetString(_localizablePropertyDescriptorStrategy.GetEnumValueId(EnumType, value)) ?? value.ToString();
        }

        private object ParseEnum(string value)
        {
            return Enum.Parse(EnumType, value);
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
