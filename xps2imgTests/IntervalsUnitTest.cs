using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using FluentAssertions;

using Xps2Img.Shared.CommandLine;

namespace Xps2ImgTests
{
    [TestFixture]
    public class IntervalsUnitTest
    {
        private static void IntervalsSplitByAndAssert(IEnumerable<Interval> intervals, int splitBy, IList<List<Interval>> expectedIntervals)
        {
            var splitted = intervals.SplitBy(splitBy);
            splitted.Should().HaveSameCount(expectedIntervals);

            for (var i = 0; i < expectedIntervals.Count; i++)
            {
                splitted[i].Should().BeEquivalentTo(expectedIntervals[i]);
            }
        }

        private static void IntervalsFromBitArrayAndAssert(IEnumerable<Interval> expectedIntervals, bool[] boolBits)
        {
            var intervals = IntervalUtils.FromBitArray(new BitArray(boolBits));
            intervals.Should().BeEquivalentTo(expectedIntervals);
        }

        [Test]
        public void Intervals_Split_AreEqual()
        {
            IntervalsSplitByAndAssert(new[] { new Interval(211, 413) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(211, 260) }, new List<Interval> { new Interval(261, 310) }, new List<Interval> { new Interval(311, 360) }, new List<Interval> { new Interval(361, 413) } });
            IntervalsSplitByAndAssert(new[] { new Interval(50, 100), new Interval(150, 201) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(50, 74) }, new List<Interval> { new Interval(75, 99) }, new List<Interval> { new Interval(100), new Interval(150, 173) }, new List<Interval> { new Interval(174, 201) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 101) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 50) }, new List<Interval> { new Interval(51, 101) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 100) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 50) }, new List<Interval> { new Interval(51, 100) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 2), new Interval(4, 100), new Interval(101, 103) }, 2, new List<List<Interval>> { new List<Interval> { new Interval(1, 2), new Interval(4, 52) }, new List<Interval> { new Interval(53, 103) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1) }, 1, new List<List<Interval>> { new List<Interval> { new Interval(1) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 6) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1) }, new List<Interval> { new Interval(2) }, new List<Interval> { new Interval(3) }, new List<Interval> { new Interval(4, 6) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 8) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1, 2) }, new List<Interval> { new Interval(3, 4) }, new List<Interval> { new Interval(5, 6) }, new List<Interval> { new Interval(7, 8) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 9) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(1, 2) }, new List<Interval> { new Interval(3, 4) }, new List<Interval> { new Interval(5, 6) }, new List<Interval> { new Interval(7, 9) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 9) }, 3, new List<List<Interval>> { new List<Interval> { new Interval(1, 3) }, new List<Interval> { new Interval(4, 6) }, new List<Interval> { new Interval(7, 9) } });
            IntervalsSplitByAndAssert(new[] { new Interval(7, 9) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(7) }, new List<Interval> { new Interval(8) }, new List<Interval> { new Interval(9) } });
            IntervalsSplitByAndAssert(new[] { new Interval(7) }, 4, new List<List<Interval>> { new List<Interval> { new Interval(7) } });
            IntervalsSplitByAndAssert(new[] { new Interval(1, 101) }, 0, new List<List<Interval>> { new List<Interval> { new Interval(1, 101) } });
        }

        [Test]
        public void Intervals_FromBitArray_AreEqual()
        {
            IntervalsFromBitArrayAndAssert(new[] {new Interval(2), new Interval(8), new Interval(10, 13)},
                                           new[] { false, false, true, false, false, false, false, false, true, false, true, true, true,  true});

            IntervalsFromBitArrayAndAssert(new[] { new Interval(1, 8) },
                                           new[] { true, true, true, true, true, true, true, true, true });

            IntervalsFromBitArrayAndAssert(new[] { new Interval(8) },
                                           new[] { true, false, false, false, false, false, false, false, true });

            IntervalsFromBitArrayAndAssert(new[] { new Interval(9) },
                                           new[] { true, false, false, false, false, false, false, false, false, true });

            IntervalsFromBitArrayAndAssert(new Interval[0],
                                           new[] { false, false, false, false, false, false, false, false, false });
        }
    }
}
