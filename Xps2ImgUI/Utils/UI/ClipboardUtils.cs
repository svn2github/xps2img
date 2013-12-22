using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public static class ClipboardUtils
    {
        public static void CopyToClipboard(Func<string> messageFunc)
        {
            CopyToClipboard(messageFunc());
        }

        public static void CopyToClipboard(string str, int tries = 2)
        {
            if (String.IsNullOrEmpty(str))
            {
                return;
            }

            if(tries < 1)
            {
                tries = 2;
            }

            while (tries-- != 0)
            {
                try
                {
                    Clipboard.SetDataObject(str, true, 2, 100);
                    break;
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }
        }
    }
}
