using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class TiffCompressOptionEnumConverter : ValueDescriptionEnumConverter<TiffCompressOption>
    {
        protected override bool IsValueVisible(TiffCompressOption value)
        {
            return value != TiffCompressOption.Default;
        }
    }
}
