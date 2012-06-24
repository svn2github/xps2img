using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Xps2ImgUI.Converters
{
    public class ProcessorsNumberConverter : StandardValuesTypeConverter
    {
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

        public override string[] Values
        {
            get { return Processors; }
        }
    }
}
