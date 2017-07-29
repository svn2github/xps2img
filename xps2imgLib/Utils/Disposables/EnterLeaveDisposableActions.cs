using System;

namespace Xps2ImgLib.Utils.Disposables
{
    public class EnterLeaveDisposableActions : IDisposable
    {
        private readonly Action _leaveAction;

        public EnterLeaveDisposableActions(Action enterAction, Action leaveAction)
        {
            enterAction();

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
        ~EnterLeaveDisposableActions()
        {
            System.Diagnostics.Debug.Fail("Use using(new EnterLeaveDisposableActions(...))!");
        }
        #endif
    }
}
