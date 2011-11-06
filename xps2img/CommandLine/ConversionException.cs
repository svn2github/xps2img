using System;

namespace Xps2Img.CommandLine
{
    public class ConversionException: Exception
    {
        public readonly CommandLine.ReturnCode ReturnCode;

        public ConversionException(string message, CommandLine.ReturnCode returnCode) :
            base(message)
        {
            ReturnCode = returnCode;
        }
    }
}
