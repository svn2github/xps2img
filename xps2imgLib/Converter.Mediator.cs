using System;

namespace Xps2ImgLib
{
    public partial class Converter
    {
        private class Mediator : IMediator
        {
            private Action _actionDispose;

            private Action<ProgressEventArgs> _fireProgress;
            private Action<ErrorEventArgs> _fireError;

            public void Init(Action actionCreate, Action actionDispose)
            {
                _actionDispose = actionDispose;

                actionCreate();
            }

            public void Convert(Action convertAction, Action<ProgressEventArgs> fireProgress, Action<ErrorEventArgs> fireError)
            {
                _fireProgress = fireProgress;
                _fireError = fireError;

                convertAction();
            }

            public void RequestStop()
            {
            }

            public void FireOnProgress(ProgressEventArgs args)
            {
                _fireProgress(args);
            }

            public void FireOnError(ErrorEventArgs args)
            {
                _fireError(args);
            }

            public void Dispose()
            {
                _actionDispose();

                GC.SuppressFinalize(this);
            }
        }
    }
}