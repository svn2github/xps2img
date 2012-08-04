namespace Xps2ImgUI.Dialogs
{
    public class SelectXpsFileEditor : SelectFileEditor
    {
        public SelectXpsFileEditor() :
            base(Resources.Strings.FilterXPSFiles + Resources.Strings.FilterAllFiles)
        {
            Title = Resources.Strings.SelectXpsFile;
        }
    }
}