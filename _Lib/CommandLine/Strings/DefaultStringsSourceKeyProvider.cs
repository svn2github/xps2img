using System;

using CommandLine.Interfaces;

namespace CommandLine.Strings
{
    public class DefaultStringsSourceKeyProvider : IStringsSourceKeyProvider
    {
        public string FormatKey(Type type, string id, string idCategory)
        {
            // Type_IdCategory
            return string.Format("{0}_{1}{2}", type.Name, id, idCategory);
        }

        public static readonly IStringsSourceKeyProvider Instance = new DefaultStringsSourceKeyProvider();
    }
}
