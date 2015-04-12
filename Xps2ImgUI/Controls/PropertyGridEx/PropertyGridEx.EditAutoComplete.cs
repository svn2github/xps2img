using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public partial class PropertyGridEx
    {
        public class EditAutoComplete
        {
            public EditAutoComplete(string propName, AutoCompleteSource autoCompleteSource, AutoCompleteMode autoCompleteMode = AutoCompleteMode.SuggestAppend)
            {
                PropName = propName;
                AutoCompleteMode = autoCompleteMode;
                AutoCompleteSource = autoCompleteSource;
            }

            public readonly string PropName;
            public readonly AutoCompleteMode AutoCompleteMode;
            public readonly AutoCompleteSource AutoCompleteSource;
        }
    }
}
