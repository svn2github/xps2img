using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    class ToolStripMenuItemEx : ToolStripMenuItem, ILocalizableToolStripItem
    {
        private readonly LocalizableToolStripItem _localizableToolStripItem;

        public ToolStripMenuItemEx(Func<string> textFunc)
        {
            _localizableToolStripItem = new LocalizableToolStripItem(this, textFunc);
        }

        public void RefreshLocalization()
        {
            _localizableToolStripItem.RefreshLocalization();
        }
    }
}
