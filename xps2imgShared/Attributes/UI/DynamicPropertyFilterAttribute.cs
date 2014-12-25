using System;

namespace Xps2Img.Shared.Attributes.UI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DynamicPropertyFilterAttribute : Attribute
    {
        public string PropertyName { get; private set; }
        public object ShowOn { get; private set; }

        public DynamicPropertyFilterAttribute(string propertyName, object showOn)
        {
            PropertyName = propertyName;
            ShowOn = showOn;
        }
    }
}
