using System;
using System.ComponentModel;
using System.Reflection;

using CommandLine.GetOpt;
using CommandLine.Interfaces;

namespace CommandLine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        private static readonly string GenerateNameFromProperty = String.Empty;

        public OptionAttribute(string description) :
            this(description, GenerateNameFromProperty)
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

        public OptionAttribute(ArgumentExpectancy hasArg) :
            this(GenerateNameFromProperty, hasArg)
        {
        }

        public OptionAttribute(string name, ArgumentExpectancy hasArg = ArgumentExpectancy.Required) :
            this(name, ShortOptionType.Auto, hasArg)
        {
        }

        public OptionAttribute(char shortOption) :
            this(GenerateNameFromProperty, shortOption)
        {
        }

        public OptionAttribute(char shortOption, ArgumentExpectancy hasArg) :
            this(GenerateNameFromProperty, shortOption, hasArg)
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

            _longOptEx.DescriptionKey = DescriptionKey;
            _longOptEx.BoundPropertyName = name;
            _longOptEx.TypeConverter = (TypeConverter)(ConverterType == null ? TypeDescriptor.GetConverter(propertyInfo.PropertyType) : Activator.CreateInstance(ConverterType));
            _longOptEx.BoundObject = optionsObject;
            _longOptEx.DefaultValue = DefaultValue;

            if (!ignoreDefault && DefaultValue != null && _longOptEx.TypeConverter != null && _longOptEx.TypeConverter.GetType() != typeof(NullableConverter))
            {
                _longOptEx.SetPropertyValue(DefaultValue);
            }

            _longOptEx.IsInternal = IsInternal;
            _longOptEx.IsNoDefaultValueDescription = IsNoDefaultValueDescription;

            _longOptEx.Validator = CreateValidator(propertyInfo.PropertyType);

            return _longOptEx;
        }

        private IValidator CreateValidator(Type propertyType)
        {
            if (IsNoValidation || ValidationExpression == null)
            {
                return null;
            }

            var validationType = ValidationExpression as Type;
            if (validationType != null)
            {
                return (IValidator)Activator.CreateInstance(validationType);
            }

            var validationExpression = ValidationExpression as string;
            if (!String.IsNullOrEmpty(validationExpression))
            {
                return Validation.Parser.Parse(_longOptEx.IsEnum ? propertyType : ValidationExpression);
            }

            throw new InvalidCastException("Only 'string' or 'Type' types are supported");
        }

        public string DescriptionKey { get; set; }

        public string DefaultValue { get; set; }

        public OptionFlags Flags { get; set; }

        public object ValidationExpression { get; set; }

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
        public UnnamedOptionAttribute(bool isRequired = true) :
            base(null, isRequired)
        {
        }

        public UnnamedOptionAttribute(string description, bool isRequired = true) :
            base(description, isRequired)
        {
        }
    }
}
