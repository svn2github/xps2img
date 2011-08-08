﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

namespace Xps2ImgUI.Dialogs
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
            Filter = String.IsNullOrEmpty(filter) ? Utils.Filter.AllFiles : filter;
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

            using (var dialog = new OpenFileDialog { Filter = Filter, InitialDirectory = InitialDirectory, FileName = fileName })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }

            return value ?? String.Empty;
        }
    }
}
