namespace Xps2ImgUI.Dialogs
{
    public class SelectXpsFileEditor : SelectFileEditor
    {
        public SelectXpsFileEditor() :
            base("XPS Files (*.xps)|*.xps|" + Utils.Filter.AllFiles)
        {
            Title = Resources.Strings.SelectXpsFile;
        }
    }
}