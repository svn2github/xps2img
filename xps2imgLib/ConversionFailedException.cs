using System;

namespace Xps2ImgLib
{
    public class ConversionFailedException : ConversionException
    {
        public int Page { get; private set; }

        public ConversionFailedException(string message, int page, Exception innerException = null) :
            base(message, innerException)
        {
            Page = page;
        }
    }
}
