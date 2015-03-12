using System;

namespace Xps2ImgLib.Utils
{
    public static class FormattingUtils
    {
        public static string GetNumberFormat(this int position, double count, bool padZeros)
        {
            return String.Format(padZeros ? "{{{0}:D{1}}}" : "{{{0},{1}}}", position, (int)(Math.Log10(count) + 1));
        }

        public static string GetNumberFormat(this double count)
        {
            return GetNumberFormat(0, count, true);
        }

        public static string GetNumberFormat(this int count)
        {
            return GetNumberFormat((double)count);
        }
    }
}
