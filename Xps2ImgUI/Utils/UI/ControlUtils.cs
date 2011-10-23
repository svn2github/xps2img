using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public static class ControlUtils
    {
        public static void Invoke(this Control control, Action action)
        {
            control.BeginInvoke(action);
        }

        public static void InvokeIfNeeded(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

    }
}
