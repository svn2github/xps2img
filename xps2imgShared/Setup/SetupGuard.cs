using System.Threading;

namespace Xps2Img.Shared.Setup
{
    public static class SetupGuard
    {
        private static Mutex _mutex;

        public static void Enter()
        {
            if (_mutex == null)
            {
                _mutex = new Mutex(true, "Xps2ImgInnoSetupGuard");
            }
        }

        public static void Leave()
        {
            if (_mutex == null)
            {
                return;
            }

            _mutex.Close();
            _mutex = null;
        }
    }
}
