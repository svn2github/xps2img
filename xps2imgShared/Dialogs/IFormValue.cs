namespace Xps2Img.Shared.Dialogs
{
    public interface IFormValue<T> where T : new()
    {
        T Value { get; set; }
    }
}
