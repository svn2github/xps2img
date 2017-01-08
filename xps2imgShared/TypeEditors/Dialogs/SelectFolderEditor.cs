using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

using Xps2Img.Shared.Utils.UI;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class SelectFolderEditor : SelectFileFolderEditorBase
    {
        public string Description { get; set; }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var path = (string) value;

            if (String.IsNullOrEmpty(path))
            {
                path = DefaultFolder;
            }

            using (new ModalGuard())
            {
                using (var dialog = new FolderBrowserDialog {SelectedPath = path})
                {
                    if (!String.IsNullOrEmpty(Description))
                    {
                        dialog.Description = Description;
                    }

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        return dialog.SelectedPath;
                    }
                }
            }

            return value;
        }
    }
}

