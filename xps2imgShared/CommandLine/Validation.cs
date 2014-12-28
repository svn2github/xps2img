using System;

using CommandLine.Validation;

namespace Xps2Img.Shared.CommandLine
{
    public static class Validation
    {
        public static void ValidateProperty(object propertyValue, string validatorExpresion, Func<string, bool> predicate = null)
        {
            if (String.IsNullOrEmpty(propertyValue as string))
            {
                return;
            }

            try
            {
                var validator = Parser.Parse(validatorExpresion);
                validator.Validate(propertyValue.ToString(), predicate);
            }
            catch (ValidationException ex)
            {
                var message = ex.Message.ToCharArray();
                message[0] = Char.ToUpper(message[0]);
                throw new ValidationException(new string(message));
            }
        }

        public static bool IsAutoValue(string value)
        {
            return String.Compare((value ?? String.Empty).Trim(), Options.ValidationExpressions.AutoValue, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
