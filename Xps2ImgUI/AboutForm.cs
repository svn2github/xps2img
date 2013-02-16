using System;
using System.ComponentModel;
using System.Windows.Forms;

using TKageyu.Utils;

using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.System;

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
            labelAbout.Text = AssemblyInfo.Description.AppendDot();

            Text = String.Format(Text, Resources.Strings.WindowTitle, AssemblyInfo.AssemblyVersion);

            checkForUpdatesLinkLabel.Enabled = CheckForUpdatesEnabled;

            base.OnLoad(e);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            hevent.Handled = true;
            ShowHelp();
        }

        private void SiteLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Explorer.ShellExecute(AssemblyInfo.Company);
        }

        private void AboutFormHelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowHelp();
        }

        private void ShowHelp()
        {
            this.ShowHelpTableOfContents();
        }

        private void HistoryLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ShowHelpTopicId(HelpUtils.HelpTopicHistory);
        }

        private void CheckForUpdatesLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CheckForUpdates = true;
            Close();
        }

        public bool CheckForUpdatesEnabled { get; set; }
        public bool CheckForUpdates { get; private set; }
    }
}
