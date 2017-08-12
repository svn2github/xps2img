namespace Xps2Img.Shared.Dialogs
{
    partial class RequiredSizeForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.intControl1 = new Xps2Img.Shared.Controls.IntControl();
            this.intControl2 = new Xps2Img.Shared.Controls.IntControl();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(201, 111);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(278, 111);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // intControl1
            // 
            this.intControl1.DefaultValue = 0;
            this.intControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.intControl1.Location = new System.Drawing.Point(0, 0);
            this.intControl1.MaxValue = 0;
            this.intControl1.MinValue = 0;
            this.intControl1.Name = "intControl1";
            this.intControl1.SelectedValue = 0;
            this.intControl1.Size = new System.Drawing.Size(358, 55);
            this.intControl1.TabIndex = 0;
            this.intControl1.Title = null;
            this.intControl1.TrackBarLargeChange = 0;
            this.intControl1.TrackBarTickFrequency = 0;
            this.intControl1.Value = null;
            this.intControl1.Values = null;
            // 
            // intControl2
            // 
            this.intControl2.DefaultValue = 0;
            this.intControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.intControl2.Location = new System.Drawing.Point(0, 55);
            this.intControl2.MaxValue = 0;
            this.intControl2.MinValue = 0;
            this.intControl2.Name = "intControl2";
            this.intControl2.SelectedValue = 0;
            this.intControl2.Size = new System.Drawing.Size(358, 55);
            this.intControl2.TabIndex = 1;
            this.intControl2.Title = null;
            this.intControl2.TrackBarLargeChange = 0;
            this.intControl2.TrackBarTickFrequency = 0;
            this.intControl2.Value = null;
            this.intControl2.Values = null;
            // 
            // RequiredSizeForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(358, 141);
            this.Controls.Add(this.intControl2);
            this.Controls.Add(this.intControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "RequiredSizeForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Controls.IntControl intControl1;
        private Controls.IntControl intControl2;
    }
}