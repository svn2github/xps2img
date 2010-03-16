﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CommandLine.Validation
{
  internal class IntValidator: IValidator
  {
    public static readonly Regex Filter = new Regex(@"^\s*(?<lowerBound>\d+)\s*-\s*(?<upperBound>\d+)\s*$");
  
    public static IValidator Create(object validation)
    {
      if (!IsTypeOf<string>(validation))
      {
        return null;
      }
      
      var match = Filter.Match((string)validation);
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