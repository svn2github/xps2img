using System;
using System.Text;

namespace Xps2Img.Shared.Internal
{
    public static class ProcessOutput
    {
        public static string Encode(string str, bool encode)
        {
            return encode ? Convert.ToBase64String(Encoding.UTF8.GetBytes(str)) : str;
        }

        public static bool TryDecode(string str, out string decodedStr)
        {
            decodedStr = str.Trim();

            try
            {
                decodedStr = Encoding.UTF8.GetString(Convert.FromBase64String(decodedStr));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
