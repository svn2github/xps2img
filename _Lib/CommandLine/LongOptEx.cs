using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using CommandLine.Validation;

using Gnu.Getopt;

namespace CommandLine
{
    internal class LongOptEx : LongOpt
    {
        private const string Unnamed = "UnnamedE7CD7C04EA3D40178BBB8AB31CE75FDA";

        public static string GetLongName(string name)
        {
            var attrName = String.Empty;
            Array.ForEach(name.ToCharArray(), c => attrName += (Char.IsUpper(c) ? (String.IsNullOrEmpty(attrName) ? "" : "-") + Char.ToLower(c).ToString(CultureInfo.InvariantCulture) : c.ToString(CultureInfo.InvariantCulture)));
            return attrName;
        }

        public LongOptEx(string description, bool isRequired) :
            this(description, Unnamed, ShortOptionType.Auto, isRequired ? ArgumentExpectancy.Required : ArgumentExpectancy.Optional)
        {
        }

        public LongOptEx(string description, string name) :
            this(description, name, ArgumentExpectancy.Required)
        {
        }

        public LongOptEx(string description, ArgumentExpectancy hasArg) :
            this(description, String.Empty, hasArg)
        {
        }

        public LongOptEx(string description, string name, ArgumentExpectancy hasArg) :
            this(description, name, ShortOptionType.Auto, hasArg)
        {
        }

        public LongOptEx(string description, string name, char shortOption) :
            this(description, name, shortOption, ArgumentExpectancy.Required)
        {
        }

        public LongOptEx(string description, string name, char shortOption, ArgumentExpectancy hasArg) :
            base(name, (Argument)hasArg, null,
                name == Unnamed ? ShortOptionType.Auto :
                  shortOption != ShortOptionType.Auto ? shortOption :
                    String.IsNullOrEmpty(name) ? ShortOptionType.Auto : Char.ToLower(name[0]))
        {
            Description = description;
        }

        public void SetPropertyValue(string value)
        {
            SetPropertyValue(value, null);
        }

        public void SetPropertyValue(string value, Action<LongOptEx, string> validationErrorAction)
        {
            try
            {
                if (value != null && Validator != null)
                {
                    Validator.Validate(value);
                }

                var propertyInfo = BoundObject.GetType().GetProperty(BoundPropertyName);
                var typeConverter = ReflectionUtils.GetTypeConverter(propertyInfo);

                propertyInfo.SetValue(BoundObject, typeConverter.ConvertFromInvariantString(IsFlag ? "true" : value ?? DefaultValue), null);
            }
            catch (Exception ex)
            {
                if (validationErrorAction != null)
                {
                    validationErrorAction(this, ex.Message);
                }
            }
        }

        public object GetPropertyValue()
        {
            var propertyInfo = BoundObject.GetType().GetProperty(BoundPropertyName);
            var propertyValue = propertyInfo.GetValue(BoundObject, null);
            return propertyValue;
        }

        public readonly string Description;

        public string BoundPropertyName { get; set; }

        public string DisplayName { get { return IsUnnamed && !String.IsNullOrEmpty(BoundPropertyName) ? GetLongName(BoundPropertyName) : Name; } }
        public string DisplayArgName { get { return Name.Split(new[] { '-' }).Last(); } }

        public object BoundObject { get; set; }

        internal IValidator Validator { get; set; }

        public string DefaultValue { get; set; }
        public bool HasDefaultValue { get { return !String.IsNullOrEmpty(DefaultValue); } }

        public TypeConverter TypeConverter { get; set; }
        public bool IsEnum { get { return TypeConverter.GetType() == typeof(EnumConverter); } }

        public bool IsShortOptionAuto { get { return Val == ShortOptionType.Auto; } }
        public bool IsShortOptionNone { get { return Val < ShortOptionType.Auto; } }
        public bool HasShortOption { get { return !IsShortOptionAuto && !IsShortOptionNone; } }

        public bool IsUnnamed { get { return Name == Unnamed; } }
        public bool IsNamed { get { return !IsUnnamed; } }

        public bool IsFlag { get { return HasArg == (Argument)ArgumentExpectancy.No; } }
        public bool IsRequired { get { return HasArg == (Argument)ArgumentExpectancy.Required; } }
        public bool IsOptional { get { return HasArg == (Argument)ArgumentExpectancy.Optional; } }

        public bool IsInternal { get; set; }

        public string ShortOptionToString() { return HasShortOption ? String.Format("-{0}", Convert.ToChar(Val)) : String.Empty; }
        public string LongOptionToString() { return String.Format("--{0}", Name); }

        public string OptionNameToString() { return IsUnnamed ? DisplayName : String.Format(HasShortOption ? "{0}, {1}" : "{1}", ShortOptionToString(), LongOptionToString()); }

        public override string ToString()
        {
            Func<object, string> formatParam = x => (x != null) ? String.Format("\"{0}\"", TypeConverter.ConvertToInvariantString(x)) : String.Empty;

            var propertyValue = GetPropertyValue();
            var propertyValueFormatted = formatParam(propertyValue);

            // Unnamed
            if (IsUnnamed)
            {
                return propertyValueFormatted;
            }

            // Named.
            var optionName = HasShortOption ? ShortOptionToString() : LongOptionToString();

            if (IsFlag)
            {
                return Convert.ToBoolean(propertyValue) ? optionName : String.Empty;
            }

            if ((IsRequired || IsOptional) && propertyValue == null)
            {
                return String.Empty;
            }

            return String.Format("{0}{1}{2}",
                    optionName,
                    HasShortOption ? (IsOptional ? "" : " ") : "=",
                    propertyValueFormatted);
        }

        public string FormatOptional(string text)
        {
            return String.Format(IsRequired ? "{0}" : "[{0}]", text);
        }
    }
}
