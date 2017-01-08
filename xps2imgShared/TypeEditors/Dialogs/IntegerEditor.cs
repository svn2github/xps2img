using System;
using System.ComponentModel;
using System.Globalization;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public abstract class IntegerEditor : FormEditorBase<IntegerForm, int?>
    {
        protected override void InitForm(IntegerForm form, ITypeDescriptorContext context, int? value)
        {
            form.DefaultValue = DefaultValue;
            form.Value = value;
            form.Values = Values;

            form.MinValue = MinValue;
            form.MaxValue = MaxValue;

            form.Title = context != null ? context.PropertyDescriptor.DisplayName : String.Empty;

            form.TrackBarTickFrequency = TrackBarTickFrequency;
            form.TrackBarLargeChange = TrackBarLargeChange;
        }

        protected static int IntToString(string value)
        {
            return Int32.Parse(value, CultureInfo.InvariantCulture);
        }

        public abstract int DefaultValue { get; }
        public abstract int MaxValue { get; }
        public abstract int MinValue { get; }

        public abstract int[] Values { get; }

        public abstract int TrackBarLargeChange { get; }
        public abstract int TrackBarTickFrequency { get; }
    }
}
