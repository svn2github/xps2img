using System.ComponentModel;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class JpegNullableIntTypeConverter : CheckedNullableIntTypeConverter
    {
        public JpegNullableIntTypeConverter()
            : base(Options.ValidationExpressions.JpegQuality)
        {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Options.Defaults.JpegQualityValues);
        }
    }
}
