namespace Xps2Img.Shared.TypeConverters
{
    public class NullableIntTypeConverter : ErrorReporterTypeConverter<int?>
    {
        public NullableIntTypeConverter()
            : base(Resources.Strings.ValueIsNotValidIntegerNumber)
        {
        }
    }
}
