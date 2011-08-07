using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2ImgUI.Model;
using Xps2ImgUI.Properties;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            _xps2ImgModel.OutputDataReceived += Xps2ImgOutputDataReceived;
            _xps2ImgModel.ErrorDataReceived += Xps2ImgErrorDataReceived;
            _xps2ImgModel.Completed += Xps2ImgCompleted;
            _xps2ImgModel.LaunchFailed += Xps2ImgLaunchFailed;
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = Resources.WindowTitle;
            convertButton.Text = Resources.Launch;

            MinimumSize = new Size(Size.Width, Size.Height);

            AdjustPropertyGrid();

            ResetSettings();

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
                var dialogResult = MessageBox.Show(this, Resources.ClosingQuery,
                                                   Resources.WindowTitle,
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
            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            // Remove Property Pages button.
            settingsPropertyGrid.RemoveLastToolStripButton();

            // Show Command Line button.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripButton(Resources.ShowCommandLine, showCommandLineToolStripButton_Click);

            UpdateShowCommandLineCommand();

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Reset Settings button.
            _resetToolStripButton = settingsPropertyGrid.AddToolStripButton(Resources.ResetSettings, resetSettingsToolStripButton_Click);
            _resetToolStripButton.Enabled = _isDirty = false;
        }

        private void ResetSettings()
        {
            _xps2ImgModel.Reset();
            settingsPropertyGrid.SelectedObject = _xps2ImgModel.OptionsObject;
        }

        private void UpdateProgress(int percent, string pages)
        {
            Text = String.Format(Resources.WindowTitleProgressFormat, Resources.WindowTitle, percent, pages);
            progressBar.Value = percent;

            this.SetProgressValue(progressBar.Value, progressBar.Maximum);
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                Text = Resources.WindowTitle;
            }

            convertButton.Text = isRunning ? Resources.Cancel : Resources.Launch;
            settingsPropertyGrid.PropertyGridViewEnaled = !isRunning;
            _resetToolStripButton.Enabled = !isRunning && _isDirty;

            progressBar.Value = 0;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.NoProgress);
        }

        private void UpdateFailedStatus(string message)
        {
            _conversionFailed = true;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Error);

            FlashForm();

            MessageBox.Show(this, String.Format(Resources.Xps2ImgError, Environment.NewLine, message),
                            Resources.WindowTitle,
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
                                                                  ? Resources.ShowCommandLine
                                                                  : Resources.HideCommandLine;
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

        private static readonly Regex OutputRegex = new Regex(@"^\[\s*(?<percent>\d+)%\].+\(\s*(?<pages>\d+/\d+)\)");

        private void Xps2ImgOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var match = OutputRegex.Match(e.Data);
            if (match.Success)
            {
                var percent = Convert.ToInt32(match.Groups["percent"].Value);
                var pages = match.Groups["pages"].Value;
                this.InvokeIfNeeded(() => UpdateProgress(percent, pages));
            }
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
                            ? String.Format(Resources.Xps2ImgNotFount, Environment.NewLine, e.Exception.Message)
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
                    MessageBox.Show(this, String.Format(Resources.SpecifyValue, firstRequiredOptionLabel),
                                    Resources.WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)))
                {
                    return;
                }

                this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Indeterminate);

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
            _resetToolStripButton.Enabled = _isDirty = true;
            UpdateCommandLine();
        }

        // ReSharper disable InconsistentNaming
        private void settingsPropertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            UpdateCommandLine();
        }

        // ReSharper disable InconsistentNaming
        private void resetSettingsToolStripButton_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            ((ToolStripButton)sender).Enabled = false;
            ResetSettings();
        }

        // ReSharper disable InconsistentNaming
        private void showCommandLineToolStripButton_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            settingsSplitContainer.Panel2Collapsed = !settingsSplitContainer.Panel2Collapsed;
            UpdateShowCommandLineCommand();
        }

        private bool _isDirty;

        private volatile bool _conversionFailed;

        private readonly Xps2ImgModel _xps2ImgModel = new Xps2ImgModel();

        private ToolStripButton _resetToolStripButton;
        private ToolStripButton _showCommandLineToolStripButton;
    }
}
