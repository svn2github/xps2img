using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public abstract class FormEditorBase<T, TV> : UITypeEditor where T: Form, IFormValue<TV>, new() where TV: new()
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var windowsFormsEditorService = provider.GetWindowsFormsEditorService();
            if (windowsFormsEditorService == null)
            {
                return value;
            }

            using (var form = new T())
            {
                InitForm(form, context, (TV)value);

                CancelEventHandler formClosing = delegate { windowsFormsEditorService.CloseDropDown(); };

                form.Closing += formClosing;
                windowsFormsEditorService.DropDownControl(form);
                form.Closing -= formClosing;

                return form.DialogResult == DialogResult.OK ? form.Value : value;
            }
        }

        protected abstract void InitForm(T form, ITypeDescriptorContext context, TV value);

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get { return true; }
        }
    }
}
