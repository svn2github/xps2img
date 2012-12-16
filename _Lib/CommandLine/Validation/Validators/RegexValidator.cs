using System.Text.RegularExpressions;

namespace CommandLine.Validation.Validators
{
    public class RegexValidator : ValidatorBase
    {
        private static readonly Regex Filter = new Regex(@"^/(?<regex>[\s\S]+?)/(?<options>[ims]*)$");

        public static IValidator Create(object validation)
        {
            var val = validation as string;
            if (val == null)
            {
                return null;
            }

            var match = Filter.Match(val);
            return match.Success ? new RegexValidator(match.Groups["regex"].Value, match.Groups["options"].Value) : null;
        }

        private readonly Regex _validator;

        public RegexValidator(string regexp, string options)
        {
            var regexOptions = RegexOptions.None;
            foreach (var option in options.ToCharArray())
            {
                switch (option)
                {
                    case 'i': regexOptions |= RegexOptions.IgnoreCase; break;
                    case 'm': regexOptions |= RegexOptions.Multiline; break;
                    case 's': regexOptions |= RegexOptions.Singleline; break;
                }
            }

            _validator = new Regex(regexp, regexOptions);
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