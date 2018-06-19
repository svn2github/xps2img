using System.Runtime.CompilerServices;
using System.Windows;

namespace Xps2ImgLib
{
    public static unsafe class ImageMeasurer
    {
        private const MethodImplOptions AggressiveInlining = (MethodImplOptions)0x100;

        public static Int32Rect GetCropRectangle(void* bitmap, int stride, int width, int height)
        {
            var left = width;
            var top = height;
            var right = 0;
            var bottom = 0;

            for (var row = 0; row < height; row++)
            {
                int column;

                var rowData = (uint*)((byte*)bitmap + row * stride);
                var data = rowData;

                for (column = 0; column < width && SkipColor(data++); column++)
                {
                }

                if (column < left)
                {
                    left = column;
                    if (row < top)
                    {
                        top = row;
                    }
                }

                data = rowData + width - 1;

                var prevColumn = column;
                for (; column < width && SkipColor(data--); column++)
                {
                }

                if (column >= width)
                {
                    continue;
                }

                column = width - column + prevColumn;
                if (column > right)
                {
                    right = column;
                }

                if (row > bottom)
                {
                    bottom = row;
                }
            }

            if (left >= width)
            {
                left = 0;
            }

            if (top >= height)
            {
                top = 0;
            }

            return new Int32Rect(left, top, right - left, bottom - top);
        }

        [MethodImpl(AggressiveInlining)]
        private static bool SkipColor(uint* data)
        {
            const int colorToSkip = 299 * 0xFF + 587 * 0xFF + 114 * 0xFF;
            const int colorToSkipThreshold = 140000;

            var rgb = (byte*)data;

            var r = *(rgb + 2);
            var g = *(rgb + 1);
            var b = *rgb;

            return colorToSkip - (299 * r + 587 * g + 114 * b) < colorToSkipThreshold;
        }
    }
}