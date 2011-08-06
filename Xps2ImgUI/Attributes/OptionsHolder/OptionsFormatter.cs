using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Xps2ImgUI.Attributes.Options;

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

        public static string GetOptionValue(bool exceptionIfNoRequired, BaseOptionAttribute optionAttribute, object boundObject)
        {
            var propertyInfo = optionAttribute.PropertyInfo;
            var typeConverter = TypeDescriptor.GetConverter(propertyInfo.GetType());

            var value = propertyInfo.GetValue(boundObject, null);
            if (value is bool)
            {
                return (bool)value ? String.Empty : null;
            }

            var defaultAttribute = propertyInfo.GetCustomAttributes(true).OfType<DefaultValueAttribute>().FirstOrDefault();
            if (defaultAttribute != null && defaultAttribute.Value.Equals(value))
            {
                return null;
            }

            var optionValue = typeConverter != null ? typeConverter.ConvertToInvariantString(value) : value.ToString();
            
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

        private static readonly char[] TrimSymbols = { '"' };

        private static string FormatOptionValue(string optionValue)
        {
            if(String.IsNullOrEmpty(optionValue))
            {
                return String.Empty;
            }

            optionValue = optionValue.Trim(TrimSymbols);
            return optionValue.Contains("\x20")
                    ? String.Format("\"{0}\"", optionValue)
                    : optionValue;
        }

        private static string FormatOption(bool exceptionIfNoRequired, object boundObject, BaseOptionAttribute optionAttribute, OptionFormatInfo optionFormatInfo)
        {
            var strValue = GetOptionValue(exceptionIfNoRequired, optionAttribute, boundObject);

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

        public static string FormatCommandLine(bool exceptionIfNoRequired, object optionsObject, IEnumerable<BaseOptionAttribute> optionAttributes)
        {
            const string optionsSeparator = " ";
            
            return String.Join(optionsSeparator, optionAttributes
                    .Select(o => FormatOption(exceptionIfNoRequired, optionsObject, o, CurrentOptionFormatInfo))
                    .Where(f => !String.IsNullOrEmpty(f))
                    .ToArray());
        }
    }
}
