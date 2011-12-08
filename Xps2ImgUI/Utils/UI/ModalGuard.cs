namespace Xps2ImgUI.Utils.UI
{
    public class ModalGuard : DisposableActions
    {
        private static int _counter;

        public static bool IsEntered
        {
            get { return _counter != 0; }
        }

        public ModalGuard()
            : base(() => _counter++, () => { _counter--; System.Diagnostics.Debug.Assert(_counter >= 0); })
        {
        }
    }
}
