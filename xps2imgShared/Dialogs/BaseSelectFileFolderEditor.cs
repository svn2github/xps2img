using System.Drawing.Design;

namespace Xps2Img.Shared.Dialogs
{
    public class BaseSelectFileFolderEditor : UITypeEditor
    {
        public string Title { get; set; }

        public string DefaultFolder
        {
            #if DEBUG
            get { return System.Windows.Forms.Application.ExecutablePath; }
            #else
            get { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); }
            #endif
        }
    }
}
