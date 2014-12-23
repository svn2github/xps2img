using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;

using CommandLine;
using CommandLine.Utils;

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
    public partial class Options
    {
        public Options()
        {
            ReflectionUtils.SetDefaultValues(this);
        }

        [UnnamedOption(DescriptionKey = "SrcFileCmd", ConverterType = typeof(TrimStringTypeConverter))]
        [UIUnnamedOption]
        [Category(CategoryParameters)]
        [Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
        [DefaultValue(null)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string SrcFile { get; set; }

        [UnnamedOption(false, ConverterType = typeof(TrimStringTypeConverter))]
        [UIUnnamedOption(false)]
        [Editor(typeof(SelectXpsFolderEditor), typeof(UITypeEditor))]
        [Category(CategoryParameters)]
        [DefaultValue(null)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string OutDir { get; set; }

        [Option(PostActionShortOption, ConverterType = typeof(PostActionTypeConverter))]
        [Category(CategoryParameters)]
        [TypeConverter(typeof(PostActionTypeConverter))]
        [DefaultValue(PostAction.DoNothing)]
        public PostAction PostAction { get; set; }
        
        [Option(PagesShortOption, ConverterType = typeof(PagesTypeConverter), ValidationExpression = Validation.PagesValidationExpression)]
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

        [Option(FileTypeShortOption, DefaultValue = FileTypeDefaultValue)]
        [UIOption(FileTypeShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(ImageType.Png)]
        [TypeConverter(typeof(OptionsEnumConverter<ImageType>))]
        public ImageType FileType { get; set; }

        [Option(JpegQualityShortOption, DefaultValue = JpegQualityDefaultValue, ValidationExpression = Validation.JpegQualityValidationExpression)]
        [UIOption(JpegQualityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(85)]
        [DynamicPropertyFilter("FileType", "Jpeg")]
        [TypeConverter(typeof(JpegNullableIntTypeConverter))]
        public int? JpegQuality { get; set; }

        [Option(TiffCompressionShortOption, DefaultValue = TiffCompressionDefaultValue, ConverterType = typeof(TiffCompressOptionEnumConverter))]
        [UIOption(TiffCompressionShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter("FileType", "Tiff")]
        [TypeConverter(typeof(TiffCompressOptionEnumConverter))]
        public TiffCompressOption TiffCompression { get; set; }

        [Option(RequiredSizeOption, ConverterType = typeof(CheckedRequiredSizeTypeConverter), ValidationExpression = Validation.RequiredSizeValidationExpression)]
        [UIOption(RequiredSizeOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedRequiredSizeTypeConverter))]
        public Size? RequiredSize { get; set; }

        [Option(DpiShortOption, DefaultValue = DpiDefaultValue, ValidationExpression = Validation.DpiValidationExpression)]
        [UIOption(DpiShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(120)]
        [TypeConverter(typeof(CheckedDpiTypeConverter))]
        public int? Dpi { get; set; }

        [Option(ImageNameShortOption, ValidationExpression = Validation.ImageNameValidationExpression)]
        [UIOption(ImageNameShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedImageNameTypeConverter))]
        public string ImageName { get; set; }

        [Option(FirstPageIndexShortOption, DefaultValue = FirstPageIndexDefaultValue, ValidationExpression = Validation.FirstPageIndexValidationExpression, Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(FirstPageIndexShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(1)]
        [TypeConverter(typeof(CheckedFirstPageIndexTypeConverter))]
        public int? FirstPageIndex { get; set; }

        [Option(PrelimsPrefixShortOption, DefaultValue = PrelimsPrefixDefaultValue, ValidationExpression = Validation.PrelimsPrefixValidationExpression)]
        [UIOption(PrelimsPrefixShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(PrelimsPrefixDefaultValue)]
        [TypeConverter(typeof(CheckedPrelimsPrefixTypeConverter))]
        public string PrelimsPrefix { get; set; }

        [Option(ShortenExtensionOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        [UIOption(ShortenExtensionOption)]
        public bool ShortenExtension { get; set; }

        [Option(ShortOptionType.None10, Flags = OptionFlags.Internal)]
        [Browsable(false)]
        public virtual string CancellationObjectIds { get; set; }

        private IEnumerable<string> SyncObjectsNames { get { return CancellationObjectIds.Split('-'); } }

        [Browsable(false)]
        public string CancellationEventName { get { return SyncObjectsNames.First(); } }

        [Browsable(false)]
        public string ParentAppMutexName { get { return SyncObjectsNames.Last(); } }

        [Option(ShortOptionType.None12, ConverterType = typeof(ProcessorsNumberTypeConverter), Flags = OptionFlags.Internal)]
        [UIOption(ProcessorsOption)]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessorsNumberTypeConverter))]
        [DefaultValue(ProcessorsDefaultValue)]
        public int ProcessorsNumber { get; set; }

        [Browsable(false)]       
        public int SafeProcessorsNumber
        {
            get { return ProcessorsNumber == ProcessorsDefaultValue ? Environment.ProcessorCount : ProcessorsNumber; }
        }

        [Option(ProcessPriorityShortOption, DefaultValue = ProcessPriorityDefaultValue, DescriptionKey = "ProcessPriorityCmd", ConverterType = typeof(ProcessPriorityClassTypeConverter), Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ProcessPriorityShortOption)]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessPriorityClassTypeConverter))]
        [DefaultValue(ProcessPriorityClassTypeConverter.Auto)]
        public ProcessPriorityClass ProcessPriority { get; set; }

        public static readonly string[] ExcludedOnSave = { CancellationObjectIdsName, BatchOption };
        public static readonly string[] ExcludedUIOptions = { ProcessorsOption, BatchOption };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { PagesShortOption.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck = { Properties.ProcessorsNumber, Properties.ProcessPriority, Properties.PostAction, Properties.IgnoreExisting, Properties.IgnoreErrors };

        private bool _batch;

        [Browsable(false)]
        [Option(BatchShortOption, ArgumentExpectancy.No, Flags = OptionFlags.Internal)]
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

        [Option(CpuAffinityShortOption, DescriptionKey = "CpuAffinityCmd", ValidationExpression = Validation.CpuAffinityValidationExpression)]
        [UIOption(CpuAffinityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        [Editor(typeof(CpuAffinityUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CpuAffinityTypeConverter))]
        public IntPtr? CpuAffinity { get; set; }

        [Option(IgnoreExistingShortOption, ArgumentExpectancy.No, DescriptionKey = "IgnoreExistingCmd")]
        [UIOption(IgnoreExistingShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IgnoreExisting { get; set; }

        [Option(IgnoreErrorsShortOption, ArgumentExpectancy.No)]
        [UIOption(IgnoreErrorsShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IgnoreErrors { get; set; }
        
        [Option(SilentModeShortOption, ArgumentExpectancy.No)]
        [Browsable(false)]
        public virtual bool Silent { get; set; }

        [Option(TestShortOption, ArgumentExpectancy.No)]
        [UIOption(TestShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool Test { get; set; }

        [Option(ShortOptionType.None1, ArgumentExpectancy.No)]
        [Browsable(false)]
        public bool Clean { get; set; }
    }
}
