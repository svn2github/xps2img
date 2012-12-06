using System;
using System.ComponentModel;
using System.Globalization;

namespace Xps2Img.Shared.TypeConverters
{
    public class TrimStringTypeConverter : TypeConverter
    {
        private static readonly char[] FileNameTrimCharacters = "\"\x20\t\r\n".ToCharArray();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return TrimFileName(value as string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return TrimFileName(value as string);
        }

        private static string TrimFileName(string val)
        {
            return val == null ? null : val.Trim(FileNameTrimCharacters);
        }
    }
}
