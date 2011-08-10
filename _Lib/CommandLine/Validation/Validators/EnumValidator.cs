using System;
using System.Linq;

namespace CommandLine.Validation.Validators
{
    internal class EnumValidator : IValidator
    {
        public static IValidator Create(object validation)
        {
            var type = validation as Type;
            return type != null && type.BaseType != null && type.BaseType.Name == "Enum"
                       ? new EnumValidator(((Type)validation).UnderlyingSystemType)
                       : null;
        }

        private readonly string[] names;

        public EnumValidator(Type enumType)
        {
            names = Array.ConvertAll(Enum.GetNames(enumType), x => x.ToLowerInvariant());
        }

        public void Validate(string value)
        {
            var lowerValue = value.ToLowerInvariant();
            if (!names.Contains(lowerValue))
            {
                throw new ValidationException(Resources.Strings.Validation_EnumValidator);
            }
        }
    }
}