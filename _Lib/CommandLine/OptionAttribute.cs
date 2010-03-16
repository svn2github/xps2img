using System;
using System.ComponentModel;
using System.Reflection;

namespace CommandLine
{
  [Flags]
  public enum OptionFlags
  {
    Internal    = 0x0001,
    Ignore      = 0x0002,
    NoValidation= 0x0004
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class OptionAttribute: Attribute
  {
    private static readonly string GenerateNameFromProperty = String.Empty;
  
    public OptionAttribute(string description):
      this(description, GenerateNameFromProperty)
    {
    }

    public OptionAttribute(string description, string name) :
      this(description, name, ArgumentExpectancy.Required)
    {
    }

    public OptionAttribute(string description, ArgumentExpectancy hasArg) :
      this(description, GenerateNameFromProperty, hasArg)
    {
    }

    public OptionAttribute(string description, string name, ArgumentExpectancy hasArg) :
      this(description, name, LongOptEx.NoShortOptionMark, hasArg)
    {
    }

    public OptionAttribute(string description, char shortOption) :
      this(description, GenerateNameFromProperty, shortOption)
    {
    }

    public OptionAttribute(string description, string name, char shortOption) :
      this(description, name, shortOption, ArgumentExpectancy.Required)
    {
    }

    public OptionAttribute(string description, char shortOption, ArgumentExpectancy hasArg) :
      this(description, GenerateNameFromProperty, shortOption, hasArg)
    {
    }
    
    public OptionAttribute(string description, string name, char shortOption, ArgumentExpectancy hasArg)
    {
      longOptEx = new LongOptEx(description, name, shortOption, hasArg);
    }

    internal LongOptEx Create(object optionsObject, PropertyInfo propertyInfo, bool ignoreDefault)
    {
      var name = propertyInfo.Name;
      
      if (String.IsNullOrEmpty(longOptEx.Name))
      {
        var attrName = LongOptEx.GetLongName(name);
        longOptEx = longOptEx.HasShortOption ?
                      new LongOptEx(longOptEx.Description, attrName, (char)longOptEx.Val, (ArgumentExpectancy)longOptEx.HasArg) :
                      new LongOptEx(longOptEx.Description, attrName, (ArgumentExpectancy)longOptEx.HasArg);
      }
      
      longOptEx.BoundPropertyName = name;

      longOptEx.TypeConverter = (TypeConverter)(ConverterType == null ?
                                    TypeDescriptor.GetConverter(propertyInfo.PropertyType) :
                                    Activator.CreateInstance(ConverterType));

      longOptEx.BoundObject = optionsObject;
      longOptEx.DefaultValue = DefaultValue;

      if (!ignoreDefault && DefaultValue != null)
      {
        longOptEx.SetPropertyValue(DefaultValue);
      }

      longOptEx.IsInternal = IsInternal;

      longOptEx.Validator = IsNoValidation ? null :
        Validation.Parser.Parse((String.IsNullOrEmpty(ValidationExpression) && longOptEx.IsEnum) ?
                                  propertyInfo.PropertyType :
                                  (object)ValidationExpression);

      return longOptEx;
    }
    
    public string DefaultValue { get; set; }

    public OptionFlags Flags { get; set; }

    public string ValidationExpression { get; set; }

    public bool IsInternal { get { return (Flags & OptionFlags.Internal) != 0; } }
    public bool IsNoValidation { get { return (Flags & OptionFlags.NoValidation) != 0; } }

    public Type ConverterType { get; set; }

    private LongOptEx longOptEx;

    // Unnamed options only.
    protected OptionAttribute(string description, bool isRequired)
    {
      longOptEx = new LongOptEx(description, isRequired);
    }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class UnnamedOptionAttribute : OptionAttribute
  {
    public UnnamedOptionAttribute(string description) :
      this(description, true)
    {
    }
    
    public UnnamedOptionAttribute(string description, bool isRequired) :
      base(description, isRequired)
    {
    }
  }
}
