using System;

using CommandLine.Strings;

namespace Xps2Img.Shared.Localization
{
    public class DefaultLocalizablePropertyDescriptorStrategy  : ILocalizablePropertyDescriptorStrategy
    {
        private static string FormatId(Type type, string id, string idCategory)
        {
            return DefaultStringsSourceKeyProvider.Instance.FormatKey(type, id.Trim(), idCategory);
        }

        public string GetDisplayNameId(Type type, string propertyName)
        {
            return FormatId(type, propertyName, "Name");
        }

        public string GetCategoryId(Type type, string category)
        {
            return FormatId(type, category, "Category");
        }

        public string GetDescriptionId(Type type, string propertyName)
        {
            return FormatId(type, propertyName, "Description");
        }

        public string GetEnumValueId(Type type, object value)
        {
            return FormatId(type, value.ToString(), "Value");
        }

        public static readonly ILocalizablePropertyDescriptorStrategy Instance = new DefaultLocalizablePropertyDescriptorStrategy();
    }
}
