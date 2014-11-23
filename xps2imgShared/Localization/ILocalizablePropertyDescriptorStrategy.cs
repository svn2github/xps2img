using System;

namespace Xps2Img.Shared.Localization
{
    public interface ILocalizablePropertyDescriptorStrategy
    {
        string GetDisplayNameId(Type type, string propertyName);
        string GetCategoryId(Type type, string category);
        string GetDescriptionId(Type type, string propertyName);
        string GetEnumValueId(Type type, object value);
    }
}
