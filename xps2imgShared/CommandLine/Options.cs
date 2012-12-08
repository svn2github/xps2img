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

namespace Xps2Img.Shared.CommandLine
{
    [Description(OptionsDescription)]
    public partial class Options
    {
        public Options()
        {
            ReflectionUtils.SetDefaultValues(this);
        }

        [UnnamedOption(SrcFileDescription, ConverterType = typeof(TrimStringTypeConverter))]
        [DisplayName(SrcFileDisplayName)]
        [UIUnnamedOption]
        [Category(CategoryParameters)]
        [Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
        [DefaultValue(null)]
        [TabbedDescription(SrcFileTabbedDescription)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string SrcFile { get; set; }

        [UnnamedOption(OutDirDescription, false, ConverterType = typeof(TrimStringTypeConverter))]
        [DisplayName(OutDirDisplayName)]
        [UIUnnamedOption(false)]
        [Editor(typeof(SelectXpsFolderEditor), typeof(UITypeEditor))]
        [Category(CategoryParameters)]
        [DefaultValue(null)]
        [TabbedDescription(OutDirDescription)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string OutDir { get; set; }

        [DisplayName(PostActionDisplayName)]
        [Category(CategoryParameters)]
        [TypeConverter(typeof(PostActionTypeConverter))]
        [DefaultValue(PostActionTypeConverter.Default)]
        [TabbedDescription(PostActionDescription)]
        public string PostAction
        {
            get; set;
        }

        [Option(PagesDescription, PagesShortOption, DefaultValue = null, ConverterType = typeof(PagesTypeConverter), ValidationExpression = Validation.PagesValidationExpression)]
        [DisplayName(PagesDisplayName)]
        [TabbedDescription(PagesDescription)]
        [Category(CategoryOptions)]
        [UIOption(PagesShortOption)]
        [DefaultValue(typeof(string), null)]
        [Editor(typeof(OrdinalUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PagesTypeConverter))]
        public List<Interval> Pages { get; set; }

        [Option(FileTypeDescription, FileTypeShortOption, DefaultValue = FileTypeDefaultValue)]
        [DisplayName(FileTypeDisplayName)]
        [TabbedDescription(FileTypeDescription)]
        [UIOption(FileTypeShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(ImageType.Png)]
        public ImageType FileType { get; set; }

        [Option(JpegQualityDescription, JpegQualityShortOption, DefaultValue = JpegQualityDefaultValue, ValidationExpression = Validation.JpegQualityValidationExpression)]
        [DisplayName(JpegQualityDisplayName)]
        [TabbedDescription(JpegQualityDescription)]
        [UIOption(JpegQualityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(85)]
        [DynamicPropertyFilter("FileType", "Jpeg")]
        [TypeConverter(typeof(JpegNullableIntTypeConverter))]
        public int? JpegQuality { get; set; }

        [Option(TiffCompressionDescription, TiffCompressionShortOption, DefaultValue = TiffCompressionDefaultValue)]
        [DisplayName(TiffCompressionDisplayName)]
        [TabbedDescription(TiffCompressionDescription)]
        [UIOption(TiffCompressionShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter("FileType", "Tiff")]
        public TiffCompressOption TiffCompression { get; set; }

        [Option(RequiredSizeDescription, RequiredSizeOption, ConverterType = typeof(CheckedRequiredSizeTypeConverter), ValidationExpression = Validation.RequiredSizeValidationExpression)]
        [DisplayName(RequiredSizeDisplayName)]
        [TabbedDescription(RequiredSizeDescription)]
        [UIOption(RequiredSizeOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedRequiredSizeTypeConverter))]
        public Size? RequiredSize { get; set; }

        [Option(DpiDescription, DpiShortOption, DefaultValue = DpiDefaultValue, ValidationExpression = Validation.DpiValidationExpression)]
        [DisplayName(DpiDisplayName)]
        [TabbedDescription(DpiDescription)]
        [UIOption(DpiShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(120)]
        [TypeConverter(typeof(CheckedDpiTypeConverter))]
        public int? Dpi { get; set; }

        [Option(ImageNameDescription, ImageNameShortOption, ValidationExpression = Validation.ImageNameValidationExpression)]
        [DisplayName(ImageNameDisplayName)]
        [TabbedDescription(ImageNameDescription)]
        [UIOption(ImageNameShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedImageNameTypeConverter))]
        public string ImageName { get; set; }

        [Option(FirstPageIndexDescription, FirstPageIndexShortOption, DefaultValue = FirstPageIndexDefaultValue, ValidationExpression = Validation.FirstPageIndexValidationExpression)]
        [DisplayName(FirstPageIndexDisplayName)]
        [TabbedDescription(FirstPageIndexDescription)]
        [UIOption(FirstPageIndexShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(1)]
        [TypeConverter(typeof(CheckedFirstPageIndexTypeConverter))]
        public int? FirstPageIndex { get; set; }

        [Option(PrelimsPrefixDescription, PrelimsPrefixShortOption, DefaultValue = PrelimsPrefixDefaultValue, ValidationExpression = Validation.PrelimsPrefixValidationExpression)]
        [DisplayName(PrelimsPrefixDisplayName)]
        [TabbedDescription(PrelimsPrefixDescription)]
        [UIOption(PrelimsPrefixShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(PrelimsPrefixDefaultValue)]
        [TypeConverter(typeof(CheckedPrelimsPrefixTypeConverter))]
        public string PrelimsPrefix { get; set; }

        [Option(ShortenExtensionDescription, ShortenExtensionOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        [UIOption(ShortenExtensionOption)]
        public bool ShortenExtension { get; set; }

        [Option("", ShortOptionType.None10, Flags = OptionFlags.Internal)]
        [UIOption(CancellationObjectIdName)]
        [Browsable(false)]
        public virtual string CancellationObjectIds { get; set; }

        private IEnumerable<string> SyncObjectsNames { get { return CancellationObjectIds.Split('-'); } }

        [Browsable(false)]
        public string CancellationEventName { get { return SyncObjectsNames.First(); } }

        [Browsable(false)]
        public string ParentAppMutexName { get { return SyncObjectsNames.Last(); } }

        [Option("", ShortOptionType.None12, DefaultValue = ProcessorsDefaultValue, Flags = OptionFlags.Internal)]
        [UIOption(ProcessorsOption)]
        [DisplayName(ProcessorsDisplayName)]
        [TabbedDescription(ProcessorsTabbedDescription)]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessorsNumberTypeConverter))]
        [DefaultValue(ProcessorsDefaultValue)]
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

        [Option(ProcessPriorityDescription, ProcessPriorityShortOption, DefaultValue = ProcessPriorityDefaultValue, Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ProcessPriorityShortOption)]
        [DisplayName(ProcessPriorityDisplayName)]
        [TabbedDescription(ProcessPriorityTabbedDescription)]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessPriorityClassTypeConverter))]
        [DefaultValue(ProcessPriorityDefaultValue)]
        public string ProcessPriority
        {
            get { return _processPriority; }

            set
            {
                value = value.RemoveSpaces();
                _processPriority =
                    String.CompareOrdinal(Validation.AutoValue, value) != 0 &&
                    EnumUtils.HasValue<ProcessPriorityClass>(value)
                        ? value
                        : Validation.AutoValue;
            }
        }

        private string _processPriority;

        [Browsable(false)]
        public ProcessPriorityClass ProcessorsPriorityAsEnum
        {
            get
            {
                ProcessPriorityClass processPriorityClass;
                return EnumUtils.TryParse(ProcessPriority, out processPriorityClass)
                        ? processPriorityClass
                        : ProcessPriorityClass.Normal;
            }
        }

        public static readonly string[] ExcludedOnSave = new[] { CancellationObjectIdName, BatchOption };
        public static readonly string[] ExcludedUIOptions = new[] { ProcessorsOption, BatchOption };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { PagesShortOption.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck = new[] { ProcessorsDisplayName, ProcessPriorityDisplayName, PostActionDisplayName };

        private bool _batch;

        [Browsable(false)]
        [Option("", BatchShortOption, ArgumentExpectancy.No, Flags = OptionFlags.Internal)]
        [UIOption(BatchOption)]
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

        [Option(SilentModeDescription, SilentModeShortOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        public virtual bool Silent { get; set; }

        [Option(TestDescription, TestShortOption, ArgumentExpectancy.No)]
        [DisplayName(TestDisplayName)]
        [TabbedDescription(TestDescription)]
        [UIOption(TestShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool Test { get; set; }

        [Option(CleanDescpriction, ShortOptionType.None1, ArgumentExpectancy.No)]
        [Browsable(false)]
        public bool Clean { get; set; }
    }
}
