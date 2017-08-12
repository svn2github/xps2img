using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class DpiEditor : IntEditor
    {
        protected override int DefaultValue { get { return Options.Defaults.DpiValue; } }

        protected override int MinValue { get { return Options.ValidationExpressions.MinDpiValue; } }
        protected override int MaxValue { get { return Options.ValidationExpressions.MaxDpiValue; } }

        protected override int TrackBarTickFrequency { get { return 72; } }
        protected override int TrackBarLargeChange { get { return 72; } }

        protected override int[] Values { get { return Options.Defaults.DpiValues; } }
    }
}
