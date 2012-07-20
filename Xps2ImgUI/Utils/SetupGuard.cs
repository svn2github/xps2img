using System.Threading;

namespace Xps2ImgUI.Utils
{
    public class SetupGuard
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
