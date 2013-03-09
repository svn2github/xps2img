using System;

namespace Xps2Img.CommandLine
{
    public class ConversionException: Exception
    {
        public readonly int ContextData;

        public ConversionException(string message, int contextData, Exception innerException = null) :
            base(message, innerException)
        {
            ContextData = contextData;
        }
    }
}
