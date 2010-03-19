using System;
using System.Diagnostics;

using CommandLine;

namespace Xps2Img.CommandLine
{
    public static class CommandLine
    {     
      #region Enums

      public enum ReturnCode
      {
          // OK.
          OK              = 0,
          // Errors.
          InvalidArg      = 1,
          NoArgs          = 2,
          Failed          = 3,
          
          ChildCompleted  = 100,
          OutOfMemorySign = -1
      }

      #endregion

      #region Methods.
      
      public static Options Parse(string[] args)
      {
        return Parser.Parse<Options>(args, Parser.ApplicationName);
      }

      public static bool IsUsageDisplayed<T>(string[] args)
      {
        if (Parser.IsUsageRequiested(args))
        {
          Console.WriteLine(Parser.GetUsageString<T>());
          return true;
        }
        return false;
      }
      
      public static int DisplayError(Exception ex)
      {
        Console.Error.WriteLine(String.Format("{0}{1}", Resources.Strings.Error_Header, ex.Message));
        return (int)ReturnCode.Failed;
      }

      public static int MakeOutOfMemoryRetCode(this Converter xps2Img)
      {
        var xml = xps2Img.ConverterState.Serialize();
        Trace.WriteLine(xml);
        Console.WriteLine(xml);
        return (int)ReturnCode.OutOfMemorySign;
      }

      public static ConverterState ConverterState { get; private set; }
      
      public static bool GetProcessExitState(string output)
      {
        if (ConverterState.IsSerializedTo(output))
        {
          ConverterState = ConverterState.Deserialize(output);
          return true;
        }
        return false;
      }

      #endregion
    }
}
