using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommandLine.Strings;

namespace Xps2Img.Shared.Localization
{
    public class LocalizableTypeDescriptor : CustomTypeDescriptor
    {
        private readonly List<LocalizablePropertyDescriptor> _customPropertyDescriptors = new List<LocalizablePropertyDescriptor>();
        private readonly ILocalizablePropertyDescriptorStrategy _localizablePropertyDescriptorStrategy;
        private readonly StringsSource _stringsSource;

        public LocalizableTypeDescriptor(ICustomTypeDescriptor parentCustomTypeDescriptor, Type stringsSourceType, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy)
            : base(parentCustomTypeDescriptor)
        {
            _localizablePropertyDescriptorStrategy = localizablePropertyDescriptorStrategy;
            _stringsSource = new StringsSource(stringsSourceType);
        }

        private List<LocalizablePropertyDescriptor> CustomPropertyDescriptors
        {
            get
            {
                if (!_customPropertyDescriptors.Any())
                {
                    foreach (PropertyDescriptor propertyDescriptor in base.GetProperties())
                    {
                        if (propertyDescriptor is LocalizablePropertyDescriptor)
                        {
                            continue;
                        }

                        var customPropertyDescriptor = new LocalizablePropertyDescriptor(_stringsSource.Type, propertyDescriptor, _localizablePropertyDescriptorStrategy);
                        _customPropertyDescriptors.Add(customPropertyDescriptor);
                    }
                }
                return _customPropertyDescriptors;
            }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(CustomPropertyDescriptors.ToArray());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(CustomPropertyDescriptors.FindAll(pd => pd.Attributes.Contains(attributes)).ToArray());
        }
    }
}

