using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xps2Img.CommandLine;

namespace Xps2ImgTests
{
    [TestClass]
    public class IntervalUnitTest
    {
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
        
        public static List<List<Interval>> Split(List<Interval> intervals, int partsCount)
        {
            if (partsCount <= 0)
            {
                throw new ArgumentException("Parts count should be greater than zero.", "partsCount");
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

            var intervalsLength = intervals.GetTotalLength();
            var partSize = intervalsLength/partsCount;

            var lastIndex = 1;

            for (int i = 0; i < intervalsLength; i += partsCount)
            {
                splittedIntervals.Add(GetFromBitArray(bits, ref lastIndex, partsCount));
            }

            return splittedIntervals;
        }

        [TestMethod]
        public void SplitIntervals()
        {
            var intervals = new [] { new Interval(1, 101) };
            var splitted = Split(intervals.ToList(), 50);

            intervals = new[] { new Interval(1, 100) };
            splitted = Split(intervals.ToList(), 50);

            intervals = new[] { new Interval(1, 2), new Interval(4, 100), new Interval(101, 103) };
            splitted = Split(intervals.ToList(), 50);

            intervals = new[] { new Interval(1, 1) };
            splitted = Split(intervals.ToList(), 50);
        }
    }
}
