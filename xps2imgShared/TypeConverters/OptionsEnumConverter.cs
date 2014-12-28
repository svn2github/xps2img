using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Localization.TypeConverters;

namespace Xps2Img.Shared.TypeConverters
{
    public class OptionsEnumConverter<T> : LocalizableEnumConverter
    {
        public OptionsEnumConverter() :
            base(typeof(T), typeof(Resources.Strings), DefaultLocalizablePropertyDescriptorStrategy.Instance)
        {
        }
    }
}
