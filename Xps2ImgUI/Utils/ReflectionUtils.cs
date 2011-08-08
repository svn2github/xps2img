using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Xps2ImgUI.Utils
{
    public static class ReflectionUtils
    {
        public static void SetDefaultValues(object obj, Func<PropertyInfo, bool> propertyFilter)
        {
            ForEachPropertyInfo(
              obj,
              propertyInfo =>
              {
                  if (propertyFilter == null || propertyFilter(propertyInfo))
                  {
                      var defaultValueAttribute = propertyInfo.FirstOrDefaultAttribute<DefaultValueAttribute>();
                      if (defaultValueAttribute != null)
                      {
                          propertyInfo.SetValue(obj, defaultValueAttribute.Value, null);
                      }
                  }
              }
            );
        }

        public static void SetDefaultValues(object obj)
        {
            SetDefaultValues(obj, null);
        }

        public static void ForEachPropertyInfo(object optionsObject, Action<PropertyInfo> propertyInfoAction)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            Array.ForEach(optionsObject.GetType().GetProperties(bindingFlags), propertyInfoAction);
        }

        public static T FirstOrDefaultAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return (T)memberInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
