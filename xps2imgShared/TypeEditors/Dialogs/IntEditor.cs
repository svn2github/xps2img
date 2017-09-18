﻿using System;
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

            form.MinValue = MinValue;
            form.MaxValue = MaxValue;

            form.Title = context != null && context.PropertyDescriptor != null ? context.PropertyDescriptor.DisplayName ?? String.Empty : String.Empty;

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
