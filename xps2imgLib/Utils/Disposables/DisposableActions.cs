using System;

namespace Xps2ImgLib.Utils.Disposables
{
    public class DisposableActions : IDisposable
    {
        private readonly Action[] _leaveActions;

        public DisposableActions(params Action[] leaveActions)
        {
            _leaveActions = leaveActions;
        }

        void IDisposable.Dispose()
        {
            for (var i = _leaveActions.Length-1; i >= 0; i--)
            {
                _leaveActions[i]();
            }

            #if DEBUG
            GC.SuppressFinalize(this);
            #endif
        }

        #if DEBUG
        ~DisposableActions()
        {
            System.Diagnostics.Debug.Fail("Use using(new DisposableActions(...))!");
        }
        #endif
    }
}
