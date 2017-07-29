using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.Data
{
    public class MenuItemDescriptor
    {
        public Func<string> Title { get; private set; }
        public EventHandler Command { get; private set; }

        public Func<bool> CheckedGetter { get; private set; }
        public Action<bool> CheckedSetter { get; private set; }

        public MenuItemDescriptor(Func<string> title = null, Action command = null, Func<bool> checkedGetter = null, Action<bool> checkedSetter = null)
        {
            Title = title;
            Command = command != null ? new EventHandler((_, __) => command()) : null;
            CheckedGetter = checkedGetter;
            CheckedSetter = checkedSetter;
        }

        public bool IsSeparator
        {
            get { return Title == null; }
        }

        public bool Checkable
        {
            get { return CheckedGetter != null; }
        }

        public bool HasCommand
        {
            get { return Command != null; }
        }

        public bool Enabled
        {
            get { return IsSeparator || HasCommand || CheckedSetter != null; }
        }

        public void SetupChecked(ToolStripItem toolStripItem)
        {
            if (!Checkable)
            {
                return;
            }

            var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
            Debug.Assert(toolStripMenuItem != null, "ToolStripMenuItem type expected");

            toolStripMenuItem.CheckOnClick = true;

            var contextMenuStrip = toolStripMenuItem.Owner as ContextMenuStrip;
            if (contextMenuStrip != null)
            {
                contextMenuStrip.Opening += (_, __) => toolStripMenuItem.Checked = CheckedGetter();
            }
            else
            {
                var toolStripDropDown = toolStripMenuItem.Owner as ToolStripDropDown;
                if (toolStripDropDown != null)
                {
                    toolStripDropDown.Opening += (_, __) => toolStripMenuItem.Checked = CheckedGetter();
                }
                else
                {
                    Debug.Fail("ToolStripMenuItem owner should be either ContextMenuStrip or ToolStripDropDown");
                }
            }

            toolStripMenuItem.CheckedChanged += (_, __) => CheckedSetter(toolStripMenuItem.Checked);
        }

    }
}