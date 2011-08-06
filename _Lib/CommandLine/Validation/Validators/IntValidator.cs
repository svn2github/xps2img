using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CommandLine.Validation.Validators
{
  internal class IntValidator: Validator
  {
    private static readonly Regex filter = new Regex(@"^\s*(?<lowerBound>\d+)\s*-\s*(?<upperBound>\d+)\s*$");
  
    public static Validator Create(object validation)
    {
      if (!IsTypeOf<string>(validation))
      {
        return null;
      }
      
      var match = filter.Match((string)validation);
      return match.Success ?
              new IntValidator(match.Groups["lowerBound"].Value, match.Groups["upperBound"].Value) :
              null;
    }

    private readonly int LowerBound;
    private readonly int UpperBound;
    
    public IntValidator(string lowerBound, string upperBound)
    {
      LowerBound = Int32.Parse(lowerBound);
      UpperBound = Int32.Parse(upperBound);
      Debug.Assert(LowerBound <= UpperBound);
    }
    
    public override void Validate(string value)
    {
      int val;
      if (!Int32.TryParse(value, out val) || val < LowerBound || val > UpperBound)
      {
        throw new ValidationException(String.Format(Resources.Strings.Validation_IntValidator, LowerBound, UpperBound));
      }
    }
  }
}