using System;
using System.ComponentModel;
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
            labelAbout.Text = AssemblyInfo.Description.AppendDot();

            Text = String.Format(Text, Resources.Strings.WindowTitle, AssemblyInfo.AssemblyVersion);

            checkForUpdatesLinkLabel.Enabled = CheckForUpdatesEnabled;

            base.OnLoad(e);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
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
            Help.ShowHelp(this, Program.HelpFile, HelpNavigator.TableOfContents);
        }

        private void HistoryLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowHelp(this, Program.HelpFile, HelpNavigator.TopicId, Program.HelpTopicHistory);
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
