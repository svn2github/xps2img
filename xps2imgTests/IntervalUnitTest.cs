using System.Collections;
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
                CompareIntervals(expectedIntervals[i], splitted[i]);
            }
        }

        private static void VerifyIntervalsFromBitArray(IList<Interval> expectedIntervals, bool[] boolBits)
        {
            var intervals = IntervalUtils.IntervalsFromBitArray(new BitArray(boolBits));

            CompareIntervals(expectedIntervals, intervals);
        }

        private static void CompareIntervals(IList<Interval> expectedIntervals, IList<Interval> intervals)
        {
            Assert.AreEqual(expectedIntervals.Count, intervals.Count);

            for (var i = 0; i < intervals.Count; i++)
            {
                var interval = intervals[i];
                var expectedInterval = expectedIntervals[i];

                Assert.AreEqual(interval.Begin, expectedInterval.Begin);
                Assert.AreEqual(interval.End, expectedInterval.End);
            }
        }

        [TestMethod]
        public void SplitIntervals()
        {
            VerifySplitIntervals(new[] { new Interval(211, 413) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(211, 260) }, new List<Interval> { new Interval(261, 310) }, new List<Interval> { new Interval(311, 360) }, new List<Interval> { new Interval(361, 413) } });
            VerifySplitIntervals(new[] { new Interval(50, 100), new Interval(150, 201) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(50, 74) }, new List<Interval> { new Interval(75, 99) }, new List<Interval> { new Interval(100), new Interval(150, 173) }, new List<Interval> { new Interval(174, 201) } });
            VerifySplitIntervals(new[] { new Interval(1, 101) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 50) }, new List<Interval> { new Interval(51, 101) } });
            VerifySplitIntervals(new[] { new Interval(1, 100) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 50) }, new List<Interval> { new Interval(51, 100) } });
            VerifySplitIntervals(new[] { new Interval(1, 2), new Interval(4, 100), new Interval(101, 103) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 2), new Interval(4, 52) }, new List<Interval> { new Interval(53, 103) } });
            VerifySplitIntervals(new[] { new Interval(1) }, 1, new List<List<Interval>> { new List<Interval> { new Interval(1) } });
            VerifySplitIntervals(new[] { new Interval(1) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1) } });
            VerifySplitIntervals(new[] { new Interval(1, 6) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1) }, new List<Interval> { new Interval(2) }, new List<Interval> { new Interval(3) }, new List<Interval> { new Interval(4, 6) } });
            VerifySplitIntervals(new[] { new Interval(1, 8) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1, 2) }, new List<Interval> { new Interval(3, 4) }, new List<Interval> { new Interval(5, 6) }, new List<Interval> { new Interval(7, 8) } });
            VerifySplitIntervals(new[] { new Interval(1, 9) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1, 2) }, new List<Interval> { new Interval(3, 4) }, new List<Interval> { new Interval(5, 6) }, new List<Interval> { new Interval(7, 9) } });
            VerifySplitIntervals(new[] { new Interval(1, 9) }, 3, new List<List<Interval>> { new List<Interval> { new Interval(1, 3) }, new List<Interval> { new Interval(4, 6) }, new List<Interval> { new Interval(7, 9) } });
            VerifySplitIntervals(new[] { new Interval(7, 9) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(7) }, new List<Interval> { new Interval(8) }, new List<Interval> { new Interval(9) } });
            VerifySplitIntervals(new[] { new Interval(7) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(7) } });
            VerifySplitIntervals(new[] { new Interval(1, 101) }, 0, new List<List<Interval>> { new List<Interval> { new Interval(1, 101) } });
        }

        [TestMethod]
        public void BitArrayToIntervals()
        {
            VerifyIntervalsFromBitArray(new[] {new Interval(2), new Interval(8), new Interval(10, 13)},
                                        new[] { false, false, true, false, false, false, false, false, true,
                                                false, true,  true, true,  true});

            VerifyIntervalsFromBitArray(new[] { new Interval(1, 8) },
                                        new[] { true, true, true, true, true, true, true, true, true });

            VerifyIntervalsFromBitArray(new[] { new Interval(8) },
                                        new[] { true, false, false, false, false, false, false, false, true });

            VerifyIntervalsFromBitArray(new[] { new Interval(9) },
                                        new[] { true, false, false, false, false, false, false, false, false, true });

            VerifyIntervalsFromBitArray(new Interval[0],
                                        new[] { false, false, false, false, false, false, false, false, false });
        }
    }
}
