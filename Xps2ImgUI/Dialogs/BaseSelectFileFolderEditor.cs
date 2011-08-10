using System.Drawing.Design;
using System.IO;

namespace Xps2ImgUI.Dialogs
{
    public class BaseSelectFileFolderEditor : UITypeEditor
    {
        public string DefaultFolder
        {
            get { return Directory.GetCurrentDirectory(); }
        }
    }
}
