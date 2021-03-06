﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Utils;

namespace CommandLine
{
    public static partial class Parser
    {
        public static readonly string EntryAssemblyLocation = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location;
        public static readonly string ApplicationName = Path.GetFileNameWithoutExtension(EntryAssemblyLocation);

        public static string ToCommandLine(object optionsObject)
        {
            if(optionsObject == null)
            {
                throw new ArgumentNullException("optionsObject");
            }

            var longOpts = GetOptionsList(optionsObject, true);
            var stringBuilder = new StringBuilder(longOpts.Count * 4);

            foreach (var str in longOpts.Select(longOpt => longOpt.ToString()).Where(str => !String.IsNullOrEmpty(str)))
            {
                stringBuilder.AppendFormat(" {0}", str);
            }

            return stringBuilder.ToString();
        }

        public static string GetUsageString<T>()
        {
            var optionsObjectType = typeof(T);

            IntegrityCheck.Perform(optionsObjectType, GetOptionsList(optionsObjectType));

            var longOpts = GetOptionsList(optionsObjectType);
            var stringBuilder = new StringBuilder(longOpts.Count * 80);

            stringBuilder.AppendFormat(Resources.Strings.Format_Usage, ApplicationName);

            longOpts.Where(longOpt => longOpt.IsUnnamed).Aggregate(stringBuilder, (sb, longOpt) => sb.AppendFormat(" {0}", longOpt.FormatOptional(longOpt.DisplayName)));
            if (longOpts.Any(longOpt => longOpt.IsNamed))
            {
                stringBuilder.Append(Resources.Strings.Format_OptionalOptions);
            }

            stringBuilder.AppendLine();

            var descriptionAttribute = (DescriptionAttribute)optionsObjectType.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();

            stringBuilder.AppendLine(descriptionAttribute != null ? descriptionAttribute.Description : GetDescription<T>(null));
            stringBuilder.AppendLine();

            stringBuilder.AppendLine(Resources.Strings.Message_MandatoryArgs);

            var lines = new List<string>();

            foreach (var longOpt in longOpts.Where(longOpt => !longOpt.IsInternal))
            {
                var optStringBuilder = new StringBuilder(80);

                optStringBuilder.Append(Resources.Strings.Message_OptPadLeft);
                if (longOpt.IsUnnamed)
                {
                    optStringBuilder.AppendFormat(longOpt.FormatOptional(longOpt.DisplayName));
                }
                else
                {
                    optStringBuilder.AppendFormat(longOpt.HasShortOption
                                                  ? (longOpt.IsOptional
                                                      ? Resources.Strings.Format_ShortOptionalAndLongOpt
                                                      : Resources.Strings.Format_ShortAndLongOpt)
                                                  : Resources.Strings.Format_LongOpt,
                                                  longOpt.ShortOptionToString(),
                                                  longOpt.LongOptionToString(),
                                                  longOpt.FormatOptional(longOpt.DisplayArgName));
                    if (!longOpt.IsFlag)
                    {
                        optStringBuilder.AppendFormat(longOpt.FormatOptional("=" + longOpt.DisplayArgName));
                    }
                }

                var description = longOpt.HasDescription
                                    ? longOpt.Description
                                    : GetDescription<T>(longOpt.HasDescriptionKey ? longOpt.DescriptionKey : longOpt.BoundPropertyName);

                optStringBuilder.AppendFormat("{1}{0}", description, ColumnFormatter.Separator);

                if (longOpt.HasDefaultValue && !longOpt.IsNoDefaultValueDescription)
                {
                    optStringBuilder.AppendFormat(Resources.Strings.Format_DefaultValue, longOpt.DefaultValue);
                }

                if (longOpt.IsEnum)
                {
                    var typeConverter = longOpt.TypeConverter;
                    optStringBuilder.AppendFormat(
                        Resources.Strings.Format_Alternatives,
                        String.Join(Resources.Strings.Format_AlternativesSeparator,
                                    Array.ConvertAll(
                                        typeConverter == null
                                            ? Enum.GetNames(longOpt.GetPropertyValue().GetType())
                                            : (typeConverter.GetStandardValues() ?? new object[0])
                                                .Cast<object>()
                                                .Where(o => o is Enum)
                                                .Select(o => o.ToString())
                                                .ToArray()
                                        , x => x.ToLowerInvariant())));
                }

                const string lineSeparator = "\n";

                lines.AddRange(optStringBuilder.Replace(lineSeparator, lineSeparator + ColumnFormatter.Separator).ToString().Split(new[] { lineSeparator }, StringSplitOptions.None));
            }

            stringBuilder.Append(ColumnFormatter.Format(lines));

            return stringBuilder.Replace("\r", String.Empty).Replace("\n", Environment.NewLine).ToString();
        }
    }
}
