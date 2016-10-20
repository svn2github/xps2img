using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    public class LocalizableToolStripItem : ILocalizableToolStripItem
    {
        private readonly ToolStripItem _toolStripItem;

        private readonly Func<string> _updateText;
        private readonly Func<string> _updateToolTipText;
        private readonly Func<Image>  _updateImage;

        public LocalizableToolStripItem(ToolStripItem toolStripItem, Func<string> updateText = null, Func<string> updateToolTipText = null, Func<Image> updateImage = null)
        {
            Debug.Assert(toolStripItem != null);
            
            _toolStripItem = toolStripItem;
            _updateText = updateText;
            _updateToolTipText = updateToolTipText;
            _updateImage = updateImage;

            RefreshLocalization();
        }

        public void RefreshLocalization()
        {
            if (_updateImage != null)
            {
                _toolStripItem.Image = _updateImage();
            }

            if (_updateText != null)
            {
                _toolStripItem.Text = _updateText();
            }

            if (_updateToolTipText != null)
            {
                _toolStripItem.ToolTipText = _updateToolTipText();
            }
        }
    }
}
