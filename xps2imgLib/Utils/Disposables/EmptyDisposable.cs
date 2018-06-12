using System;

namespace Xps2ImgLib.Utils.Disposables
{
    public sealed class EmptyDisposable : IDisposable
    {
        private EmptyDisposable()
        {
        }

        void IDisposable.Dispose()
        {
        }

        public static EmptyDisposable Default = new EmptyDisposable();
    }
}
