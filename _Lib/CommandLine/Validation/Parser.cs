using System;
using System.Linq;

using CommandLine.Interfaces;
using CommandLine.Validation.Validators;

namespace CommandLine.Validation
{
    public static class Parser
    {
        private static readonly Func<object, IValidator>[] Validators =
        {
            IntValidator.Create,
            RegexValidator.Create,
            EnumValidator.Create
        };

        public static IValidator Parse(object validationExpression)
        {
            var validatorType = validationExpression as Type;
            if (validatorType != null)
            {
                return (IValidator)Activator.CreateInstance(validatorType);
            }

            var validationExpressionString = validationExpression as string;
            if (String.IsNullOrEmpty(validationExpressionString))
            {
                return null;
            }

            var result = Validators.Select(creator => creator(validationExpression)).FirstOrDefault(validator => validator != null);

            if (result == null)
            {
                throw new ArgumentException(Resources.Strings.Validation_NoValidator, "validationExpression");
            }

            return result;
        }
    }
}
