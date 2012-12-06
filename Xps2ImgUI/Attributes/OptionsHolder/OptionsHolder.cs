using System;
using System.Linq;
using System.Collections.Generic;

using CommandLine;

using Xps2Img.Shared.Attributes.Options;

namespace Xps2ImgUI.Attributes.OptionsHolder
{
    public class OptionsHolder<T> where T: class
    {
        public List<BaseOptionAttribute> OptionAttributes;

        private T _optionsObject;

        public T OptionsObject
        {
            get { return _optionsObject; }
            set { SetOptionsObject(value); }
        }

        public void SetOptionsObject(T optionsObject)
        {
            var optionAttributes = new List<BaseOptionAttribute>();

            ReflectionUtils.ForEachPropertyInfo(
                optionsObject,
                propertyInfo =>
                {
                    var optionAttribute = propertyInfo.FirstOrDefaultAttribute<BaseOptionAttribute>();
                    if (optionAttribute != null)
                    {
                        optionAttribute.PropertyInfo = propertyInfo;
                        optionAttributes.Add(optionAttribute);
                    }
                }
            );

            OptionAttributes = optionAttributes;
            _optionsObject = optionsObject;
            if (OptionsObjectChanged != null)
            {
                OptionsObjectChanged(this, EventArgs.Empty);
            }
        }

        public string FormatCommandLine(params string[] optionsToExclude)
        {
            return FormatCommandLine(false, optionsToExclude);
        }

        public string FormatCommandLine(bool exceptionIfNoRequired, params string[] optionsToExclude)
        {
            return OptionsFormatter.FormatCommandLine(exceptionIfNoRequired, OptionsObject, OptionAttributes, optionsToExclude);
        }

        public string FirstRequiredOptionLabel
        {
            get
            {
                var requiredAttribute = OptionAttributes.FirstOrDefault(a => a.IsRequired);
                return requiredAttribute != null && String.IsNullOrEmpty(OptionsFormatter.GetOptionValue(false, requiredAttribute, OptionsObject)) ? requiredAttribute.Name : String.Empty;
            }
        }

        public event EventHandler OptionsObjectChanged;
    }
}
