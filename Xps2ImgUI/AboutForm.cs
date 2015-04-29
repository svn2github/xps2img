using System;
using System.ComponentModel;
using System.Windows.Forms;

using TKageyu.Utils;

using Xps2Img.Shared.Utils.System;

using Xps2ImgUI.Localization;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class AboutForm : Form, IFormLocalization
    {
        public AboutForm()
        {
            InitializeComponent();

            this.EnableFormLocalization();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            using (var iconExtractor = new IconExtractor(Application.ExecutablePath))
            {
                iconPictureBox.Image = IconExtractor.SplitIcon(iconExtractor.GetIcon(0))[0].ToBitmap();
            }

            checkForUpdatesLinkLabel.Enabled = CheckForUpdatesEnabled;

            base.OnLoad(e);
        }

        public void UICultureChanged()
        {
            copyrightLabel.Text = AssemblyInfo.Copyright;
            aboutLabel.Text = Resources.Strings.HelpAbout_Description;

            Text = String.Format(Resources.Strings.HelpAbout_Title, Resources.Strings.WindowTitle, AssemblyInfo.AssemblyVersion);

            closeButton.Text = Resources.Strings.Close;

            siteLinkLabel.Text = Resources.Strings.HelpAbout_SiteLinkLabel;
            historyLinkLabel.Text = Resources.Strings.HelpAbout_HistoryLinkLabel;
            checkForUpdatesLinkLabel.Text = Resources.Strings.HelpAbout_CheckForUpdatesLinkLabel;
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

        private static void ShowHelp()
        {
            HelpUtils.ShowHelpTableOfContents();
        }

        private void HistoryLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpUtils.ShowHelpTopicId(HelpUtils.HelpTopicHistory);
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
