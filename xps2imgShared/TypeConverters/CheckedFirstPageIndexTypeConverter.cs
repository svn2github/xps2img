using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class CheckedFirstPageIndexTypeConverter : CheckedNullableIntTypeConverter
    {
        public CheckedFirstPageIndexTypeConverter()
            : base(Options.ValidationExpressions.FirstPageIndex)
        {
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Validate(value);

            var strVal = value as string;

            if (!String.IsNullOrEmpty(strVal))
            {
                var match = Regex.Match(strVal, Options.ValidationExpressions.Regexps.FirstPageIndex);
                if (match.Success)
                {
                    var first = int.Parse(match.Groups["first"].Value, CultureInfo.InvariantCulture);

                    int second;
                    var hasSecond = int.TryParse(match.Groups["second"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out second);

                    var result = Math.Abs(second - first) + Convert.ToInt32(hasSecond);

                    Validate(result.ToString(CultureInfo.InvariantCulture));

                    return result;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
