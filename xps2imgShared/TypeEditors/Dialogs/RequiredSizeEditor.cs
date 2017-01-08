using System.ComponentModel;
using System.Drawing;

using Xps2Img.Shared.Dialogs;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class RequiredSizeEditor : FormEditorBase<RequiredSizeForm, Size?>
    {
        protected override void InitForm(RequiredSizeForm form, ITypeDescriptorContext context, Size? value)
        {
            form.Value = value;
        }
    }
}
