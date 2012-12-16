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
            var intPtrValue = value as IntPtr?;

            if (!intPtrValue.HasValue || intPtrValue.Value == IntPtr.Zero)
            {
                return Validation.AutoValue;
            }

            var intervalString = String.Empty;
            ForEachBit(intPtrValue.Value, p => intervalString += (String.IsNullOrEmpty(intervalString) ? String.Empty : ",") + p);

            return IntervalUtils.ToString(Interval.Parse(intervalString));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = value as string;

            if (String.IsNullOrEmpty(strValue) || Validation.IsAutoValue(strValue))
            {
                return null;
            }

            Action<Func<string, bool>> validateProperty = p => Validation.ValidateProperty(strValue, Validation.CpuAffinityValidationExpression, p);

            validateProperty(null);

            var bitArray = Interval.Parse(strValue).ToBitArray();

            validateProperty(_ => bitArray.Length <= Environment.ProcessorCount);

            var bitIndex = 0;
            var affinityMask = bitArray.Cast<bool>().TakeWhile(_ => bitIndex < 64).Aggregate(0L, (_, bit) => _ | ((bit ? 1L : 0L) << bitIndex++));

            return new IntPtr(affinityMask & (1 << Environment.ProcessorCount) - 1);
        }

        public static void ForEachBit(IntPtr intPtrValue, Action<int> hasBitInPosition)
        {
            var bits = (ulong)intPtrValue.ToInt64();

            var index = 0;

            do
            {
                if ((bits & 1L) != 0)
                {
                    hasBitInPosition(index);
                }

                index++;
            } while ((bits >>= 1) != 0);
        }
    }
}
