using CommandLine.Validation;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.CommandLine.Validators
{
    public abstract class PathValidator : ValidatorBase
    {
        protected override bool IsValid(string value)
        {
            string _;
            return PathUtils.TryGetAbsolutePath(value, out _);
        }
    }
}
