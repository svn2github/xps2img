using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CheckedNullableIntTypeConverter : NullableIntTypeConverter
    {
        protected readonly string ValidationExpression;

        protected CheckedNullableIntTypeConverter(string validationExpression)
        {
            ValidationExpression = validationExpression;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Validate(value);
            return base.ConvertFrom(context, culture, value);
        }

        protected void Validate(object value)
        {
            Validation.ValidateProperty(value, ValidationExpression);
        }
    }
}
