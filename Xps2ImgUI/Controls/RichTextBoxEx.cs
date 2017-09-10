using System;
using System.ComponentModel;
using System.Linq;
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
                ApplyContextMenu();
            }

            base.WndProc(ref m);
        }

        private void ApplyContextMenu()
        {
            if (ContextMenuStrip == null)
            {
                ContextMenuStrip = new ContextMenuStrip();
            }

            if (ContextMenuStripGetter != null)
            {
                ContextMenuStrip.Items.Clear();
                ContextMenuStrip.Items.AddRange(ContextMenuStripGetter().Items.Cast<ToolStripItem>().ToArray());
            }

            if (ToolStripRendererGetter != null)
            {
                ContextMenuStrip.Renderer = ToolStripRendererGetter();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<ToolStripRenderer> ToolStripRendererGetter { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<ContextMenuStrip> ContextMenuStripGetter { get; set; }
    }
}
