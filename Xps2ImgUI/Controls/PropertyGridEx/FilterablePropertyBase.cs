using System;
using System.ComponentModel;
using System.Linq;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public class FilterablePropertyBase : ICustomTypeDescriptor
    {
        protected PropertyDescriptorCollection GetFilteredProperties(Attribute[] attributes)
        {
            var pdc = TypeDescriptor.GetProperties(this, attributes, true);
            var finalProps = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

            foreach (PropertyDescriptor pd in pdc)
            {
                var include = false;
                var dynamic = false;

                foreach (var dpf in pd.Attributes.OfType<DynamicPropertyFilterAttribute>())
                {
                    dynamic = true;

                    var temp = pdc[dpf.PropertyName];

                    if (dpf.ShowOn.IndexOf((temp.GetValue(this) ?? String.Empty).ToString(), StringComparison.InvariantCulture) > -1)
                    {
                        include = true;
                    }
                }

                if (!dynamic || include)
                {
                    finalProps.Add(pd);
                }
            }

            return finalProps;
        }

        #region ICustomTypeDescriptor Members

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetFilteredProperties(attributes);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return GetFilteredProperties(new Attribute[0]);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        #endregion
    }
}