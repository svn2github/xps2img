using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
            launchButton.Text = Resources.Launch;

            MinimumSize = new Size(Size.Width, Size.Height);
            
            ModifyPropertyGridToolStrip();

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
                                                   MessageBoxButtons.YesNoCancel,
                                                   MessageBoxIcon.Exclamation,
                                                   MessageBoxDefaultButton.Button3);

                if (dialogResult == DialogResult.Yes)
                {
                    _xps2ImgModel.Stop();
                }

                e.Cancel = dialogResult == DialogResult.Cancel;
            }

            base.OnClosing(e);
        }

        private void ModifyPropertyGridToolStrip()
        {
            var toolStrip = settingsPropertyGrid.Controls.OfType<ToolStrip>().FirstOrDefault();
            if (toolStrip == null)
            {
                return;
            }

            // Remove property pages button.
            toolStrip.Items.RemoveAt(toolStrip.Items.Count - 1);

            // Show command line.
            var button = new ToolStripButton(Resources.ShowCommandLine);
            button.Click += showCommandLineToolStripButton_Click;
            toolStrip.Items.Add(button);

            // Separator.
            toolStrip.Items.Add(new ToolStripSeparator());

            // Reset settings.
            _resetToolStripButton = new ToolStripButton(Resources.ResetSettings) { Enabled = false };
            _resetToolStripButton.Click += resetSettingsToolStripButton_Click;
            toolStrip.Items.Add(_resetToolStripButton);
        }

        private void ResetSettings()
        {
            _xps2ImgModel.Reset();
            settingsPropertyGrid.SelectedObject = _xps2ImgModel.OptionsObject;
        }

        private void UpdateProgress(int percent)
        {
            Text = String.Format(Resources.WindowTitleProgressFormat, percent, Resources.WindowTitle);
            progressBar.Value = percent;

            this.SetProgressValue(progressBar.Value, progressBar.Maximum);
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                Text = Resources.WindowTitle;
            }

            launchButton.Text = isRunning ? Resources.Cancel : Resources.Launch;
            settingsPropertyGrid.Enabled = !isRunning;

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

        private void Xps2ImgOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var match = OutputRegex.Match(e.Data);
            if (match.Success)
            {
                this.InvokeIfNeeded(() => UpdateProgress(Convert.ToInt32(match.Groups[1].Value)));
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

            FlashForm();

            this.InvokeIfNeeded(() => UpdateRunningStatus(false));
        }

        private void Xps2ImgLaunchFailed(object sender, ThreadExceptionEventArgs e)
        {
            this.InvokeIfNeeded(() => UpdateFailedStatus(e.Exception.Message));
        }

        private void launchButton_Click(object sender, EventArgs e)
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

        private void settingsPropertyGrid_PropertySortChanged(object sender, EventArgs e)
        {
            var propertyGrid = (PropertyGrid) sender;
            if (propertyGrid.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid.PropertySort = PropertySort.Categorized;
            }
        }

        private void settingsPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _resetToolStripButton.Enabled = true;
            UpdateCommandLine();
        }

        private void settingsPropertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateCommandLine();
        }

        private void resetSettingsToolStripButton_Click(object sender, EventArgs e)
        {
            ((ToolStripButton)sender).Enabled = false;
            ResetSettings();
        }

        private void showCommandLineToolStripButton_Click(object sender, EventArgs e)
        {
            var collapsed = settingsSplitContainer.Panel2Collapsed;
            var button = (ToolStripButton) sender;
            settingsSplitContainer.Panel2Collapsed = !collapsed;
            button.Text = button.ToolTipText = collapsed ? Resources.HideCommandLine : Resources.ShowCommandLine;
        }

        private static readonly Regex OutputRegex = new Regex(@"^\[(\d+)%\]");

        private volatile bool _conversionFailed;

        private readonly Xps2ImgModel _xps2ImgModel = new Xps2ImgModel();

        private ToolStripButton _resetToolStripButton;
    }
}
