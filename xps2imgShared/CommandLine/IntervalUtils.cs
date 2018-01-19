using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xps2Img.Shared.CommandLine
{
    public static class IntervalUtils
    {
        public static string ToString(this IEnumerable<Interval> intervals, int minValue = Interval.MinValue)
        {
            var sb = new StringBuilder(64);
            var first = true;
            foreach (var interval in intervals ?? new List<Interval>())
            {
                if (!first)
                {
                    sb.Append(',');
                }
                sb.Append(interval);
                first = false;
            }
            var result = sb.ToString();
            return result == (minValue + "-") ? String.Empty : result;
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

        public static bool LessThan(this IEnumerable<Interval> intervals, int val)
        {
            var lastInterval = intervals.LastOrDefault();
            return lastInterval != null && lastInterval.LessThan(val);
        }   

        public static List<Interval> FromBitArray(BitArray bits)
        {
            var pageNumber = 1;
            return FromBitArray(bits, ref pageNumber, bits.Count + 1);
        }

        public static List<Interval> FromBitArray(BitArray bits, ref int pageNumber, int count)
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

            if (startPage != null && lastPage != null)
            {
                intervals.Add(new Interval(startPage.Value, lastPage.Value));
            }

            return intervals;
        }

        public static BitArray ToBitArray(this List<Interval> intervals)
        {
            var bits = new BitArray(intervals.Last().End + 1, false);

            foreach (var interval in intervals)
            {
                for (var i = interval.Begin; i <= interval.End; i++)
                {
                    bits.Set(i, true);
                }
            }

            return bits;
        }

        public static List<List<Interval>> SplitBy(this IEnumerable<Interval> intervals, int intervalsCount)
        {
            return SplitBy(intervals.ToList(), intervalsCount);
        }

        public static List<List<Interval>> SplitBy(this List<Interval> intervals, int intervalsCount)
        {
            if (intervalsCount <= 0)
            {
                return new List<List<Interval>> { intervals };
            }

            var bits = ToBitArray(intervals);

            var splittedIntervals = new List<List<Interval>>();

            var lastIndex = 1;

            var intervalLength = intervals.GetTotalLength() / intervalsCount;

            if (intervalLength == 0)
            {
                intervalLength = 1;
            }

            while (lastIndex < bits.Length)
            {
                if (--intervalsCount == 0)
                {
                    intervalLength = bits.Length;
                }
                splittedIntervals.Add(FromBitArray(bits, ref lastIndex, intervalLength));
            }

            return splittedIntervals;
        }
    }
}