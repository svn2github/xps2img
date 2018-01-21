using System;
using System.Windows.Forms;
using System.ComponentModel;

using Xps2Img.Shared.Localization.Forms;

using Xps2ImgUI.Utils.Interfaces;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class UpdateDownloadForm : Form, IFormLocalization
    {
        private string _textFormat;

        private readonly ProgressBarStyle _progressBarStyle;
        private readonly IUpdateManager _updateManager;

        public UpdateDownloadForm(IUpdateManager updateManager)
        {
            InitializeComponent();

            _updateManager = updateManager;
            _progressBarStyle = downloadProgressBar.Style;

            this.EnableFormLocalization();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            downloadProgressBar.Style = ProgressBarStyle.Marquee;

            _updateManager.DownloadFileCompleted += DownloadFileCompleted;
            _updateManager.DownloadProgressChanged += DownloadProgressChanged;

            _updateManager.DownloadAsync();

            base.OnLoad(e);
        }

        private void DownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.InvokeIfNeeded(() =>
            {
                downloadProgressBar.Style = _progressBarStyle;
                var progressPercentage = e.ProgressPercentage;
                SetTitle(progressPercentage);
                downloadProgressBar.Value = progressPercentage;
            });
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.InvokeIfNeeded(() =>
            {
                SetTitle(downloadProgressBar.Maximum);
                this.EnableSysClose(false);
                cancelButton.Enabled = false;
                DialogResult = e.Error != null || e.Cancelled ? DialogResult.Cancel : DialogResult.OK;
            });
        }

        private void SetTitle(int percent)
        {
            Text = String.Format(_textFormat, percent);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
            {
                _updateManager.CancelDownload();
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _updateManager.DownloadFileCompleted -= DownloadFileCompleted;
            _updateManager.DownloadProgressChanged -= DownloadProgressChanged;

            base.OnClosed(e);
        }

        public void UICultureChanged()
        {
            _textFormat = Resources.Strings.Downloading;

            cancelButton.Text = Xps2Img.Shared.Resources.Strings.Cancel;

            SetTitle(0);
        }
    }
}
