using System;

namespace Xps2Img.Shared.Progress
{
    public class Estimated
    {
        private const double PercentThreshold   = 0.001;
        private const double EstimatedThreshold = 0.95;

        private TimeSpan _leftPrev;

        public TimeSpan Elapsed { get; private set; }
        public TimeSpan Left { get; private set; }

        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        public void Caclulate(double percent, bool fromTimer = false)
        {
            if (fromTimer)
            {
                Elapsed += Interval;
            }

            var timeLeft = percent < PercentThreshold
                             ? Interval
                             : TimeSpan.FromSeconds((long) (Elapsed.Ticks / percent * (100 - percent) / TimeSpan.TicksPerSecond));

            if (Math.Min(_leftPrev.TotalMilliseconds, timeLeft.TotalMilliseconds) / Math.Max(_leftPrev.TotalMilliseconds, timeLeft.TotalMilliseconds) < EstimatedThreshold)
            {
                _leftPrev = Left = timeLeft;
            }
            else
            {
                if (fromTimer)
                {
                    Left -= Interval;
                }
            }

            if (Left <= TimeSpan.Zero)
            {
                Left = Interval;
            }
        }

        public void Reset()
        {
            Elapsed = Left = _leftPrev = new TimeSpan();
        }
    }
}
