using System;

namespace Xps2ImgUI.Localization
{
    public interface IFormLocalization
    {
        event EventHandler Closed;

        void UICultureChanged();
    }
}
