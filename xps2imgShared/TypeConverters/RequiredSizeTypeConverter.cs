namespace Xps2Img.Shared.TypeConverters
{
    public class RequiredSizeTypeConverter : SizeTypeConverter
    {
        public const string ValidationRegex = @"(?:^\s*(?<width>[1-9]\d{1,7})\s*[\s+x;]?\s*$)|(?:^\s*[\s+x;]\s*(?<height>[1-9]\d{1,7})\s*$)";

        public RequiredSizeTypeConverter() : base(ValidationRegex)
        {
        }
    }
}
