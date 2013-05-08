using System;
using System.Windows.Forms;

using Windows7.DesktopIntegration;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Controls;
using Xps2ImgUI.Controls.PropertyGridEx;
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
                new PropertyGridEx.EditAutoComplete(Options.SrcFileDisplayName, AutoCompleteSource.FileSystem),
                new PropertyGridEx.EditAutoComplete(Options.OutDirDisplayName, AutoCompleteSource.FileSystemDirectories)
            };

            settingsPropertyGrid.DragDrop += MainFormDragDrop;
            settingsPropertyGrid.DragEnter += MainFormDragEnter;

            settingsPropertyGrid.DocLines = 9;
            settingsPropertyGrid.SetDocMonospaceFont();

            Action<Action> modalAction = action => { using (new ModalGuard()) { action(); } };

            // Remove Property Pages button.
            settingsPropertyGrid.RemoveLastToolStripItem();

            // Preferences button.
            var preferencesToolStripSplitButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Preferences, PreferencesToolStripButtonClick);

            _shortenExtensionToolStripMenuItem = new ToolStripMenuItem(Resources.Strings.ShortenImageExtension) { CheckOnClick = true, Checked = _preferences.ShortenExtension };
            _shortenExtensionToolStripMenuItem.CheckedChanged += (s, e) =>
            {
                _preferences.ShortenExtension = _shortenExtensionToolStripMenuItem.Checked;
                Model.OptionsObject.ShortenExtension = _preferences.ShortenExtension;
                UpdateCommandLine();
            };

            var autoSaveSettingsToolStripMenuItem = new ToolStripMenuItem(Resources.Strings.AutoSaveSettings) { CheckOnClick = true, Checked = _preferences.AutoSaveSettings };
            autoSaveSettingsToolStripMenuItem.CheckedChanged += (s, e) => _preferences.AutoSaveSettings = autoSaveSettingsToolStripMenuItem.Checked;

            preferencesToolStripSplitButton.DropDownItems.Add(_shortenExtensionToolStripMenuItem);
            preferencesToolStripSplitButton.DropDownItems.Add(new ToolStripSeparator());
            preferencesToolStripSplitButton.DropDownItems.Add(autoSaveSettingsToolStripMenuItem);

            preferencesToolStripSplitButton.DropDownOpening += (s, e) =>
            {
                autoSaveSettingsToolStripMenuItem.Checked = _preferences.AutoSaveSettings;
                _shortenExtensionToolStripMenuItem.Checked = _preferences.ShortenExtension;
            };

            preferencesToolStripSplitButton.Enabled = Model.IsUserMode;

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            Func<bool, string> copyBatch = eh => String.Format(Resources.Strings.UIBatchCommandLineFormat + (eh ? Resources.Strings.UIBatchCommandLineErrorHandling : String.Empty), _uiCommandLine);

            // Show Command Line button.
            _showCommandLineToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ShowCommandLine, ShowCommandLineToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.CopyCommandLineToClipboard, (s, e) => CopyToClipboard(commandLineTextBox.Text)),
                new ToolStripButtonItem(Resources.Strings.CopyUICommandLineToClipboard, (s, e) => CopyToClipboard(_uiCommandLine)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyUIBatchCommandLineToClipboard, (s, e) => CopyToClipboard(() => copyBatch(true))),
                new ToolStripButtonItem(Resources.Strings.CopyUIBatchCommandLineWithoutErrorHandlingToClipboard, (s, e) => CopyToClipboard(() => copyBatch(false)))
             );

            UpdateShowCommandLineCommand();

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Load/save settings.
            _loadToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.LoadSettings, (s, e) => modalAction(() => Model = SettingsManager.LoadSettings()),
                new ToolStripButtonItem(Resources.Strings.SaveSettings, (s, e) => modalAction(() => SettingsManager.SaveSettings(Model)))
            );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Reset Settings button.
            _resetToolStripButton = settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.ResetOptions, (s, e) => ResetByCategory(Options.CategoryOptions),
                new ToolStripButtonItem(Resources.Strings.ResetParameters, (s, e) => ResetByCategory(Options.CategoryParameters)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.Reset, (s, e) => Model.Reset())
             );

            // Separator.
            settingsPropertyGrid.AddToolStripSeparator();

            // Explorer browse.

            ToolStripButtonItem xpsCopyButton, xpsBrowseButton;
            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.BrowseImages, BrowseConvertedImagesToolStripButtonClick,
                new ToolStripButtonItem(Resources.Strings.BrowseImagesFolder, (s, e) => Explorer.Select(ConvertedImagesFolder)),
                xpsBrowseButton = new ToolStripButtonItem(Resources.Strings.BrowseXPSFile, (s, e) => Explorer.Select(Model.SrcFile)),
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.CopyImagesFolderPathToClipboard, (s, e) => CopyToClipboard(ConvertedImagesFolder)),
                xpsCopyButton = new ToolStripButtonItem(Resources.Strings.CopyXPSFilePathToClipboard, (s, e) => CopyToClipboard(Model.SrcFile))
            ).DropDownOpening += (s, a) => xpsCopyButton.ToolStripItem.Enabled = xpsBrowseButton.ToolStripItem.Enabled = !String.IsNullOrEmpty(Model.SrcFile);

            //  Help.
            _updatesToolStripButtonItem = new ToolStripButtonItem(Resources.Strings.CheckForUpdates, (s, e) => CheckForUpdates());

            settingsPropertyGrid.AddToolStripSplitButton(Resources.Strings.Help, (s, e) => ShowHelp(), _updatesToolStripButtonItem,
                new ToolStripButtonItem(),
                new ToolStripButtonItem(Resources.Strings.About,
                (s, e) => modalAction(() =>
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

            CheckForUpdatesEnabled = Model.IsUserMode;
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
