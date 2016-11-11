using System;
using System.Threading;
using System.Windows.Threading;

namespace Xps2ImgLib
{
    public partial class Converter
    {
        private class MediatorThread : IMediator
        {
            private readonly AutoResetEvent _mainEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _workerEvent = new AutoResetEvent(false);

            private Action _actionDispose;

            private Thread _converterThread;

            private Action _currentAction;
            private Exception _exception;

            public void Init(Action actionCreate, Action actionDispose)
            {
                _actionDispose = actionDispose;

                _currentAction = actionCreate;

                _converterThread = new Thread(WorkerThread);
                _converterThread.SetApartmentState(ApartmentState.STA);
                _converterThread.Start();

                WaitForWorker();
            }

            private void WorkerThread()
            {
                while (true)
                {
                    try
                    {
                        _exception = null;

                        if (IsStopRequested)
                        {
                            _actionDispose();
                            Dispatcher.CurrentDispatcher.InvokeShutdown();
                            return;
                        }

                        _currentAction();
                        SwitchToMainAndWait();
                    }
                    catch (Exception ex)
                    {
                        _exception = ex;
                        SwitchToMainAndWait();
                    }
                }
            }

            private void WaitForWorker()
            {
                _workerEvent.WaitOne();

                var ex = _exception;
                _exception = null;

                if (ex == null)
                {
                    return;
                }

                RequestStop();
                SwitchToWorker();

                throw ex;
            }

            private void SwitchToWorker()
            {
                _mainEvent.Set();              
            }

            private void SwitchToMain()
            {
                _workerEvent.Set();
            }

            private void SwitchToMainAndWait()
            {
                SwitchToMain();
                _mainEvent.WaitOne();
            }

            public void Convert(Action convertAction, Action<ProgressEventArgs> fireProgress, Action<ErrorEventArgs> fireError)
            {
                _exception = null;
                _currentAction = convertAction;

                while (true)
                {
                    SwitchToWorker();
                    WaitForWorker();

                    if (IsStopRequested)
                    {
                        break;
                    }

                    FireIfNotNull(_progressEventArgs, fireProgress);
                    FireIfNotNull(_errorEventArgs, fireError);
                }
            }

            public void RequestStop()
            {
                _currentAction = null;
            }

            private bool IsStopRequested
            {
                get { return _currentAction == null; }
            }

            private ProgressEventArgs _progressEventArgs;
            private ErrorEventArgs _errorEventArgs;

            private void SetEventArgs(ProgressEventArgs progressEventArgs, ErrorEventArgs errorEventArgs)
            {
                _exception = null;
                _progressEventArgs = progressEventArgs;
                _errorEventArgs = errorEventArgs;
            }

            private void SetEventArgs(ProgressEventArgs progressEventArgs)
            {
                SetEventArgs(progressEventArgs, null);
            }

            private void SetEventArgs(ErrorEventArgs errorEventArgs)
            {
                SetEventArgs(null, errorEventArgs);
            }

            public void FireOnProgress(ProgressEventArgs args)
            {
                SetEventArgs(args);
                SwitchToMainAndWait();
            }

            public void FireOnError(ErrorEventArgs args)
            {
                SetEventArgs(args);
                SwitchToMainAndWait();
            }

            private static void FireIfNotNull<T>(T args, Action<T> fireAction) where T : EventArgs
            {
                if (args != null)
                {
                    fireAction(args);
                }
            }

            public void Dispose()
            {
                RequestStop();
                SwitchToWorker();

                try
                {
                    _converterThread.Join();
                }
                catch
                {
                    // ignored
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}