using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public  const string CategoryParameters = "\tParameters";
        public  const string CategoryOptions    = "Options";
        public  const string EmptyOption        = "\"\"";

        private const char   PostActionShortOption = 'w';

        public  const char   PagesShortOption = 'p';

        private const char   FileTypeShortOption  = 'f';
        private const string FileTypeDefaultValue = "png";

        private const char   JpegQualityShortOption  = 'q';
        private const string JpegQualityDefaultValue = "85";

        private const char   TiffCompressionShortOption  = 't';
        private const string TiffCompressionDefaultValue = "zip";

        private const char   RequiredSizeOption      = 'r';

        private const char   DpiShortOption  = 'd';
        private const string DpiDefaultValue = "120";

        private const char   ImageNameShortOption = 'i';

        private const char   FirstPageIndexShortOption  = 'a';
        private const string FirstPageIndexDefaultValue = "1";

        private const char   PrelimsPrefixShortOption  = 'x';
        private const string PrelimsPrefixDefaultValue = "$";

        private const char   ShortenExtensionOption      = 'n';

        private const string ProcessorsOption            = "processors-number";
        private const int    ProcessorsDefaultValue      = ProcessorsNumberTypeConverter.AutoValue;

        private const char   CpuAffinityShortOption       = 'y';
        
        private const char   ProcessPriorityShortOption       = 'c';
        private const string ProcessPriorityDefaultValue      = Validation.AutoValue;

        private const string BatchOption      = "batch";
        private const char   BatchShortOption = 'b';

        private const char   SilentModeShortOption = 's';

        private const char   IgnoreExistingShortOption = 'k';

        private const char   IgnoreErrorsShortOption = 'g';

        private const char   TestShortOption = 'e';

        public  const string CleanOption       = " --clean";

        public  const string CancellationObjectIdsName = "cancellation-object-ids";
    }
}
