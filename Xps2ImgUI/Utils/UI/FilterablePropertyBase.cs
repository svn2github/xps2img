using System;
using System.ComponentModel;
using System.Linq;

using Xps2Img.Shared.Attributes.UI;

namespace Xps2ImgUI.Utils.UI
{
    public class FilterablePropertyBaseFacade: ICustomTypeDescriptor
    {
        private readonly object _object;

        public FilterablePropertyBaseFacade(object @object)
        {
            _object = @object;
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
                    finalProps.Add(pd);
                }
            }

            return finalProps;
        }

        #region ICustomTypeDescriptor Members

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(_object, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(_object, attributes, true); }
        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(_object, true); }
        public string GetComponentName() { return TypeDescriptor.GetComponentName(_object, true); }
        public object GetPropertyOwner(PropertyDescriptor pd) { return _object; }
        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(_object, true); }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetFilteredProperties(attributes); }
        public PropertyDescriptorCollection GetProperties() { return GetFilteredProperties(new Attribute[0]); }
        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(_object, editorBaseType, true); }
        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(_object, true); }
        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(_object, true); }
        public string GetClassName() { return TypeDescriptor.GetClassName(_object, true); }

        #endregion
    }

    public class FilterablePropertyBase : ICustomTypeDescriptor
    {
        private readonly FilterablePropertyBaseFacade _facade;

        protected FilterablePropertyBase()
        {
            _facade = new FilterablePropertyBaseFacade(this);
        }

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes() { return _facade.GetAttributes(); }
        public string GetClassName() { return _facade.GetClassName(); }
        public string GetComponentName() { return _facade.GetComponentName(); }
        public TypeConverter GetConverter() { return _facade.GetConverter(); }
        public EventDescriptor GetDefaultEvent() { return _facade.GetDefaultEvent(); }
        public PropertyDescriptor GetDefaultProperty() { return _facade.GetDefaultProperty(); }
        public object GetEditor(Type editorBaseType) { return _facade.GetEditor(editorBaseType); }
        public EventDescriptorCollection GetEvents() { return _facade.GetEvents(); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return _facade.GetEvents(attributes); }
        public PropertyDescriptorCollection GetProperties() { return _facade.GetProperties(); }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return _facade.GetProperties(attributes); }
        public object GetPropertyOwner(PropertyDescriptor pd) { return _facade.GetPropertyOwner(pd); }

        #endregion
    }
}