using System;
using System.Collections.Generic;
using System.Linq;

using Gnu.Getopt;

namespace CommandLine.Utils
{
    internal static class LongOptUtils
    {
        public static string BuildOptString(this IEnumerable<LongOpt> longOpts)
        {
            return longOpts.Aggregate(String.Empty,
              (current, longOpt) =>
                current + ((char)longOpt.Val +
                            (longOpt.HasArg == Argument.Required ? ":" :
                                longOpt.HasArg == Argument.Optional ? "::" : String.Empty)));
        }
    }
}
