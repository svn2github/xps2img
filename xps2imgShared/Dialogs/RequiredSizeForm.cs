using System;
using System.Drawing;
using System.Windows.Forms;

using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Localization.Forms;

namespace Xps2Img.Shared.Dialogs
{
    public partial class RequiredSizeForm : Form, IFormLocalization
    {
        public RequiredSizeForm()
        {
            TopLevel = false;
            InitializeComponent();

            this.EnableFormLocalization();
        }

        private void Close(bool ok)
        {
            DialogResult = ok ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void OKButtonClick(object sender, EventArgs e)
        {
            Close(true);
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close(false);
        }

        public Size? RequiredSize { get; set; }

        public void UICultureChanged()
        {
            okButton.Text = Resources.Strings.OK;
            cancelButton.Text = Resources.Strings.Cancel;
        }
    }
}
