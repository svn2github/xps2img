using System.Threading;

using Xps2ImgLib.Utils.Disposables;

namespace Xps2Img.Shared.Utils.UI
{
    public class ModalGuard : EnterLeaveDisposableActions
    {
        private static int _counter;

        public static bool IsEntered
        {
            get { return _counter != 0; }
        }

        public ModalGuard()
            : base(() => Interlocked.Increment(ref _counter), () => { Interlocked.Decrement(ref _counter); global::System.Diagnostics.Debug.Assert(_counter >= 0); })
        {
        }
    }
}
