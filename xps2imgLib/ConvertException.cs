using System;

namespace Xps2ImgLib
{
    public class ConvertException: Exception
    {
        public int Page { get; private set; }

        public ConvertException(string message, int page, Exception innerException = null) :
            base(message, innerException)
        {
            Page = page;
        }
    }
}
