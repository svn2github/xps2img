using System.Globalization;
using System.Linq;

using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class ProcessPriorityEditor : IntEditor
    {
        protected override int DefaultValue { get { return (int)ProcessPriorityClassTypeConverter.Auto; } }

        protected override int MinValue { get { return DefaultValue; } }
        protected override int MaxValue { get { return Values.Length - 1; } }

        protected override object[] Objects
        {
            get
            {
                var c = new ProcessPriorityClassTypeConverter();

                return ProcessPriorityClassTypeConverter.PriorityClasses.Select(v => c.ConvertTo(null, CultureInfo.CurrentUICulture, v, typeof(string))).ToArray();
            }
        }

        protected override int[] Values { get { return ProcessPriorityClassTypeConverter.PriorityClasses.Cast<int>().ToArray(); } }
    }
}
