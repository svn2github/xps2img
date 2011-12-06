using System;

namespace Xps2ImgUI.Utils.UI
{
    public class ModalGuard : IDisposable
    {
        private static int _counter;

        public static bool IsEntered
        {
            get { return _counter != 0; }
        }

        public ModalGuard()
        {
            _counter++;
        }

        void IDisposable.Dispose()
        {
            _counter--;
            System.Diagnostics.Debug.Assert(_counter >= 0);
        }
    }
}
