using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    // http://social.msdn.microsoft.com/Forums/da-DK/winforms/thread/fa180d07-9c1f-4125-981b-b4ea783cd451
    public class PropertyGridExUIService : IUIService
    {
        private readonly PropertyGrid _propertyGrid;

        internal PropertyGridExUIService(PropertyGrid grid)
        {
            _propertyGrid = grid;
        }

        public DialogResult ShowDialog(Form form)
        {
            var propertyDescriptor = _propertyGrid.SelectedGridItem.PropertyDescriptor;

            // Extract the error details from the GridErrorDlg instance.
            var detailsField = form.GetType().GetField("details", BindingFlags.NonPublic | BindingFlags.Instance);
            var detailsTextBox = (TextBox)detailsField.GetValue(form);

            using (new ModalGuard())
            {
                return MessageBox.Show(_propertyGrid,
                        String.Format("{0}{1}{2}", detailsTextBox.Text, detailsTextBox.Text.EndsWith(".") ? String.Empty : ".", Resources.Strings.CancelToUsePreviousValidValue.Replace(@"\n", Environment.NewLine)),
                        propertyDescriptor.DisplayName,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);
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
