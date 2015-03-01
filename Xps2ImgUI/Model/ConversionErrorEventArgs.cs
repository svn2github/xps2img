using System;
using System.Globalization;

namespace Xps2ImgUI.Model
{
    public class ConversionErrorEventArgs: EventArgs
    {
        public string Message { get; private set; }
        public int? Page { get; private set; }

        public ConversionErrorEventArgs(string message, string pageText = null)
        {
            Message = message;

            int page;
            if (!String.IsNullOrEmpty(pageText) && int.TryParse(pageText, NumberStyles.Integer, CultureInfo.InvariantCulture, out page) && page > 0)
            {
                Page = page;
            }
        }
    }
}
