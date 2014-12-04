using System.ComponentModel;

using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Utils;

namespace Xps2ImgUI.Controls.SettingsPropertyGrid
{
    public class FilterableLocalizablePropertyDescriptor : LocalizablePropertyDescriptor
    {
        public FilterableLocalizablePropertyDescriptor(PropertyDescriptor propertyDescriptor)
            : base(Xps2Img.Shared.Resources.Strings.ResourceManager, propertyDescriptor, DefaultLocalizablePropertyDescriptorStrategy.Instance)
        {
        }

        public override string Description
        {
            get { return base.Description.TabsToSpaces(8); }
        }
    }
}