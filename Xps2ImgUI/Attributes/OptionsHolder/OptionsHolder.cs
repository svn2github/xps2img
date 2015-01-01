using System;
using System.Linq;
using System.Collections.Generic;

using CommandLine.Utils;

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

        public string FormatCommandLine(bool exceptionIfNoRequired, bool includeFiltered, string[] optionsToExclude)
        {
            return OptionsFormatter.FormatCommandLine(exceptionIfNoRequired, includeFiltered, OptionsObject, OptionAttributes, optionsToExclude);
        }

        public string FirstRequiredPropertyName
        {
            get
            {
                var requiredAttribute = OptionAttributes.FirstOrDefault(a => a.IsRequired);
                return requiredAttribute != null && String.IsNullOrEmpty(OptionsFormatter.GetOptionValue(false, false, requiredAttribute, OptionsObject)) ? requiredAttribute.Name : String.Empty;
            }
        }

        public event EventHandler OptionsObjectChanged;
    }
}
