using System;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class DpiListFormEditor : ListFormEditorBase
    {
        public override int DefaultValue
        {
            get { return 72; }
        }

        public override int MaxValue
        {
            get { return 2350; }
        }

        public override int MinValue
        {
            get { return 72; }
        }

        public override int TrackBarLargeChange
        {
            get { return 72; }
        }

        public override int TrackBarTickFrequency
        {
            get { return 72; }
        }

        public override int[] Values
        {
            get { return new[] {72, 96}; }
        }
    }
}
