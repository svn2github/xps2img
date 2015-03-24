using System;

namespace Xps2ImgLib
{
    public class ConversionException : Exception
    {
        public ConversionException()
        {
        }

        public ConversionException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
