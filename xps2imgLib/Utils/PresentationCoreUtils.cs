using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

using Xps2ImgLib.Utils.Disposables;

namespace Xps2ImgLib.Utils
{
    public static class PresentationCoreUtils
    {
        private const BindingFlags NonPublicInstanceBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private static Assembly _presentationCoreAssembly;

        private static PropertyInfo _wicSourceHandlePropertyPropertyInfo;
        private static FieldInfo _handleFieldInfo;

        private static ConstructorInfo _safeHandleConstructorInfo;
        private static bool _safeHandleConstructorInfoLegacy;

        private static void LoadAssembly()
        {
            if(_presentationCoreAssembly == null)
            { 
                #pragma warning disable 618
                _presentationCoreAssembly = Assembly.LoadWithPartialName("PresentationCore");
                #pragma warning restore 618
            }
        }

        public static void CheckHResult(this int code)
        {
            Marshal.ThrowExceptionForHR(code);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static IntPtr GetHandle(this BitmapSource bitmapSource)
        {
            LoadAssembly();
            
            if (_wicSourceHandlePropertyPropertyInfo == null)
            {
                _wicSourceHandlePropertyPropertyInfo = bitmapSource.GetType().GetProperty("WicSourceHandle", NonPublicInstanceBindingFlags);
                _handleFieldInfo = _wicSourceHandlePropertyPropertyInfo.PropertyType.GetField("handle", NonPublicInstanceBindingFlags);
            }

            return (IntPtr)_handleFieldInfo.GetValue(_wicSourceHandlePropertyPropertyInfo.GetValue(bitmapSource, null));
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static IDisposable ToSafeHandle(this IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return EmptyDisposable.Default;
            }

            if (_safeHandleConstructorInfo == null)
            {
                LoadAssembly();

                Func<Type[], ConstructorInfo> getConstructor = parameters => _presentationCoreAssembly.GetType("System.Windows.Media.SafeMILHandle").GetConstructor(NonPublicInstanceBindingFlags, null, parameters, null);

                _safeHandleConstructorInfo = getConstructor(new[] { typeof(IntPtr) });
                if (_safeHandleConstructorInfo == null)
                {
                    _safeHandleConstructorInfo = getConstructor(new[] { typeof(IntPtr), typeof(long) });
                    _safeHandleConstructorInfoLegacy = true;
                }
            }
            
            return (IDisposable)_safeHandleConstructorInfo.Invoke(_safeHandleConstructorInfoLegacy ? new object[] { handle, 0L } :  new object[] { handle });
        }
    }
}
