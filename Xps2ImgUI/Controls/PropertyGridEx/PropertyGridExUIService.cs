using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Microsoft.WindowsAPICodePack.Dialogs;

using Windows7.Dialogs;

using Xps2Img.Shared.Utils;
using Xps2Img.Shared.Utils.UI;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    // http://social.msdn.microsoft.com/Forums/da-DK/winforms/thread/fa180d07-9c1f-4125-981b-b4ea783cd451
    public class PropertyGridExUIService : IUIService
    {
        private readonly PropertyGridEx _propertyGrid;

        public PropertyGridExUIService(PropertyGridEx grid)
        {
            _propertyGrid = grid;
        }

        public DialogResult ShowDialog(Form form)
        {
            using (new ModalGuard())
            {
                var propertyDescriptor = _propertyGrid.SelectedGridItem.PropertyDescriptor;

                // Extract the error details from the GridErrorDlg instance.
                var detailsField = form.GetType().GetField("details", BindingFlags.NonPublic | BindingFlags.Instance);
                var detailsTextBox = (TextBox)detailsField.GetValue(form);
                var displayName = propertyDescriptor.DisplayName;

                var title = String.Format(Resources.Strings.ValidationErrorFormat, displayName);
                var message = detailsTextBox.Text.AppendDot();

                var dialogResult = TaskDialogUtils.Show(
                                    _propertyGrid.Handle,
                                    title,
                                    displayName,
                                    message,
                                    TaskDialogStandardIcon.Error,
                                    null,
                                    new TaskDialogCommandInfo(TaskDialogResult.Ok, Resources.Strings.ValueEdit), 
                                    new TaskDialogCommandInfo(TaskDialogResult.Cancel, Resources.Strings.ValueRevert));

                if (dialogResult == TaskDialogUtils.NotSupported)
                {
                    dialogResult = MessageBox.Show(_propertyGrid,
                                    String.Format("{0}{1}", message, Resources.Strings.CancelToUsePreviousValidValue.Replace(@"\n", Environment.NewLine)),
                                    title,
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Error);
                }

                _propertyGrid.HasErrors = dialogResult == DialogResult.OK;

                return dialogResult;
            }
        }

        public bool CanShowComponentEditor(object component)
        {
            throw new NotSupportedException();
        }

        public IWin32Window GetDialogOwnerWindow()
        {
            throw new NotSupportedException();
        }

        public void SetUIDirty()
        {
            throw new NotSupportedException();
        }

        public bool ShowComponentEditor(object component, IWin32Window parent)
        {
            throw new NotSupportedException();
        }

        public void ShowError(Exception ex, string message)
        {
            throw new NotSupportedException();
        }

        public void ShowError(Exception ex)
        {
            throw new NotSupportedException();
        }

        public void ShowError(string message)
        {
            throw new NotSupportedException();
        }

        public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons)
        {
            throw new NotSupportedException();
        }

        public void ShowMessage(string message, string caption)
        {
            throw new NotSupportedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotSupportedException();
        }

        public bool ShowToolWindow(Guid toolWindow)
        {
            throw new NotSupportedException();
        }

        public IDictionary Styles
        {
            get { throw new NotSupportedException(); }
        }
    }
}
