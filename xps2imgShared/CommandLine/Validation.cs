﻿using System;

using CommandLine.Validation;

namespace Xps2Img.Shared.CommandLine
{
    public static class Validation
    {
        public static void ValidateProperty(object propertyValue, object validatorExpresion, Func<string, bool> predicate = null)
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
            return String.Compare((value ?? String.Empty).Trim(), Resources.Strings.Auto, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
