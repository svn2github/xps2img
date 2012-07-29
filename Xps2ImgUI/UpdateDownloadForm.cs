using System;
using System.Windows.Forms;
using System.ComponentModel;

using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class UpdateDownloadForm : Form
    {
        private string _textFormat;
        private readonly IUpdateManager _updateManager;

        public UpdateDownloadForm(IUpdateManager updateManager)
        {
            InitializeComponent();
            _updateManager = updateManager;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            _textFormat = Text;

            SetTitle(0);

            _updateManager.DownloadFileCompleted += DownloadFileCompleted;
            _updateManager.DownloadProgressChanged += DownloadProgressChanged;

            _updateManager.DownloadAsync();

            base.OnLoad(e);
        }

        private void DownloadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.InvokeIfNeeded(() =>
            {
                var progressPercentage = e.ProgressPercentage;
                SetTitle(progressPercentage);
                downloadProgressBar.Value = progressPercentage;
            });
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.InvokeIfNeeded(() =>
            {
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
    }
}
