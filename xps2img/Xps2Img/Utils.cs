using System;

namespace Xps2Img.Xps2Img
{
	public static class Utils
	{
		public static string GetNumberFormat(this int position, double count, bool padZeros)
		{
			return String.Format(padZeros ? "{{{0}:D{1}}}" : "{{{0},{1}}}", position, (int)Math.Round(Math.Log10(count) + 0.5));
		}

		public static string GetNumberFormat(this double count)
		{
			return GetNumberFormat(0, count, true);
		}

		public static string GetNumberFormat(this int count)
		{
			return GetNumberFormat((double)count);
		}
	}
}
