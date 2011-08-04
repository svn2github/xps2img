using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public struct WaitCursor : IDisposable
    {
        // ReSharper disable UnusedParameter.Local
        private WaitCursor(int dummy)
        {
        }
        // ReSharper restore UnusedParameter.Local

        public static void Set()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        public static void Reset()
        {
            Cursor.Current = Cursors.Default;
        }

        public static WaitCursor Create()
        {
            Set();
            return new WaitCursor(0);
        }

        public void Dispose()
        {
            Reset();
        }
    }

    public static class WaitCursorEventHelper
    {
        public static void Fire<T>(this EventHandler<T> eventHandler, object sender, T eventArgs) where T : EventArgs
        {
            using (WaitCursor.Create())
            {
                if (eventHandler != null)
                {
                    eventHandler(sender, eventArgs);
                }
            }
        }

        public static void Fire(this EventHandler<EventArgs> eventHandler, object sender)
        {
            Fire(eventHandler, sender, EventArgs.Empty);
        }
    }
}
