using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class DpiEditor : IntegerEditor
    {
        public override int DefaultValue { get { return Options.Defaults.DpiValue; } }

        public override int MinValue { get { return IntToString(Options.ValidationExpressions.MinDpi); } }
        public override int MaxValue { get { return IntToString(Options.ValidationExpressions.MaxDpi); } }

        public override int TrackBarTickFrequency { get { return 72; } }
        public override int TrackBarLargeChange { get { return 72; } }

        public override int[] Values { get { return Options.Defaults.DpiValues; } }
    }
}
