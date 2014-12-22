using System;
using System.Linq;

using CommandLine.Interfaces;

namespace CommandLine.Validation.Validators
{
    public class EnumValidator : ValidatorBase
    {
        public static IValidator Create(object validation)
        {
            var type = validation as Type;
            return type != null && type.BaseType != null && type.BaseType.Name == "Enum"
                       ? new EnumValidator(((Type)validation).UnderlyingSystemType)
                       : null;
        }

        private readonly string[] _names;

        public EnumValidator(Type enumType)
        {
            _names = Array.ConvertAll(Enum.GetNames(enumType), x => x.ToLowerInvariant());
        }

        protected override bool IsValid(string value)
        {
            return _names.Contains(value.ToLowerInvariant().Replace("\x20", String.Empty));
        }

        protected override string Message
        {
            get { return Resources.Strings.Validation_EnumValidator; }
        }
    }
}