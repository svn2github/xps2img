using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Xps2Img.Shared.Attributes.Options;
using Xps2Img.Shared.Attributes.UI;
using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Utils;

namespace Xps2ImgUI.Attributes.OptionsHolder
{
    public static class OptionsFormatter
    {
        private class OptionFormatInfo
        {
            public readonly string Short;
            public readonly string Long;

            public OptionFormatInfo(string @short, string @long)
            {
                Short = @short;
                Long = @long;
            }
        }

        public static string GetOptionValue(bool exceptionIfNoRequired, bool includeFiltered, BaseOptionAttribute optionAttribute, object boundObject)
        {
            var propertyInfo = optionAttribute.PropertyInfo;

            TypeConverter typeConverter = null;

            if (!optionAttribute.AlwaysFormat && !includeFiltered)
            {
                var dynamicPropertyFilterAttribute = (DynamicPropertyFilterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(DynamicPropertyFilterAttribute));
                if (dynamicPropertyFilterAttribute != null)
                {
                    var propertyValue = boundObject.GetType().GetProperty(dynamicPropertyFilterAttribute.PropertyName).GetValue(boundObject, null);
                    if (!propertyValue.Equals(dynamicPropertyFilterAttribute.ShowOn))
                    {
                        return null;
                    }
                }
            }

            var typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TypeConverterAttribute));
            if (typeConverterAttribute != null)
            {
                var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
                if (converterType != null)
                {
                    typeConverter = (TypeConverter)Activator.CreateInstance(converterType);
                }
            }

            if (typeConverter == null)
            {
                typeConverter = TypeDescriptor.GetConverter(propertyInfo.GetType());
            }

            var value = propertyInfo.GetValue(boundObject, null);
            if (value is bool)
            {
                return (bool)value ? String.Empty : null;
            }

            var defaultAttribute = propertyInfo.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault();
            if (defaultAttribute != null && (defaultAttribute.Value == null ? defaultAttribute.Value == value : defaultAttribute.Value.Equals(value)))
            {
                return null;
            }

            var optionValue = typeConverter.ConvertToInvariantString(value);
            
            var formattedValue = FormatOptionValue(optionValue);

            if (String.IsNullOrEmpty(formattedValue))
            {
                if (optionAttribute.IsRequired)
                {
                    if (exceptionIfNoRequired)
                    {
                        throw new Exception(String.Format("{0} is required.", optionAttribute.Name));
                    }
                    return String.Empty;
                }

                return null;
            }

            return formattedValue;
        }

        private static readonly char[] TrimSymbols = { '"', '\\', '/' };
        private static readonly char[] EscapeSymbols = { StringUtils.SpaceChar, '\t', '\'' };
        
        private static string FormatOptionValue(string optionValue)
        {
            if(String.IsNullOrEmpty(optionValue))
            {
                return String.Empty;
            }

            var escape = optionValue == Options.Names.Empty;
            optionValue = optionValue.Trim(TrimSymbols);

            return escape || optionValue.IndexOfAny(EscapeSymbols) != -1
                    ? String.Format("\"{0}\"", optionValue)
                    : optionValue;
        }

        private static string FormatOption(bool exceptionIfNoRequired, bool includeFiltered, object boundObject, BaseOptionAttribute optionAttribute, OptionFormatInfo optionFormatInfo)
        {
            var strValue = GetOptionValue(exceptionIfNoRequired, includeFiltered, optionAttribute, boundObject);

            if (strValue == null)
            {
                return String.Empty;
            }

            return optionAttribute.IsUnnamed ?
                    strValue :
                    String.Format(
                      optionAttribute.IsLongName ? optionFormatInfo.Long : optionFormatInfo.Short,
                      optionAttribute.Name, strValue).TrimEnd();
        }

        private static readonly OptionFormatInfo CurrentOptionFormatInfo = new OptionFormatInfo("-{0} {1}", "--{0}={1}");

        public static string FormatCommandLine(bool exceptionIfNoRequired, bool includeFiltered, object optionsObject, IEnumerable<BaseOptionAttribute> optionAttributes, string[] optionsToExclude)
        {
            const string optionsSeparator = " ";

            return String.Join(optionsSeparator, optionAttributes
                    .Select(o => optionsToExclude != null && optionsToExclude.Contains(o.Name)
                                    ? String.Empty
                                    : FormatOption(exceptionIfNoRequired, includeFiltered, optionsObject, o, CurrentOptionFormatInfo))
                    .Where(f => !String.IsNullOrEmpty(f))
                    .ToArray());
        }
    }
}
