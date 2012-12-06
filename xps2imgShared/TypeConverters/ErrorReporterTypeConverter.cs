using System;
using System.ComponentModel;
using System.Globalization;

namespace Xps2Img.Shared.TypeConverters
{
    public class ErrorReporterTypeConverter<T> : TypeConverter
    {
        private readonly string _message;
        private readonly TypeConverter _typeConverter;
        
        public ErrorReporterTypeConverter(string message)
        {
            _message = message;
            _typeConverter = TypeDescriptor.GetConverter(typeof (T));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return _typeConverter.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                return _typeConverter.ConvertTo(context, culture, value, destinationType);
            }
            catch
            {               
                throw new Exception(_message);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            try
            {
                return _typeConverter.ConvertFrom(context, culture, value);
            }
            catch
            {
                throw new Exception(_message);
            }
        }
    }
}
