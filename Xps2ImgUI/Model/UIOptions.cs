using System;
using System.ComponentModel;
using System.Globalization;
using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Localization;

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
        private static readonly Func<string, string, string> JoinInternalParameters = (s1, s2) => String.Join(InternalParametersSeparator.ToString(), new[] { s1, s2 });

        private static readonly string CancelObjectIdsStatic = JoinInternalParameters(GetGuidNamePart(), GetGuidNamePart());

        [UIOption(Names.Internal)]
        public override string Internal
        {
            get { return JoinInternalParameters(CancelObjectIdsStatic, LocalizationManager.CurrentUICulture.LCID.ToString(CultureInfo.InvariantCulture)); }
            set { }
        }

        public override bool Silent
        {
            get { return false; }
            set { }
        }

        public override bool Test
        {
            get { return false; }
            set { base.Test = value; }
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
