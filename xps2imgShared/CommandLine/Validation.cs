using System;

using CommandLine.Validation;

using Xps2Img.Shared.TypeConverters;

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
            return String.Compare((value ?? String.Empty).Trim(), AutoValue, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public const string AutoValue = "Auto";
        public const string AutoValueValidationRegex = @"(^\s*" + AutoValue + @"\s*$)";
        public const string CpuAffinityValidationExpression = "/" + AutoValueValidationRegex + "|" + Interval.ValidationRegex0 + "/i";
        public const string DpiValidationExpression = "16-2350";
        public const string FileNameCharactersNotAllowed = " <>:\"/\\|?* characters are not allowed";
        public const string FileNameValidationRegex = @"/^([^\x00-\x1F<>:""/\\|?*])*$/";
        public const string FirstPageIndexValidationExpression = "1-10000";
        public const string ImageNameValidationExpression = FileNameValidationRegex;
        public const string JpegQualityValidationExpression = "10-100";
        public const string PagesValidationExpression = "/" + Interval.ValidationRegex + "/";
        public const string PrelimsPrefixValidationExpression = FileNameValidationRegex;
        public const string RequiredSizeValidationExpression = "/" + @"^$|" + RequiredSizeTypeConverter.ValidationRegex + "/";
    }
}
