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
            message = message.TrimEnd();
            return message.EndsWith(".") ? message : message + ".";
        }
    }
}
