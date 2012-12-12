using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class ProcessorsNumberTypeConverter : TypeConverter
    {
        public const int AutoValue = 0;

        public static readonly int ProcessorCount = Environment.ProcessorCount;
        public static readonly int[] Processors = EnumProcessors().ToArray();

        private static IEnumerable<int> EnumProcessors()
        {
            yield return AutoValue;
            for (var i = 1; i <= ProcessorCount; i++)
            {
                yield return i;
            }
        }

        private static bool IsProcessorsCountValid(int processorsNumber)
        {
            return processorsNumber > 0 && processorsNumber <= ProcessorCount;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;

            int processorsNumber;
            return String.Compare(strValue, Validation.AutoValue, StringComparison.InvariantCultureIgnoreCase) == 0
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
                    : Validation.AutoValue;
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
