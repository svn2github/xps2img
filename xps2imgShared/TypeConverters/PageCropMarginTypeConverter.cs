namespace Xps2Img.Shared.TypeConverters
{
    public class PageCropMarginTypeConverter : SizeTypeConverter
    {
        public const string ValidationRegex = @"(?:^\s*(?<width>[1-9]\d{0,7})\s*[\s+x;]?\s*$)|(?:^\s*[\s+x;]\s*(?<height>[1-9]\d{0,7})\s*$)|(?:^\s*(?<width>[1-9]\d{0,7})\s*[\s+x;]?\s*(?<height>[1-9]\d{0,7})\s*$)";

        public PageCropMarginTypeConverter() : base(ValidationRegex)
        {
        }
    }
}
