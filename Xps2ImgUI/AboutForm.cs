using System;
using System.Windows.Forms;

using TKageyu.Utils;

using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            using (var iconExtractor = new IconExtractor(Application.ExecutablePath))
            {
                iconPictureBox.Image = IconExtractor.SplitIcon(iconExtractor.GetIcon(0))[0].ToBitmap();
            }

            labelCopyright.Text = AssemblyInfo.Copyright;
            labelAbout.Text = AssemblyInfo.Description;

            Text = String.Format(Text, Resources.Strings.WindowTitle, AssemblyInfo.AssemblyVersion);

            base.OnLoad(e);
        }

        // ReSharper disable InconsistentNaming
        private void siteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            Explorer.ShellExecute(((Control)sender).Text);
            Close();
        }
    }
}
