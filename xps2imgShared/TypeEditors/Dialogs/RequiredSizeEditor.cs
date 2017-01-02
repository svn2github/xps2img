using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class RequiredSizeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var windowsFormsEditorService = provider.GetWindowsFormsEditorService();
            if (windowsFormsEditorService == null)
            {
                return value;
            }

            using (var requiredSizeForm = new RequiredSizeForm { Value = (Size?)value })
            {
                CancelEventHandler requiredSizeFormClosing = delegate { windowsFormsEditorService.CloseDropDown(); };

                requiredSizeForm.Closing += requiredSizeFormClosing;
                windowsFormsEditorService.DropDownControl(requiredSizeForm);
                requiredSizeForm.Closing -= requiredSizeFormClosing;

                return requiredSizeForm.DialogResult == DialogResult.OK ? requiredSizeForm.Value : value;
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
    }
}
