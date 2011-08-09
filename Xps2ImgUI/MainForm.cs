﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2ImgUI.Controls;
using Xps2ImgUI.Model;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm : Form
    {
        public MainForm(Xps2ImgModel xps2ImgModel)
        {
            InitializeComponent();

            _xps2ImgModel = xps2ImgModel;

            _xps2ImgModel.OutputDataReceived += Xps2ImgOutputDataReceived;
            _xps2ImgModel.ErrorDataReceived += Xps2ImgErrorDataReceived;
            _xps2ImgModel.Completed += Xps2ImgCompleted;
            _xps2ImgModel.LaunchFailed += Xps2ImgLaunchFailed;
            _xps2ImgModel.OptionsObjectChanged += Xps2ImgOptionsObjectChanged;

            _xps2ImgModel.Init();
        }

        private void Xps2ImgOptionsObjectChanged(object sender, EventArgs e)
        {
            settingsPropertyGrid.SelectedObject = _xps2ImgModel.OptionsObject;
            Refresh();
            UpdateCommandLine();
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = Resources.Strings.WindowTitle;
            convertButton.Text = Resources.Strings.Launch;

            MinimumSize = new Size(Size.Width, Size.Height);

            AdjustPropertyGrid();
            
            FocusFirstRequiredOption(null);
            
            base.OnLoad(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            this.StopFlashing();
            base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_xps2ImgModel.IsRunning)
            {
                var dialogResult = MessageBox.Show(this, Resources.Strings.ClosingQuery,
                                                   Resources.Strings.WindowTitle,
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Exclamation,
                                                   MessageBoxDefaultButton.Button2);

                if (dialogResult == DialogResult.Yes)
                {
                    _xps2ImgModel.Stop();
                }

                e.Cancel = dialogResult == DialogResult.No;
            }

            base.OnClosing(e);
        }

        private void AdjustPropertyGrid()
        {
            settingsPropertyGrid.DragDrop += MainForm_DragDrop;
            settingsPropertyGrid.DragEnter += MainForm_DragEnter;

            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            // Remove Property Pages button.
            settingsPropertyGrid.RemoveLastToolStripButton();

            // Show Command Line button.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ShowCommandLine, showCommandLineToolStripButton_Click,
                new ToolStripButtonItem(Resources.Strings.CopyCommandLineToClipboard, (s, e) => Clipboard.SetDataObject(commandLineTextBox.Text, true))
             );

            UpdateShowCommandLineCommand();

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Reset Settings button.
            _resetToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ResetOptions, (s, e) => _xps2ImgModel.ResetOptions(),
                new ToolStripButtonItem(Resources.Strings.ResetParameters, (s, e) => _xps2ImgModel.ResetParameters()),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.Reset, (s, e) => _xps2ImgModel.Reset())
             );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Explorer browse folders.
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.BrowseConvertedImages, browseConvertedImagesToolStripButton_Click,
                new ToolStripButtonItem(Resources.Strings.BrowseXPSFile, (s, e) => Explorer.Select(_xps2ImgModel.OptionsObject.SrcFile)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyConvertedImagesPathToClipboard, (s, e) => Clipboard.SetDataObject(ConvertedImagesFolder, true))
            );
        }

        private void UpdateProgress(int percent, string pages, string file)
        {
            Text = String.Format(Resources.Strings.WindowTitleProgressFormat, Resources.Strings.WindowTitle, percent, pages, Path.GetFileName(file));
            progressBar.Value = percent;

            this.SetProgressValue(progressBar.Value, progressBar.Maximum);
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                Text = Resources.Strings.WindowTitle;
            }

            convertButton.Text = isRunning ? Resources.Strings.Cancel : Resources.Strings.Launch;

            settingsPropertyGrid.ReadOnly = isRunning;
            _resetToolStripButton.Enabled = !isRunning;

            progressBar.Value = 0;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.NoProgress);
        }

        private void UpdateFailedStatus(string message)
        {
            _conversionFailed = true;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Error);

            FlashForm();

            MessageBox.Show(this, String.Format(Resources.Strings.Xps2ImgError, Environment.NewLine, message),
                            Resources.Strings.WindowTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            UpdateRunningStatus(false);
        }

        private void UpdateCommandLine()
        {
            var commandLine = _xps2ImgModel.FormatCommandLine();
            var separator = String.IsNullOrEmpty(commandLine) ? String.Empty : "\x20";
            commandLineTextBox.Text = String.Format("\"{0}\"{1}{2}", Xps2ImgModel.Xps2ImgExecutable, separator, commandLine);
        }

        private void UpdateShowCommandLineCommand()
        {
            _showCommandLineToolStripButton.Text =
                _showCommandLineToolStripButton.ToolTipText = settingsSplitContainer.Panel2Collapsed
                                                                  ? Resources.Strings.ShowCommandLine
                                                                  : Resources.Strings.HideCommandLine;
        }

        private void FlashForm()
        {
            if (ActiveForm == null)
            {
                this.Flash(4);
            }
        }

        private bool FocusFirstRequiredOption(Action<string> action)
        {
            var firstRequiredOptionLabel = _xps2ImgModel.FirstRequiredOptionLabel;
            if (!String.IsNullOrEmpty(firstRequiredOptionLabel))
            {
                if (action != null)
                {
                    action(firstRequiredOptionLabel);
                }
                settingsPropertyGrid.SelectGridItem(firstRequiredOptionLabel, gi => (gi.Label ?? String.Empty).StartsWith(firstRequiredOptionLabel), true);
                return true;
            }
            return false;
        }

        // ReSharper disable InconsistentNaming
        private static string GetDragFile(IDataObject dataObject)
        {
            if(!dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                return String.Empty;
            }

            var files =  dataObject.GetData(DataFormats.FileDrop) as string[];

            var file = files != null && files.Length == 1 ? files[0] : null;

            return file != null && ((File.GetAttributes(file) & FileAttributes.Directory) == 0) ? file : null;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            e.Effect = _xps2ImgModel.IsRunning || String.IsNullOrEmpty(GetDragFile(e.Data))
                        ? DragDropEffects.None
                        : DragDropEffects.Copy;
        }

        // ReSharper disable InconsistentNaming
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            var file = GetDragFile(e.Data);

            if (!String.IsNullOrEmpty(file))
            {
                _xps2ImgModel.OptionsObject.SrcFile = file;
                settingsPropertyGrid.Refresh();
            }
        }

        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\].+\(\s*(?<pages>\d+/\d+)\).+?'(?<file>.+)'");

        private void Xps2ImgOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var match = OutputRegex.Match(e.Data);
            if (!match.Success)
            {
                return;
            }

            var percent = Convert.ToInt32(match.Groups["percent"].Value);
            var pages = match.Groups["pages"].Value;
            var file = match.Groups["file"].Value;

            ConvertedImagesFolder = Path.GetDirectoryName(file);

            this.InvokeIfNeeded(() => UpdateProgress(percent, pages, file));
        }

        private void Xps2ImgErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            this.InvokeIfNeeded(() => UpdateFailedStatus(e.Data));
        }

        private void Xps2ImgCompleted(object sender, EventArgs e)
        {
            if (_conversionFailed)
            {
                return;
            }

            this.InvokeIfNeeded(() => { FlashForm(); UpdateRunningStatus(false); });
        }

        private void Xps2ImgLaunchFailed(object sender, ThreadExceptionEventArgs e)
        {
            var message = e.Exception is Win32Exception
                            ? String.Format(Resources.Strings.Xps2ImgNotFount, Environment.NewLine, e.Exception.Message)
                            : e.Exception.Message;

            this.InvokeIfNeeded(() => UpdateFailedStatus(message));
        }

        // ReSharper disable InconsistentNaming
        private void convertButton_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            var isRunning = _xps2ImgModel.IsRunning;
            if (isRunning)
            {
                _xps2ImgModel.Stop();
            }
            else
            {
                _conversionFailed = false;

                if (FocusFirstRequiredOption(
                    firstRequiredOptionLabel =>
                    MessageBox.Show(this, String.Format(Resources.Strings.SpecifyValue, firstRequiredOptionLabel),
                                    Resources.Strings.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)))
                {
                    return;
                }

                Text = String.Format(Resources.Strings.WindowTitleLaunchingFormat, Resources.Strings.WindowTitle);

                this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Indeterminate);

                ConvertedImagesFolder = null;

                _xps2ImgModel.Launch();
            }

            UpdateRunningStatus(!isRunning);
        }

        // ReSharper disable InconsistentNaming
        private void settingsPropertyGrid_PropertySortChanged(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            var propertyGrid = (PropertyGrid) sender;
            if (propertyGrid.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid.PropertySort = PropertySort.Categorized;
            }
        }

        // ReSharper disable InconsistentNaming
        private void settingsPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            UpdateCommandLine();
        }

        // ReSharper disable InconsistentNaming
        private void settingsPropertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            UpdateCommandLine();
        }

        // ReSharper disable InconsistentNaming
        private void showCommandLineToolStripButton_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            settingsSplitContainer.Panel2Collapsed = !settingsSplitContainer.Panel2Collapsed;
            UpdateShowCommandLineCommand();
        }

        // ReSharper disable InconsistentNaming
        private void browseConvertedImagesToolStripButton_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            Explorer.Browse(ConvertedImagesFolder);
        }

        private volatile bool _conversionFailed;

        private volatile string _convertedImagesFolder;
        private string ConvertedImagesFolder
        {
            get { return _convertedImagesFolder ?? Xps2ImgModel.ApplicationFolder; }
            set { _convertedImagesFolder = value; }
        }

        private readonly Xps2ImgModel _xps2ImgModel;

        private ToolStripItem _resetToolStripButton;
        private ToolStripItem _showCommandLineToolStripButton;
    }
}
