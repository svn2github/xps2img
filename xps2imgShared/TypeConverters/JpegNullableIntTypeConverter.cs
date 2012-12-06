using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class JpegNullableIntTypeConverter : CheckedNullableIntTypeConverter
    {
        public JpegNullableIntTypeConverter() : base(Validation.JpegQualityValidationExpression) { }
    }
}
