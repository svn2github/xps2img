using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CommandLine
{
  internal class IntegrityCheck
  {
    [Conditional("DEBUG")]
    public static void Perform(Type optionsObjectType, List<LongOptEx> longOpts)
    {
      Func<LongOptEx, string> formatLOE = longOptEx =>
        String.Format(longOptEx.IsUnnamed ? "{{ BoundPropertyName: \"{2}\" }}" : "{{ Name: \"{0}\", Val: '{1}', BoundPropertyName: \"{2}\" }}",
                      longOptEx.Name, (char)longOptEx.Val, longOptEx.BoundPropertyName);
      
      var errors = new List<string>();

      LongOptEx unnamedOptionalOption = null;
      
      for(var i = 0; i < longOpts.Count; i++)
      {
        var longOptEx = longOpts[i];
        var isUnnamed = longOptEx.IsUnnamed;

        if (longOptEx.TypeConverter == null)
        {
          errors.Add(String.Format("'TypeConverter' should not be 'null' for option {0}", formatLOE(longOptEx)));
        }

        if (longOptEx.BoundObject == null)
        {
          errors.Add(String.Format("'BoundObject' should not be 'null' for option {0}", formatLOE(longOptEx)));
        }
        
        // Unnamed property.
        if (isUnnamed)
        {
          if (longOptEx.HasShortOption)
          {
            errors.Add(String.Format("'Val' should not be set for unnamed option {0}", formatLOE(longOptEx)));
          }
          
          // Unnamed required order.
          if(longOptEx.IsRequired)
          {
            if(unnamedOptionalOption != null)
            {
              errors.Add(String.Format(
                           "Unnamed required option {0} should be specified before unnamed optional option {1}",
                           formatLOE(longOptEx), formatLOE(unnamedOptionalOption)));
            }
          }
          else
          {
            unnamedOptionalOption = longOptEx;
          }
        }

        // Named property.
        if (String.IsNullOrEmpty(longOptEx.Name))
        {
          errors.Add(String.Format("'Name' is not set for {0}", formatLOE(longOptEx)));
        }

        if (!isUnnamed && !longOptEx.HasShortOption)
        {
          errors.Add(String.Format("'Val' is not set for {0}", formatLOE(longOptEx)));
        }

        if (String.IsNullOrEmpty(longOptEx.BoundPropertyName))
        {
          errors.Add(String.Format("'BoundPropertyName' is not set for {0}", formatLOE(longOptEx)));
        }
        else
        {
          // Property access.
          var propertyInfo = optionsObjectType.GetProperty(longOptEx.BoundPropertyName, BindingFlags.Instance | BindingFlags.Public);

          if (propertyInfo == null)
          {
            errors.Add(String.Format("Property '{0}' either does not exist in class '{1}' or non-public or static. Valid property declaration is: public PropName {{ get; set; }}", longOptEx.BoundPropertyName, optionsObjectType.Name));
          }
          else
          {
            if (propertyInfo.GetGetMethod() == null || propertyInfo.GetSetMethod() == null)
            {
              errors.Add(String.Format("Public {0} should be specified for property '{1}.{2}'",
                                       propertyInfo.GetSetMethod() == null ? "setter" : "getter",
                                       optionsObjectType.Name, propertyInfo.Name));
            }
          }
        }
        
        // Values uniquity check.
        for (var j = i + 1; j < longOpts.Count; j++)
        {
          var longOptExInt = longOpts[j];

          if (!isUnnamed && longOptEx.Name == longOptExInt.Name)
          {
            errors.Add(String.Format("'Name' is identical for {0} and {1}", formatLOE(longOptEx), formatLOE(longOptExInt)));
          }

          if (!isUnnamed && longOptEx.Val == longOptExInt.Val)
          {
            errors.Add(String.Format("'Val' is identical for {0} and {1}", formatLOE(longOptEx), formatLOE(longOptExInt)));
          }

          if (longOptEx.BoundPropertyName == longOptExInt.BoundPropertyName)
          {
            errors.Add(String.Format("'BoundPropertyName' is identical for {0} and {1}", formatLOE(longOptEx), formatLOE(longOptExInt)));
          }
        }
      }
      
      if(errors.Any())
      {
        throw new ArgumentException(String.Join(Environment.NewLine, errors.ToArray()));
      }
    }
  }
}
