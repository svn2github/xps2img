using System;

namespace Xps2Img.CommandLine
{
    public class ConversionException: Exception
    {
        public readonly int ReturnCode;

        public ConversionException(string message, int returnCode) :
            base(message)
        {
            ReturnCode = returnCode;
        }
    }
}
