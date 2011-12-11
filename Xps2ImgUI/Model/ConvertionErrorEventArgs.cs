using System;

namespace Xps2ImgUI.Model
{
    public class ConvertionErrorEventArgs: EventArgs
    {
        public readonly string Message;

        public ConvertionErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}
