using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class ProcessorsNumberTypeConverter : TypeConverter
    {
        public const int AutoValue = 0;

        private static int[] Processors
        {
            get { return Enumerable.Range(0, Environment.ProcessorCount + 1).ToArray(); }
        }           

        private static bool IsProcessorsCountValid(int processorsNumber)
        {
            return processorsNumber > 0 && processorsNumber <= Environment.ProcessorCount;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;

            int processorsNumber;
            return Validation.IsAutoValue(strValue)
                    ? AutoValue
                    : Int32.TryParse(strValue, out processorsNumber) && IsProcessorsCountValid(processorsNumber)
                        ? processorsNumber
                        : AutoValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is string)
            {
                return value.ToString();
            }

            var processorsNumber = (int)value;

            return IsProcessorsCountValid(processorsNumber)
                    ? value.ToString()
                    : Resources.Strings.Auto;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Processors);
        }
    }
}
