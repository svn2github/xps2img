using System.Drawing;

namespace Xps2Img.Shared.Dialogs
{
    public partial class RequiredSizeForm : BaseForm
    {
        public Size? Value { get; set; }

        public RequiredSizeForm()
        {
            InitializeComponent();
        }

        public override void UICultureChanged()
        {
            base.UICultureChanged();
        }
    }
}
