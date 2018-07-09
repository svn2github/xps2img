using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.CommandLine.Validators;

namespace Xps2Img.Shared.TypeConverters
{
    public class PagesTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;
            Validation.ValidateProperty(strValue, typeof(PagesValidator));
            return Interval.Parse(strValue);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string)
                    ? IntervalUtils.ToString(value as List<Interval>)
                    : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
