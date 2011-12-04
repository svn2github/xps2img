using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xps2ImgTests
{
    [TestClass]
    public class PagesStateTrakerUnitTest
    {
        private static PagesStateTraker InitTracker(IEnumerable<int> positions, params int[] bits)
        {
            Func<IEnumerable<int>, int> lastOrZero = u => u.Any() ? u.Last() : 0;

            var pagesStateTracker = new PagesStateTraker();

            var sortedPositions = positions.ToArray().OrderBy(b => b);
            var sortedBits = bits.OrderBy(b => b);

            var bitArray = new BitArray(Math.Max(lastOrZero(sortedPositions), lastOrZero(sortedBits)) + 1);

            Array.ForEach(sortedBits.ToArray(), i => bitArray.Set(i, true));

            pagesStateTracker.Start(sortedPositions, bitArray);

            return pagesStateTracker;
        }

        [TestMethod]
        public void TrackPagesState()
        {
            // ReSharper disable JoinDeclarationAndInitializer
            PagesStateTraker pagesStateTracker;
            // ReSharper restore JoinDeclarationAndInitializer
            
            pagesStateTracker = InitTracker(new[] { 101 }, 100);
            Assert.IsTrue(pagesStateTracker.HasUnprocessed);

            pagesStateTracker = InitTracker(new[] { 101 }, 1 );
            Assert.IsTrue(pagesStateTracker.HasUnprocessed);

            pagesStateTracker = InitTracker(new[] { 101 });
            Assert.IsFalse(pagesStateTracker.HasUnprocessed);
        }
    }
}
