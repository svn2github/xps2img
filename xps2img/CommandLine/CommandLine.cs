using System;

using CommandLine;

using Xps2ImgUI.Utils;

namespace Xps2Img.CommandLine
{
    public static class CommandLine
    {
        #region Enums

        public enum ReturnCode
        {
            // OK.
            OK = 0,
            // Errors.
            InvalidArg = 1,
            NoArgs = 2,
            Failed = 3,
            InvalidPages = 4,
            InternalOK = -1
        }

        #endregion

        #region Methods.

        public static Options Parse(string[] args)
        {
            return Parser.Parse<Options>(args);
        }

        public static bool IsUsageDisplayed<T>(string[] args)
        {
            if (Parser.IsUsageRequiested(args))
            {
                Console.WriteLine(String.Format("{1} {2}{0}{0}{3}",
                                    Environment.NewLine,
                                    AssemblyInfo.Description.TrimEnd(new[]{ '.'}),
                                    AssemblyInfo.AssemblyVersion,
                                    Parser.GetUsageString<T>()));
                return true;
            }
            return false;
        }

        public static int DisplayError(Exception ex)
        {
            Console.Error.WriteLine(String.Format("{0}{1}", Resources.Strings.Error_Header, ex
                                                                                            #if !DEBUG
                                                                                            .Message
                                                                                            #endif
            ));
            return (int)(ex is ConversionException ? (ex as ConversionException).ReturnCode : ReturnCode.Failed);
        }

        #endregion
    }
}
