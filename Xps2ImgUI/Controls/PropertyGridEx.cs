using System;
using System.Linq;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class PropertyGridEx : PropertyGrid
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            _toolStrip = Controls.OfType<ToolStrip>().FirstOrDefault();
        }

        private ToolStrip _toolStrip;

        public void RemoveLastToolStripButton()
        {
            if (_toolStrip.Items.Count > 0)
            {
                _toolStrip.Items.RemoveAt(_toolStrip.Items.Count - 1);
            }
        }

        public ToolStripButton AddToolStripButton(string text, EventHandler eventHandler)
        {
            var toolStripButton = new ToolStripButton(text);
            toolStripButton.Click += eventHandler;
            _toolStrip.Items.Add(toolStripButton);
            return toolStripButton;
        }

        public void AddToolStripSeparator()
        {
            _toolStrip.Items.Add(new ToolStripSeparator());
        }
    }
}
