using System;
using System.ComponentModel;
using System.Globalization;

namespace Xps2Img.Shared.TypeConverters
{
    public abstract class TranformationEnumConverter<T> : FilterableEnumConverter<T>
    {
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
    }
}
