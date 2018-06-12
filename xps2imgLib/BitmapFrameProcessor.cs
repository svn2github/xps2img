﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

using Xps2ImgLib.Utils;
using Xps2ImgLib.Utils.Disposables;

namespace Xps2ImgLib
{
    public static class BitmapFrameProcessor
    {
        public static void Process(BitmapFrame bitmapFrame, Action<IntPtr, uint, int, int> processData)
        {
            var handle = bitmapFrame.GetBitmapSourceHandle();

            var lockHandle = IntPtr.Zero;

            // ReSharper disable once AccessToModifiedClosure
            using (new DisposableAction(() => lockHandle.ToSafeHandle().Dispose()))
            {
                IntPtr data;
                uint bufferSize, stride;

                var int32Rect = new Int32Rect(0, 0, bitmapFrame.PixelWidth, bitmapFrame.PixelHeight);

                Lock(handle, ref int32Rect, LockFlags.MIL_LOCK_READ, out lockHandle).CheckHResult();

                GetDataPointer(lockHandle, out bufferSize, out data).CheckHResult();
                GetStride(lockHandle, out stride).CheckHResult();

                processData(data, stride, int32Rect.Width, int32Rect.Height);
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private enum LockFlags
        {
            MIL_LOCK_READ = 1
        }

        [DllImport("WindowsCodecs.dll", EntryPoint = "IWICBitmap_Lock_Proxy")]
        private static extern int Lock(IntPtr thisPtr, ref Int32Rect prcLock, LockFlags flags, out IntPtr ppILock);

        [DllImport("WindowsCodecs.dll", EntryPoint = "IWICBitmapLock_GetDataPointer_STA_Proxy")]
        private static extern int GetDataPointer(IntPtr pILock, out uint pcbBufferSize, out IntPtr ppbData);

        [DllImport("WindowsCodecs.dll", EntryPoint = "IWICBitmapLock_GetStride_Proxy")]
        private static extern int GetStride(IntPtr pILock, out uint pcbStride);
    }
}