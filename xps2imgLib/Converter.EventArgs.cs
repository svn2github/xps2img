using System;

namespace Xps2ImgLib
{
    public partial class Converter
    {
        public class ProgressEventArgs : EventArgs
        {
            public string FullFileName { get; private set; }
            public State ConverterState { get; private set; }

            public ProgressEventArgs(string fullFileName, State converterState)
            {
                FullFileName = fullFileName;
                ConverterState = converterState;
            }
        }

        public class ErrorEventArgs : EventArgs
        {
            public Exception Exception { get; private set; }

            public ErrorEventArgs(Exception exception)
            {
                Exception = exception;
            }
        }
    }
}
