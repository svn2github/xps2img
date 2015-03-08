using System;
using System.Threading;

namespace Xps2Img.Xps2Img
{
    public partial class Converter
    {
        internal class Mediator : IDisposable
        {
            public Mediator(Action actionCreate)
            {
                _currentAction = actionCreate;

                _converterThread = new Thread(ConverterThread);
                _converterThread.SetApartmentState(ApartmentState.STA);
                _converterThread.Start();

                WaitForWorker();
            }

            private readonly Thread _converterThread;
            private readonly AutoResetEvent _mainEvent = new AutoResetEvent(false);
            private readonly AutoResetEvent _workerEvent = new AutoResetEvent(false);

            private Action _currentAction;
            private Exception _exception;

            private void ConverterThread()
            {
                while (true)
                {
                    try
                    {
                        _exception = null;

                        if (IsStopRequested)
                        {
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

            public void Convert(Action convertAction, Action<ProgressEventArgs> fireProgress, Action<ExceptionEventArgs> fireError)
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
                    FireIfNotNull(_exceptionEventArgs, fireError);
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
            private ExceptionEventArgs _exceptionEventArgs;

            private void SetEventArgs(ProgressEventArgs progressEventArgs, ExceptionEventArgs exceptionEventArgs)
            {
                _exception = null;
                _progressEventArgs = progressEventArgs;
                _exceptionEventArgs = exceptionEventArgs;
            }

            private void SetEventArgs(ProgressEventArgs progressEventArgs)
            {
                SetEventArgs(progressEventArgs, null);
            }

            private void SetEventArgs(ExceptionEventArgs exceptionEventArgs)
            {
                SetEventArgs(null, exceptionEventArgs);
            }

            public void FireOnProgress(ProgressEventArgs args)
            {
                SetEventArgs(args);
                SwitchToMainAndWait();
            }

            public void FireOnError(ExceptionEventArgs args)
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

                _converterThread.Join();

                GC.SuppressFinalize(this);
            }
        }
    }
}