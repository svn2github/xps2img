using System.Text.RegularExpressions;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.TypeConverters
{
    public class WordsSeparatedEnumConverter<T> : TranformationEnumConverter<T>
    {
        protected override string TransformTo(string value, T enumValue)
        {
            return Regex.Replace(value, @"(.)([A-Z])", @"$1 $2");
        }

        protected override string TransformFrom(string value)
        {
            return value.RemoveSpaces();
        }
    }
}
