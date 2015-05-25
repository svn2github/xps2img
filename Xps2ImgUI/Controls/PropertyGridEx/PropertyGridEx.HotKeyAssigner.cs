using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public partial class PropertyGridEx
    {
        private class HotKeyAssigner
        {
            private class CaseInsensitiveCharEqualityComparer : IEqualityComparer<char>
            {
                private readonly CultureInfo _cultureInfo;

                public CaseInsensitiveCharEqualityComparer(CultureInfo cultureInfo)
                {
                    _cultureInfo = cultureInfo;
                }

                public bool Equals(char x, char y)
                {
                    return Char.ToUpper(x, _cultureInfo) == Char.ToUpper(y, _cultureInfo);
                }

                public int GetHashCode(char obj)
                {
                    return _cultureInfo.GetHashCode();
                }
            }

            private readonly CaseInsensitiveCharEqualityComparer _caseInsensitiveCharEqualityComparer;

            public HotKeyAssigner(CultureInfo cultureInfo)
            {
                _caseInsensitiveCharEqualityComparer = new CaseInsensitiveCharEqualityComparer(cultureInfo);
            }

            public HotKeyAssigner()
                : this(CultureInfo.CurrentCulture)
            {
            }

            private const string HotkeyChar = "&";

            public IEnumerable<string> AssignHotKeysFor(IEnumerable<string> texts, params string[] exclude)
            {
                return AssignHotKeysFor(texts, GetHotKeysFor(exclude).ToArray());
            }

            public IEnumerable<string> AssignHotKeysFor(IEnumerable<string> texts, params char[] exclude)
            {
                var excluded = new HashSet<char>(exclude ?? new char[0], _caseInsensitiveCharEqualityComparer);

                return texts.Select(t => TestHotKeyFor(t, excluded) ? t : SetHotKeyFor(t.Replace(HotkeyChar, String.Empty), excluded));
            }

            private static IEnumerable<char> GetHotKeysFor(IEnumerable<string> exclude)
            {
                var list = new List<char>();
                
                foreach (var text in exclude)
                {
                    TestHotKeyFor(text, list);
                }

                return list;
            }

            private static bool TryGetHotKey(string text, out char ch)
            {
                var index = text.IndexOf(HotkeyChar, StringComparison.Ordinal);
                if (index < 0 || index == text.Length - 1)
                {
                    ch = default(char);
                    return false;
                }

                ch = text[index + 1];

                return true;
            }

            private static bool TestHotKeyFor(string text, ICollection<char> excluded)
            {
                char ch;

                if (!TryGetHotKey(text, out ch) || excluded.Contains(ch))
                {
                    return false;
                }

                excluded.Add(ch);

                return true;
            }

            private static string SetHotKeyFor(string text, ICollection<char> excluded)
            {
                for (var ci = 0; ci < text.Length; ci++)
                {
                    var ch = text[ci];

                    if (Char.IsWhiteSpace(ch) || excluded.Contains(ch))
                    {
                        continue;
                    }

                    text = String.Concat(text.Substring(0, ci), HotkeyChar, text.Substring(ci));
                    excluded.Add(ch);

                    break;
                }

                return text;
            }
        }
    }
}
