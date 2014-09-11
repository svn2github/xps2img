using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class ToolStripButtonItem
    {
        private const string Separator = "-";

        public readonly Func<string> TextFunc;
        public readonly EventHandler EventHandler;

        public ToolStripItem ToolStripItem { get; set; }

        public string Text
        {
            get { return TextFunc != null ? TextFunc() : Separator; }
        }

        public bool IsSeparator { get { return Text == Separator; } }

        public ToolStripButtonItem()
        {
        }

        public ToolStripButtonItem(Func<string> textFunc, EventHandler eventHandler)
        {
            TextFunc = textFunc;
            EventHandler = eventHandler;
        }
    }
}