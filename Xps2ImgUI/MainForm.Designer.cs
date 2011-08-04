namespace Xps2ImgUI
{
  partial class MainForm
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
            this.settingsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.settingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.commandLineTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.launchButton = new System.Windows.Forms.Button();
            this.settingsSplitContainer.Panel1.SuspendLayout();
            this.settingsSplitContainer.Panel2.SuspendLayout();
            this.settingsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsSplitContainer
            // 
            this.settingsSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsSplitContainer.Location = new System.Drawing.Point(8, 10);
            this.settingsSplitContainer.Name = "settingsSplitContainer";
            this.settingsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // settingsSplitContainer.Panel1
            // 
            this.settingsSplitContainer.Panel1.Controls.Add(this.settingsPropertyGrid);
            this.settingsSplitContainer.Panel1MinSize = 50;
            // 
            // settingsSplitContainer.Panel2
            // 
            this.settingsSplitContainer.Panel2.Controls.Add(this.commandLineTextBox);
            this.settingsSplitContainer.Panel2Collapsed = true;
            this.settingsSplitContainer.Size = new System.Drawing.Size(535, 312);
            this.settingsSplitContainer.SplitterDistance = 251;
            this.settingsSplitContainer.TabIndex = 7;
            // 
            // settingsPropertyGrid
            // 
            this.settingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.settingsPropertyGrid.Name = "settingsPropertyGrid";
            this.settingsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.settingsPropertyGrid.Size = new System.Drawing.Size(535, 312);
            this.settingsPropertyGrid.TabIndex = 0;
            this.settingsPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.settingsPropertyGrid_PropertyValueChanged);
            this.settingsPropertyGrid.PropertySortChanged += new System.EventHandler(this.settingsPropertyGrid_PropertySortChanged);
            this.settingsPropertyGrid.SelectedObjectsChanged += new System.EventHandler(this.settingsPropertyGrid_SelectedObjectsChanged);
            // 
            // commandLineTextBox
            // 
            this.commandLineTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.commandLineTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandLineTextBox.Location = new System.Drawing.Point(0, 0);
            this.commandLineTextBox.Multiline = true;
            this.commandLineTextBox.Name = "commandLineTextBox";
            this.commandLineTextBox.ReadOnly = true;
            this.commandLineTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.commandLineTextBox.Size = new System.Drawing.Size(150, 46);
            this.commandLineTextBox.TabIndex = 6;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(8, 327);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(457, 23);
            this.progressBar.TabIndex = 4;
            // 
            // launchButton
            // 
            this.launchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.launchButton.Location = new System.Drawing.Point(469, 327);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(75, 23);
            this.launchButton.TabIndex = 3;
            this.launchButton.UseVisualStyleBackColor = true;
            this.launchButton.Click += new System.EventHandler(this.launchButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 359);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.launchButton);
            this.Controls.Add(this.settingsSplitContainer);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.settingsSplitContainer.Panel1.ResumeLayout(false);
            this.settingsSplitContainer.Panel2.ResumeLayout(false);
            this.settingsSplitContainer.Panel2.PerformLayout();
            this.settingsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid settingsPropertyGrid;
    private System.Windows.Forms.TextBox commandLineTextBox;
    private System.Windows.Forms.SplitContainer settingsSplitContainer;
    private System.Windows.Forms.Button launchButton;
    private System.Windows.Forms.ProgressBar progressBar;
  }
}

