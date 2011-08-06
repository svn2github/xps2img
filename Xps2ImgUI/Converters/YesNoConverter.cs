using System;
using System.ComponentModel;

namespace Xps2ImgUI.Converters
{
	public class YesNoConverter : BooleanConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (value is bool && destinationType == typeof(string))
			{
				return _values[(bool)value ? 1 : 0];
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var str = value as string;
			if (String.Compare(_values[0], str, StringComparison.OrdinalIgnoreCase) == 0) return false;
			if (String.Compare(_values[1], str, StringComparison.OrdinalIgnoreCase) == 0) return true;
			return base.ConvertFrom(context, culture, value);
		}

		private readonly string[] _values = new[] { "No", "Yes" };
	}
}
