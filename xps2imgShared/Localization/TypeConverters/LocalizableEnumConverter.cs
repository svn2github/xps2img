using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

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

        private string GetLocalizedString(object value, Type enumType = null)
        {
            return _resourceManager.GetString(_localizablePropertyDescriptorStrategy.GetEnumValueId(enumType ?? value.GetType(), value)) ?? value.ToString();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            InitCultureEnumValues(value.GetType());

            return GetLocalizedString(value);
        }

        private void InitCultureEnumValues(Type enumType)
        {
            if (_cultureInfoToEnumValue.ContainsKey(CurrentUICulture))
            {
                return;
            }

            _cultureInfoToEnumValue[CurrentUICulture] = Enum.GetNames(enumType).ToDictionary(k => GetLocalizedString(k, enumType), k => Enum.Parse(enumType, k));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return _cultureInfoToEnumValue[CurrentUICulture][value as string];
        }
    }
}
