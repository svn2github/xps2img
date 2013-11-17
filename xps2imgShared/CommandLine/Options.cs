using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;

using CommandLine;

using Xps2Img.Shared.Attributes.UI;
using Xps2Img.Shared.Dialogs;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.TypeEditors;

using UIOption = Xps2Img.Shared.Attributes.Options.OptionAttribute;
using UIUnnamedOption = Xps2Img.Shared.Attributes.Options.UnnamedOptionAttribute;

namespace Xps2Img.Shared.CommandLine
{
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable MemberCanBeProtected.Global
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable UnusedMember.Global
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

        [Option(PostActionDescription, PostActionShortOption, ConverterType = typeof(PostActionTypeConverter))]
        [DisplayName(PostActionDisplayName)]
        [Category(CategoryParameters)]
        [TypeConverter(typeof(PostActionTypeConverter))]
        [DefaultValue(PostAction.DoNothing)]
        [TabbedDescription(PostActionDescription)]
        public PostAction PostAction { get; set; }
        
        [Option(PagesDescription, PagesShortOption, ConverterType = typeof(PagesTypeConverter), ValidationExpression = Validation.PagesValidationExpression)]
        [DisplayName(PagesDisplayName)]
        [TabbedDescription(PagesDescription)]
        [Category(CategoryOptions)]
        [UIOption(PagesShortOption)]
        [DefaultValue(null)]
        [Editor(typeof(OrdinalUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PagesTypeConverter))]
        public List<Interval> Pages { get; set; }

        [Browsable(false)]
        public List<Interval> SafePages
        {
            get { return Pages ?? new List<Interval> { new Interval(begin: 1) }; }
        }

        [Option(FileTypeDescription, FileTypeShortOption, DefaultValue = FileTypeDefaultValue)]
        [DisplayName(FileTypeDisplayName)]
        [TabbedDescription(FileTypeDescription)]
        [UIOption(FileTypeShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(ImageType.Png)]
        [TypeConverter(typeof(ToUpperEnumConverter<ImageType>))]
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

        [Option(TiffCompressionDescription, TiffCompressionShortOption, DefaultValue = TiffCompressionDefaultValue, ConverterType = typeof(TiffCompressOptionEnumConverter))]
        [DisplayName(TiffCompressionDisplayName)]
        [TabbedDescription(TiffCompressionDescription)]
        [UIOption(TiffCompressionShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter("FileType", "Tiff")]
        [TypeConverter(typeof(TiffCompressOptionEnumConverter))]
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

        [Option(FirstPageIndexDescription, FirstPageIndexShortOption, DefaultValue = FirstPageIndexDefaultValue, ValidationExpression = Validation.FirstPageIndexValidationExpression, Flags = OptionFlags.NoDefaultValueDescription)]
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
        [Browsable(false)]
        public virtual string CancellationObjectIds { get; set; }

        private IEnumerable<string> SyncObjectsNames { get { return CancellationObjectIds.Split('-'); } }

        [Browsable(false)]
        public string CancellationEventName { get { return SyncObjectsNames.First(); } }

        [Browsable(false)]
        public string ParentAppMutexName { get { return SyncObjectsNames.Last(); } }

        [Option("", ShortOptionType.None12, ConverterType = typeof(ProcessorsNumberTypeConverter), Flags = OptionFlags.Internal)]
        [UIOption(ProcessorsOption)]
        [DisplayName(ProcessorsDisplayName)]
        [TabbedDescription(ProcessorsTabbedDescription)]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessorsNumberTypeConverter))]
        [DefaultValue(ProcessorsDefaultValue)]
        public int ProcessorsNumber { get; set; }

        [Browsable(false)]       
        public int SafeProcessorsNumber
        {
            get { return ProcessorsNumber == ProcessorsDefaultValue ? Environment.ProcessorCount : ProcessorsNumber; }
        }

        [Option(ProcessPriorityDescription, ProcessPriorityShortOption, DefaultValue = ProcessPriorityDefaultValue, ConverterType = typeof(ProcessPriorityClassTypeConverter), Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ProcessPriorityShortOption)]
        [DisplayName(ProcessPriorityDisplayName)]
        [TabbedDescription(ProcessPriorityTabbedDescription)]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessPriorityClassTypeConverter))]
        [DefaultValue(ProcessPriorityClassTypeConverter.Auto)]
        public ProcessPriorityClass ProcessPriority { get; set; }

        public static readonly string[] ExcludedOnSave = { CancellationObjectIdsName, BatchOption };
        public static readonly string[] ExcludedUIOptions = { ProcessorsOption, BatchOption };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { PagesShortOption.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck = { ProcessorsDisplayName, ProcessPriorityDisplayName, PostActionDisplayName };

        private bool _batch;

        [Browsable(false)]
        [Option("", BatchShortOption, ArgumentExpectancy.No, Flags = OptionFlags.Internal)]
        [UIOption(BatchOption)]
        [DefaultValue(false)]
        public bool Batch
        {
            get
            {
                return _batch;
            }

            set
            {
                _batch = value;
                if (_batch)
                {
                    PostAction = PostActionTypeConverter.ChooseAction(PostAction, PostAction.Exit);
                }
            }
        }

        [Option(CpuAffinityDescription, CpuAffinityShortOption, ValidationExpression = Validation.CpuAffinityValidationExpression)]
        [DisplayName(CpuAffinityDisplayName)]
        [TabbedDescription(CpuAffinityTabbedDescription)]
        [UIOption(CpuAffinityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [Editor(typeof(CpuAffinityUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CpuAffinityTypeConverter))]
        public IntPtr? CpuAffinity { get; set; }

        [Option(SilentModeDescription, SilentModeShortOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        public virtual bool Silent { get; set; }

        [Option(IgnoreExistingDescription, IgnoreExistingShortOption, ArgumentExpectancy.No)]
        [DisplayName(IgnoreExistingDisplayName)]
        [TabbedDescription(IgnoreExistingDescription)]
        [UIOption(IgnoreExistingShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IgnoreExisting { get; set; }

        [Option(IgnoreErrorsDescription, IgnoreErrorsShortOption, ArgumentExpectancy.No)]
        [DisplayName(IgnoreErrorsDisplayName)]
        [TabbedDescription(IgnoreErrorsDescription)]
        [UIOption(IgnoreErrorsShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IgnoreErrors { get; set; }

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
