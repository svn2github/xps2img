using System.Collections.Generic;
using System.Reflection;

namespace Xps2Img.Shared.Utils
{
    public static class ReflectionUtils
    {
        private static readonly Dictionary<string, PropertyInfo> _cachedProperties = new Dictionary<string, PropertyInfo>();

        public static T GetPropertyValue<T>(object obj, string name, bool nonPublic = true)
        {
            PropertyInfo propertyInfo;

            var type = obj.GetType();
            var key = type + "+" + name;

            if (!_cachedProperties.TryGetValue(key, out propertyInfo))
            {
                propertyInfo = type.GetProperty(name, BindingFlags.Instance | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Public));
                _cachedProperties.Add(key, propertyInfo);
            }

            return propertyInfo != null ? (T)propertyInfo.GetValue(obj, null) : default(T);
        }

        public static object GetPropertyValue(object obj, string name, bool nonPublic = true)
        {
            return GetPropertyValue<object>(obj, name, nonPublic);
        }
    }
}
