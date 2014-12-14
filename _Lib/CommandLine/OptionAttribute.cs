using System;
using System.ComponentModel;
using System.Reflection;

namespace CommandLine
{
    [Flags]
    public enum OptionFlags
    {
        Internal     = 0x0001,
        Ignore       = 0x0002,
        NoValidation = 0x0004,
        NoDefaultValueDescription = 0x0008
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        private static readonly string GenerateNameFromProperty = String.Empty;

        public OptionAttribute(string description) :
            this(description, GenerateNameFromProperty)
        {
        }

        public OptionAttribute(string description, ArgumentExpectancy hasArg) :
            this(description, GenerateNameFromProperty, hasArg)
        {
        }

        public OptionAttribute(string description, string name, ArgumentExpectancy hasArg = ArgumentExpectancy.Required) :
            this(description, name, ShortOptionType.Auto, hasArg)
        {
        }

        public OptionAttribute(string description, char shortOption) :
            this(description, GenerateNameFromProperty, shortOption)
        {
        }

        public OptionAttribute(string description, char shortOption, ArgumentExpectancy hasArg) :
            this(description, GenerateNameFromProperty, shortOption, hasArg)
        {
        }

        public OptionAttribute(string description, string name, char shortOption, ArgumentExpectancy hasArg = ArgumentExpectancy.Required)
        {
            _longOptEx = new LongOptEx(description, name, shortOption, hasArg);
        }

        internal LongOptEx Create(object optionsObject, PropertyInfo propertyInfo, bool ignoreDefault)
        {
            var name = propertyInfo.Name;

            if (String.IsNullOrEmpty(_longOptEx.Name))
            {
                var attrName = LongOptEx.GetLongName(name);
                _longOptEx = _longOptEx.HasShortOption || _longOptEx.IsShortOptionNone ?
                              new LongOptEx(_longOptEx.Description, attrName, (char)_longOptEx.Val, (ArgumentExpectancy)_longOptEx.HasArg) :
                              new LongOptEx(_longOptEx.Description, attrName, (ArgumentExpectancy)_longOptEx.HasArg);
            }

            _longOptEx.BoundPropertyName = name;

            _longOptEx.TypeConverter = (TypeConverter)(ConverterType == null ?
                                          TypeDescriptor.GetConverter(propertyInfo.PropertyType) :
                                          Activator.CreateInstance(ConverterType));

            _longOptEx.BoundObject = optionsObject;
            _longOptEx.DefaultValue = DefaultValue;

            if (!ignoreDefault && DefaultValue != null && _longOptEx.TypeConverter != null && _longOptEx.TypeConverter.GetType() != typeof(NullableConverter))
            {
                _longOptEx.SetPropertyValue(DefaultValue);
            }

            _longOptEx.IsInternal = IsInternal;
            _longOptEx.IsNoDefaultValueDescription = IsNoDefaultValueDescription;

            _longOptEx.Validator = IsNoValidation
                                     ? null
                                     : Validation.Parser.Parse((String.IsNullOrEmpty(ValidationExpression) && _longOptEx.IsEnum)
                                       ? propertyInfo.PropertyType
                                       : (object)ValidationExpression);
            return _longOptEx;
        }

        public string DefaultValue { get; set; }

        public OptionFlags Flags { get; set; }

        public string ValidationExpression { get; set; }

        public bool IsInternal { get { return (Flags & OptionFlags.Internal) != 0; } }
        public bool IsNoValidation { get { return (Flags & OptionFlags.NoValidation) != 0; } }
        public bool IsNoDefaultValueDescription { get { return (Flags & OptionFlags.NoDefaultValueDescription) != 0; } }
        
        public Type ConverterType { get; set; }

        private LongOptEx _longOptEx;

        // Unnamed options only.
        protected OptionAttribute(string description, bool isRequired)
        {
            _longOptEx = new LongOptEx(description, isRequired);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UnnamedOptionAttribute : OptionAttribute
    {
        public UnnamedOptionAttribute(string description, bool isRequired = true) :
            base(description, isRequired)
        {
        }
    }
}
