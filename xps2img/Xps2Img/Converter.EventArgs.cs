using System;

namespace Xps2Img.Xps2Img
{
    public partial class Converter
    {
        public class ProgressEventArgs : EventArgs
        {
            public string FullFileName { get; private set; }
            public ConverterState ConverterState { get; private set; }

            public ProgressEventArgs(string fullFileName, ConverterState converterState)
            {
                FullFileName = fullFileName;
                ConverterState = converterState;
            }
        }

        public class ExceptionEventArgs : EventArgs
        {
            public Exception Exception { get; private set; }

            public ExceptionEventArgs(Exception exception)
            {
                Exception = exception;
            }
        }
    }
}
