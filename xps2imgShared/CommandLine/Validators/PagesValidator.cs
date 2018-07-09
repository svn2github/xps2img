using System;

using CommandLine.Validation.Validators;

namespace Xps2Img.Shared.CommandLine.Validators
{
    public class PagesValidator : RegexValidator
    {
        public PagesValidator() : base(Options.ValidationExpressions.Pages)
        {
        }

        protected override bool IsValid(string value)
        {
            return base.IsValid(Interval.Adjust(value ?? String.Empty));
        }
    }
}
