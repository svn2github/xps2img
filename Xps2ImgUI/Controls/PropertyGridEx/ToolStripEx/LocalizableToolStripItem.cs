using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx
{
    public class LocalizableToolStripItem : ILocalizableToolStripItem
    {
        private readonly Func<string> _textFunc;
        private readonly ToolStripItem _toolStripItem;
        private readonly bool _updateText;

        public LocalizableToolStripItem(ToolStripItem toolStripItem, Func<string> textFunc, bool updateText = true)
        {
            Debug.Assert(toolStripItem != null);
            Debug.Assert(textFunc != null);
            
            _toolStripItem = toolStripItem;
            _textFunc = textFunc;
            _updateText = updateText;

            RefreshLocalization();
        }

        public void RefreshLocalization()
        {
            if (_textFunc == null)
            {
                return;
            }

            if(_updateText)
            {
                _toolStripItem.Text = _textFunc();
            }
            else
            {
                _toolStripItem.ToolTipText = _textFunc();
            }
        }
    }
}
