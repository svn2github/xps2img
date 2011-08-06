using System;

namespace Xps2ImgUI.Attributes.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : BaseOptionAttribute
    {
        public OptionAttribute(string name) :
            base(false, name, true)
        {
        }

        public OptionAttribute(string name, bool isArgRequired) :
            base(false, name, isArgRequired)
        {
        }

        public OptionAttribute(char name) :
            this(name.ToString())
        {
        }
    }
}