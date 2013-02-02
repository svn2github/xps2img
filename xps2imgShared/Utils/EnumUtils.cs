using System;
using System.ComponentModel;
using System.Linq;

namespace Xps2Img.Shared.Utils
{
    public static class EnumUtils
    {
        public static bool HasValue<T>(string str)
        {
            T val;
            return TryParse(str, out val);
        }

        public static bool TryParse<T>(string str, out T parsed)
        {
            try
            {
                parsed = (T)Enum.Parse(typeof(T), str, true);
                return true;
            }
            catch
            {
                parsed = default(T);
                return false;
            }
        }

        public static string GetDescriptionFromValue(Enum value)
        {
            var descriptionAttribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;

            return descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("Enum type is expected.");
            }

            var fields = type.GetFields();

            var field = fields
                          .SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Attr = a })
                          .Where(a => ((DescriptionAttribute)a.Attr).Description == description)
                          .SingleOrDefault();

            T value;

            return field == null
                    ? TryParse(description, out value)
                        ? value
                        : default(T)
                    : (T)field.Field.GetRawConstantValue();
        }
    }
}
