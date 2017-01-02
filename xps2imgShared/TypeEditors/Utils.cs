using System;
using System.Windows.Forms.Design;

namespace Xps2Img.Shared.TypeEditors
{
    public static class Utils
    {
        public static IWindowsFormsEditorService GetWindowsFormsEditorService(this IServiceProvider provider)
        {
            return provider == null ? null : (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
        }
    }
}
