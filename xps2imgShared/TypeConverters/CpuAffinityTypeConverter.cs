using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CpuAffinityTypeConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var strValue = value as string;
            Validation.ValidateProperty(strValue, Validation.CpuAffinityValidationExpression);
            return String.IsNullOrEmpty(strValue) || String.Compare(strValue, Validation.AutoValue, StringComparison.InvariantCultureIgnoreCase) == 0
                                ? Validation.AutoValue
                                : IntervalUtils.ToString(Interval.Parse(strValue));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;

            if (String.IsNullOrEmpty(strValue) || String.Compare(strValue, Validation.AutoValue, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return IntPtr.Zero;
            }

            var bitArray = Interval.Parse(strValue).ToBitArray();

            var bitIndex = 0;
            var affinityMask = bitArray.Cast<bool>().TakeWhile(_ => bitIndex < 64).Aggregate(0L, (_, bit) => _ | ((bit ? 1L : 0L) << bitIndex++));

            return new IntPtr(affinityMask & ((1 << Environment.ProcessorCount) - 1));
        }
    }
}
