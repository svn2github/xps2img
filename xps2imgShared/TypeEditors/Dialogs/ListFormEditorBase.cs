using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public abstract class ListFormEditorBase : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var windowsFormsEditorService = provider.GetWindowsFormsEditorService();
            if (windowsFormsEditorService == null)
            {
                return value;
            }

            using (var listForm = new ListForm())
            {
                listForm.DefaultValue = DefaultValue;

                listForm.Value = (int?) value;

                listForm.MinValue = MinValue;
                listForm.MaxValue = MaxValue;
                listForm.Values = Values;

                listForm.Title = context != null ? context.PropertyDescriptor.DisplayName : String.Empty;

                listForm.TrackBarTickFrequency = TrackBarTickFrequency;
                listForm.TrackBarLargeChange = TrackBarLargeChange;

                CancelEventHandler formClosing = delegate { windowsFormsEditorService.CloseDropDown(); };

                listForm.Closing += formClosing;
                windowsFormsEditorService.DropDownControl(listForm);
                listForm.Closing -= formClosing;

                return listForm.DialogResult == DialogResult.OK ? listForm.Value : value;
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get { return true; }
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
