using System;

namespace Xps2ImgLib
{
    public partial class Converter
    {
        public class State
        {
            public int ActivePage { get; set; }
            public int ActivePageIndex { get; set; }
            public int LastPage { get; private set; }

            public int TotalPages { get; private set; }

            public bool HasPageCount { get { return TotalPages != 0; } }

            public void SetLastAndTotalPages(int lastPage, int totalPages)
            {
                LastPage = lastPage;
                TotalPages = totalPages;
            }

            public double Percent { get { return (double)ActivePageIndex / TotalPages * 100; } }

            public bool Done { get { return ActivePageIndex == TotalPages; } }

            public override string ToString()
            {
                return String.Format(
                        "ActivePage: {0}, ActivePageIndex: {1}, LastPage: {2}, TotalPages: {3}",
                         ActivePage, ActivePageIndex, LastPage, TotalPages);
            }
        }
    }
}