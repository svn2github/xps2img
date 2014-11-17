using System;
using System.ComponentModel;
using System.Globalization;

namespace Xps2Img.Shared.TypeConverters
{
    public class YesNoConverter : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is bool && destinationType == typeof(string))
            {
                return Values[(bool)value ? 1 : 0]();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var str = value as string;
            if (String.Compare(Values[0](), str, StringComparison.OrdinalIgnoreCase) == 0) return false;
            if (String.Compare(Values[1](), str, StringComparison.OrdinalIgnoreCase) == 0) return true;
            return base.ConvertFrom(context, culture, value);
        }

        private static readonly Func<string>[] Values = { () => Resources.Strings.No, () => Resources.Strings.Yes };
    }
}

