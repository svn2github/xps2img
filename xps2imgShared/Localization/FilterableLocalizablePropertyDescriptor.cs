using System.ComponentModel;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.Localization
{
    public class FilterableLocalizablePropertyDescriptor : LocalizablePropertyDescriptor
    {
        public FilterableLocalizablePropertyDescriptor(PropertyDescriptor propertyDescriptor)
            : base(typeof(Resources.Strings), propertyDescriptor, DefaultLocalizablePropertyDescriptorStrategy.Instance)
        {
        }

        public override string Description
        {
            get { return base.Description.TabsToSpaces(8); }
        }
    }
}