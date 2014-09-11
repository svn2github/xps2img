using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    public class ToolStripButtonEx : ToolStripButton, ILocalizableToolStripItem
    {
        private readonly LocalizableToolStripItem _localizableToolStripItem;

        public ToolStripButtonEx(Func<string> textFunc)
        {
            _localizableToolStripItem = new LocalizableToolStripItem(this, textFunc);
        }

        public void RefreshLocalization()
        {
            _localizableToolStripItem.RefreshLocalization();
        }
    }
}
