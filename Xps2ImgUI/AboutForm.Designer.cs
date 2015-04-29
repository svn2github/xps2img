namespace Xps2ImgUI
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.closeButton = new System.Windows.Forms.Button();
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.siteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.aboutLabel = new System.Windows.Forms.Label();
            this.checkForUpdatesLinkLabel = new System.Windows.Forms.LinkLabel();
            this.historyLinkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(264, 105);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(12, 22);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(48, 48);
            this.iconPictureBox.TabIndex = 1;
            this.iconPictureBox.TabStop = false;
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.copyrightLabel.Location = new System.Drawing.Point(9, 110);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(76, 13);
            this.copyrightLabel.TabIndex = 3;
            this.copyrightLabel.Text = "copyrightLabel";
            // 
            // siteLinkLabel
            // 
            this.siteLinkLabel.AutoSize = true;
            this.siteLinkLabel.Location = new System.Drawing.Point(76, 41);
            this.siteLinkLabel.Name = "siteLinkLabel";
            this.siteLinkLabel.Size = new System.Drawing.Size(69, 13);
            this.siteLinkLabel.TabIndex = 1;
            this.siteLinkLabel.TabStop = true;
            this.siteLinkLabel.Tag = "";
            this.siteLinkLabel.Text = "siteLinkLabel";
            this.siteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SiteLinkLabelLinkClicked);
            // 
            // aboutLabel
            // 
            this.aboutLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutLabel.AutoSize = true;
            this.aboutLabel.Location = new System.Drawing.Point(76, 12);
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new System.Drawing.Size(60, 13);
            this.aboutLabel.TabIndex = 2;
            this.aboutLabel.Text = "aboutLabel";
            // 
            // checkForUpdatesLinkLabel
            // 
            this.checkForUpdatesLinkLabel.AutoSize = true;
            this.checkForUpdatesLinkLabel.Location = new System.Drawing.Point(76, 75);
            this.checkForUpdatesLinkLabel.Name = "checkForUpdatesLinkLabel";
            this.checkForUpdatesLinkLabel.Size = new System.Drawing.Size(138, 13);
            this.checkForUpdatesLinkLabel.TabIndex = 5;
            this.checkForUpdatesLinkLabel.TabStop = true;
            this.checkForUpdatesLinkLabel.Text = "checkForUpdatesLinkLabel";
            this.checkForUpdatesLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CheckForUpdatesLinkLabelLinkClicked);
            // 
            // historyLinkLabel
            // 
            this.historyLinkLabel.AutoSize = true;
            this.historyLinkLabel.Location = new System.Drawing.Point(76, 58);
            this.historyLinkLabel.Name = "historyLinkLabel";
            this.historyLinkLabel.Size = new System.Drawing.Size(83, 13);
            this.historyLinkLabel.TabIndex = 4;
            this.historyLinkLabel.TabStop = true;
            this.historyLinkLabel.Text = "historyLinkLabel";
            this.historyLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HistoryLinkLabelLinkClicked);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(351, 140);
            this.Controls.Add(this.checkForUpdatesLinkLabel);
            this.Controls.Add(this.historyLinkLabel);
            this.Controls.Add(this.siteLinkLabel);
            this.Controls.Add(this.aboutLabel);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.iconPictureBox);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AboutFormHelpButtonClicked);
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.LinkLabel siteLinkLabel;
        private System.Windows.Forms.Label aboutLabel;
        private System.Windows.Forms.LinkLabel checkForUpdatesLinkLabel;
        private System.Windows.Forms.LinkLabel historyLinkLabel;
    }
}