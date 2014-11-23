using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    public class ToolStripSplitButtonEx : ToolStripSplitButton, ILocalizableToolStripItem
    {
        private readonly LocalizableToolStripItem _localizableToolStripItem;

        public ToolStripSplitButtonEx(Func<string> textFunc)
        {
            _localizableToolStripItem = new LocalizableToolStripItem(this, textFunc, false);
        }

        public void RefreshLocalization()
        {
            _localizableToolStripItem.RefreshLocalization();
        }
    }
}
