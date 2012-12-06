using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;

using CommandLine;

using Xps2Img.Shared.Attributes.UI;
using Xps2Img.Shared.Dialogs;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.TypeEditors;
using Xps2Img.Shared.Utils;

using UIOption = Xps2Img.Shared.Attributes.Options.OptionAttribute;
using UIUnnamedOption = Xps2Img.Shared.Attributes.Options.UnnamedOptionAttribute;

// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming

namespace Xps2Img.Shared.CommandLine
{
    [Description("\nConverts XPS (XML Paper Specification) document to set of images.")]
    public class Options
    {
        public Options()
        {
            ReflectionUtils.SetDefaultValues(this);
        }

        public const string CategoryParameters  = "\tParameters";
        public const string CategoryOptions     = "Options";
        public const string EmptyOption         = "\"\"";

        private const string SrcFileDescription = "XPS file to process";

        public const string XPSFileDisplayName = "XPS File";

        [UnnamedOption(SrcFileDescription, ConverterType = typeof(TrimStringTypeConverter))]
        [DisplayName(XPSFileDisplayName)]
        [UIUnnamedOption]
        [Category(CategoryParameters)]
        [Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
        [DefaultValue(null)]
        [TabbedDescription(SrcFileDescription + " (required)")]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string SrcFile { get; set; }

        private const string OutDirDescription = "Output folder\n  new folder named as document will be created in folder where document is by default";

        public const string OutputFolderDisplayName = "Output Folder";

        [UnnamedOption(OutDirDescription, false, ConverterType = typeof(TrimStringTypeConverter))]
        [DisplayName(OutputFolderDisplayName)]
        [UIUnnamedOption(false)]
        [Editor(typeof(SelectXpsFolderEditor), typeof(UITypeEditor))]
        [Category(CategoryParameters)]
        [DefaultValue(null)]
        [TabbedDescription(OutDirDescription)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string OutDir { get; set; }

        public const string PostActionDisplayName   = "After Conversion";
        private const string PostActionDescription  = "Action to execute after conversion completed";

        [DisplayName(PostActionDisplayName)]
        [Category(CategoryParameters)]
        [TypeConverter(typeof(PostActionTypeConverter))]
        [DefaultValue(PostActionTypeConverter.Default)]
        [TabbedDescription(PostActionDescription)]
        public string PostAction
        {
            get; set;
        }

        private const string PagesDescription = "Page number(s)\n  all pages by default\nSyntax:\n  all:\t\t1-\n  single:\t1\n  set:\t\t1,3\n  range:\t1-10 or -10 or 10-\n  combined:\t1,3-5,7-9,15-";
        public  const char PagesShortOption = 'p';

        [Option(
            PagesDescription,
            PagesShortOption,
            DefaultValue = null,
            ConverterType = typeof(PagesTypeConverter),
            ValidationExpression = Validation.PagesValidationExpression
        )]
        [TypeConverter(typeof(PagesTypeConverter))]
        [DisplayName("Page Number(s)")]
        [TabbedDescription(PagesDescription)]
        [Category(CategoryOptions)]
        [UIOption(PagesShortOption)]
        [DefaultValue(typeof(string), null)]
        [Editor(typeof(OrdinalUITypeEditor), typeof(UITypeEditor))]
        public List<Interval> Pages { get; set; }

        public const string FileTypeDisplayName = "Image Type";

        private const string FileTypeDescription = "Image type";
        private const char FileTypeShortOption = 'f';

        [Option(FileTypeDescription, FileTypeShortOption, DefaultValue = "png")]
        [DisplayName(FileTypeDisplayName)]
        [TabbedDescription(FileTypeDescription)]
        [UIOption(FileTypeShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(ImageType.Png)]
        public ImageType FileType { get; set; }

        private const string JpegQualityDescription = "JPEG quality level (10-100)";
        private const char JpegQualityShortOption = 'q';

        [Option(JpegQualityDescription, JpegQualityShortOption, DefaultValue = "85", ValidationExpression = Validation.JpegQualityValidationExpression)]
        [DisplayName("JPEG Quality")]
        [TabbedDescription(JpegQualityDescription)]
        [UIOption(JpegQualityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(85)]
        [DynamicPropertyFilter("FileType", "Jpeg")]
        [TypeConverter(typeof(JpegNullableIntTypeConverter))]
        public int? JpegQuality { get; set; }

        private const string TiffCompressionDescription = "TIFF compression method";
        private const char TiffCompressionShortOption = 't';

        [Option(TiffCompressionDescription, TiffCompressionShortOption, DefaultValue = "zip")]
        [DisplayName("TIFF Compression")]
        [TabbedDescription(TiffCompressionDescription)]
        [UIOption(TiffCompressionShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter("FileType", "Tiff")]
        public TiffCompressOption TiffCompression { get; set; }

        private const string RequiredSizeDescription = "Desired image size\n  DPI will be ignored if image size is specified \nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theight for portrait orientation";
        private const char RequiredSizeOption = 'r';

        [Option(
            RequiredSizeDescription,
            RequiredSizeOption,
            ConverterType = typeof(RequiredSizeTypeConverter),
            ValidationExpression = Validation.RequiredSizeValidationExpression
        )]
        [DisplayName("Image Size")]
        [TabbedDescription(RequiredSizeDescription)]
        [UIOption(RequiredSizeOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedRequiredSizeTypeConverter))]
        public Size? RequiredSize { get; set; }

        private const string DpiDescription = "Image DPI (16-2350)\n  DPI will be ignored if image size is specified";
        private const char DpiShortOption = 'd';

        [Option(DpiDescription, DpiShortOption, DefaultValue = "120", ValidationExpression = Validation.DpiValidationExpression)]
        [DisplayName("Image DPI")]
        [TabbedDescription(DpiDescription)]
        [UIOption(DpiShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(120)]
        [TypeConverter(typeof(CheckedDpiTypeConverter))]
        public int? Dpi { get; set; }

        private const string ImageNameDescription = "Image prefix\n  numeric if omitted: 01.png\n  name of src file if empty (-i \"\"): src_file-01.png";
        private const char ImageNameShortOption = 'i';

        [Option(ImageNameDescription, ImageNameShortOption, ValidationExpression = Validation.ImageNameValidationExpression)]
        [DisplayName("Image Prefix")]
        [TabbedDescription(ImageNameDescription)]
        [UIOption(ImageNameShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedImageNameTypeConverter))]
        public string ImageName { get; set; }

        private const string FirstPageIndexDescription = "Document body first page index";
        private const char FirstPageIndexShortOption = 'a';

        [Option(FirstPageIndexDescription, FirstPageIndexShortOption, DefaultValue = "1", ValidationExpression = Validation.FirstPageIndexValidationExpression)]
        [DisplayName("First Page Index")]
        [TabbedDescription(FirstPageIndexDescription)]
        [UIOption(FirstPageIndexShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(1)]
        [TypeConverter(typeof(CheckedFirstPageIndexTypeConverter))]
        public int? FirstPageIndex { get; set; }

        private const string PrelimsPrefixDescription = "Preliminaries prefix." + Validation.FileNameCharactersNotAllowed;
        private const char PrelimsPrefixShortOption = 'x';
        private const string PrelimsPrefixDefaultValue = "$";

        [Option(PrelimsPrefixDescription, PrelimsPrefixShortOption, DefaultValue = PrelimsPrefixDefaultValue, ValidationExpression = Validation.PrelimsPrefixValidationExpression)]
        [DisplayName("Preliminaries Prefix")]
        [TabbedDescription(PrelimsPrefixDescription)]
        [UIOption(PrelimsPrefixShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(PrelimsPrefixDefaultValue)]
        [TypeConverter(typeof(CheckedPrelimsPrefixTypeConverter))]
        public string PrelimsPrefix { get; set; }

        private const string ShortenExtensionDescription = "Shorten image extension down to three characters";
        private const char ShortenExtensionOption = 'n';

        [Option(ShortenExtensionDescription, ShortenExtensionOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        [UIOption(ShortenExtensionOption)]
        public bool ShortenExtension { get; set; }

        private const string CancellationObjectIdName = "cancellation-object-ids";

        [Option("", ShortOptionType.None10, Flags = OptionFlags.Internal)]
        [UIOption(CancellationObjectIdName)]
        [Browsable(false)]
        public virtual string CancellationObjectIds { get; set; }

        private IEnumerable<string> SyncObjectsNames { get { return CancellationObjectIds.Split('-'); } }

        [Browsable(false)]
        public string CancellationEventName { get { return SyncObjectsNames.First(); } }

        [Browsable(false)]
        public string ParentAppMutexName { get { return SyncObjectsNames.Last(); } }

        private const string ProcessorsDisplayName = "Processors";
        private const string ProcessorsNameDefaultValue = Validation.AutoValue;
        private const string ProcessorsName = "processors-number";

        [Option("", ShortOptionType.None12, DefaultValue = ProcessorsNameDefaultValue, Flags = OptionFlags.Internal)]
        [UIOption(ProcessorsName)]
        [DisplayName(ProcessorsDisplayName)]
        [TabbedDescription("Number of simultaneously running document processors\n  number of logical CPUs by default")]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessorsNumberTypeConverter))]
        [DefaultValue(ProcessorsNameDefaultValue)]
        public string ProcessorsNumber
        {
            get { return _processorsNumber; }

            set
            {
                var processesNumber = 1;

                _processorsNumber =
                    (String.CompareOrdinal(Validation.AutoValue, value) != 0 && !Int32.TryParse(value, out processesNumber)) ||
                    (processesNumber <= 0 || processesNumber > ProcessorsNumberTypeConverter.ProcessorCount)
                        ? Validation.AutoValue
                        : value;
            }
        }

        private string _processorsNumber;

        [Browsable(false)]
        public int ProcessorsNumberAsInt
        {
            get
            {
                return Validation.AutoValue == ProcessorsNumber
                        ? ProcessorsNumberTypeConverter.ProcessorCount
                        : Int32.Parse(ProcessorsNumber);
            }
        }

        private const string ProcessPriorityDisplayName = "Processors Priority";
        private const string ProcessPriorityNameDefaultValue = Validation.AutoValue;
        private const char ShortProcessPriorityName = 'c';

        [Option("", ShortProcessPriorityName, DefaultValue = ProcessPriorityNameDefaultValue)]
        [UIOption(ShortProcessPriorityName)]
        [DisplayName(ProcessPriorityDisplayName)]
        [TabbedDescription("Document processors priority\n  Normal by default")]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessPriorityClassTypeConverter))]
        [DefaultValue(ProcessPriorityNameDefaultValue)]
        public string ProcessPriority
        {
            get { return _processPriority; }

            set
            {
                _processPriority =
                    String.CompareOrdinal(Validation.AutoValue, value) != 0 &&
                    !Enum.IsDefined(typeof(ProcessPriorityClass), value.RemoveSpaces())
                        ? Validation.AutoValue
                        : value;
            }
        }

        private string _processPriority;
        
        [Browsable(false)]
        public ProcessPriorityClass ProcessorsPriorityAsEnum
        {
            get
            {
                var processorsPriority = ProcessPriority.RemoveSpaces();
                return Enum.IsDefined(typeof(ProcessPriorityClass), processorsPriority)
                        ? (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), processorsPriority)
                        : ProcessPriorityClass.Normal;
            }
        }

        public static readonly string[] ExcludedOnSave = new[] { CancellationObjectIdName, BatchName };
        public static readonly string[] ExcludedUIOptions = new[] { ProcessorsName, BatchName };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { PagesShortOption.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck = new[] { ProcessorsDisplayName, ProcessPriorityDisplayName, PostActionDisplayName };

        private const string BatchName = "batch";
        private const char BatchShortOption = 'b';

        private bool _batch;

        [Browsable(false)]
        [Option("", BatchShortOption, ArgumentExpectancy.No)]
        [UIOption(BatchName)]
        [DefaultValue(false)]
        public bool Batch
        {
            get { return _batch; }
            set
            {
                _batch = value;
                if (_batch)
                {
                    PostAction = PostActionTypeConverter.ChooseAction(PostAction, PostActionTypeConverter.Exit);
                }
            }
        }

#if NOT_READY
        private const string CpuAffinityDescription =
            "CPUs process" +
            #if XPS2IMG_UI
            "ors" + 
            #endif
            " will be executed on\n  all by default\nSyntax:\n  all:\t\t0-\n  single:\t0\n  set:\t\t0,2\n  range:\t0-2 or -2 or 2-\n  combined:\t0,2-";

        private const char CpuAffinityShortOption = 'y';

        [Option(CpuAffinityDescription, CpuAffinityShortOption, ValidationExpression = Validation.CpuAffinityValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Processors Affinity")]
        [TabbedDescription(CpuAffinityDescription)]
        [UIOption(CpuAffinityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(Validation.AutoValue)]
        [Editor(typeof(CpuAffinityUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CpuAffinityTypeConverter))]
        public IntPtr CpuAffinity { get; set; }
#if dfdf
        public string CpuAffinity
        {
            get { return _cpuAffinity; }
            set
            {
                ValidateProperty(value, CpuAffinityValidationExpression);
                value = value.Trim();
                _cpuAffinity = String.IsNullOrEmpty(value) || String.Compare(value, AutoValue, StringComparison.InvariantCultureIgnoreCase) == 0
                                    ? AutoValue
                                    : IntervalUtils.ToString(Interval.Parse(value));
            }
        }
        private string _cpuAffinity;
#endif
        #else
        public string CpuAffinity { get; set; }
        #endif

        #if !XPS2IMG_UI

        public IntPtr ActualCpuAffinity
        {
            get
            {
                if(String.IsNullOrEmpty(CpuAffinity) || CpuAffinity == AutoValue)
                {
                    return IntPtr.Zero;
                }

                var bitArray = Interval.Parse(CpuAffinity).ToBitArray();

                var bitIndex = 0;
                var affinityMask = bitArray.Cast<bool>().TakeWhile(_ => bitIndex < 64).Aggregate(0L, (_, bit) => _ | ((bit ? 1L : 0L) << bitIndex++));

                return new IntPtr(affinityMask & ((1 << Environment.ProcessorCount) - 1));
            }
        }

#endif
#endif

        private const string SilentModeDescription = "Silent mode (no progress will be shown)";
        private const char SilentModeShortOption = 's';

        [Option(SilentModeDescription, SilentModeShortOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        public virtual bool Silent { get; set; }

        private const string TestDescription = "Test mode (no file operations will be performed)";
        private const char TestShortOption = 'e';

        [Option(TestDescription, TestShortOption, ArgumentExpectancy.No)]
        [DisplayName("Test Mode")]
        [TabbedDescription(TestDescription)]
        [UIOption(TestShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool Test { get; set; }

        [Option("Clean (delete images)", ShortOptionType.None1, ArgumentExpectancy.No)]
        [Browsable(false)]
        public bool Clean { get; set; }

        public const string CleanOption = " --clean";
    }
}
