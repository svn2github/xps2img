using System.Text.RegularExpressions;

namespace CommandLine.Validation.Validators
{
  internal class RegexValidator: Validator
  {
    private static readonly Regex filter = new Regex(@"^/(?<regex>[\s\S]+?)/(?<options>[ims]*)$");
  
    public static Validator Create(object validation)
    {
      if (!IsTypeOf<string>(validation))
      {
        return null;
      }
      
      var match = filter.Match((string)validation);
      return match.Success ? new RegexValidator(match.Groups["regex"].Value, match.Groups["options"].Value) : null;
    }

    private readonly Regex Validator;
    
    public RegexValidator(string regexp, string options)
    {
      var regexOptions = RegexOptions.None;
      foreach (var option in options.ToCharArray())
      {
        switch(option)
        {
          case 'i': regexOptions |= RegexOptions.IgnoreCase;  break;
          case 'm': regexOptions |= RegexOptions.Multiline;   break;
          case 's': regexOptions |= RegexOptions.Singleline;  break;
        }
      }

      Validator = new Regex(regexp, regexOptions);
    }
    
    public override void Validate(string value)
    {
      if (!Validator.IsMatch(value))
      {
        throw new ValidationException(Resources.Strings.Validation_RegexValidator);
      }
    }
  }
}