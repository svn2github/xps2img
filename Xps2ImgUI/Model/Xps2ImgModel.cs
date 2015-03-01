using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;

using Xps2ImgUI.Attributes.OptionsHolder;

namespace Xps2ImgUI.Model
{
    public partial class Xps2ImgModel
    {
        public Xps2ImgModel()
            : this(null)
        {
        }

        public Xps2ImgModel(UIOptions options)
        {
            InitOptionsHolder();

            _optionsHolder.OptionsObject = options ?? new UIOptions();

            if (OptionsObject.ImageName == String.Empty)
            {
                OptionsObject.ImageName = Options.Names.Empty;
            }
        }

        public void Init()
        {
            var initOptions = _optionsHolder != null ? _optionsHolder.OptionsObject : new UIOptions();

            InitOptionsHolder();

            Debug.Assert(_optionsHolder != null);

            _optionsHolder.OptionsObject = initOptions;
        }

        public void Reset()
        {
            InitOptionsHolder();
            _optionsHolder.OptionsObject = new UIOptions();
        }

        public void Launch(ConversionType conversionType)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException(Resources.Strings.UnexpectedConversionIsInProgress);  
            }

            LaunchInternal(conversionType);
        }

        public void CancelShutdownRequest()
        {
            _userCancelled = true;
        }

        public void Cancel()
        {
            CancelShutdownRequest();
            Stop();
        }

        private void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            try
            {
                CancelEvent.Set();
            }
            catch(InvalidOperationException)
            {
            }

            _isRunning = false;
        }

        public string FormatCommandLine(string[] optionsToExclude, bool includeFiltered = false)
        {
            return _optionsHolder.FormatCommandLine(false, includeFiltered, optionsToExclude);
        }

        public void FireOptionsObjectChanged()
        {
            OptionsObjectChanged.SafeInvoke(this);
        }

        public UIOptions OptionsObject
        {
            get { return _optionsHolder.OptionsObject; }
        }

        public bool IsBatchMode
        {
            get { return OptionsObject.Batch; }
        }

        public bool IsUserMode
        {
            get { return !IsBatchMode; }
        }

        public string SrcFile
        {
            get { return OptionsObject.SrcFile; }
            set { OptionsObject.SrcFile = value; }
        }

        public string FirstRequiredPropertyName
        {
            get { return _optionsHolder.FirstRequiredPropertyName; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        private bool IsProgressStarted
        {
            get { return _progressStarted; }
        }

        public bool IsStopPending
        {
            get { return CancelEvent.WaitOne(0); }
        }

        public bool IsDeleteMode
        {
            get { return _conversionType == ConversionType.Delete; }
        }

        public bool CanResume
        {
            get
            {
                return _processLastConvertedPage != null && _processedIntervals != null;
            }
            set
            {
                if (value)
                {
                    return;
                }

                _processLastConvertedPage = null;
                _processedIntervals = null;
            }
        }

        public PostAction ShutdownType
        {
            get { return OptionsObject.PostAction; }
        }

        public bool ShutdownRequested
        {
            get
            {
                return ShutdownType != PostAction.DoNothing
                       && !IsDeleteMode
                       && (IsBatchMode || (IsProgressStarted && !_userCancelled));
            }
        }

        public int PagesProcessedTotal
        {
            get { return _pagesProcessedDelta + _pagesProcessed; }
        }

        public int PagesProcessed
        {
            get { return _pagesProcessed; }
        }

        private int _pagesTotal;

        public int PagesTotal
        {
            get { return _pagesTotal; }
            private set
            {
                _pagesTotal = value > 0 ? value : 1;
                _errorPages = new BitArray(_pagesTotal + 1);
            }
        }

        private BitArray _errorPages;

        public List<Interval> ErrorPages
        {
            get { return IntervalUtils.FromBitArray(_errorPages); }
        }

        private const int ExitOK = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode.InternalOK;

        public int ExitCode
        {
            get { return Interlocked.CompareExchange(ref _processExitCode, 0, 0); }
            set { Interlocked.CompareExchange(ref _processExitCode, value, ExitOK); }
        }

        public event EventHandler<ConversionProgressEventArgs> OutputDataReceived;
        public event EventHandler<ConversionErrorEventArgs> ErrorDataReceived;
        public event EventHandler Completed;

        public event EventHandler OptionsObjectChanged;

        public event ThreadExceptionEventHandler LaunchFailed;
        public event EventHandler LaunchSucceeded;

        private EventWaitHandle CancelEvent
        {
            get { return _cancelEvent ?? (_cancelEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _optionsHolder.OptionsObject.InternalCancelEventName)); }
        }

        private bool IsCreationMode
        {
            get { return IsConvertMode || IsResumeMode; }
        }

        private bool IsConvertMode
        {
            get { return _conversionType == ConversionType.Convert; }
        }
        
        private bool IsResumeMode
        {
            get { return _conversionType == ConversionType.Resume; }
        }

        private void InitOptionsHolder()
        {
            if (_optionsHolder != null)
            {
                _optionsHolder.OptionsObjectChanged -= OptionsHolderOptionsObjectChanged;
            }

            _optionsHolder = new OptionsHolder<UIOptions>();
            _optionsHolder.OptionsObjectChanged += OptionsHolderOptionsObjectChanged;
        }

        private void OptionsHolderOptionsObjectChanged(object sender, EventArgs e)
        {
            FireOptionsObjectChanged();
        }

        private OptionsHolder<UIOptions> _optionsHolder;
    }
}
