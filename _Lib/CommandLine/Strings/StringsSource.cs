using System;
using System.Reflection;

namespace CommandLine.Strings
{
    public class StringsSource
    {
        public Type Type { get; private set; }

        public StringsSource(Type type)
        {
            Type = type;
        }

        public string GetString(string key)
        {
            var propertyInfo = Type.GetProperty(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return propertyInfo != null ? (string)(propertyInfo.GetGetMethod(true)).Invoke(null, null) : key;
        }
    }
}
