using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class ToolStripButtonItem
    {
        public readonly string Text;
        public readonly EventHandler EventHandler;

        public ToolStripItem ToolStripItem { get; set; }

        public bool IsSeparator { get { return Text == null; } }

        public ToolStripButtonItem()
        {
        }

        public ToolStripButtonItem(string text, EventHandler eventHandler)
        {
            Text = text;
            EventHandler = eventHandler;
        }
    }
}