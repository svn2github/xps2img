using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

using Xps2Img.Shared.Utils.UI;

namespace Xps2Img.Shared.Dialogs
{
    public class SelectFileEditor : BaseSelectFileFolderEditor
    {
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }

        public SelectFileEditor()
            : this(null, null)
        {
        }

        public SelectFileEditor(string filter)
            : this(filter, null)
        {
        }

        public SelectFileEditor(string filter, string initialDirectory)
        {
            Filter = String.IsNullOrEmpty(filter) ? Resources.Strings.FilterAllFiles : filter;
            InitialDirectory = String.IsNullOrEmpty(initialDirectory) ? DefaultFolder : initialDirectory;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var fileName = (string)value ?? String.Empty;

            if (!String.IsNullOrEmpty(fileName))
            {
                var folder = Path.GetDirectoryName(fileName);
                if (!String.IsNullOrEmpty(folder))
                {
                    InitialDirectory = folder;
                }
            }

            using (new ModalGuard())
            {
                using (var dialog = new OpenFileDialog { Filter = Filter, InitialDirectory = InitialDirectory, FileName = Path.GetFileName(fileName) })
                {
                    if (!String.IsNullOrEmpty(Title))
                    {
                        dialog.Title = Title;
                    }

                    var tryCount = 2;

                    while (tryCount-- > 0)
                    {
                        try
                        {
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                return dialog.FileName;
                            }
                            break;
                        }
                        catch
                        {
                            dialog.FileName = String.Empty;
                        }
                    }
                }
            }

            return value ?? String.Empty;
        }
    }
}
