using System;
using System.ComponentModel;
using System.Linq;

namespace Xps2Img.Shared.TypeConverters
{
    public abstract class FilterableEnumConverter<T> : OptionsEnumConverter<T>
    {
        private readonly T[] _names;

        protected FilterableEnumConverter()
        {
            _names = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        protected abstract bool IsValueVisible(T value);

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(_names.Where(IsValueVisible).ToArray());
        }
    }
}
