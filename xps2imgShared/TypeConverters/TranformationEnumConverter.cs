using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Xps2Img.Shared.TypeConverters
{
    public abstract class TranformationEnumConverter<T> : EnumConverter
    {
        private readonly T[] _names;

        protected TranformationEnumConverter()
            : base(typeof(T))
        {
            _names = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        protected abstract string TransformTo(string value, T enumValue);
        protected abstract string TransformFrom(string value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return TransformTo((string)base.ConvertTo(context, culture, value, destinationType) ?? String.Empty, (T)value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            return base.ConvertFrom(context, culture, String.IsNullOrEmpty(strValue) ? value : TransformFrom(strValue));
        }

        protected virtual bool IsValueVisible(T value)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(_names.Where(IsValueVisible).ToArray());
        }
    }
}
