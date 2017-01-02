namespace Xps2Img.Shared.TypeEditors.Dialogs
{
    public class SelectXpsFileEditor : SelectFileEditor
    {
        public SelectXpsFileEditor() :
            base(() => Resources.Strings.FilterXPSFiles + Resources.Strings.FilterAllFiles)
        {
            Title = () => Resources.Strings.SelectXpsFile;
        }
    }
}