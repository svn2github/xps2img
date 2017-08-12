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

        protected abstract int DefaultValue { get; }
        protected abstract int MaxValue { get; }
        protected abstract int MinValue { get; }

        protected abstract int[] Values { get; }

        protected abstract int TrackBarLargeChange { get; }
        protected abstract int TrackBarTickFrequency { get; }
    }
}
