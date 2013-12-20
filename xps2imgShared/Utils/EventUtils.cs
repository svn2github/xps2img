using System;

namespace Xps2Img.Shared.Utils
{
    public static class EventUtils
    {
        public static void SafeInvoke(this EventHandler eventHandler, object sender)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, EventArgs.Empty);
            }
        }

        public static void SafeInvoke<T>(this EventHandler<T> eventHandler, object sender, T eventArgs) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(sender, eventArgs);
            }
        }
    }
}
