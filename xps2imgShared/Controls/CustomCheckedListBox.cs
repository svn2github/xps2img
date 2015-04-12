using System;
using System.Windows.Forms;

namespace Xps2Img.Shared.Controls
{
    public class CustomCheckedListBox : CheckedListBox
    {
        public bool ExitedByEscape { get; private set; }

        protected override void OnEnter(EventArgs e)
        {
            ExitedByEscape = false;
            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
            base.OnEnter(e);
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            if (key == Keys.Escape)
            {
                ExitedByEscape = true;
            }
            return base.ProcessDialogKey(key);
        }
    }
}
