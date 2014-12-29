using System;
using System.ComponentModel;
using System.Linq;

using Xps2Img.Shared.Attributes.UI;

namespace Xps2Img.Shared.Localization
{
    public class FilterableCustomTypeDescriptor: LocalizableTypeDescriptor
    {
        private readonly object _object;

        public FilterableCustomTypeDescriptor(object obj) : base(null, null, null)
        {
            _object = obj;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var objectPropertyDescriptorCollection = TypeDescriptor.GetProperties(_object, attributes, true);
            var propertyDescriptorCollection = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

            foreach (PropertyDescriptor pd in objectPropertyDescriptorCollection)
            {
                var include = false;
                var dynamic = false;

                foreach (var dynamicPropertyFilterAttribute in pd.Attributes.OfType<DynamicPropertyFilterAttribute>())
                {
                    dynamic = true;

                    var propertyDescriptor = objectPropertyDescriptorCollection[dynamicPropertyFilterAttribute.PropertyName];

                    include = propertyDescriptor != null && Equals(dynamicPropertyFilterAttribute.ShowOn, propertyDescriptor.GetValue(_object));

                    if(include)
                    {
                        break;
                    }
                }

                if (!dynamic || include)
                {
                    propertyDescriptorCollection.Add(new FilterableLocalizablePropertyDescriptor(pd));
                }
            }

            return propertyDescriptorCollection;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[0]);
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _object;
        }

        public override AttributeCollection GetAttributes()
        {
            return ObjectTypeDescriptor.GetAttributes();
        }

        private ICustomTypeDescriptor ObjectTypeDescriptor
        {
            get { return TypeDescriptor.GetProvider(_object).GetTypeDescriptor(_object); }
        }
    }
}