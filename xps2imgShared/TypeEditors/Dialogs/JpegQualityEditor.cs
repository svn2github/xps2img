using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class JpegQualityEditor : ListFormEditorBase
    {
        public override int DefaultValue { get { return IntToString(Options.Defaults.JpegQuality); } }

        public override int MinValue { get { return IntToString(Options.ValidationExpressions.MinJpegQuality); } }
        public override int MaxValue { get { return IntToString(Options.ValidationExpressions.MaxJpegQuality); } }

        public override int TrackBarTickFrequency { get { return 5; } }
        public override int TrackBarLargeChange { get { return 5; } }

        public override int[] Values { get { return Options.Defaults.JpegQualityValues; } }
    }
}
