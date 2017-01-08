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

            if (!DesignMode)
            {
                FormBorderStyle = FormBorderStyle.None;
                this.EnableFormLocalization();
            }
        }

        private new bool DesignMode
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
        
        protected virtual bool CanClose()
        {
            return true;
        }

        private void Close(bool ok)
        {
            DialogResult = ok ? DialogResult.OK : DialogResult.Cancel;
            if (!ok || CanClose())
            {
                Close();
            }
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
