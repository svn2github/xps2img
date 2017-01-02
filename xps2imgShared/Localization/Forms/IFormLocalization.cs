using System;

namespace Xps2Img.Shared.Localization.Forms
{
    public interface IFormLocalization
    {
        event EventHandler Closed;

        void UICultureChanged();
    }
}
