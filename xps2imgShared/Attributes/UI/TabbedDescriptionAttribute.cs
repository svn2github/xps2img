using System.ComponentModel;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.Attributes.UI
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
