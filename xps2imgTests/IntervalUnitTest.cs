using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xps2Img.CommandLine;

namespace Xps2ImgTests
{
    [TestClass]
    public class IntervalUnitTest
    {
        private static void VerifySplitIntervals(IEnumerable<Interval> intervals, int splitBy, IList<List<Interval>> expectedIntervals)
        {
            var splitted = intervals.SplitBy(splitBy);

            Assert.AreEqual(expectedIntervals.Count, splitted.Count);
            for (var i = 0; i < expectedIntervals.Count; i++)
            {
                var splittedSubIntervals = splitted[i];
                var expectedSubIntervals = expectedIntervals[i];

                Assert.AreEqual(expectedSubIntervals.Count, splittedSubIntervals.Count);

                for (var j = 0; j < expectedSubIntervals.Count; j++)
                {
                    var expectedSubInterval = expectedSubIntervals[j];
                    var splittedSubInterval = splittedSubIntervals[j];

                    Assert.AreEqual(expectedSubInterval.Begin, splittedSubInterval.Begin);
                    Assert.AreEqual(expectedSubInterval.End, splittedSubInterval.End);
                }
            }
        }

        [TestMethod]
        public void SplitIntervals()
        {
            VerifySplitIntervals(new[] { new Interval(50, 100), new Interval(150, 201) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(50, 99) }, new List<Interval> { new Interval(100), new Interval(150, 198) }, new List<Interval> { new Interval(199, 201) } });
            VerifySplitIntervals(new[] { new Interval(1, 101) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 50) }, new List<Interval> { new Interval(51, 101) } });
            VerifySplitIntervals(new[] { new Interval(1, 100) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 50) }, new List<Interval> { new Interval(51, 100) } });
            VerifySplitIntervals(new[] { new Interval(1, 2), new Interval(4, 100), new Interval(101, 103) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 2), new Interval(4, 52) }, new List<Interval> { new Interval(53, 103) } });
            VerifySplitIntervals(new[] { new Interval(1) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1) } });
            VerifySplitIntervals(new[] { new Interval(1, 6) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1) }, new List<Interval> { new Interval(2) }, new List<Interval> { new Interval(3) }, new List<Interval> { new Interval(4, 6) } });
            VerifySplitIntervals(new[] { new Interval(1, 8) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1, 2) }, new List<Interval> { new Interval(3, 4) }, new List<Interval> { new Interval(5, 6) }, new List<Interval> { new Interval(7, 8) } });
            VerifySplitIntervals(new[] { new Interval(1, 9) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1, 2) }, new List<Interval> { new Interval(3, 4) }, new List<Interval> { new Interval(5, 6) }, new List<Interval> { new Interval(7, 9) } });
            VerifySplitIntervals(new[] { new Interval(1, 9) }, 3, new List<List<Interval>> { new List<Interval> { new Interval(1, 3) }, new List<Interval> { new Interval(4, 6) }, new List<Interval> { new Interval(7, 9) } });
        }
    }
}
