using System;
using System.ComponentModel;
using System.Windows.Forms;

using Xps2Img.Shared.Localization.Forms;

namespace Xps2Img.Shared.Dialogs
{
    public class BaseForm : Form, IFormLocalization
    {
        protected BaseForm()
        {
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;

            if (!DesignMode)
            {
                this.EnableFormLocalization();
            }
        }

        protected new bool DesignMode
        {
            get { return base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                ((Button)AcceptButton).Click += OKButtonClick;
                ((Button)CancelButton).Click += CancelButtonClick;

                UICultureChanged();
            }
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

        public virtual void UICultureChanged()
        {
            if (IsHandleCreated)
            {
                ((Button)AcceptButton).Text = Resources.Strings.OK;
                ((Button)CancelButton).Text = Resources.Strings.Cancel;
            }
        }
    }
}
