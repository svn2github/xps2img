using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Resources;

namespace CommandLine
{
  public static partial class Parser
  {
    public static readonly string EntryAssemblyLocation = Assembly.GetEntryAssembly().Location;
    public static readonly string ApplicationName       = Path.GetFileNameWithoutExtension(EntryAssemblyLocation);

    public static string ToCommandLine(object optionsObject)
    {
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
      var optionsObjectType = typeof (T);

      IntegrityCheck.Perform(optionsObjectType, GetOptionsList(optionsObjectType));
      
      var longOpts = GetOptionsList(optionsObjectType);
      var stringBuilder = new StringBuilder(longOpts.Count * 80);

      stringBuilder.AppendFormat(Strings.Format_Usage, ApplicationName);
      
      longOpts.Where(longOpt => longOpt.IsUnnamed).Aggregate(stringBuilder, (sb, longOpt) => sb.AppendFormat(" {0}", longOpt.FormatOptional(longOpt.DisplayName)));
      if(longOpts.Where(longOpt => longOpt.IsNamed).Any())
      {
        stringBuilder.Append(Strings.Format_OptionalOptions);
      }

      stringBuilder.AppendLine();
      
      var description = (DescriptionAttribute)optionsObjectType.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
      if (description != null)
      {
        stringBuilder.AppendLine(description.Description);
        stringBuilder.AppendLine();
      }

      stringBuilder.AppendLine(Strings.Message_MandatoryArgs);

      var lines = new List<string>();
     
      foreach (var longOpt in longOpts.Where(longOpt => !longOpt.IsInternal))
      {
        var optStringBuilder = new StringBuilder(80);
        
        optStringBuilder.Append(Strings.Message_OptPadLeft);
        if (longOpt.IsUnnamed)
        {
          optStringBuilder.AppendFormat(longOpt.FormatOptional(longOpt.DisplayName));
        }
        else
        {
          optStringBuilder.AppendFormat(longOpt.HasShortOption
                                        ? (longOpt.IsOptional
                                            ? Strings.Format_ShortOptionalAndLongOpt
                                            : Strings.Format_ShortAndLongOpt)
                                        : Strings.Format_LongOpt,
                                        longOpt.ShortOptionToString(),
                                        longOpt.LongOptionToString(),
                                        longOpt.FormatOptional(longOpt.DisplayArgName));
          if (!longOpt.IsFlag)
          {
            optStringBuilder.AppendFormat(longOpt.FormatOptional("=" + longOpt.DisplayArgName));  
          }
        }

        optStringBuilder.AppendFormat("{1}{0}", longOpt.Description ?? String.Empty, ColumnFormatter.Separator);

        if (longOpt.HasDefaultValue)
        {
          optStringBuilder.AppendFormat(Strings.Format_DefaultValue, longOpt.DefaultValue);
        }

        if (longOpt.IsEnum)
        {
          optStringBuilder.AppendFormat(
            Strings.Format_Alternatives,
            String.Join(Strings.Format_AlternativesSeparator,
              Array.ConvertAll(Enum.GetNames(longOpt.GetPropertyValue().GetType()), x => x.ToLowerInvariant())
            )
          );
        }

        const string lineSeparator = "\n";
    
        lines.AddRange(optStringBuilder.Replace(lineSeparator, lineSeparator + ColumnFormatter.Separator).ToString().Split(new [] { lineSeparator }, StringSplitOptions.None));
      }

      stringBuilder.Append(ColumnFormatter.Format(lines));

      return stringBuilder.Replace("\r", String.Empty).Replace("\n", Environment.NewLine).ToString();
    }
  }
}
