using System.ComponentModel;

namespace Xps2ImgUI.Converters
{
    public class ProcessPriorityClassConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Processors);
        }

        private static readonly string[] Processors =
        {
            Xps2Img.CommandLine.Options.AutoValue,
            "Idle",
            "BelowNormal",
            "Normal",
            "AboveNormal",
            "High"
        };
    }
}
