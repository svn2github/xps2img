using System;

namespace Xps2ImgLib
{
    public static partial class ImageProcessor
    {
        public class Parameters<T>
        {
            public IntPtr Data { get; private set; }
            public uint Stride { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }
            public T Parameter { get; private set; }

            public Parameters(IntPtr data, uint stride, int width, int height, T parameter)
            {
                Data = data;
                Stride = stride;
                Width = width;
                Height = height;
                Parameter = parameter;
            }
        }

        public static class Parameters
        {
            public static Parameters<T> Create<T>(IntPtr data, uint stride, int width, int height, T param)
            {
                return new Parameters<T>(data, stride, width, height, param);
            }
        }
    }
}