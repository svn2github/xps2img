using System;
using System.Text.RegularExpressions;

using CommandLine.Interfaces;

namespace CommandLine.Validation.Validators
{
    public class RegexValidator : ValidatorBase
    {
        private static readonly Regex Filter = new Regex(@"^/(?<regex>[\s\S]+?)/(?<options>[ims]*)$");

        private readonly Regex _validator;

        public static IValidator Create(object validation)
        {
            try
            {
                return new RegexValidator(validation as string);
            }
            catch
            {
                return null;
            }
        }

        public RegexValidator(string validation)
        {
            if (validation == null)
            {
                throw new ArgumentNullException("validation");
            }

            var match = Filter.Match(validation);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid validation string");
            }

            _validator = CreateRegexp(match.Groups["regex"].Value, match.Groups["options"].Value);
        }

        public RegexValidator(string regexp, string options = null)
        {
            _validator = CreateRegexp(regexp, options);
        }

        private static Regex CreateRegexp(string regexp, string options)
        {
            var regexOptions = RegexOptions.None;
            foreach (var option in (options ?? String.Empty).ToCharArray())
            {
                switch (option)
                {
                    case 'i': regexOptions |= RegexOptions.IgnoreCase; break;
                    case 'm': regexOptions |= RegexOptions.Multiline;  break;
                    case 's': regexOptions |= RegexOptions.Singleline; break;
                }
            }

            return new Regex(regexp, regexOptions);
        }

        protected override bool IsValid(string value)
        {
            return _validator.IsMatch(value);
        }

        protected override string Message
        {
            get { return Resources.Strings.Validation_RegexValidator; }
        }
    }
}