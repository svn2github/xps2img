using System;

namespace Xps2ImgUI.Settings
{
    public interface ISettings
    {
        object GetSettings();
        void SetSettings(object serialized);
        Type GetSettingsType();
    }
}
