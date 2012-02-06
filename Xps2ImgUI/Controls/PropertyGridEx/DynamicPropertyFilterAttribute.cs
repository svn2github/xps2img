using System;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DynamicPropertyFilterAttribute : Attribute
    {
        public string PropertyName { get; private set; }
        public string ShowOn { get; private set; }

        public DynamicPropertyFilterAttribute(string propertyName, string showOn)
        {
            PropertyName = propertyName;
            ShowOn = showOn;
        }
    }
}
