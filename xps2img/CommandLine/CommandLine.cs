using System;
using System.IO;

using CommandLine;

using Xps2ImgUI.Utils;

namespace Xps2Img.CommandLine
{
    public static partial class CommandLine
    {
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
                                    AssemblyInfo.Description.TrimEnd(new[]{ '.' }),
                                    AssemblyInfo.AssemblyVersion,
                                    Parser.GetUsageString<T>()));
                return true;
            }
            return false;
        }

        private static string GetExceptionHint<T>(Exception ex, string message) where T: Exception
        {
            return ex is T || ex.InnerException is T ? message : null;
        }

        public static string GetExceptionHint(Exception ex)
        {
            return
                GetExceptionHint<IOException>(ex, Resources.Strings.Message_DiskStorage) ??
                GetExceptionHint<UnauthorizedAccessException>(ex, Resources.Strings.Message_FileAccess) ??
                GetExceptionHint<OutOfMemoryException>(ex, Resources.Strings.Message_Memory) ??
                String.Empty;
        }

        public static int DisplayError(Exception ex)
        {
            var exceptionHint = GetExceptionHint(ex);

            Console.Error.WriteLine(String.Format("{0}{1}" + (String.IsNullOrEmpty(exceptionHint) ? String.Empty : " {2}"),
                                    Resources.Strings.Error_Header,
                                    ex
                                    #if !DEBUG
                                    .Message
                                    #endif
                                    , exceptionHint));

            return ex is ConversionException ? (ex as ConversionException).ReturnCode : ReturnCode.Failed;
        }

        #endregion
    }
}
