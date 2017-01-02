using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class RequiredSizeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }

            var windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (windowsFormsEditorService == null)
            {
                return value;
            }

            using (var requiredSizeForm = new RequiredSizeForm { RequiredSize = (Size?)value })
            {
                CancelEventHandler requiredSizeFormClosing = delegate { windowsFormsEditorService.CloseDropDown(); };

                requiredSizeForm.Closing += requiredSizeFormClosing;
                windowsFormsEditorService.DropDownControl(requiredSizeForm);
                requiredSizeForm.Closing -= requiredSizeFormClosing;

                if (requiredSizeForm.DialogResult != DialogResult.OK)
                {
                    return value;
                }

                return value;
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
