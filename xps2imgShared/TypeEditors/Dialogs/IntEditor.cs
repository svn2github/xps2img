using System;
using System.ComponentModel;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public abstract class IntEditor : FormEditorBase<IntForm, int?>
    {
        protected override void InitForm(IntForm form, ITypeDescriptorContext context, int? value)
        {
            form.DefaultValue = DefaultValue;
            form.Value = value;
            form.Values = Values;
            form.Objects = Objects;

            form.MinValue = MinValue;
            form.MaxValue = MaxValue;

            form.Title = context != null && context.PropertyDescriptor != null ? context.PropertyDescriptor.DisplayName ?? String.Empty : String.Empty;

            form.TrackBarTickFrequency = TrackBarTickFrequency;
            form.TrackBarLargeChange = TrackBarLargeChange;

            form.MapDefaultValueTo = MapDefaultValueTo;
        }

        protected abstract int DefaultValue { get; }
        protected abstract int MaxValue { get; }
        protected abstract int MinValue { get; }

        protected abstract int[] Values { get; }

        protected virtual object[] Objects { get { return null; } }
        protected virtual int? MapDefaultValueTo { get { return null; } }
        
        protected virtual int TrackBarLargeChange { get { return 1; } }
        protected virtual int TrackBarTickFrequency { get { return 1; } }
    }
}
