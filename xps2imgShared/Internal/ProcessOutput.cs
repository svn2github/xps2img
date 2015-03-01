using System;
using System.Text;

namespace Xps2Img.Shared.Internal
{
    public static class ProcessOutput
    {
        public static string Encode(string str, bool encode)
        {
            return encode
                     ? Convert.ToBase64String(Encoding.UTF8.GetBytes(str))
                     : str;
        }

        public static string Decode(string str)
        {
            str = str.Trim();

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(str));
            }
            catch
            {
                return str;
            }
        }
    }
}
