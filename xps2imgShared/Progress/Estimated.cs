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

        public bool IsNone
        {
            get { return Elapsed == TimeSpan.Zero; }
        }

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

        public string FormatRatio(int pagesTotal, bool clean, string template, string shortTemplate = "")
        {
            return FormatRatio(pagesTotal, pagesTotal, clean, template, shortTemplate);
        }

        public string FormatRatio(int pagesProcessed, int pagesTotal, bool clean, string template, string shortTemplate = "")
        {
            var timeAbbrev = Resources.Strings.AbbrevSeconds;

            Func<double, double> par = e => e > 0.001 ? pagesProcessed / e : pagesProcessed;

            var pp = par(Elapsed.TotalSeconds);
            var ppMinute = par(Elapsed.TotalMinutes);
            var ppHour = par(Elapsed.TotalHours);

            if (pp < 1.0)
            {
                pp = ppMinute;
                timeAbbrev = Resources.Strings.AbbrevMinutes;
            }

            if (pp < 1.0)
            {
                pp = ppHour;
                timeAbbrev = Resources.Strings.AbbrevHours;
            }

            if (pp < 1.0)
            {
                pp = pagesProcessed;
                timeAbbrev = Resources.Strings.AbbrevDays;
            }

            var ppInt = (int)Math.Round(pp);

            Func<int, string> getPagesString = p => p == 1 ? Resources.Strings.AbbrevPage : Resources.Strings.AbbrevPages;

            return String.Format(clean || pagesProcessed == 0 ? shortTemplate : template,
                                 Elapsed.Hours, Elapsed.Minutes, Elapsed.Seconds,
                                 ppInt, getPagesString(ppInt), timeAbbrev,
                                 pagesProcessed, pagesTotal);
        }

        public void Reset()
        {
            Elapsed = Left = _leftPrev = new TimeSpan();
        }
    }
}
