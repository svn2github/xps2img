using System;

namespace Xps2ImgUI.Model
{
    public class ConversionErrorEventArgs: EventArgs
    {
        public readonly string Message;

        public ConversionErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}
