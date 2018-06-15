using System;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Enums;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgLib.Utils.Disposables;

using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        private void OptionsObjectChanged(object sender, EventArgs e)
        {
            settingsPropertyGrid.SelectedObject = Model.OptionsObject;
            Refresh();

            UpdateCommandLine();
        }

        private void BatchLaunchIdle(object sender, EventArgs eventArgs)
        {
            UnregisterIdleHandler(BatchLaunchIdle);
            if (Model.Clean)
            {
                ExecuteDeleteImages();
            }
            else
            {
                ExecuteConversion(ConversionType.Convert);
            }
        }

        private void MainFormDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = Model.IsRunning || String.IsNullOrEmpty(GetDragFile(e.Data))
                        ? DragDropEffects.None
                        : DragDropEffects.Copy;
        }

        private void MainFormDragDrop(object sender, DragEventArgs e)
        {
            var file = GetDragFile(e.Data);

            if (!String.IsNullOrEmpty(file))
            {
                Model.SrcFile = file;
                settingsPropertyGrid.Refresh();
                UpdateCommandLine();
            }
        }

        private void OutputDataReceived(object sender, ConversionProgressEventArgs e)
        {
            _handle.PostMessage(WmProgress, e);
        }

        private void ErrorDataReceived(object sender, ConversionErrorEventArgs e)
        {
            this.InvokeIfNeeded(() => UpdateFailedStatus(e.Message, page: e.Page));
        }

        private void HandleErrorPages()
        {
            var errorPages = Model.ErrorPages;

            if (!errorPages.Any())
            {
                return;
            }

            var pages = IntervalUtils.ToString(errorPages);

            if (Model.IsBatchMode)
            {
                ShowErrorMessageBox(Resources.Strings.ConversionCompletedWithErrors + String.Format(Resources.Strings.NotConvertedPageNumbers, pages));
                return;
            }

            if (ShowConfirmationMessageBox(Resources.Strings.ConversionCompletedWithErrors, String.Format(Resources.Strings.NotConvertedPageNumbersConfirmToCopy, pages)))
            {
                ClipboardUtils.CopyToClipboard(pages);
            }
        }

        private void Completed(object sender, EventArgs e)
        {
            if (_conversionFailed)
            {
                this.InvokeIfNeeded(() => EnableConvertControls());
                return;
            }

            this.InvokeIfNeeded(() =>
            {
                UpdateRunningStatus(false);
                EnableConvertControls();
                FlashForm();
                HandleErrorPages();
            });
        }

        private void LaunchFailed(object sender, ThreadExceptionEventArgs e)
        {
            var message = e.Exception.Message.AppendDot();

            if (e.Exception is Win32Exception)
            {
                message = String.Format(Resources.Strings.Xps2ImgNotFountFormat, Environment.NewLine, message);
            }
            
            this.InvokeIfNeeded(() =>
            {
                UpdateFailedStatus(message, e.Exception);
                EnableConvertControls();
            });
        }

        private void LaunchSucceeded(object sender, EventArgs e)
        {
            if (Model.IsDeleteMode)
            {
                return;
            }

            this.InvokeIfNeeded(() => EnableConvertControls());
        }

        private void CloseFormHandler(object sender, EventArgs e)
        {
            if (Model.IsRunning)
            {
                return;
            }

            UnregisterIdleHandler(CloseFormHandler);

            if (Model.IsBatchMode && Model.ExitCode == ReturnCode.UserCancelled)
            {
                ShowErrorMessageBox(Resources.Strings.ConversionWasAbortedByUser);
            }

            if (_preferences.ConfirmOnAfterConversion
                && Model.ShutdownRequested
                && Model.ShutdownType != PostAction.Exit
                && !Model.IsBatchMode
                && !IsShutdownConfirmed())
            {
                return;
            }

            _autoClosed = true;

            Close();
        }

        private void SettingsPropertyGridPropertySortChanged(object sender, EventArgs e)
        {
            var propertyGrid = (PropertyGrid) sender;
            if (propertyGrid.PropertySort == PropertySort.CategorizedAlphabetical)
            {
                propertyGrid.PropertySort = PropertySort.Categorized;
            }
        }

        private readonly string[] _forceRefreshForProperties = { Options.Properties.FileType, Options.Properties.PostAction, Options.Properties.UseFileNameAsImageName, Options.Properties.PreferDpiOverSize, Options.Properties.PageCrop };

        private void UpdateCategoryReset()
        {
            if (_resetToolStripButton != null)
            {
                _resetToolStripButton.Enabled = settingsPropertyGrid.IsResetByCategoryEnabled(Options.Categories.Options) || settingsPropertyGrid.IsResetByCategoryEnabled(Options.Categories.Parameters);
            }
        }
        
        private void SettingsPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            var propertyDescriptor = e.ChangedItem.PropertyDescriptor;

            var propertyName = propertyDescriptor == null ? String.Empty : propertyDescriptor.Name;

            Func<string[], bool> hasOneOf = o => o.Contains(propertyName);

            var forceRefresh = false;
            var canResume = hasOneOf(Options.ExcludeOnResumeCheck);

            if (!canResume && propertyName != Options.Properties.IgnoreExisting && Model.IgnoreExisting)
            {
                Model.IgnoreExisting = false;
                forceRefresh = true;
            }

            if (forceRefresh || hasOneOf(_forceRefreshForProperties))
            {
                settingsPropertyGrid.Refresh();
            }

            if (propertyName == Options.Properties.SrcFile && String.Compare((string)e.OldValue, (string)e.ChangedItem.Value, StringComparison.OrdinalIgnoreCase) != 0)
            {
                Model.CanResume = false;

                UpdateElapsedTime(true);
            }

            UpdateCommandLine(canResume);
            UpdateConvertButtons();
            UpdateCategoryReset();
        }

        private void SettingsPropertyGridSelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateCommandLine();
            UpdateCategoryReset();
        }

        private void ClassicLookChanged(Preferences preferences)
        {
            settingsPropertyGrid.ModernLook = !preferences.ClassicLook;
        }

        private void AlwaysResumeChanged(Preferences preferences)
        {
            _activeAlwaysResume = preferences.AlwaysResume;
            if (!Model.IsRunning)
            {
                UpdateConvertButtons();
            }
        }

        private void UseFullExePathChanged(Preferences preferences)
        {
            _useFullExePath = preferences.UseFullExePath;
            UpdateCommandLine(true);
        }

        private void PreferencesToolStripButtonClick(object sender, EventArgs e)
        {
            using (new ModalGuard())
            {
                using (new DisposableActions(() => _activeAlwaysResume = null, () => _useFullExePath = null))
                {
                    using (var preferencesForm = new PreferencesForm(_preferences, Model, ClassicLookChanged, AlwaysResumeChanged, UseFullExePathChanged) { PreferencesPropertySort = _preferencesPropertySort })
                    {
                        var dialogResult = preferencesForm.ShowDialog(this);

                        _preferencesPropertySort = preferencesForm.PreferencesPropertySort;

                        if (dialogResult != DialogResult.OK)
                        {
                            return;
                        }

                        _preferences = preferencesForm.Preferences;

                        ApplyPreferences(true);
                    }
                }
            }
        }

        private void ShowCommandLineToolStripButtonClick(object sender, EventArgs e)
        {
            IsCommandLineVisible = !IsCommandLineVisible;
            UpdateShowCommandLineCommand();
        }

        private void BrowseConvertedImagesToolStripButtonClick(object sender, EventArgs e)
        {
            Explorer.Browse(ConvertedImagesFolder);
        }

        private void DeleteImagesToolStripMenuItemClick(object sender, EventArgs e)
        {
            ExecuteDeleteImages();
        }

        private void СonvertContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            var menu = (ContextMenuStrip) sender;

            menu.Renderer = settingsPropertyGrid.ToolStripRenderer;
            
            convertToolStripMenuItem.Text = Resources.Strings.Launch;
            resumeToolStripMenuItem.Text = Resources.Strings.Resume;
            deleteImagesToolStripMenuItem.Text = Resources.Strings.DeleteImages;

            var items = new[] { convertToolStripMenuItem, resumeToolStripMenuItem };

            Array.ForEach(items, menu.Items.Remove);

            if(!Model.CanResume)
            {
                menu.Items.Insert(0, convertToolStripMenuItem);
                return;
            }

            var index = 0;
            Array.ForEach(_preferences.AlwaysResume ? items.Reverse().ToArray() : items, i => menu.Items.Insert(index++, i));
        }

        private bool _autoClosed;

        private bool? _activeAlwaysResume;
        private bool? _useFullExePath;
    }
}
