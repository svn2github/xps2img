using System.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xps2ImgTests
{
    [TestClass]
    public class PagesStateTrakerUnitTest
    {
        [TestMethod]
        public void TrackPagesState()
        {
            var pagesStateTracker = new PagesStateTraker();

            pagesStateTracker.Start(new BitArray(new byte[]{ 0x01, 0x43, 0x00 }));
            Assert.IsTrue(pagesStateTracker.HasUnprocessed);

            pagesStateTracker.Start(new BitArray(new[] { true }));
            Assert.IsTrue(pagesStateTracker.HasUnprocessed);

            pagesStateTracker.Start(new BitArray(new byte[] { 0x00, 0x00, 0x00 }));
            Assert.IsFalse(pagesStateTracker.HasUnprocessed);
        }
    }
}
