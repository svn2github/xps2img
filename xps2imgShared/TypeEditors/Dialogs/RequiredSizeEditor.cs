using System;
using System.ComponentModel;
using System.Drawing.Design;
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
                return null;
            }

            var windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (windowsFormsEditorService == null)
            {
                return null;
            }

            using (var requiredSizeForm = new RequiredSizeForm())
            {
                windowsFormsEditorService.DropDownControl(requiredSizeForm);
            }

            return null;
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
