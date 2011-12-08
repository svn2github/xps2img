using System;

namespace Xps2ImgUI.Utils
{
    public class DisposableActions : IDisposable
    {
        private readonly Action _leaveAction;

        public DisposableActions(Action leaveAction)
            : this(null, leaveAction)
        {
        }

        public DisposableActions(Action enterAction, Action leaveAction)
        {
            if (enterAction != null)
            {
                enterAction();
            }

            _leaveAction = leaveAction;
        }

        void IDisposable.Dispose()
        {
            if(_leaveAction != null)
            {
                _leaveAction();
            }
        }
    }
}
