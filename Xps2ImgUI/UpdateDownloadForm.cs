using System;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;

using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class UpdateDownloadForm : Form
    {
        private string _textFormat;
        private readonly UpdateManager _updateManager;

        public UpdateDownloadForm(UpdateManager updateManager)
        {
            InitializeComponent();
            _updateManager = updateManager;
        }

        protected override void OnLoad(EventArgs e)
        {
            _textFormat = Text;

            SetTitle(0);

            this.RemoveSystemMenuDisabledItems();

            _updateManager.DownloadFileCompleted += DownloadFileCompleted;
            _updateManager.DownloadProgressChanged += DownloadProgressChanged;

            _updateManager.DownloadAsync();

            base.OnLoad(e);
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.InvokeIfNeeded(() =>
            {
                if (_stopDownload)
                {
                    ((WebClient)sender).CancelAsync();
                    Close();
                    return;
                }
                var progressPercentage = e.ProgressPercentage;
                SetTitle(progressPercentage);
                downloadProgressBar.Value = progressPercentage;
            });
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.InvokeIfNeeded(() => DialogResult = _stopDownload ? DialogResult.Cancel : e.Error != null ? DialogResult.None : DialogResult.OK);
        }

        private void SetTitle(int percent)
        {
            Text = String.Format(_textFormat, percent);
        }

        private bool _stopDownload;

        protected override void OnClosing(CancelEventArgs e)
        {
            _stopDownload = true;
            e.Cancel = DialogResult == DialogResult.None;
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
