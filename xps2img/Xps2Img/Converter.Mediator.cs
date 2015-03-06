using System;
using System.Threading;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Xps2Img
{
    public partial class Converter
    {
        //internal class Mediator(Converter converter)
        //public Mediator(Converter converter)
        //{
        private readonly Thread _converterThread;
        private readonly AutoResetEvent _mainEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _converterEvent = new AutoResetEvent(false);
        private Action _currentAction;

        private Exception _exception;

        private void WaitConverter()
        {
            _converterEvent.WaitOne();
            var ex = _exception;
            _exception = null;
            if (ex != null)
            {
                _currentAction = null;
                _mainEvent.Set();
                throw ex;
            }
        }

        private void ConverterThread()
        {
            while (true)
            {
                try
                {
                    _exception = null;
                    if (_currentAction == null)
                    {
                        _xpsDocument.Close();
                        return;
                    }
                    _currentAction();
                    _converterEvent.Set();
                    _mainEvent.WaitOne();
                }
                catch (Exception ex)
                {
                    _exception = ex;
                    _converterEvent.Set();
                    _mainEvent.WaitOne();
                }
            }
        }

        public void Convert(Parameters parameters)
        {
            _exception = null;

            _currentAction = () => ConvertInternal(parameters);
            _mainEvent.Set();

            while (true)
            {
                WaitConverter();

                if (_currentAction == null)
                {
                    break;
                }

                OnProgress.SafeInvoke(this, _progressEventArgs);
                
                _mainEvent.Set();
            }
        }

        private void FireOnProgress(string fileName)
        {
            ConverterState.ActivePageIndex++;

            _progressEventArgs = new ProgressEventArgs(fileName, ConverterState);
            _converterEvent.Set();
            _mainEvent.WaitOne();
        }

        public void Dispose()
        {
            _currentAction = null;
            _mainEvent.Set();
            _converterThread.Join();
            GC.SuppressFinalize(this);
        }
    //}
    }
}