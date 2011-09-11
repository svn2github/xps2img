namespace Xps2ImgUI.Model
{
    public interface ISettingsSerialization
    {
        object Serialize();
        void Deserialize(string serialized);
    }
}
