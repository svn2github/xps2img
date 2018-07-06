using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

using Xps2Img.Shared.Utils;
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
                path = PathUtils.GetAbsolutePath(path);

                EnsureDirectory(path);

                using (var dialog = new FolderBrowserDialog { SelectedPath = path, ShowNewFolderButton = true })
                {
                    if (!String.IsNullOrEmpty(Description))
                    {
                        dialog.Description = Description;
                    }

                    FocusSelection();

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        return dialog.SelectedPath;
                    }
                }
            }

            return value;
        }

        private static void FocusSelection()
        {
            SendKeys.Send("{TAB}{TAB}{RIGHT}");
        }

        private static void EnsureDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            // ReSharper disable once RedundantCatchClause
            catch
            {
                // ignored
            }
        }
    }
}

