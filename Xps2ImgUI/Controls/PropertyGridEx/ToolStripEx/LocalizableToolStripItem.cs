using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    public class LocalizableToolStripItem : ILocalizableToolStripItem
    {
        private readonly Func<string> _textFunc;
        private readonly ToolStripItem _toolStripItem;

        public LocalizableToolStripItem(ToolStripItem toolStripItem, Func<string> textFunc)
        {
            Debug.Assert(toolStripItem != null);
            Debug.Assert(textFunc != null);
            
            _toolStripItem = toolStripItem;
            _textFunc = textFunc;

            toolStripItem.Text = textFunc();
        }

        public void RefreshLocalization()
        {
            if(_textFunc != null)
            {
                _toolStripItem.Text = _textFunc();
            }
        }
    }
}
