using System;
using System.Linq;

namespace CommandLine.Validation.Validators
{
  internal class EnumValidator: Validator
  {
    public static Validator Create(object validation)
    {
      if (validation == null || !(validation is Type) || ((Type)validation).BaseType.Name != "Enum")
      {
        return null;
      }

      return new EnumValidator(((Type)validation).UnderlyingSystemType);
    }

    private readonly string[] names;

    public EnumValidator(Type enumType)
    {
      names = Array.ConvertAll(Enum.GetNames(enumType), x => x.ToLowerInvariant());
    }
    
    public override void Validate(string value)
    {
      var lowerValue = value.ToLowerInvariant();
      if (!names.Contains(lowerValue))
      {
        throw new ValidationException(Resources.Strings.Validation_EnumValidator);
      }
    }
  }
}