﻿using System;
using System.ComponentModel;
using System.Globalization;

using CommandLine.Validation;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    internal class PathTypeConverterStatic
    {
        internal static readonly char[] FileNameTrimCharacters = "\"\x20\t\r\n".ToCharArray();
    }

    public class PathTypeConverter<T> : TypeConverter where T : ValidatorBase
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var path = TrimFileName(value as string);
            Validation.ValidateProperty(path, typeof(T));
            return path;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return TrimFileName(value as string);
        }

        private static string TrimFileName(string val)
        {
            return val == null ? null : val.Trim(PathTypeConverterStatic.FileNameTrimCharacters);
        }
    }
}
