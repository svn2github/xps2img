using System.ComponentModel;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class JpegNullableIntTypeConverter : CheckedNullableIntTypeConverter
    {
        public JpegNullableIntTypeConverter()
            : base(Validation.JpegQualityValidationExpression)
        {
        }

        private static readonly int[] JpegQuality = { 10, 15, 25, 35, 45, 55, 65, 75, 85, 95, 100 };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(JpegQuality);
        }
    }
}
