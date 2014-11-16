using System;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

using Xps2ImgUI.Model;
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
            if (Model.OptionsObject.Clean)
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
            ConvertedImagesFolder = e.File;
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
                return;
            }

            this.InvokeIfNeeded(() => { UpdateRunningStatus(false); EnableConvertControls(); FlashForm(); HandleErrorPages(); });
        }

        private void LaunchFailed(object sender, ThreadExceptionEventArgs e)
        {
            var message = e.Exception.Message.AppendDot();

            if (e.Exception is Win32Exception)
            {
                message = String.Format(Resources.Strings.Xps2ImgNotFountFormat, Environment.NewLine, message);
            }
            
            this.InvokeIfNeeded(() => { UpdateFailedStatus(message, e.Exception); EnableConvertControls(); });
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

        private readonly string[] _forceRefreshForProperties = { Options.FileTypeDisplayName, Options.PostActionDisplayName };

        private void SettingsPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Func<string[], bool> hasOneOf = o => o.Contains(e.ChangedItem.Label);

            var forceRefresh = false;
            var canResume = hasOneOf(Options.ExcludeOnResumeCheck);

            if (Model.OptionsObject.IgnoreExisting && !canResume)
            {
                Model.OptionsObject.IgnoreExisting = false;
                forceRefresh = true;
            }

            if (forceRefresh || hasOneOf(_forceRefreshForProperties))
            {
                settingsPropertyGrid.Refresh();
            }

            UpdateCommandLine(canResume);
        }

        private void SettingsPropertyGridSelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateCommandLine();
        }

        private void PreferencesToolStripButtonClick(object sender, EventArgs e)
        {
            using (new ModalGuard())
            {
                using (var preferencesForm = new PreferencesForm(_preferences, Model.IsRunning))
                {
                    if (preferencesForm.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    _preferences = preferencesForm.Preferences;
                    ApplyPreferences(true);
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

            convertToolStripMenuItem.Text = Resources.Strings.Launch;
            resumeToolStripMenuItem.Text = Resources.Strings.Resume;
            deleteImagesToolStripMenuItem.Text = Resources.Strings.DeleteImages;

            if(Model.CanResume)
            {
                menu.Items.Insert(_resumeToolStripMenuItemPosition, resumeToolStripMenuItem);
            }
            else
            {
                menu.Items.Remove(resumeToolStripMenuItem);
            }
        }

        private bool _autoClosed;
    }
}
