using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Xps2Img.CommandLine
{
  public class IntervalTypeConverter : TypeConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      return CanConvertTo(destinationType) ? IntervalUtils.ToString((IEnumerable<Interval>)value) : null;
    }
  
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      return CanConvertFrom(value.GetType()) ? Interval.Parse((string)value) : null;
    }
  }
}