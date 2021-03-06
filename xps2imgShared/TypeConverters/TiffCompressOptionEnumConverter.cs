﻿using System.ComponentModel;
using System.Globalization;

using Xps2ImgLib;

namespace Xps2Img.Shared.TypeConverters
{
    public class TiffCompressOptionEnumConverter : FilterableEnumConverter<TiffCompressOption>
    {
        protected override bool IsValueVisible(TiffCompressOption value)
        {
            return value != TiffCompressOption.Default;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var converted = base.ConvertFrom(context, culture, value);
            return converted == null || !IsValueVisible((TiffCompressOption) converted)
                     ? TiffCompressOption.Zip
                     : converted;
        }
    }
}
