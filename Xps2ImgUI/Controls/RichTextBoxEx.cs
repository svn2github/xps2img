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
                var item = contextMenuStripItem;
                var toolStripItem = ContextMenuStrip.Items.Add(isMenuItem ? contextMenuStripItem.Key() : "-", null, hasEventHandler ? (_, __) => item.Value() : (EventHandler)null);
                toolStripItem.Enabled = !isMenuItem || hasEventHandler;
            }

            if (ToolStripRendererGetter != null)
            {
                ContextMenuStrip.Renderer = ToolStripRendererGetter();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<KeyValuePair<Func<string>, Action>> ContextMenuStripItems { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<ToolStripRenderer> ToolStripRendererGetter { get; set; }
    }
}