using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class ProcessorsNumberTypeConverter : StandardValuesTypeConverter
    {
        public static readonly int ProcessorCount = Environment.ProcessorCount;
        public static readonly string[] Processors = EnumProcessors().ToArray();

        private static IEnumerable<string> EnumProcessors()
        {
            yield return Validation.AutoValue;
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
