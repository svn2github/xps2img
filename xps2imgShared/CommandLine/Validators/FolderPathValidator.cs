namespace Xps2Img.Shared.CommandLine.Validators
{
    public class FolderPathValidator : PathValidator
    {
        protected override string Message
        {
            get { return Resources.Strings.FolderPathIsNotValid; }
        }
    }
}
