using System;

namespace Xps2Img.Shared.Attributes.UI
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DynamicPropertyFilterAttribute : Attribute
    {
        public readonly string PropertyName;
        public readonly string ShowOn;

        public DynamicPropertyFilterAttribute() :
            this(null, null)
        {
        }

        public DynamicPropertyFilterAttribute(string propertyName, string showOn)
        {
            PropertyName = propertyName;
            ShowOn = showOn;
        }
    }
}
