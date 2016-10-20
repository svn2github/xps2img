using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    class ToolStripMenuItemEx : ToolStripMenuItem, ILocalizableToolStripItem
    {
        private readonly LocalizableToolStripItem _localizableToolStripItem;

        public ToolStripMenuItemEx(Func<string> updateText, Func<string> updateToolTipText = null, Func<Image> updateImage = null)
        {
            _localizableToolStripItem = new LocalizableToolStripItem(this, updateText, updateToolTipText, updateImage);
        }

        public void RefreshLocalization()
        {
            _localizableToolStripItem.RefreshLocalization();
        }
    }
}
