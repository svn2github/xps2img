namespace Xps2ImgUI
{
    partial class CountdownForm
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
            this.components = new System.ComponentModel.Container();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.countdownTimer = new System.Windows.Forms.Timer(this.components);
            this.countdownProgressBar = new System.Windows.Forms.ProgressBar();
            this.textLabel = new System.Windows.Forms.Label();
            this.confirmCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(444, 64);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.AutoSize = true;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(367, 64);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(7, 8);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(32, 32);
            this.iconPictureBox.TabIndex = 2;
            this.iconPictureBox.TabStop = false;
            // 
            // countdownTimer
            // 
            this.countdownTimer.Interval = 1000;
            this.countdownTimer.Tick += new System.EventHandler(this.CountdownTimerTick);
            // 
            // countdownProgressBar
            // 
            this.countdownProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.countdownProgressBar.Location = new System.Drawing.Point(8, 48);
            this.countdownProgressBar.Name = "countdownProgressBar";
            this.countdownProgressBar.Size = new System.Drawing.Size(511, 10);
            this.countdownProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.countdownProgressBar.TabIndex = 4;
            // 
            // textLabel
            // 
            this.textLabel.AutoSize = true;
            this.textLabel.Location = new System.Drawing.Point(45, 18);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(0, 13);
            this.textLabel.TabIndex = 3;
            // 
            // confirmCheckBox
            // 
            this.confirmCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.confirmCheckBox.AutoSize = true;
            this.confirmCheckBox.Location = new System.Drawing.Point(8, 70);
            this.confirmCheckBox.Name = "confirmCheckBox";
            this.confirmCheckBox.Size = new System.Drawing.Size(15, 14);
            this.confirmCheckBox.TabIndex = 2;
            this.confirmCheckBox.UseVisualStyleBackColor = true;
            // 
            // CountdownForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(527, 95);
            this.Controls.Add(this.confirmCheckBox);
            this.Controls.Add(this.countdownProgressBar);
            this.Controls.Add(this.iconPictureBox);
            this.Controls.Add(this.textLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CountdownForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.Timer countdownTimer;
        private System.Windows.Forms.ProgressBar countdownProgressBar;
        private System.Windows.Forms.Label textLabel;
        private System.Windows.Forms.CheckBox confirmCheckBox;

    }
}

