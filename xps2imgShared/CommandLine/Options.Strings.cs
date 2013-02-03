using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        private const string OptionsDescription = "\nConverts XPS (XML Paper Specification) document to set of images.";

        public  const string CategoryParameters = "\tParameters";
        public  const string CategoryOptions    = "Options";
        public  const string EmptyOption        = "\"\"";

        private const string SrcFileDescription       = "XPS file to process";
        public  const string SrcFileDisplayName       = "XPS File";
        private const string SrcFileTabbedDescription = SrcFileDescription + " (required)";

        private const string OutDirDescription = "Output folder\n  new folder named as document will be created in folder where document is by default";
        public  const string OutDirDisplayName = "Output Folder";

        private const string PostActionDescription = "Action to execute after conversion completed";
        public  const string PostActionDisplayName = "After Conversion";
        private const char   PostActionShortOption = 'w';

        private const string PagesDescription = "Page number(s)\n  all pages by default\nSyntax:\n  all:\t\t1-\n  single:\t1\n  set:\t\t1,3\n  range:\t1-10 or -10 or 10-\n  combined:\t1,3-5,7-9,15-";
        private const string PagesDisplayName = "Page Number(s)";
        public  const char   PagesShortOption = 'p';

        private const string FileTypeDescription  = "Image type";
        public  const string FileTypeDisplayName  = "Image Type";
        private const char   FileTypeShortOption  = 'f';
        private const string FileTypeDefaultValue = "png";

        private const string JpegQualityDescription  = "JPEG quality level (10-100)";
        private const string JpegQualityDisplayName  = "JPEG Quality";
        private const char   JpegQualityShortOption  = 'q';
        private const string JpegQualityDefaultValue = "85";

        private const string TiffCompressionDescription  = "TIFF compression method\n  CCITT3, CCITT4 and RLE produce black and white images";
        private const string TiffCompressionDisplayName  = "TIFF Compression";
        private const char   TiffCompressionShortOption  = 't';
        private const string TiffCompressionDefaultValue = "zip";

        private const string RequiredSizeDescription = "Desired image size (greater or equal 10)\n  DPI will be ignored if image size is specified \nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theight for portrait orientation";
        private const string RequiredSizeDisplayName = "Image Size";
        private const char   RequiredSizeOption      = 'r';

        private const string DpiDescription  = "Image DPI (16-2350)\n  DPI will be ignored if image size is specified";
        private const string DpiDisplayName  = "Image DPI";
        private const char   DpiShortOption  = 'd';
        private const string DpiDefaultValue = "120";

        private const string ImageNameDescription = "Image prefix\n  numeric if omitted: 01.png\n  name of src file if empty (-i \"\"): src_file-01.png";
        private const string ImageNameDisplayName = "Image Prefix";
        private const char   ImageNameShortOption = 'i';

        private const string FirstPageIndexDescription  = "Document body first page index";
        private const string FirstPageIndexDisplayName  = "First Page Index";
        private const char   FirstPageIndexShortOption  = 'a';
        private const string FirstPageIndexDefaultValue = "1";

        private const string PrelimsPrefixDescription  = "Preliminaries prefix." + Validation.FileNameCharactersNotAllowed;
        private const string PrelimsPrefixDisplayName  = "Preliminaries Prefix";
        private const char   PrelimsPrefixShortOption  = 'x';
        private const string PrelimsPrefixDefaultValue = "$";

        private const string ShortenExtensionDescription = "Shorten image extension down to three characters";
        private const char   ShortenExtensionOption      = 'n';

        private const string ProcessorsDisplayName       = "Processors";
        private const string ProcessorsTabbedDescription = "Number of simultaneously running document processors\n  number of logical CPUs by default";
        private const string ProcessorsOption            = "processors-number";
        private const int    ProcessorsDefaultValue      = ProcessorsNumberTypeConverter.AutoValue;

        private const string CpuAffinityTabbedDescription = "CPUs processors" + CpuAffinityDescriptionMore;
        private const string CpuAffinityDescription       = "CPUs process" + CpuAffinityDescriptionMore;
        private const string CpuAffinityDescriptionMore   = " will be executed on\n  all by default\nSyntax:\n  all:\t\t0-\n  single:\t0\n  set:\t\t0,2\n  range:\t0-2 or -2 or 2-\n  combined:\t0,2-";
        private const string CpuAffinityDisplayName       = "Processors Affinity";
        private const char   CpuAffinityShortOption       = 'y';
        
        private const string ProcessPriorityDescription       = "Process priority\n  normal by default";
        private const string ProcessPriorityTabbedDescription = "Document processors priority\n  Normal by default";
        private const string ProcessPriorityDisplayName       = "Processors Priority";
        private const char   ProcessPriorityShortOption       = 'c';
        private const string ProcessPriorityDefaultValue      = Validation.AutoValue;

        private const string BatchOption      = "batch";
        private const char   BatchShortOption = 'b';

        private const string SilentModeDescription = "Silent mode (no progress will be shown)";
        private const char   SilentModeShortOption = 's';

        private const string TestDescription = "Test mode (no file operations will be performed)";
        private const string TestDisplayName = "Test Mode";
        private const char   TestShortOption = 'e';

        private const string CleanDescpriction = "Clean (delete images)";
        public  const string CleanOption       = " --clean";

        public  const string CancellationObjectIdsName = "cancellation-object-ids";
    }
}
