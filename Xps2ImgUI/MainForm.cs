﻿#define SHOW_ELAPSED_TIME

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2Img.CommandLine;

using Xps2ImgUI.Controls;
using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm : Form, ISettings
    {
        public MainForm()
        {
            InitializeComponent();
            SetModel(new Xps2ImgModel());
        }

        public void SetModel(Xps2ImgModel xps2ImgModel)
        {
            if (xps2ImgModel == null)
            {
                // ReSharper disable LocalizableElement
                throw new ArgumentException("XPS2ImgModel could not be null", "xps2ImgModel");
                // ReSharper restore LocalizableElement
            }

            if (_xps2ImgModel != null)
            {
                _xps2ImgModel.OutputDataReceived -= Xps2ImgOutputDataReceived;
                _xps2ImgModel.ErrorDataReceived -= Xps2ImgErrorDataReceived;
                _xps2ImgModel.Completed -= Xps2ImgCompleted;
                _xps2ImgModel.LaunchFailed -= Xps2ImgLaunchFailed;
                _xps2ImgModel.LaunchSucceeded -= Xps2ImgLaunchSucceeded;
                _xps2ImgModel.OptionsObjectChanged -= Xps2ImgOptionsObjectChanged;
            }

            _xps2ImgModel = xps2ImgModel;

            _xps2ImgModel.OutputDataReceived += Xps2ImgOutputDataReceived;
            _xps2ImgModel.ErrorDataReceived += Xps2ImgErrorDataReceived;
            _xps2ImgModel.Completed += Xps2ImgCompleted;
            _xps2ImgModel.LaunchFailed += Xps2ImgLaunchFailed;
            _xps2ImgModel.LaunchSucceeded += Xps2ImgLaunchSucceeded;
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
            if (!IsProcessOperationPending && _xps2ImgModel.IsRunning)
            {
                Activate();

                var dialogResult = ShowMessageBox(Resources.Strings.ClosingQuery,
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

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            ShowHelp();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Windows7Taskbar.WM_TaskbarButtonCreated)
            {
                _thumbButtonManager = new ThumbButtonManager(Handle);
                _thumbButton = _thumbButtonManager.CreateThumbButton(Resources.Icons.Play, ConvertButtonCleanText, (s, e) => ExecuteConvertion());
                _thumbButtonManager.AddThumbButtons(_thumbButton);
            }

            if (_thumbButtonManager != null)
            {
                _thumbButtonManager.DispatchMessage(ref m);
            }

            base.WndProc(ref m);
        }

        private void AdjustPropertyGrid()
        {
            settingsPropertyGrid.DragDrop += MainFormDragDrop;
            settingsPropertyGrid.DragEnter += MainFormDragEnter;

            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            // Remove Property Pages button.
            settingsPropertyGrid.RemoveLastToolStripButton();

            // Show Command Line button.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ShowCommandLine, ShowCommandLineToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.CopyCommandLineToClipboard, (s, e) => Clipboard.SetDataObject(commandLineTextBox.Text, true))
             );

            UpdateShowCommandLineCommand();

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Load/save settings.
            _loadToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.LoadSettings, (s, e) => SetModel(SettingsManager.LoadSettings()),
                new ToolStripButtonItem(Resources.Strings.SaveSettings, (s, e) => SettingsManager.SaveSettings(_xps2ImgModel))
            );

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

            // Explorer browse.
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.BrowseConvertedImages, BrowseConvertedImagesToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.BrowseXPSFile, (s, e) => Explorer.Select(_xps2ImgModel.OptionsObject.SrcFile)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyConvertedImagesPathToClipboard, (s, e) => Clipboard.SetDataObject(ConvertedImagesFolder, true))
            );

            //  Help.
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Help, (s, e) => ShowHelp(),
                new ToolStripButtonItem(Resources.Strings.About, (s, e) => new AboutForm().ShowDialog(this))
            ).Alignment = ToolStripItemAlignment.Right;
        }

        private void UpdateProgress(int percent, string pages, string file)
        {
            Text = String.Format(Resources.Strings.WindowTitleProgressFormat, Resources.Strings.WindowTitle, percent, pages, Path.GetFileName(file), _srcFileDisplayName);
            progressBar.Value = percent;

            this.SetProgressValue(progressBar.Value, progressBar.Maximum);
        }

        private void UpdateRunningStatus(bool isRunning)
        {
            if (!isRunning)
            {
                #if SHOW_ELAPSED_TIME
                _stopwatch.Stop();
                var ts = _stopwatch.Elapsed;
                ShowMessageBox(String.Format("{0:00}:{1:00}:{2:00}.{3}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds), MessageBoxButtons.OK, MessageBoxIcon.Information);
                #endif
                Text = Resources.Strings.WindowTitle;
            }

            convertButton.Text = isRunning ? Resources.Strings.Stop : Resources.Strings.Launch;

            if (Windows7Taskbar.Supported)
            {
                _thumbButton.Tooltip = ConvertButtonCleanText;
                _thumbButton.Icon = isRunning ? Resources.Icons.Stop : Resources.Icons.Play;
                _thumbButtonManager.RefreshThumbButtons();
            }

            settingsPropertyGrid.ReadOnly = isRunning;
            _resetToolStripButton.Enabled = _loadToolStripButton.Enabled = !isRunning;
            
            progressBar.Value = 0;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.NoProgress);
        }

        private void UpdateFailedStatus(string message)
        {
            _conversionFailed = true;

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Error);

            FlashForm();

            ShowMessageBox(String.Format(Resources.Strings.Xps2ImgError, Environment.NewLine, message), MessageBoxButtons.OK, MessageBoxIcon.Error);

            UpdateRunningStatus(false);
        }

        private void EnableConvertControls()
        {
            EnableConvertControls(true, true);
        }

        private void EnableConvertControls(bool enable, bool focus)
        {
            if (_thumbButton != null)
            {
                _thumbButton.Enabled = enable;
            }

            convertButton.Enabled = enable;

            if (enable && focus)
            {
                convertButton.Focus();
            }
        }

        private void UpdateCommandLine()
        {
            _srcFileDisplayName = Path.GetFileNameWithoutExtension(_xps2ImgModel.OptionsObject.SrcFile);
            var commandLine = _xps2ImgModel.FormatCommandLine(Options.ExcludedOnView);
            var separator = String.IsNullOrEmpty(commandLine) ? String.Empty : "\x20";
            commandLineTextBox.Text = String.Format("\"{0}\"{1}{2}", Xps2ImgModel.Xps2ImgExecutable, separator, commandLine);
        }

        private void UpdateShowCommandLineCommand()
        {
            _showCommandLineToolStripButton.Text = IsCommandLineVisible
                                                        ? Resources.Strings.HideCommandLine
                                                        : Resources.Strings.ShowCommandLine;
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

        private void MainFormDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = _xps2ImgModel.IsRunning || String.IsNullOrEmpty(GetDragFile(e.Data))
                        ? DragDropEffects.None
                        : DragDropEffects.Copy;
        }

        private void MainFormDragDrop(object sender, DragEventArgs e)
        {
            var file = GetDragFile(e.Data);

            if (!String.IsNullOrEmpty(file))
            {
                _xps2ImgModel.OptionsObject.SrcFile = file;
                settingsPropertyGrid.Refresh();
                UpdateCommandLine();
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
            this.InvokeIfNeeded(() => UpdateFailedStatus(e.Data));
        }

        private void Xps2ImgCompleted(object sender, EventArgs e)
        {
            if (_conversionFailed)
            {
                return;
            }

            this.InvokeIfNeeded(() => { FlashForm(); UpdateRunningStatus(false); EnableConvertControls(); });
        }

        private void Xps2ImgLaunchFailed(object sender, ThreadExceptionEventArgs e)
        {
            var message = e.Exception is Win32Exception
                            ? String.Format(Resources.Strings.Xps2ImgNotFount, Environment.NewLine, e.Exception.Message)
                            : e.Exception.Message;

            this.InvokeIfNeeded(() => { UpdateFailedStatus(message); EnableConvertControls(); });
        }

        private void Xps2ImgLaunchSucceeded(object sender, EventArgs e)
        {
            this.InvokeIfNeeded(EnableConvertControls);
        }

        private new void Activate()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Restore();
            }
            else
            {
                base.Activate();
            }
        }

        private void ShowOptionIsRequiredMessage(string firstRequiredOptionLabel)
        {
            _isModalWindowOpened = true;

            Activate();

            ShowMessageBox(String.Format(Resources.Strings.SpecifyValue, firstRequiredOptionLabel), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            _isModalWindowOpened = false;
        }

        private void ShowMessageBox(string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            ShowMessageBox(text, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        private DialogResult ShowMessageBox(string text, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton messageBoxDefaultButton)
        {
            if (IsDisposed)
            {
                return DialogResult.Cancel;
            }

            if (OwnedForms.Length != 0)
            {
                Array.ForEach(OwnedForms, f => f.Close());
                Application.DoEvents();
            }

            return MessageBox.Show(this, text, Resources.Strings.WindowTitle, buttons, icon, messageBoxDefaultButton);
        }

        private void ShowHelp()
        {
            Help.ShowHelp(this, "xps2img.chm", HelpNavigator.TableOfContents);
        }

#if SHOW_ELAPSED_TIME
        private Stopwatch _stopwatch;
#endif

        private void ExecuteConvertion()
        {
            EnableConvertControls(false, false);

            if (_isModalWindowOpened)
            {
                Activate();
                EnableConvertControls();
                return;
            }

            if (_xps2ImgModel.IsRunning)
            {
                _xps2ImgModel.Stop();
                return;
            }

            _conversionFailed = false;

            if (FocusFirstRequiredOption(ShowOptionIsRequiredMessage))
            {
                EnableConvertControls(true, false);
                return;
            }

            Text = String.Format(Resources.Strings.WindowTitleLaunchingFormat, Resources.Strings.WindowTitle);

            this.SetProgressState(Windows7Taskbar.ThumbnailProgressState.Indeterminate);

            ConvertedImagesFolder = null;

            #if SHOW_ELAPSED_TIME
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            #endif
                
            _xps2ImgModel.Launch();

            UpdateRunningStatus(true);
        }

        private void ConvertButtonClick(object sender, EventArgs e)
        {
            ExecuteConvertion();
        }

        private void SettingsPropertyGridPropertySortChanged(object sender, EventArgs e)
        {
            var propertyGrid = (PropertyGrid) sender;
            if (propertyGrid.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid.PropertySort = PropertySort.Categorized;
            }
        }

        private void SettingsPropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateCommandLine();
        }

        private void SettingsPropertyGridSelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateCommandLine();
        }

        private void ShowCommandLineToolStripButtonClick(object sender, EventArgs e)
        {
            settingsSplitContainer.Panel2Collapsed = !settingsSplitContainer.Panel2Collapsed;
            UpdateShowCommandLineCommand();
        }

        private void BrowseConvertedImagesToolStripButtonClick(object sender, EventArgs e)
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

        private string ConvertButtonCleanText
        {
            get { return convertButton.Text.Replace("&", String.Empty); }
        }

        private bool IsCommandLineVisible
        {
            get { return !settingsSplitContainer.Panel2Collapsed; }
            set { settingsSplitContainer.Panel2Collapsed = !value; }
        }

        private bool IsProcessOperationPending
        {
            get { return !convertButton.Enabled; }
        }

        private bool _isModalWindowOpened;

        private Xps2ImgModel _xps2ImgModel;

        private string _srcFileDisplayName;

        private ToolStripItem _resetToolStripButton;
        private ToolStripItem _loadToolStripButton;
        private ToolStripItem _showCommandLineToolStripButton;

        private ThumbButtonManager _thumbButtonManager;
        private ThumbButton _thumbButton;
    }
}
