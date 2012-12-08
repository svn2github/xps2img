using System;

namespace Xps2Img.Shared.Utils
{
    public static class EnumUtils
    {
        public static bool HasValue<T>(string str)
        {
            T val;
            return TryParse(str, out val);
        }

        public static bool TryParse<T>(string str, out T parsed)
        {
            try
            {
                parsed = (T)Enum.Parse(typeof(T), str, true);
                return true;
            }
            catch
            {
                parsed = default(T);
                return false;
            }
        }
    }
}
