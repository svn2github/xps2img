using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using CommandLine.Strings;

using Xps2Img.Shared.Localization.TypeConverters;

namespace Xps2Img.Shared.Localization
{
    public class LocalizablePropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor _propertyDescriptor;
        private readonly ILocalizablePropertyDescriptorStrategy _localizablePropertyDescriptorStrategy;
        private readonly StringsSource _stringsSource;

        private readonly Dictionary<Type, LocalizableEnumConverter> _typeToLocalizableEnumConverter = new Dictionary<Type, LocalizableEnumConverter>();

        public LocalizablePropertyDescriptor(Type stringsSourceType, PropertyDescriptor propertyDescriptor, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy)
            : base(propertyDescriptor)
        {
            _propertyDescriptor = propertyDescriptor;
            _localizablePropertyDescriptorStrategy = localizablePropertyDescriptorStrategy;
            _stringsSource = new StringsSource(stringsSourceType);

            AdjustDefaultValue();
        }

        private void AdjustDefaultValue()
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            var converter = _propertyDescriptor.Converter;
            if (converter == null)
            {
                return;
            }

            var defaultValueProperty = _propertyDescriptor.GetType().GetProperty("DefaultValue", bindingFlags);
            if (defaultValueProperty == null)
            {
                return;
            }

            var defaultValue = defaultValueProperty.GetValue(_propertyDescriptor, null);
            var defaultValueAsString = defaultValue as string;
            if (defaultValueAsString == null || defaultValue.GetType() == PropertyType)
            {
                return;
            }

            var defaultValueField = _propertyDescriptor.GetType().GetField("defaultValue", bindingFlags);
            if (defaultValueField == null)
            {
                return;
            }

            defaultValueField.SetValue(_propertyDescriptor, converter.ConvertFromInvariantString(defaultValueAsString));
        }

        private string GetLocalizedString(string id)
        {
            var str = _stringsSource.GetString(id);
            return string.IsNullOrEmpty(str) ? id : str;
        }

        public override bool CanResetValue(object component)
        {
            return _propertyDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return _propertyDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            _propertyDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _propertyDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _propertyDescriptor.ShouldSerializeValue(component);
        }

        public override Type ComponentType
        {
            get { return _propertyDescriptor.ComponentType; }
        }

        public override bool IsReadOnly
        {
            get { return _propertyDescriptor.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _propertyDescriptor.PropertyType; }
        }

        public override string DisplayName
        {
            get { return GetLocalizedString(_localizablePropertyDescriptorStrategy.GetDisplayNameId(ComponentType, Name)); }
        }

        public override string Category
        {
            get { return GetLocalizedString(_localizablePropertyDescriptorStrategy.GetCategoryId(ComponentType, base.Category)); }
        }

        public override string Description
        {
            get { return GetLocalizedString(_localizablePropertyDescriptorStrategy.GetDescriptionId(ComponentType, Name)); }
        }

        public override TypeConverter Converter
        {
            get
            {
                if (!PropertyType.IsEnum || Attributes.OfType<TypeConverterAttribute>().Any())
                {
                    return base.Converter;
                }

                LocalizableEnumConverter localizableEnumConverter;
                    
                if(!_typeToLocalizableEnumConverter.TryGetValue(PropertyType, out localizableEnumConverter))
                {
                    localizableEnumConverter = new LocalizableEnumConverter(PropertyType, _stringsSource.Type, _localizablePropertyDescriptorStrategy);
                    _typeToLocalizableEnumConverter[PropertyType] = localizableEnumConverter;
                }

                return localizableEnumConverter;
            }
        }
    }
}
