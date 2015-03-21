using System.Linq;

namespace Xps2Img.Shared.TypeConverters
{
    public class OrderedByNameEnumConverter<T> : FilterableEnumConverter<T>
    {
        protected OrderedByNameEnumConverter()
        {
            Names = Names.OrderBy(x => x.ToString()).ToArray();
        }
    }
}
