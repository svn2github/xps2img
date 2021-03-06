﻿using System;
using System.ComponentModel;
using System.Reflection;

using CommandLine.Utils;

namespace Xps2Img.Shared.Attributes.Options
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BaseOptionAttribute : Attribute
    {
        public string Name { get; set; }
        public bool AlwaysFormat { get; set; }

        public bool IsArgRequired { get; private set; }
        public bool IsRequired { get; private set; }
        public bool IsNamed { get; private set; }
        public bool IsUnnamed { get; private set; }
        public bool IsLongName { get; private set; }

        private PropertyInfo _propertyInfo;

        private static readonly char[] ObligatoryMark = { '*' };

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

        public BaseOptionAttribute(bool isRequired, string name, bool isArgRequired = true)
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
