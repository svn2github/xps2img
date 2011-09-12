using System;

namespace Xps2ImgUI.Model
{
    public interface ISettingsSerialization
    {
        object Serialize();
        void Deserialize(object serialized);
        Type GetSerializableType();
    }
}
