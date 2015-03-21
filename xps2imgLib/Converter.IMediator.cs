using System;

namespace Xps2ImgLib
{
    public partial class Converter
    {
        private interface IMediator : IDisposable
        {
            void Init(Action actionCreate, Action actionDispose);

            void Convert(Action convertAction, Action<ProgressEventArgs> fireProgress, Action<ErrorEventArgs> fireError);
            void RequestStop();

            void FireOnProgress(ProgressEventArgs args);
            void FireOnError(ErrorEventArgs args);
        }
    }
}