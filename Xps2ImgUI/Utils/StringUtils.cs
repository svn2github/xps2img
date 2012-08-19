using System;
using System.Linq;
using System.Text;

namespace Xps2ImgUI.Utils
{
    public static class StringUtils
    {
        public static string TabsToSpaces(this string srcString, int tabSize)
        {
            var strings = srcString.Split('\n');
            var stringBuilder = new StringBuilder(srcString.Length + strings.Length * 8);

            foreach (var parts in strings.Select(str => str.Split('\t')))
            {
                foreach (var part in parts)
                {
                    stringBuilder.Append(part.PadRight(tabSize * (part.Length / tabSize + 1)));
                }
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        public static string AppendDot(this string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return String.Empty;
            }

            message = message.TrimEnd();

            return ",.?!-:*&`~$\'\")]".Contains(message.Last()) ? message : message + ".";
        }

        public static string RemoveSpaces(this string str)
        {
            return str.Replace(SpaceString, String.Empty);
        }

        public const char   SpaceChar   = '\x20';
        public const string SpaceString = "\x20";
    }
}
