namespace Xps2Img.Shared.Controls
{
    partial class IntControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelPanel = new System.Windows.Forms.Panel();
            this.headerLabel = new System.Windows.Forms.Label();
            this.controlsPanel = new System.Windows.Forms.Panel();
            this.valueTrackBar = new System.Windows.Forms.TrackBar();
            this.valueComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel.SuspendLayout();
            this.labelPanel.SuspendLayout();
            this.controlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.controlsPanel, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(333, 55);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // labelPanel
            // 
            this.labelPanel.AutoSize = true;
            this.labelPanel.Controls.Add(this.headerLabel);
            this.labelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPanel.Location = new System.Drawing.Point(0, 0);
            this.labelPanel.Margin = new System.Windows.Forms.Padding(0);
            this.labelPanel.Name = "labelPanel";
            this.labelPanel.Size = new System.Drawing.Size(70, 55);
            this.labelPanel.TabIndex = 0;
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Location = new System.Drawing.Point(1, 13);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(66, 13);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "headerLabel";
            // 
            // controlsPanel
            // 
            this.controlsPanel.AutoSize = true;
            this.controlsPanel.Controls.Add(this.valueTrackBar);
            this.controlsPanel.Controls.Add(this.valueComboBox);
            this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlsPanel.Location = new System.Drawing.Point(70, 0);
            this.controlsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.controlsPanel.Name = "controlsPanel";
            this.controlsPanel.Size = new System.Drawing.Size(263, 55);
            this.controlsPanel.TabIndex = 1;
            // 
            // valueTrackBar
            // 
            this.valueTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTrackBar.Location = new System.Drawing.Point(63, 6);
            this.valueTrackBar.Name = "valueTrackBar";
            this.valueTrackBar.Size = new System.Drawing.Size(197, 45);
            this.valueTrackBar.TabIndex = 2;
            this.valueTrackBar.ValueChanged += new System.EventHandler(this.ValueTrackBarValueChanged);
            this.valueTrackBar.Enter += new System.EventHandler(this.ValueTrackBarEnter);
            // 
            // valueComboBox
            // 
            this.valueComboBox.FormattingEnabled = true;
            this.valueComboBox.Location = new System.Drawing.Point(0, 10);
            this.valueComboBox.MaxDropDownItems = 15;
            this.valueComboBox.Name = "valueComboBox";
            this.valueComboBox.Size = new System.Drawing.Size(59, 21);
            this.valueComboBox.TabIndex = 1;
            this.valueComboBox.DropDown += new System.EventHandler(this.ValueComboBoxDropDown);
            this.valueComboBox.TextUpdate += new System.EventHandler(this.ValueComboBoxTextUpdate);
            this.valueComboBox.SelectedValueChanged += new System.EventHandler(this.ValueComboBoxSelectedValueChanged);
            // 
            // IntControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "IntControl";
            this.Size = new System.Drawing.Size(333, 55);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.labelPanel.ResumeLayout(false);
            this.labelPanel.PerformLayout();
            this.controlsPanel.ResumeLayout(false);
            this.controlsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel labelPanel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Panel controlsPanel;
        private System.Windows.Forms.TrackBar valueTrackBar;
        private System.Windows.Forms.ComboBox valueComboBox;
    }
}
