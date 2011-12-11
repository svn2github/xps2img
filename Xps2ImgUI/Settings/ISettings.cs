using System;

namespace Xps2ImgUI.Settings
{
    public interface ISettings
    {
        bool CanSezialize { get; }
        object GetSettings();
        void SetSettings(object serialized);
        Type GetSettingsType();
    }
}
