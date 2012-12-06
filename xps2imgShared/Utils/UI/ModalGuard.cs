namespace Xps2Img.Shared.Utils.UI
{
    public class ModalGuard : DisposableActions
    {
        private static int _counter;

        public static bool IsEntered
        {
            get { return _counter != 0; }
        }

        public ModalGuard()
            : base(() => _counter++, () => { _counter--; global::System.Diagnostics.Debug.Assert(_counter >= 0); })
        {
        }
    }
}
