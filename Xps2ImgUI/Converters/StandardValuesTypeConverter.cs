using System.ComponentModel;

namespace Xps2ImgUI.Converters
{
    public abstract class StandardValuesTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Values);
        }

        public abstract string[] Values { get; }
    }
}
