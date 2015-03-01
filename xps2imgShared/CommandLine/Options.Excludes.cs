using System.Globalization;
using System.Linq;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static readonly string[] ExcludedOnSave    = { Names.Internal, Names.Batch };
        public static readonly string[] ExcludedUIOptions = { Names.Processors, Names.Batch };
        public static readonly string[] ExcludedOnLaunch  = ExcludedUIOptions.Concat(new[] { ShortOptions.Pages.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView    = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck =
        {
            Properties.ProcessorsNumber,
            Properties.ProcessPriority,
            Properties.PostAction,
            Properties.IgnoreExisting,
            Properties.IgnoreErrors
        };
    }
}
