﻿using System;
using System.Drawing;
using System.Windows.Forms;

using Xps2Img.Shared.Localization.Forms;
using Xps2Img.Shared.Utils;

using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class CountdownForm : Form, IFormLocalization
    {
        public CountdownForm(string headerText, int countdownSeconds)
        {
            InitializeComponent();

            _headerText = headerText.ToLowerInvariant();

            countdownProgressBar.Maximum = countdownProgressBar.Value = countdownSeconds;

            this.EnableFormLocalization();
        }

        private void UpdateLabelText()
        {
            var value = countdownProgressBar.Value;
            textLabel.Text = String.Format(Resources.Strings.GoingToActionInFormat, _headerText.ToUpperFirst(), value);
        }

        private readonly string _headerText;

        private const string HotkeyMarker = "&";

        public string OKText
        {
            get { return okButton.Text; }
            set { okButton.Text = value.Contains(HotkeyMarker) ? value : HotkeyMarker + value; }
        }

        public bool ConfirmChecked
        {
            get { return confirmCheckBox.Checked; }
            set { confirmCheckBox.Checked = value; }
        }
        
        protected override void OnLoad(EventArgs args)
        {
            this.RemoveSystemMenuDisabledItems();

            confirmCheckBox.Text = Resources.Strings.AlwaysConfirmAndDoNotAskAgain;

            Text = Resources.Strings.WindowTitle;
            iconPictureBox.Image = SystemIcons.Exclamation.ToBitmap();

            ManageEvents(true);

            countdownTimer.Start();
            base.OnLoad(args);
        }

        protected override void OnClosed(EventArgs e)
        {
            iconPictureBox.Image.Dispose();
            countdownTimer.Dispose();

            ManageEvents(false);

            base.OnClosed(e);
        }

        private void ManageEvents(bool subscribe)
        {
            if (subscribe)
            {
                cancelButton.GotFocus += CancelButtonGotFocus;
            }
            else
            {
                cancelButton.GotFocus -= CancelButtonGotFocus;
            }
        }

        private void CancelButtonGotFocus(object sender, EventArgs e)
        {
            ManageEvents(false);

            countdownTimer.Stop();

            textLabel.Text = String.Format(Resources.Strings.PressTheButtonFormat, _headerText);

            countdownProgressBar.Visible = false;
        }

        private void CountdownTimerTick(object sender, EventArgs e)
        {
            if (countdownProgressBar.Value == 0)
            {
                countdownTimer.Stop();
                DialogResult = okButton.DialogResult;
                Close();
                return;
            }
            countdownProgressBar.Value--;
            UpdateLabelText();
        }

        public void UICultureChanged()
        {
            okButton.Text = Xps2Img.Shared.Resources.Strings.OK;
            cancelButton.Text = Xps2Img.Shared.Resources.Strings.Cancel;

            UpdateLabelText();
        }
    }
}
