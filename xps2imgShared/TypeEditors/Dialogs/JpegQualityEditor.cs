using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class JpegQualityEditor : IntegerEditor
    {
        protected override int DefaultValue { get { return Options.Defaults.JpegQualityValue; } }

        protected override int MinValue { get { return Options.ValidationExpressions.MinJpegQualityValue; } }
        protected override int MaxValue { get { return Options.ValidationExpressions.MaxJpegQualityValue; } }

        protected override int TrackBarTickFrequency { get { return 5; } }
        protected override int TrackBarLargeChange { get { return 5; } }

        protected override int[] Values { get { return Options.Defaults.JpegQualityValues; } }
    }
}
