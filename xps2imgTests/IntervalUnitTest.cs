using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xps2Img.CommandLine;

namespace Xps2ImgTests
{
    [TestClass]
    public class IntervalUnitTest
    {
        [TestMethod]
        public void SplitIntervals()
        {
            //var intervals = new[] { new Interval(50, 100), new Interval(150, 201) };
            //var splitted = intervals.ToList().SplitBy(50);

            var intervals = new[] { new Interval(1, 101) };
            var splitted = intervals.ToList().SplitBy(50);

            intervals = new[] { new Interval(1, 100) };
            splitted = intervals.ToList().SplitBy(50);

            intervals = new[] { new Interval(1, 2), new Interval(4, 100), new Interval(101, 103) };
            splitted = intervals.ToList().SplitBy(50);

            intervals = new[] { new Interval(1, 1) };
            splitted = intervals.ToList().SplitBy(50);

        }
    }
}
