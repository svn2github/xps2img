using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.TypeConverters
{
    public class ToUpperEnumConverter<T> : TranformationEnumConverter<T>
    {
        protected override string TransformTo(string value, T enumValue)
        {
            return value.ToUpperInvariant();
        }

        protected override string TransformFrom(string value)
        {
            return value.ToUpperFirst();
        }
    }
}
