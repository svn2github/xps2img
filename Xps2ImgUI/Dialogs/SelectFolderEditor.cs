using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Xps2ImgUI.Dialogs
{
    public class SelectFolderEditor : BaseSelectFileFolderEditor
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

            using (var dialog = new FolderBrowserDialog { SelectedPath = path })
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

            return value ?? String.Empty;
        }
    }
}

