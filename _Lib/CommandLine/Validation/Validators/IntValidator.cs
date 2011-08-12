using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CommandLine.Validation.Validators
{
    internal class IntValidator : IValidator
    {
        private static readonly Regex Filter = new Regex(@"^\s*(?<lowerBound>\d+)\s*-\s*(?<upperBound>\d+)\s*$");

        public static IValidator Create(object validation)
        {
            var val = validation as string;
            if (val == null)
            {
                return null;
            }

            var match = Filter.Match(val);
            return match.Success ?
                    new IntValidator(match.Groups["lowerBound"].Value, match.Groups["upperBound"].Value) :
                    null;
        }

        private readonly int _lowerBound;
        private readonly int _upperBound;

        public IntValidator(string lowerBound, string upperBound)
        {
            _lowerBound = Int32.Parse(lowerBound);
            _upperBound = Int32.Parse(upperBound);
            Debug.Assert(_lowerBound <= _upperBound);
        }

        public void Validate(string value)
        {
            int val;
            if (!Int32.TryParse(value, out val) || val < _lowerBound || val > _upperBound)
            {
                throw new ValidationException(String.Format(Resources.Strings.Validation_IntValidator, _lowerBound, _upperBound));
            }
        }
    }
}