using System;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class ProcessorsNumberEditor : IntEditor
    {
        protected override int DefaultValue { get { return MaxValue; } }

        protected override int MinValue { get { return Values.First(); } }
        protected override int MaxValue { get { return Values.Last()-1; } }

        protected override object[] Objects { get { return Values.Select((_, i) => i == 0 ? Resources.Strings.Auto : Convert.ToString(i, CultureInfo.InvariantCulture)).Cast<object>().ToArray(); } }
        protected override int? MapDefaultValueTo { get { return MaxValue; }}

        protected override int[] Values { get { return ProcessorsNumberTypeConverter.Processors; } }
    }
}
