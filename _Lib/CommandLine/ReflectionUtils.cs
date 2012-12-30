using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommandLine
{
    public static class ReflectionUtils
    {
        public static void SetDefaultValues(object obj, Func<PropertyInfo, bool> propertyFilter)
        {
            Action<PropertyInfo> setDefaultValue = propertyInfo => { if (propertyFilter == null || propertyFilter(propertyInfo)) SetDefaultValue(obj, propertyInfo); };
            ForEachPropertyInfo(obj, setDefaultValue);
        }

        public static TypeConverter GetTypeConverter(PropertyInfo propertyInfo)
        {
            TypeConverter typeConverter = null;

            var typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TypeConverterAttribute));
            if (typeConverterAttribute != null)
            {
                var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
                if (converterType != null)
                {
                    typeConverter = (TypeConverter)Activator.CreateInstance(converterType);
                }
            }

            return typeConverter ?? TypeDescriptor.GetConverter(propertyInfo.PropertyType);
        }

        public static void SetDefaultValues(object obj)
        {
            SetDefaultValues(obj, null);
        }

        public static void SetDefaultValue(object obj, PropertyInfo propertyInfo)
        {
            var defaultValueAttribute = propertyInfo.FirstOrDefaultAttribute<DefaultValueAttribute>();
            if (defaultValueAttribute != null)
            {
                propertyInfo.SetValue(obj,
                    defaultValueAttribute.Value is string
                        ? GetTypeConverter(propertyInfo).ConvertFromInvariantString(defaultValueAttribute.Value as string)
                        : defaultValueAttribute.Value,
                    null);
            }
        }

        public static void ForEachPropertyInfo(object optionsObject, Action<PropertyInfo> propertyInfoAction)
        {
            ForEachPropertyInfo(optionsObject, pi => { propertyInfoAction(pi); return true; });
        }

        public static bool ForEachPropertyInfo(object optionsObject, Func<PropertyInfo, bool> propertyInfoAction)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            return optionsObject.GetType().GetProperties(bindingFlags).All(propertyInfoAction);
        }

        public static T FirstOrDefaultAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return (T)memberInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static T FirstOrNewAttribute<T>(this MemberInfo memberInfo) where T : Attribute, new ()
        {
            return FirstOrDefaultAttribute<T>(memberInfo) ?? new T();
        }

        public static T FirstOrNewAttribute<T>(this MemberInfo memberInfo, Func<T> activator) where T : Attribute
        {
            return FirstOrDefaultAttribute<T>(memberInfo) ?? activator();
        }

        public static string GetPropertyName(Expression<Func<object>> propertyExpression)
        {
            var body = propertyExpression.Body is UnaryExpression
                            ? (MemberExpression) ((UnaryExpression) propertyExpression.Body).Operand
                            : (MemberExpression) propertyExpression.Body;

            return body.Member.Name;
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(memoryStream, obj);
                memoryStream.Position = 0;

                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static void SetReadOnly<T>(bool readOnly, Expression<Func<object>> propertyExpression)
        {
            SetReadOnly<T>(readOnly, GetPropertyName(propertyExpression));
        }

        public static void SetReadOnly<T>(bool readOnly, string propertyName)
        {
            var descriptor = TypeDescriptor.GetProperties(typeof(T))[propertyName];
            if (descriptor == null)
            {
                return;
            }

            var readOnlyAttribute = descriptor.Attributes[typeof(ReadOnlyAttribute)] as ReadOnlyAttribute;
            if (readOnlyAttribute == null)
            {
                return;
            }

            var fieldToChange = readOnlyAttribute.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldToChange != null)
            {
                fieldToChange.SetValue(readOnlyAttribute, readOnly);
            }
        }
    }
}
