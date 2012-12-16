using System;

namespace CommandLine.Validation
{
    public abstract class ValidatorBase: IValidator
    {
        public virtual void Validate(string value)
        {
            Validate(value, null);
        }

        public virtual void Validate(string value, Func<string, bool> validationPredicate)
        {
            if (!IsValid(value) || (validationPredicate != null && !validationPredicate(value)))
            {
                throw new ValidationException(Message);
            }
        }

        protected abstract string Message { get; }

        protected abstract bool IsValid(string value);
    }
}