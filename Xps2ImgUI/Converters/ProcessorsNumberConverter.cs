using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Xps2ImgUI.Converters
{
    public class ProcessorsNumberConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Processors);
        }

        public  static readonly int ProcessorCount = Environment.ProcessorCount;

        private static readonly string[] Processors = EnumProcessors().ToArray();

        private static IEnumerable<string> EnumProcessors()
        {
            yield return Xps2Img.CommandLine.Options.AutoValue;
            for (var i = 1; i <= ProcessorCount; i++)
            {
                yield return i.ToString(CultureInfo.InvariantCulture);        
            }
        }
    }
}
