using System;

namespace Xps2Img
{
	public class ConverterState
	{
		public int ActivePage { get; set; }
		public int ActivePageIndex { get; set; }
		public int LastPage { get; set; }

		public int TotalPages { get; set; }

		public bool HasPageCount { get { return TotalPages != 0; } }

		public void SetLastAndTotalPages(int lastPage, int totalPages)
		{
			LastPage = lastPage;
			TotalPages = totalPages;
		}

		public double Percent { get { return (double)ActivePageIndex / TotalPages * 100; } }

		public override string ToString()
		{
			return String.Format(
					"ActivePage: {0}, ActivePageIndex: {1}, LastPage: {2}, TotalPages: {3}",
					 ActivePage, ActivePageIndex, LastPage, TotalPages);
		}
	}
}