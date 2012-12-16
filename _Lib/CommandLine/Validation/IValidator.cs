using System;

namespace CommandLine.Validation
{
    public interface IValidator
    {
        void Validate(string value);
        void Validate(string value, Func<string, bool> validationPredicate);
    }
}
