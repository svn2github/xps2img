using System;
using System.ComponentModel;

using Xps2Img.Shared.CommandLine;

using Xps2ImgUI.Controls.SettingsPropertyGrid;

using UIOption = Xps2Img.Shared.Attributes.Options.OptionAttribute;

namespace Xps2ImgUI.Model
{
    public class UIOptions : Options, ICustomTypeDescriptor
    {
        private readonly FilterableCustomTypeDescriptor _facade;

        public UIOptions()
        {
            _facade = new FilterableCustomTypeDescriptor(this);
        }

        private static readonly Func<string> GetGuidNamePart = () => Guid.NewGuid().ToString().Substring(0, 8);
        private static readonly string CancellationObjectIdStatic = String.Format("{0}-{1}", GetGuidNamePart(), GetGuidNamePart());

        [UIOption(CancellationObjectIdsName)]
        public override string CancellationObjectIds
        {
            get { return CancellationObjectIdStatic; }
            set { }
        }

        public override bool Silent
        {
            get { return false; }
            set { }
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
