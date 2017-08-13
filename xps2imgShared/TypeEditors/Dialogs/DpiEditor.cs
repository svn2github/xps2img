namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class DpiEditor : IntEditor
    {
        protected override int DefaultValue { get { return Controls.Settings.Dpi.DefaultValue; } }

        protected override int MinValue { get { return Controls.Settings.Dpi.MinValue; } }
        protected override int MaxValue { get { return Controls.Settings.Dpi.MaxValue; } }

        protected override int TrackBarTickFrequency { get { return Controls.Settings.Dpi.TrackBarTickFrequency; } }
        protected override int TrackBarLargeChange { get { return Controls.Settings.Dpi.TrackBarLargeChange; } }

        protected override int[] Values { get { return Controls.Settings.Dpi.Values; } }
    }
}
