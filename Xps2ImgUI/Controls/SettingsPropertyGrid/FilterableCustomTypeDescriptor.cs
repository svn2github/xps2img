using System;
using System.ComponentModel;
using System.Linq;

using Xps2Img.Shared.Attributes.UI;
using Xps2Img.Shared.Localization;

namespace Xps2ImgUI.Controls.SettingsPropertyGrid
{
    public class FilterableCustomTypeDescriptor: LocalizableTypeDescriptor
    {
        private readonly object _object;

        public FilterableCustomTypeDescriptor(object obj) : base(null, null, null)
        {
            _object = obj;
        }

        protected PropertyDescriptorCollection GetFilteredProperties(Attribute[] attributes)
        {
            var propertyDescriptorCollection = TypeDescriptor.GetProperties(_object, attributes, true);
            var finalProps = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

            foreach (PropertyDescriptor pd in propertyDescriptorCollection)
            {
                var include = false;
                var dynamic = false;

                foreach (var dpf in pd.Attributes.OfType<DynamicPropertyFilterAttribute>())
                {
                    dynamic = true;

                    if (String.IsNullOrEmpty(dpf.PropertyName))
                    {
                        break;
                    }

                    var propertyDescriptor = propertyDescriptorCollection[dpf.PropertyName];

                    if (propertyDescriptor != null && dpf.ShowOn.IndexOf((propertyDescriptor.GetValue(_object) ?? String.Empty).ToString(), StringComparison.InvariantCulture) > -1)
                    {
                        include = true;
                    }
                }

                if (!dynamic || include)
                {
                    finalProps.Add(new FilterableLocalizablePropertyDescriptor(pd));
                }
            }

            return finalProps;
        }

        #region ICustomTypeDescriptor Members

        public override object GetPropertyOwner(PropertyDescriptor pd) { return _object; }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetFilteredProperties(attributes); }
        public override PropertyDescriptorCollection GetProperties() { return GetFilteredProperties(new Attribute[0]); }

        #endregion
    }
}