using System.Drawing;

namespace Xps2Img.Shared.Dialogs
{
    public partial class RequiredSizeForm : BaseForm, IFormValue<Size?>
    {
        public RequiredSizeForm()
        {
            InitializeComponent();
        }

        public override void UICultureChanged()
        {
            base.UICultureChanged();
        }

        public Size? Value { get; set; }
    }
}
