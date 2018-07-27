using System.Threading;

namespace Xps2Img.Shared.Utils
{
    public static class InterlockedUtils
    {
        private const int Undefined = -1;
        private const int False     = 0;
        private const int True      = 1;

        public static void Set(ref int value, bool set)
        {
            GetAndSet(ref value, set);
        }

        public static bool Get(ref int value)
        {
            return Interlocked.CompareExchange(ref value, Undefined, Undefined) == True;
        }

        public static bool GetAndSet(ref int value, bool set)
        {
            return Interlocked.Exchange(ref value, set ? True : False) == True;
        }
    }
}
