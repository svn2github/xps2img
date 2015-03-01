using System;
using System.Globalization;
using System.IO;
using System.Windows.Markup;

using CommandLine;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.Internal;
using Xps2Img.Shared.Utils.System;

using ReturnCode = Xps2Img.Shared.CommandLine.CommandLine.ReturnCode;

// ReSharper disable LocalizableElement

namespace Xps2Img.CommandLine
{
    public static class CommandLine
    {
        #region Methods.

        public static Options Parse(string[] args)
        {
            return Parser.Parse<Options>(args);
        }

        public static bool IsUsageDisplayed<T>(string[] args)
        {
            if (!Parser.IsUsageRequested(args))
            {
                return false;
            }

            Console.WriteLine("{1} {2}{0}{0}{3}",
                              Environment.NewLine,
                              AssemblyInfo.Description.TrimEnd(new[]{ '.' }),
                              AssemblyInfo.AssemblyVersion,
                              Parser.GetUsageString<T>());

            return true;
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

        public static int DisplayError(Exception ex, bool launchedAsInternal)
        {
            var exceptionHint = GetExceptionHint(ex);

            var conversionException = ex as ConversionException;

            var page = launchedAsInternal && conversionException != null && conversionException.InnerException is XamlParseException
                        ? conversionException.ContextData
                        : -1;

            var exceptionMessage = ex
                                    #if DEBUG
                                    .ToString();
                                    #else
                                    .Message;
                                    #endif

            var hintFormat = String.IsNullOrEmpty(exceptionHint)
                               ? String.Empty
                               : String.Format((launchedAsInternal ? "{0}{0}" : "\x20") + "{{2}}", Environment.NewLine);

            var message = String.Format("{0}: {1}" + hintFormat,
                                        launchedAsInternal
                                            ? page.ToString(CultureInfo.InvariantCulture)
                                            : Resources.Strings.Error_Header,
                                        exceptionMessage,
                                        exceptionHint);

            Console.Error.WriteLine(ProcessOutput.Encode(message, launchedAsInternal));

            return !launchedAsInternal || conversionException == null
                    ? ReturnCode.Failed
                    : page != -1
                        ? page
                        : conversionException.ContextData;
        }

        #endregion
    }
}
