using System.Collections;
using System.Linq;

namespace Xps2ImgTests
{
    public class PagesStateTraker
    {
        private BitArray _pages;
        private BitArray _processedPages;

        public void Start(BitArray pages)
        {
            _pages = pages;
            _processedPages = new BitArray(pages);
        }

        public void Set(int index)
        {
            _processedPages.Set(index, true);
        }

        public bool HasUnprocessed
        {
            get { return _processedPages.Cast<bool>().Any(b => b); }
        }
    }
}
