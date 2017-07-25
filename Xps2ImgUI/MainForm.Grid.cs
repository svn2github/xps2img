using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Controls;
using Xps2ImgUI.Controls.PropertyGridEx;
using Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx;

using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private void AdjustPropertyGrid()
        {
            settingsPropertyGrid.AutoCompleteSettings = new[]
            {
                new PropertyGridEx.EditAutoComplete(Options.Properties.SrcFile, AutoCompleteSource.FileSystem),
                new PropertyGridEx.EditAutoComplete(Options.Properties.OutDir,  AutoCompleteSource.FileSystemDirectories)
            };

            settingsPropertyGrid.UseF4OnDoubleClickForProperties = new[]
            {
                Options.Properties.SrcFile,
                Options.Properties.OutDir,
                Options.Properties.Pages,
                Options.Properties.RequiredSize,
                Options.Properties.CpuAffinity
            };
            
            settingsPropertyGrid.DragDrop += MainFormDragDrop;
            settingsPropertyGrid.DragEnter += MainFormDragEnter;

            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            Action<Action> modalAction = action => { using (new ModalGuard()) { action(); } };

            // Remove Property Pages.
            settingsPropertyGrid.RemoveLastToolStripItem();

            // Reset Settings.
            settingsPropertyGrid.ResetAllAction = () => Model.Reset();

            _resetToolStripButton = settingsPropertyGrid.AddToolStripButton(Resources.Images.Eraser, () => Resources.Strings.Reset, (_, __) => settingsPropertyGrid.ResetAllAction());

            // Load/save Settings.
            _loadToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Images.LoadSettings, () => Resources.Strings.LoadSettings, (_, __) => modalAction(() => Model = SettingsManager.LoadSettings()),
                new ToolStripButtonItem(() => Resources.Strings.SaveSettings, (_, __) => modalAction(() => SettingsManager.SaveSettings(Model)))
            );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Explorer/Browse.
            ToolStripButtonItem xpsCopyButton, xpsBrowseButton;
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Images.BrowseImages, () => Resources.Strings.BrowseImages, BrowseConvertedImagesToolStripButtonClick,
                new ToolStripButtonItem(() => Resources.Strings.BrowseImagesFolder, (_, __) => Explorer.Select(ConvertedImagesFolder)),
                xpsBrowseButton = new ToolStripButtonItem(() => Resources.Strings.BrowseXPSFile, (_, __) => Explorer.Select(Model.SrcFile)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(() => Resources.Strings.CopyImagesFolderPathToClipboard, (_, __) => ClipboardUtils.CopyToClipboard(ConvertedImagesFolder)),
                xpsCopyButton = new ToolStripButtonItem(() => Resources.Strings.CopyXPSFilePathToClipboard, (_, __) => ClipboardUtils.CopyToClipboard(Model.SrcFile))
            ).DropDownOpening += (s, a) => xpsCopyButton.ToolStripItem.Enabled = xpsBrowseButton.ToolStripItem.Enabled = !String.IsNullOrEmpty(Model.SrcFile);

            // Help.
            _updatesToolStripButtonItem = new ToolStripButtonItem(() => Resources.Strings.CheckForUpdates, (_, __) => CheckForUpdates());

            settingsPropertyGrid.AddToolStripSplitButton(Resources.Images.Help, () => Resources.Strings.Help, (_, __) => ShowHelp(), _updatesToolStripButtonItem,
                new ToolStripButtonItem(),
                new ToolStripButtonItem(() => Resources.Strings.About,
                (_, __) => modalAction(() =>
                {
                    using (var aboutForm = new AboutForm { CheckForUpdatesEnabled = CheckForUpdatesEnabled })
                    {
                        aboutForm.ShowDialog(this);
                        if (aboutForm.CheckForUpdates)
                        {
                            CheckForUpdates();
                        }
                    }
                }
            ))).Alignment = ToolStripItemAlignment.Right;

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator(true);

            // Preferences.
            var preferencesToolStripSplitButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Images.Preferences, () => Resources.Strings.Preferences, PreferencesToolStripButtonClick);
            preferencesToolStripSplitButton.Alignment = ToolStripItemAlignment.Right;

            _shortenExtensionToolStripMenuItem = new ToolStripMenuItemEx(() => Resources.Strings.ShortenImageExtension) { CheckOnClick = true, Checked = _preferences.ShortenExtension };
            _shortenExtensionToolStripMenuItem.CheckedChanged += (_, __) =>
            {
                _preferences.ShortenExtension = _shortenExtensionToolStripMenuItem.Checked;
                Model.ShortenExtension = _preferences.ShortenExtension;
                UpdateCommandLine();
            };

            var autoSaveSettingsToolStripMenuItem = new ToolStripMenuItemEx(() => Resources.Strings.AutoSaveSettings) { CheckOnClick = true, Checked = _preferences.AutoSaveSettings };
            autoSaveSettingsToolStripMenuItem.CheckedChanged += (_, __) => _preferences.AutoSaveSettings = autoSaveSettingsToolStripMenuItem.Checked;

            preferencesToolStripSplitButton.DropDownItems.Add(_shortenExtensionToolStripMenuItem);
            preferencesToolStripSplitButton.DropDownItems.Add(new ToolStripSeparator());
            preferencesToolStripSplitButton.DropDownItems.Add(autoSaveSettingsToolStripMenuItem);

            preferencesToolStripSplitButton.DropDownOpening += (_, __) =>
            {
                autoSaveSettingsToolStripMenuItem.Checked = _preferences.AutoSaveSettings;
                _shortenExtensionToolStripMenuItem.Checked = _preferences.ShortenExtension;
            };

            preferencesToolStripSplitButton.Enabled = Model.IsUserMode;

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator(true);

            // Show Command Line.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Images.CommandLine, GetShowCommandLineToolTipText, ShowCommandLineToolStripButtonClick,
                CommandLineMenu.Select(kvp => kvp.Key == null ? new ToolStripButtonItem() : new ToolStripButtonItem(kvp.Key, (_, __) => kvp.Value())).ToArray()
            );

            commandLineTextBox.ContextMenuStripItems = CommandLineTextBoxMenu.Concat(CommandLineMenu);
            commandLineTextBox.ToolStripRendererGetter = () => settingsPropertyGrid.ToolStripRenderer;

            _showCommandLineToolStripButton.Alignment = ToolStripItemAlignment.Right;

            UpdateShowCommandLineCommand();
            UpdateCategoryReset();

            CheckForUpdatesEnabled = Model.IsUserMode;
        }

        private IEnumerable<KeyValuePair<Func<string>, Action>> CommandLineTextBoxMenu
        {
            get
            {
                yield return new KeyValuePair<Func<string>, Action>(() => Resources.Strings.ClipboardCopy, commandLineTextBox.SelectionLength > 0 ? () => ClipboardUtils.CopyToClipboard(commandLineTextBox.SelectedText) : (Action)null);
                yield return new KeyValuePair<Func<string>, Action>(null, null);
                yield return new KeyValuePair<Func<string>, Action>(() => Resources.Strings.ClipboardSelectAll, commandLineTextBox.SelectionLength != commandLineTextBox.TextLength ? () => commandLineTextBox.SelectAll() : (Action)null);
                yield return new KeyValuePair<Func<string>, Action>(null, null);
            }
        }

        private IEnumerable<KeyValuePair<Func<string>, Action>> CommandLineMenu
        {
            get
            {
                Func<bool, string> copyBatch = eh => String.Format(Resources.Strings.UIBatchCommandLineFormat + (eh ? Resources.Strings.UIBatchCommandLineErrorHandling : String.Empty), _uiCommandLine);

                yield return new KeyValuePair<Func<string>, Action>(() => Resources.Strings.CopyCommandLineToClipboard, () => ClipboardUtils.CopyToClipboard(commandLineTextBox.Text));
                yield return new KeyValuePair<Func<string>, Action>(() => Resources.Strings.CopyUICommandLineToClipboard, () => ClipboardUtils.CopyToClipboard(_uiCommandLine));
                yield return new KeyValuePair<Func<string>, Action>(null, null);
                yield return new KeyValuePair<Func<string>, Action>(() => Resources.Strings.CopyUIBatchCommandLineToClipboard, () => ClipboardUtils.CopyToClipboard(() => copyBatch(true)));
                yield return new KeyValuePair<Func<string>, Action>(() => Resources.Strings.CopyUIBatchCommandLineWithoutErrorHandlingToClipboard, () => ClipboardUtils.CopyToClipboard(() => copyBatch(false)));
            }
        }

        private ToolStripItem _resetToolStripButton;
        private ToolStripButtonItem _updatesToolStripButtonItem;
        private ToolStripSplitButton _loadToolStripButton;
        private ToolStripItem _showCommandLineToolStripButton;
        private ToolStripMenuItem _shortenExtensionToolStripMenuItem;

        private ThumbButtonManager _thumbButtonManager;
        private ThumbButton _thumbButton;
    }
}
