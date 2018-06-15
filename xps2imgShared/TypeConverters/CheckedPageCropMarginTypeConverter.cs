using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CheckedPageCropMarginTypeConverter : PageCropMarginTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Validation.ValidateProperty(value, Options.ValidationExpressions.PageCropMargin);
            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Options.Defaults.PageCropMarginValues);
        }
    }
}
