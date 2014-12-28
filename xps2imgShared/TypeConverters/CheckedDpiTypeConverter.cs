using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CheckedDpiTypeConverter : DpiTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Validation.ValidateProperty(value, Options.ValidationExpressions.Dpi);
            return base.ConvertFrom(context, culture, value);
        }
    }
}
