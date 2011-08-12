using System;
using System.Linq;

using CommandLine.Validation.Validators;

namespace CommandLine.Validation
{
    public static class Parser
    {
        private static readonly Func<object, IValidator>[] Validators = new Func<object, IValidator>[]
        {
            IntValidator.Create,
            RegexValidator.Create,
            EnumValidator.Create
        };

        public static IValidator Parse(object validationExpression)
        {
            if (validationExpression == null || (validationExpression is string && String.IsNullOrEmpty(validationExpression as string)))
            {
                return null;
            }

            var result = Validators.Select(creator => creator(validationExpression)).Where(validator => validator != null).FirstOrDefault();

            if (result == null)
            {
                throw new ArgumentException(Resources.Strings.Validation_NoValidator, "validationExpression");
            }

            return result;
        }
    }
}
