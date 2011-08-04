using System;

namespace Xps2ImgUI.Attributes.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NamedOptionAttribute : BaseOptionAttribute
    {
        public NamedOptionAttribute(string name) :
            base(false, name, true)
        {
        }

        public NamedOptionAttribute(string name, bool isArgRequired) :
            base(false, name, isArgRequired)
        {
        }
    }
}