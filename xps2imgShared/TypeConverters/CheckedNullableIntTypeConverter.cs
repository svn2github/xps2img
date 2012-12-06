using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CheckedNullableIntTypeConverter : NullableIntTypeConverter
    {
        private readonly string _validationExpression;

        protected CheckedNullableIntTypeConverter(string validationExpression)
        {
            _validationExpression = validationExpression;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Validation.ValidateProperty(value, _validationExpression);
            return base.ConvertFrom(context, culture, value);
        }
    }
}
