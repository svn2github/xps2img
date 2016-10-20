using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    public class ToolStripSplitButtonEx : ToolStripSplitButton, ILocalizableToolStripItem
    {
        private readonly LocalizableToolStripItem _localizableToolStripItem;

        public ToolStripSplitButtonEx(Func<string> updateText = null, Func<string> updateToolTipText = null, Func<Image> updateImage = null)
        {
            _localizableToolStripItem = new LocalizableToolStripItem(this, updateText, updateToolTipText, updateImage);
        }

        public void RefreshLocalization()
        {
            _localizableToolStripItem.RefreshLocalization();
        }
    }
}
