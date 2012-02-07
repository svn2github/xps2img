using Xps2ImgUI.Controls;

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
            this.components = new System.ComponentModel.Container();
            this.settingsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.settingsPropertyGrid = new Xps2ImgUI.Controls.PropertyGridEx.PropertyGridEx();
            this.commandLineTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.convertContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertButton = new wyDay.Controls.SplitButton();
            this.settingsSplitContainer.Panel1.SuspendLayout();
            this.settingsSplitContainer.Panel2.SuspendLayout();
            this.settingsSplitContainer.SuspendLayout();
            this.convertContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsSplitContainer
            // 
            this.settingsSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.settingsSplitContainer.Location = new System.Drawing.Point(8, 1);
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
            this.settingsSplitContainer.Size = new System.Drawing.Size(680, 493);
            this.settingsSplitContainer.SplitterDistance = 426;
            this.settingsSplitContainer.TabIndex = 7;
            // 
            // settingsPropertyGrid
            // 
            this.settingsPropertyGrid.AllowDrop = true;
            this.settingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPropertyGrid.HasErrors = false;
            this.settingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.settingsPropertyGrid.Name = "settingsPropertyGrid";
            this.settingsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.settingsPropertyGrid.Size = new System.Drawing.Size(680, 493);
            this.settingsPropertyGrid.TabIndex = 0;
            this.settingsPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.SettingsPropertyGridPropertyValueChanged);
            this.settingsPropertyGrid.PropertySortChanged += new System.EventHandler(this.SettingsPropertyGridPropertySortChanged);
            this.settingsPropertyGrid.SelectedObjectsChanged += new System.EventHandler(this.SettingsPropertyGridSelectedObjectsChanged);
            // 
            // commandLineTextBox
            // 
            this.commandLineTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.commandLineTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandLineTextBox.Location = new System.Drawing.Point(0, 0);
            this.commandLineTextBox.Multiline = true;
            this.commandLineTextBox.Name = "commandLineTextBox";
            this.commandLineTextBox.ReadOnly = true;
            this.commandLineTextBox.Size = new System.Drawing.Size(150, 46);
            this.commandLineTextBox.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(8, 499);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(603, 23);
            this.progressBar.TabIndex = 0;
            // 
            // convertContextMenuStrip
            // 
            this.convertContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertToolStripMenuItem,
            this.resumeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deleteImagesToolStripMenuItem});
            this.convertContextMenuStrip.Name = "convertContextMenuStrip";
            this.convertContextMenuStrip.Size = new System.Drawing.Size(144, 76);
            this.convertContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.СonvertContextMenuStripOpening);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.convertToolStripMenuItem.Text = "&Convert";
            this.convertToolStripMenuItem.Click += new System.EventHandler(this.ConvertButtonClick);
            // 
            // resumeToolStripMenuItem
            // 
            this.resumeToolStripMenuItem.Name = "resumeToolStripMenuItem";
            this.resumeToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.resumeToolStripMenuItem.Text = "&Resume";
            this.resumeToolStripMenuItem.Click += new System.EventHandler(this.ResumeToolStripMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(140, 6);
            // 
            // deleteImagesToolStripMenuItem
            // 
            this.deleteImagesToolStripMenuItem.Name = "deleteImagesToolStripMenuItem";
            this.deleteImagesToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.deleteImagesToolStripMenuItem.Text = "&Delete Images";
            this.deleteImagesToolStripMenuItem.Click += new System.EventHandler(this.DeleteImagesToolStripMenuItemClick);
            // 
            // convertButton
            // 
            this.convertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.convertButton.AutoSize = true;
            this.convertButton.Location = new System.Drawing.Point(614, 499);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(75, 23);
            this.convertButton.TabIndex = 1;
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.ConvertButtonClick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 530);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.settingsSplitContainer);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainFormDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainFormDragEnter);
            this.settingsSplitContainer.Panel1.ResumeLayout(false);
            this.settingsSplitContainer.Panel2.ResumeLayout(false);
            this.settingsSplitContainer.Panel2.PerformLayout();
            this.settingsSplitContainer.ResumeLayout(false);
            this.convertContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Controls.PropertyGridEx.PropertyGridEx settingsPropertyGrid;
    private System.Windows.Forms.TextBox commandLineTextBox;
    private System.Windows.Forms.SplitContainer settingsSplitContainer;
    private wyDay.Controls.SplitButton convertButton;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.ContextMenuStrip convertContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem deleteImagesToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem resumeToolStripMenuItem;
  }
}

