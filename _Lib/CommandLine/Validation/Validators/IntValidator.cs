using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

using CommandLine.Interfaces;

namespace CommandLine.Validation.Validators
{
    public class IntValidator : ValidatorBase
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

        protected override bool IsValid(string value)
        {
            int val;
            return Int32.TryParse(value, out val) && val >= _lowerBound && val <= _upperBound;
        }

        protected override string Message
        {
            get { return String.Format(Resources.Strings.Validation_IntValidator, _lowerBound, _upperBound); }
        }
    }
}