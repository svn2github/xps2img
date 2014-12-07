using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Xps2Img.Shared.Diagnostics
{
    public static class ValidateProperties
    {
        [Conditional("DEBUG")]
        public static void For<T>(Type propertiesClassType) where T : class
        {
            var propertiesDefinitions = propertiesClassType.GetFields(BindingFlags.Public | BindingFlags.Static).Select(p => new { p.Name, Value = (string)p.GetValue(null) }).ToArray();

            Debug.Assert(propertiesDefinitions.All(p => p.Name == p.Value));

            var typeProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetSetMethod() != null).ToLookup(p => p.Name);

            var missingProperties = propertiesDefinitions.Where(p => !typeProperties.Contains(p.Name)).ToArray();

            Debug.Assert(!missingProperties.Any());
        }
    }

}
