using System;
using System.Linq;
using CommandLine.Validation.Validators;

namespace CommandLine.Validation
{
  internal static class Parser
  {
    private static readonly Func<object, Validator>[] validators = new Func<object, Validator>[]
    {
      IntValidator.Create,
      RegexValidator.Create,
      EnumValidator.Create
    };
    
    public static Validator Parse(object validationExpression)
    {
      if (validationExpression == null || (Validator.IsTypeOf<string>(validationExpression) && String.IsNullOrEmpty(validationExpression as string)))
      {
        return null;
      }

      var result = validators.Select(creator => creator(validationExpression)).Where(validator => validator != null).FirstOrDefault();

      if(result == null)
      {
        throw new ArgumentException(Resources.Strings.Validation_NoValidator, "validationExpression");
      }

      return result;
    }
  }
}
