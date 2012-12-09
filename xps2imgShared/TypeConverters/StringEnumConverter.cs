using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.TypeConverters
{
    public class StringEnumConverter<T> : EnumConverter
    {
        private readonly T[] _names;

        public StringEnumConverter()
            : base(typeof (T))
        {
            _names = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var strValue = (string)base.ConvertTo(context, culture, value, destinationType);
            return Regex.Replace(strValue ?? String.Empty, @"(.)([A-Z])", @"$1 $2");
        }

        protected virtual bool IsValueVisible(T value)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            if (!String.IsNullOrEmpty(strValue))
            {
                value = strValue.RemoveSpaces();
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(_names.Where(IsValueVisible).ToArray());
        }
    }
}
