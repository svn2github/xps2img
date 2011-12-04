using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xps2ImgTests
{
    public class PagesStateTraker
    {
        private BitArray _pages;
        private BitArray _processedPages;
        private List<int> _startPages;

        public void Start(IEnumerable<int> startPages, BitArray pages)
        {
            _pages = pages;

            _startPages = new List<int>(startPages.OrderBy(p => p));

            var lastPage = pages.Length - 1;
            if (_startPages.Count == 1 || _startPages.Last() != lastPage)
            {
                _startPages.Add(lastPage);
            }

            _processedPages = new BitArray(pages);
        }

        public void SetDone(int index)
        {
            _processedPages.Set(index, false);
        }

        public bool HasUnprocessed
        {
            get { return _processedPages.Cast<bool>().Any(b => b); }
        }

        public void Adjust()
        {
            for(var i = 0; i < _startPages.Count-1; i++)
            {
                for (var j = _startPages[i]; j < _startPages[i + 1]; j++)
                {
                    
                }
            }
        }
    }
}
