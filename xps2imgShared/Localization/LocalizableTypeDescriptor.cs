using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;

namespace Xps2Img.Shared.Localization
{
    public class LocalizableTypeDescriptor : CustomTypeDescriptor
    {
        private readonly List<LocalizablePropertyDescriptor> _customPropertyDescriptors = new List<LocalizablePropertyDescriptor>();
        private readonly ILocalizablePropertyDescriptorStrategy _localizablePropertyDescriptorStrategy;
        private readonly ResourceManager _resourceManager;

        public LocalizableTypeDescriptor(ICustomTypeDescriptor parentCustomTypeDescriptor, ResourceManager resourceManager,  ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy)
            : base(parentCustomTypeDescriptor)
        {
            _localizablePropertyDescriptorStrategy = localizablePropertyDescriptorStrategy;
            _resourceManager = resourceManager;
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

                        var customPropertyDescriptor = new LocalizablePropertyDescriptor(_resourceManager, propertyDescriptor, _localizablePropertyDescriptorStrategy);
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

