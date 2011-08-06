using System.ComponentModel;

namespace Xps2ImgUI.Utils.UI
{
    public class TabbedDescriptionAttribute : DescriptionAttribute
    {
        public TabbedDescriptionAttribute(string description)
            : base(description)
        {
        }

        private string _formattedDescription;

        public override string Description
        {
            get { return _formattedDescription ?? (_formattedDescription = base.Description.TabsToSpaces(8)); }
        }

    }
}
