using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Xps2Img.Shared.TypeConverters
{
    public class ProcessPriorityClassTypeConverter : OptionsEnumConverter<ProcessPriorityClass>
    {
        public const ProcessPriorityClass Auto = 0;

        private readonly object[] _allowedValues =
        {
            (int)Auto,
            ProcessPriorityClass.Idle,
            ProcessPriorityClass.BelowNormal,
            ProcessPriorityClass.Normal,
            ProcessPriorityClass.AboveNormal,
            ProcessPriorityClass.High
        };

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(_allowedValues);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value != null && (ProcessPriorityClass)value == Auto
                    ? Resources.Strings.Auto
                    : base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return String.CompareOrdinal(Resources.Strings.Auto, value as string) == 0
                    ? Auto
                    : base.ConvertFrom(context, culture, value);
        }
    }
}
