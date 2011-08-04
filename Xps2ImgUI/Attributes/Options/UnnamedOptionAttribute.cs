using System;

namespace Xps2ImgUI.Attributes.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UnnamedOptionAttribute : BaseOptionAttribute
    {
        public UnnamedOptionAttribute() :
            this(true)
        {
        }

        public UnnamedOptionAttribute(bool isRequired) :
            base(isRequired, null, false)
        {
        }
    }
}