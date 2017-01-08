using System;
using System.Drawing.Design;

namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class SelectFileFolderEditorBase : UITypeEditor
    {
        protected Func<string> Title { get; set; }

        protected static string DefaultFolder
        {
            #if DEBUG
            get { return System.Windows.Forms.Application.ExecutablePath; }
            #else
            get { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); }
            #endif
        }
    }
}
