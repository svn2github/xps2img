using System.IO;

namespace Xps2ImgUI.Model
{
    public interface ISettingsSerialization
    {
        void Serialize(Stream stream);
        void Deserialize(Stream stream);
    }
}
