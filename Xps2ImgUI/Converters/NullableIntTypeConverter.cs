namespace Xps2ImgUI.Converters
{
    public class NullableIntTypeConverter : ErrorReporterTypeConverter<int?>
    {
        public NullableIntTypeConverter()
            : base(Resources.Strings.ValueIsNotValidIntegerNumber)
        {
        }
    }
}
