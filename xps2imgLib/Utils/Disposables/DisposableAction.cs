using System;

namespace Xps2ImgLib.Utils.Disposables
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _leaveAction;

        public DisposableAction(Action leaveAction)
        {
            _leaveAction = leaveAction;
        }

        void IDisposable.Dispose()
        {
            _leaveAction();

            #if DEBUG
            GC.SuppressFinalize(this);
            #endif
        }

        #if DEBUG
        ~DisposableAction()
        {
            System.Diagnostics.Debug.Fail("Use using(new DisposableAction(...))!");
        }
        #endif
    }
}
