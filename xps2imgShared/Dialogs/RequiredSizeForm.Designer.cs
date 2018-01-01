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
            this.paperTypeIntControl = new Xps2Img.Shared.Controls.IntControl();
            this.dpiIntControl = new Xps2Img.Shared.Controls.IntControl();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(201, 98);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(278, 98);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // paperTypeIntControl
            // 
            this.paperTypeIntControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.paperTypeIntControl.HideTrackBar = true;
            this.paperTypeIntControl.Location = new System.Drawing.Point(0, 0);
            this.paperTypeIntControl.Name = "paperTypeIntControl";
            this.paperTypeIntControl.Size = new System.Drawing.Size(358, 35);
            this.paperTypeIntControl.TabIndex = 0;
            // 
            // dpiIntControl
            // 
            this.dpiIntControl.AlignTitleWidthWith = this.paperTypeIntControl;
            this.dpiIntControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.dpiIntControl.Location = new System.Drawing.Point(0, 35);
            this.dpiIntControl.Name = "dpiIntControl";
            this.dpiIntControl.Size = new System.Drawing.Size(358, 55);
            this.dpiIntControl.TabIndex = 1;
            // 
            // RequiredSizeForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(358, 128);
            this.Controls.Add(this.dpiIntControl);
            this.Controls.Add(this.paperTypeIntControl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "RequiredSizeForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Controls.IntControl paperTypeIntControl;
        private Controls.IntControl dpiIntControl;
    }
}