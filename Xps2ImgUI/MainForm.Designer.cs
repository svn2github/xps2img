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
            this.commandLineTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.convertContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.deleteImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.settingsPropertyGrid = new Xps2ImgUI.Controls.PropertyGridEx.PropertyGridEx();
            this.convertButton = new wyDay.Controls.SplitButton();
            this.settingsSplitContainer.Panel1.SuspendLayout();
            this.settingsSplitContainer.Panel2.SuspendLayout();
            this.settingsSplitContainer.SuspendLayout();
            this.convertContextMenuStrip.SuspendLayout();
            this.progressTableLayoutPanel.SuspendLayout();
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
            this.settingsSplitContainer.Size = new System.Drawing.Size(704, 505);
            this.settingsSplitContainer.SplitterDistance = 444;
            this.settingsSplitContainer.TabIndex = 7;
            // 
            // commandLineTextBox
            // 
            this.commandLineTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.commandLineTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandLineTextBox.Location = new System.Drawing.Point(0, 0);
            this.commandLineTextBox.Multiline = true;
            this.commandLineTextBox.Name = "commandLineTextBox";
            this.commandLineTextBox.ReadOnly = true;
            this.commandLineTextBox.Size = new System.Drawing.Size(704, 57);
            this.commandLineTextBox.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(3, 3);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(627, 23);
            this.progressBar.TabIndex = 1;
            // 
            // convertContextMenuStrip
            // 
            this.convertContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertToolStripMenuItem,
            this.resumeToolStripMenuItem,
            this.toolStripSeparator,
            this.deleteImagesToolStripMenuItem});
            this.convertContextMenuStrip.Name = "convertContextMenuStrip";
            this.convertContextMenuStrip.Size = new System.Drawing.Size(68, 76);
            this.convertContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.СonvertContextMenuStripOpening);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            this.convertToolStripMenuItem.Click += new System.EventHandler(this.ConvertButtonClick);
            // 
            // resumeToolStripMenuItem
            // 
            this.resumeToolStripMenuItem.Name = "resumeToolStripMenuItem";
            this.resumeToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            this.resumeToolStripMenuItem.Click += new System.EventHandler(this.ResumeToolStripMenuItemClick);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(64, 6);
            // 
            // deleteImagesToolStripMenuItem
            // 
            this.deleteImagesToolStripMenuItem.Name = "deleteImagesToolStripMenuItem";
            this.deleteImagesToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            this.deleteImagesToolStripMenuItem.Click += new System.EventHandler(this.DeleteImagesToolStripMenuItemClick);
            // 
            // progressTableLayoutPanel
            // 
            this.progressTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressTableLayoutPanel.ColumnCount = 2;
            this.progressTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.progressTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.progressTableLayoutPanel.Controls.Add(this.convertButton, 1, 0);
            this.progressTableLayoutPanel.Controls.Add(this.progressBar, 0, 0);
            this.progressTableLayoutPanel.Location = new System.Drawing.Point(5, 508);
            this.progressTableLayoutPanel.Name = "progressTableLayoutPanel";
            this.progressTableLayoutPanel.RowCount = 1;
            this.progressTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.progressTableLayoutPanel.Size = new System.Drawing.Size(708, 29);
            this.progressTableLayoutPanel.TabIndex = 0;
            // 
            // settingsPropertyGrid
            // 
            this.settingsPropertyGrid.AllowDrop = true;
            this.settingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.settingsPropertyGrid.Name = "settingsPropertyGrid";
            this.settingsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.settingsPropertyGrid.ResetGroupCallback = null;
            this.settingsPropertyGrid.Size = new System.Drawing.Size(704, 505);
            this.settingsPropertyGrid.TabIndex = 0;
            this.settingsPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.SettingsPropertyGridPropertyValueChanged);
            this.settingsPropertyGrid.PropertySortChanged += new System.EventHandler(this.SettingsPropertyGridPropertySortChanged);
            this.settingsPropertyGrid.SelectedObjectsChanged += new System.EventHandler(this.SettingsPropertyGridSelectedObjectsChanged);
            // 
            // convertButton
            // 
            this.convertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.convertButton.AutoSize = true;
            this.convertButton.Location = new System.Drawing.Point(633, 3);
            this.convertButton.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.convertButton.MinimumSize = new System.Drawing.Size(75, 23);
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
            this.ClientSize = new System.Drawing.Size(720, 541);
            this.Controls.Add(this.settingsSplitContainer);
            this.Controls.Add(this.progressTableLayoutPanel);
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
            this.progressTableLayoutPanel.ResumeLayout(false);
            this.progressTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

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
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
    private System.Windows.Forms.ToolStripMenuItem resumeToolStripMenuItem;
    private System.Windows.Forms.TableLayoutPanel progressTableLayoutPanel;
  }
}

