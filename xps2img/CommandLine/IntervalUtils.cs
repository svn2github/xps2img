using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xps2Img.CommandLine
{
    public static class IntervalUtils
    {
        public static string ToString(this IEnumerable<Interval> intervals)
        {
            var sb = new StringBuilder(16);
            var first = true;
            foreach (var interval in intervals)
            {
                if (!first)
                {
                    sb.Append(',');
                }
                sb.Append(interval);
                first = false;
            }
            return sb.ToString();
        }

        public static int GetTotalLength(this IEnumerable<Interval> intervals)
        {
            return intervals.Sum(interval => interval.Length);
        }

        public static void SetEndValue(this IEnumerable<Interval> intervals, int endValue)
        {
            foreach (var interval in intervals)
            {
                interval.SetEndValue(endValue);
            }
        }

        public static List<Interval> AdjustBeginValue(this IEnumerable<Interval> intervals, int beginValue)
        {
            return intervals
                .Where(interval => interval.End >= beginValue)
                .Select(interval => (interval.Begin >= beginValue) ? interval : new Interval(beginValue, interval.End))
                .ToList();
        }

        private static List<Interval> GetFromBitArray(BitArray bits, ref int pageNumber, int count)
        {
            var intervals = new List<Interval>();
            int? startPage = null;
            int? lastPage = null;

            for (; pageNumber < bits.Length; pageNumber++)
            {
                if (count == 0)
                {
                    break;
                }

                // 0
                if (!bits[pageNumber])
                {
                    if (startPage != null)
                    {
                        intervals.Add(new Interval(startPage.Value, pageNumber - 1));
                        startPage = lastPage = null;
                    }
                    continue;
                }

                // 1
                if (startPage == null)
                {
                    startPage = pageNumber;
                }

                lastPage = pageNumber;

                count--;
            }

            if (lastPage != null)
            {
                intervals.Add(new Interval(startPage.Value, lastPage.Value));
            }

            return intervals;
        }

        public static List<List<Interval>> SplitBy(this List<Interval> intervals, int intervalLength)
        {
            if (intervalLength <= 0)
            {
                return new List<List<Interval>> { intervals };
            }

            var bits = new BitArray(intervals.Last().End + 1, false);

            foreach (var interval in intervals)
            {
                for (var i = interval.Begin; i <= interval.End; i++)
                {
                    bits.Set(i, true);
                }
            }

            var splittedIntervals = new List<List<Interval>>();

            var lastIndex = 1;

            var doubledIntervalLength = intervalLength*2;

            while (lastIndex < bits.Length)
            {
                if (bits.Length - lastIndex < doubledIntervalLength)
                {
                    intervalLength = doubledIntervalLength;
                }
                splittedIntervals.Add(GetFromBitArray(bits, ref lastIndex, intervalLength));
            }

            return splittedIntervals;
        }
    }
}