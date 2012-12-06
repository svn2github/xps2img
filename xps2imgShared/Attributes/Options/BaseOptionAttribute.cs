using System;
using System.ComponentModel;
using System.Reflection;

using CommandLine;

namespace Xps2Img.Shared.Attributes.Options
{
    public class BaseOptionAttribute : Attribute
    {
        public string Name { get; set; }

        public readonly bool IsArgRequired;
        public readonly bool IsRequired;
        public readonly bool IsNamed;
        public readonly bool IsUnnamed;
        public readonly bool IsLongName;

        private PropertyInfo _propertyInfo;

        private static readonly char[] ObligatoryMark = new[] { '*' };

        public PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }

            set
            {
                _propertyInfo = value;
                if (!String.IsNullOrEmpty(Name))
                {
                    return;
                }

                Name = _propertyInfo.Name;

                var displayNameAttribute = _propertyInfo.FirstOrDefaultAttribute<DisplayNameAttribute>();
                if (displayNameAttribute != null)
                {
                    Name = displayNameAttribute.DisplayName.TrimEnd(ObligatoryMark);
                    return;
                }

                var descriptionAttribute = _propertyInfo.FirstOrDefaultAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    Name = descriptionAttribute.Description;
                }
            }
        }

        public BaseOptionAttribute(bool isRequired, string name) :
            this(isRequired, name, true)
        {
        }

        public BaseOptionAttribute(bool isRequired, string name, bool isArgRequired)
        {
            Name = name ?? String.Empty;
            IsRequired = isRequired;
            IsArgRequired = isArgRequired;
            IsUnnamed = String.IsNullOrEmpty(Name);
            IsNamed = !IsUnnamed;
            IsLongName = IsNamed && Name.Length > 1;
        }
    }
}
