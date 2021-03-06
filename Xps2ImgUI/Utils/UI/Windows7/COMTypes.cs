using System;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Windows7.DesktopIntegration.Interop
{
    [SuppressUnmanagedCodeSecurity]
    [ComImportAttribute]
    [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITaskbarList3
    {
        // ITaskbarList
        [PreserveSig]
        void HrInit();
        [PreserveSig]
        void AddTab(IntPtr hwnd);
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2
        [PreserveSig]
        void MarkFullscreenWindow(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags);
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        void UnregisterTab(IntPtr hwndTab);
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, TBATFLAG tbatFlags);
        void ThumbBarAddButtons(
            IntPtr hwnd,
            uint cButtons,
            [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        void ThumbBarUpdateButtons(
            IntPtr hwnd,
            uint cButtons,
            [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
        void SetOverlayIcon(
            IntPtr hwnd,
            IntPtr hIcon,
            [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
        void SetThumbnailTooltip(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
        void SetThumbnailClip(
            IntPtr hwnd,
            /*[MarshalAs(UnmanagedType.LPStruct)]*/ ref RECT prcClip);
    }

    [SuppressUnmanagedCodeSecurity]
    [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    [ComImportAttribute]
    internal class TaskbarList { }

    internal class TaskbarListStub : ITaskbarList3
    {
        public void HrInit() { }
        public void AddTab(IntPtr hwnd) { }
        public void DeleteTab(IntPtr hwnd) { }
        public void ActivateTab(IntPtr hwnd) { }
        public void SetActiveAlt(IntPtr hwnd) { }
        public void MarkFullscreenWindow(IntPtr hwnd, bool fFullscreen) { }
        public void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal) { }
        public void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags) { }
        public void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI) { }
        public void UnregisterTab(IntPtr hwndTab) { }
        public void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore) { }
        public void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, TBATFLAG tbatFlags) { }
        public void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButtons) { }
        public void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButtons) { }
        public void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl) { }
        public void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, string pszDescription) { }
        public void SetThumbnailTooltip(IntPtr hwnd, string pszTip) { }
        public void SetThumbnailClip(IntPtr hwnd, ref RECT prcClip) { }
    }
}