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

        private const string DescriptionKeyPostfix = "Cmd";

        [Category(Categories.Parameters)]
        [UnnamedOption(DescriptionKey = Properties.Consts.SrcFile + DescriptionKeyPostfix, ConverterType = typeof(TrimStringTypeConverter))]
        [UIUnnamedOption]
        [Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
        [DefaultValue(null)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string SrcFile { get; set; }

        [Category(Categories.Parameters)]
        [UnnamedOption(false, ConverterType = typeof(TrimStringTypeConverter))]
        [UIUnnamedOption(false)]
        [Editor(typeof(SelectXpsFolderEditor), typeof(UITypeEditor))]
        [DefaultValue(null)]
        [TypeConverter(typeof(TrimStringTypeConverter))]
        public string OutDir { get; set; }

        [Category(Categories.Parameters)]
        [Option(ShortOptions.PostAction, ConverterType = typeof(PostActionTypeConverter))]
        [TypeConverter(typeof(PostActionTypeConverter))]
        [DefaultValue(PostAction.DoNothing)]
        public PostAction PostAction { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.Pages, ConverterType = typeof(PagesTypeConverter), ValidationExpression = Validation.PagesValidationExpression)]
        [UIOption(ShortOptions.Pages)]
        [DefaultValue(null)]
        [Editor(typeof(OrdinalUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PagesTypeConverter))]
        public List<Interval> Pages { get; set; }

        [Browsable(false)]
        public List<Interval> SafePages
        {
            get { return Pages ?? new List<Interval> { new Interval(begin: 1) }; }
        }

        [Category(Categories.Options)]
        [Option(ShortOptions.FileType, DefaultValue = Defaults.FileType)]
        [UIOption(ShortOptions.FileType)]
        [DefaultValue(ImageType.Png)]
        [TypeConverter(typeof(OptionsEnumConverter<ImageType>))]
        public ImageType FileType { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.JpegQuality, DefaultValue = Defaults.JpegQuality, ValidationExpression = Validation.JpegQualityValidationExpression)]
        [UIOption(ShortOptions.JpegQuality)]
        [DefaultValue(85)]
        [DynamicPropertyFilter(Properties.Consts.FileType, ImageType.Jpeg)]
        [TypeConverter(typeof(JpegNullableIntTypeConverter))]
        public int? JpegQuality { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.TiffCompression, DefaultValue = Defaults.TiffCompression, ConverterType = typeof(TiffCompressOptionEnumConverter))]
        [UIOption(ShortOptions.TiffCompression)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter(Properties.Consts.FileType, ImageType.Tiff)]
        [TypeConverter(typeof(TiffCompressOptionEnumConverter))]
        public TiffCompressOption TiffCompression { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.RequiredSize, ConverterType = typeof(CheckedRequiredSizeTypeConverter), ValidationExpression = Validation.RequiredSizeValidationExpression)]
        [UIOption(ShortOptions.RequiredSize)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedRequiredSizeTypeConverter))]
        public Size? RequiredSize { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.Dpi, DefaultValue = Defaults.Dpi, ValidationExpression = Validation.DpiValidationExpression)]
        [UIOption(ShortOptions.Dpi)]
        [DefaultValue(120)]
        [TypeConverter(typeof(CheckedDpiTypeConverter))]
        public int? Dpi { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.ImageName, ValidationExpression = Validation.ImageNameValidationExpression)]
        [UIOption(ShortOptions.ImageName)]
        [DefaultValue(null)]
        [TypeConverter(typeof(CheckedImageNameTypeConverter))]
        public string ImageName { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.FirstPageIndex, DefaultValue = Defaults.FirstPageIndex, ValidationExpression = Validation.FirstPageIndexValidationExpression, Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ShortOptions.FirstPageIndex)]
        [DefaultValue(1)]
        [TypeConverter(typeof(CheckedFirstPageIndexTypeConverter))]
        public int? FirstPageIndex { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.PrelimsPrefix, DefaultValue = Defaults.PrelimsPrefix, ValidationExpression = Validation.PrelimsPrefixValidationExpression)]
        [UIOption(ShortOptions.PrelimsPrefix)]
        [DefaultValue(Defaults.PrelimsPrefix)]
        [TypeConverter(typeof(CheckedPrelimsPrefixTypeConverter))]
        public string PrelimsPrefix { get; set; }

        [Option(ShortOptions.ShortenExtension, ArgumentExpectancy.No)]
        [Browsable(false)]
        [UIOption(ShortOptions.ShortenExtension)]
        public bool ShortenExtension { get; set; }

        [Option(ShortOptionType.None10, Flags = OptionFlags.Internal)]
        [Browsable(false)]
        public virtual string CancellationObjectIds { get; set; }

        private IEnumerable<string> SyncObjectsNames { get { return CancellationObjectIds.Split('-'); } }

        [Browsable(false)]
        public string CancellationEventName { get { return SyncObjectsNames.First(); } }

        [Browsable(false)]
        public string ParentAppMutexName { get { return SyncObjectsNames.Last(); } }

        [Category(Categories.Options)]
        [Option(ShortOptionType.None12, ConverterType = typeof(ProcessorsNumberTypeConverter), Flags = OptionFlags.Internal)]
        [UIOption(Names.Processors)]
        [TypeConverter(typeof(ProcessorsNumberTypeConverter))]
        [DefaultValue(Defaults.Processors)]
        public int ProcessorsNumber { get; set; }

        [Browsable(false)]       
        public int SafeProcessorsNumber
        {
            get { return ProcessorsNumber == Defaults.Processors ? Environment.ProcessorCount : ProcessorsNumber; }
        }

        [Category(Categories.Options)]
        [Option(ShortOptions.ProcessPriority, DefaultValue = Defaults.ProcessPriority, DescriptionKey = Properties.Consts.ProcessPriority + DescriptionKeyPostfix, ConverterType = typeof(ProcessPriorityClassTypeConverter), Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ShortOptions.ProcessPriority)]
        [TypeConverter(typeof(ProcessPriorityClassTypeConverter))]
        [DefaultValue(ProcessPriorityClassTypeConverter.Auto)]
        public ProcessPriorityClass ProcessPriority { get; set; }

        public static readonly string[] ExcludedOnSave = { Names.CancellationObjectIds, Names.Batch };
        public static readonly string[] ExcludedUIOptions = { Names.Processors, Names.Batch };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { ShortOptions.Pages.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck = { Properties.ProcessorsNumber, Properties.ProcessPriority, Properties.PostAction, Properties.IgnoreExisting, Properties.IgnoreErrors };

        private bool _batch;

        [Browsable(false)]
        [Option(ShortOptions.Batch, ArgumentExpectancy.No, Flags = OptionFlags.Internal)]
        [UIOption(Names.Batch)]
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

        [Category(Categories.Options)]
        [Option(ShortOptions.CpuAffinity, DescriptionKey = Properties.Consts.CpuAffinity + DescriptionKeyPostfix, ValidationExpression = Validation.CpuAffinityValidationExpression)]
        [UIOption(ShortOptions.CpuAffinity)]
        [DefaultValue(null)]
        [Editor(typeof(CpuAffinityUITypeEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CpuAffinityTypeConverter))]
        public IntPtr? CpuAffinity { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.IgnoreExisting, ArgumentExpectancy.No, DescriptionKey = Properties.Consts.IgnoreExisting + DescriptionKeyPostfix)]
        [UIOption(ShortOptions.IgnoreExisting)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IgnoreExisting { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.IgnoreErrors, ArgumentExpectancy.No)]
        [UIOption(ShortOptions.IgnoreErrors)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool IgnoreErrors { get; set; }

        [Option(ShortOptions.SilentMode, ArgumentExpectancy.No)]
        [Browsable(false)]
        public virtual bool Silent { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.Test, ArgumentExpectancy.No)]
        [UIOption(ShortOptions.Test)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public bool Test { get; set; }

        [Option(ShortOptionType.None1, ArgumentExpectancy.No)]
        [Browsable(false)]
        public bool Clean { get; set; }
    }
}
