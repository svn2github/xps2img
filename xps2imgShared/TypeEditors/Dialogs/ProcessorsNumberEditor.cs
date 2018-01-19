using System;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class ProcessorsNumberEditor : IntEditor
    {
        protected override int DefaultValue { get { return ProcessorsNumberTypeConverter.AutoValue; } }

        protected override int MinValue { get { return DefaultValue; } }
        protected override int MaxValue { get { return Values.Last(); } }

        protected override int TrackBarTickFrequency { get { return 1; } }
        protected override int TrackBarLargeChange { get { return TrackBarTickFrequency; } }
        protected override object[] Objects { get { return Values.Select((_, i) => i == 0 ? Resources.Strings.Auto : Convert.ToString(i, CultureInfo.InvariantCulture)).Cast<object>().ToArray(); } }

        protected override int[] Values { get { return ProcessorsNumberTypeConverter.Processors; } }
    }
}
