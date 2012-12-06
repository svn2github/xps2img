using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CheckedRequiredSizeTypeConverter : RequiredSizeTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Validation.ValidateProperty(value, Validation.RequiredSizeValidationExpression);
            return base.ConvertFrom(context, culture, value);
        }
    }
}
