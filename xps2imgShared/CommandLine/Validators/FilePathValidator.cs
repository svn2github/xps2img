namespace Xps2Img.Shared.CommandLine.Validators
{
    public class FilePathValidator : PathValidator
    {
        protected override string Message
        {
            get { return Resources.Strings.FilePathIsNotValid; }
        }
    }
}
