namespace Xps2ImgUI.Dialogs
{
    public class SelectXpsFileEditor : SelectFileEditor
    {
        SelectXpsFileEditor() :
            base("XPS Files (*.xps)|*.xps|" + Utils.Filter.AllFiles)
        {
        }
    }
}