using System;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.TypeConverters
{
    public class ValueDescriptionEnumConverter<T> : TranformationEnumConverter<T>
    {
        protected override string TransformTo(string value, T enumValue)
        {
            return EnumUtils.GetDescriptionFromValue(enumValue as Enum);
        }

        protected override string TransformFrom(string value)
        {
            return EnumUtils.GetValueFromDescription<T>(value).ToString();
        }
    }
}
