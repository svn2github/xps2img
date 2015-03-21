using System;
using System.ComponentModel;
using System.Linq;

namespace Xps2Img.Shared.TypeConverters
{
    public class FilterableEnumConverter<T> : OptionsEnumConverter<T>
    {
        protected T[] Names;

        protected FilterableEnumConverter()
        {
            Names = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        protected virtual bool IsValueVisible(T value)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Names.Where(IsValueVisible).ToArray());
        }
    }
}
