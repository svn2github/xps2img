﻿using System.ComponentModel;

namespace Xps2ImgUI.Converters
{
    public class DpiTypeConverter : NullableIntTypeConverter
    {
        private static readonly int[] Dpis = new[] { 72, 96, 120, 150, 300, 600, 900, 1200, 1600, 1800, 2350 };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Dpis);
        }
    }
}