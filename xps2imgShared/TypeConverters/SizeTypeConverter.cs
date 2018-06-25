﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Xps2Img.Shared.TypeConverters
{
    public class SizeTypeConverter : TypeConverter
    {
        private readonly Regex _filter;
        private readonly bool _supportsBoth;

        protected SizeTypeConverter(string validationRegex, bool supportsBoth)
        {
            _supportsBoth = supportsBoth;
            _filter = new Regex(validationRegex);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!CanConvertTo(destinationType))
            {
                return null;
            }

            var nullableSize = value as Size?;
            if (nullableSize == null)
            {
                return null;
            }

            var size = nullableSize.Value;

            var converted = new StringBuilder(16);

            if (size.Width > 0)
            {
                converted.AppendFormat("{0}", size.Width);
                if (!_supportsBoth)
                {
                    return converted.ToString();
                }
            }

            if (size.Height > 0)
            {
                converted.AppendFormat("x{0}", size.Height);
            }

            return converted.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null && !CanConvertFrom(value.GetType()) || String.IsNullOrEmpty((string)value))
            {
                return null;
            }

            var match = _filter.Match((string)value);
            if (!match.Success)
            {
                throw new ArgumentException(@"UNEXPECTED: Invalid input string. Validation failed", "value");
            }

            Func<string, int> convertToInt = group =>
            {
                var strVal = match.Groups[group].Value;
                return String.IsNullOrEmpty(strVal) ? 0 : int.Parse(strVal, CultureInfo.InvariantCulture);
            };

            return new Size(convertToInt("width"), convertToInt("height"));
        }
    }
}