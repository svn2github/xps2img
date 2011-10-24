using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xps2Img.CommandLine;

namespace Xps2ImgTests
{
    [TestClass]
    public class IntervalUnitTest
    {
        public static List<List<Interval>> Split(List<Interval> intervals, int partsCount)
        {
            if (partsCount <= 0)
            {
                throw new ArgumentException("Parts count should be greater than zero.", "partsCount");
            }

            var splittedIntervals = new List<List<Interval>>();

            var length = intervals.GetTotalLength();

            var itemList = new List<Interval>();
            var left = new List<Interval>();

            foreach(var interval in intervals)
            {
                if (interval.Length <= partsCount)
                {
                    splittedIntervals.Add(new List<Interval>{ interval });
                    continue;
                }

                var newInterval = new Interval();



            }

            return splittedIntervals;
        }

        [TestMethod]
        public void SplitIntervals()
        {
            var interval = new [] { new Interval(1, 101) };


        }
    }
}
