using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class RichTextBoxEx : RichTextBox
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.Messages.WM_CONTEXTMENU)
            {
                Focus();
                BuildContextMenu();
            }

            base.WndProc(ref m);
        }

        private void BuildContextMenu()
        {
            ContextMenuStrip = ContextMenuStrip ?? new ContextMenuStrip();
            ContextMenuStrip.Items.Clear();

            if (ContextMenuStripItems == null)
            {
                return;
            }

            foreach (var contextMenuStripItem in ContextMenuStripItems)
            {
                var hasEventHandler = contextMenuStripItem.Value != null;
                var isMenuItem = contextMenuStripItem.Key != null;
                var toolStripItem = ContextMenuStrip.Items.Add(isMenuItem ? contextMenuStripItem.Key() : "-", null, hasEventHandler ? (_, __) => contextMenuStripItem.Value() : (EventHandler)null);
                toolStripItem.Enabled = !isMenuItem || hasEventHandler;
            }
        }

        [Browsable(false)]
        public IEnumerable<KeyValuePair<Func<string>, Action>> ContextMenuStripItems { get; set; }
    }
}