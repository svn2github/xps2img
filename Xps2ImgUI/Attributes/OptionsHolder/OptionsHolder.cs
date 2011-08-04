using System;
using System.Linq;
using System.Collections.Generic;

using Xps2ImgUI.Attributes.Options;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Attributes.OptionsHolder
{
    public class OptionsHolder
    {
        public readonly List<BaseOptionAttribute> OptionAttributes;

        public object OptionsObject { get; private set; }

        protected OptionsHolder(object optionsObject, List<BaseOptionAttribute> optionAttributes)
        {
            OptionsObject = optionsObject;

            OptionAttributes = optionAttributes;
        }

        public static OptionsHolder Create(object optionsObject)
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

            return new OptionsHolder(optionsObject, optionAttributes);
        }

        public string FormatCommandLine()
        {
            return FormatCommandLine(false);
        }

        public string FormatCommandLine(bool exceptionIfNoRequired)
        {
            return OptionsFormatter.FormatCommandLine(exceptionIfNoRequired, OptionsObject, OptionAttributes);
        }

        public string FirstRequiredOptionLabel
        {
            get
            {
                var requiredAttribute = OptionAttributes.FirstOrDefault(a => a.IsRequired);
                return requiredAttribute != null && String.IsNullOrEmpty(OptionsFormatter.GetOptionValue(false, requiredAttribute, OptionsObject)) ? requiredAttribute.Name : String.Empty;
            }
        }
    }
}
