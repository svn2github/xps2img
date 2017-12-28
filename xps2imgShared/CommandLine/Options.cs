using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

using CommandLine;
using CommandLine.Utils;

using Xps2Img.Shared.Attributes.UI;
using Xps2Img.Shared.Enums;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.TypeEditors;
using Xps2Img.Shared.TypeEditors.Dialogs;

using Xps2ImgLib;

using UIOption = Xps2Img.Shared.Attributes.Options.OptionAttribute;
using UIUnnamedOption = Xps2Img.Shared.Attributes.Options.UnnamedOptionAttribute;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        // ReSharper disable once MemberCanBeProtected.Global
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
        [Option(ShortOptions.Pages, ConverterType = typeof(PagesTypeConverter), ValidationExpression = typeof(PagesValidator))]
        [UIOption(ShortOptions.Pages)]
        [DefaultValue(null)]
        [Editor(typeof(OrdinalEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(PagesTypeConverter))]
        public List<Interval> Pages { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.FileType, DefaultValue = Defaults.FileType)]
        [UIOption(ShortOptions.FileType)]
        [DefaultValue(ImageType.Png)]
        [TypeConverter(typeof(OptionsEnumConverter<ImageType>))]
        public ImageType FileType { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.JpegQuality, DefaultValue = Defaults.JpegQuality, ValidationExpression = ValidationExpressions.JpegQuality)]
        [UIOption(ShortOptions.JpegQuality)]
        [DefaultValue(85)]
        [DynamicPropertyFilter(Properties.Consts.FileType, ImageType.Jpeg)]
        [TypeConverter(typeof(JpegNullableIntTypeConverter))]
        [Editor(typeof(JpegQualityEditor), typeof(UITypeEditor))]
        public int? JpegQuality { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.TiffCompression, DefaultValue = Defaults.TiffCompression, ConverterType = typeof(TiffCompressOptionEnumConverter))]
        [UIOption(ShortOptions.TiffCompression)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter(Properties.Consts.FileType, ImageType.Tiff)]
        [TypeConverter(typeof(TiffCompressOptionEnumConverter))]
        public TiffCompressOption TiffCompression { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptionType.None1, ArgumentExpectancy.No, Flags = OptionFlags.Internal)]
        [DefaultValue(true)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool PreferDpiOverSize { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.RequiredSize, ConverterType = typeof (CheckedRequiredSizeTypeConverter), ValidationExpression = ValidationExpressions.RequiredSize)]
        [UIOption(ShortOptions.RequiredSize)]
        [DefaultValue(null)]
        [DynamicPropertyFilter(Properties.Consts.PreferDpiOverSize, false)]
        [TypeConverter(typeof (CheckedRequiredSizeTypeConverter))]
        [Editor(typeof (RequiredSizeEditor), typeof (UITypeEditor))]
        public Size? RequiredSize { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.Dpi, DefaultValue = Defaults.Dpi, ValidationExpression = ValidationExpressions.Dpi)]
        [UIOption(ShortOptions.Dpi)]
        [DefaultValue(Defaults.DpiValue)]
        [DynamicPropertyFilter(Properties.Consts.PreferDpiOverSize, true)]
        [TypeConverter(typeof(CheckedDpiTypeConverter))]
        [Editor(typeof(DpiEditor), typeof(UITypeEditor))]
        public int? Dpi { get; set; }

        [Category(Categories.Options)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool UseFileNameAsImageName
        {
            get { return ImageName == Names.Empty; }
            set { ImageName = value ? Names.Empty : Defaults.ImageName; }
        }

        [Category(Categories.Options)]
        [Option(ShortOptions.ImageName, DescriptionKey = Properties.Consts.ImageName + DescriptionKeyPostfix, ValidationExpression = ValidationExpressions.ImageName)]
        [UIOption(ShortOptions.ImageName, AlwaysFormat = true)]
        [DefaultValue(Defaults.ImageName)]
        [DynamicPropertyFilter(Properties.Consts.UseFileNameAsImageName, false)]
        [TypeConverter(typeof(CheckedImageNameTypeConverter))]
        public string ImageName { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.FirstPageIndex, DefaultValue = Defaults.FirstPageIndex, ValidationExpression = ValidationExpressions.FirstPageIndex, Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ShortOptions.FirstPageIndex)]
        [DefaultValue(1)]
        [TypeConverter(typeof(CheckedFirstPageIndexTypeConverter))]
        public int? FirstPageIndex { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.PrelimsPrefix, DefaultValue = Defaults.PrelimsPrefix, ValidationExpression = ValidationExpressions.PrelimsPrefix)]
        [UIOption(ShortOptions.PrelimsPrefix)]
        [DefaultValue(Defaults.PrelimsPrefix)]
        [TypeConverter(typeof(CheckedPrelimsPrefixTypeConverter))]
        public string PrelimsPrefix { get; set; }

        [Option(ShortOptions.ShortenExtension, ArgumentExpectancy.No)]
        [Browsable(false)]
        [UIOption(ShortOptions.ShortenExtension)]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool ShortenExtension { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptionType.None2, ConverterType = typeof(ProcessorsNumberTypeConverter), Flags = OptionFlags.Internal)]
        [UIOption(Names.Processors)]
        [TypeConverter(typeof(ProcessorsNumberTypeConverter))]
        [DefaultValue(Defaults.Processors)]
        public int ProcessorsNumber { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.ProcessPriority, DefaultValue = Defaults.ProcessPriority, ConverterType = typeof(ProcessPriorityClassTypeConverter), Flags = OptionFlags.NoDefaultValueDescription)]
        [UIOption(ShortOptions.ProcessPriority)]
        [TypeConverter(typeof(ProcessPriorityClassTypeConverter))]
        [DefaultValue(ProcessPriorityClassTypeConverter.Auto)]
        public ProcessPriorityClass ProcessPriority { get; set; }

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
        [Option(ShortOptions.CpuAffinity, DescriptionKey = Properties.Consts.CpuAffinity + DescriptionKeyPostfix, ValidationExpression = ValidationExpressions.CpuAffinity)]
        [UIOption(ShortOptions.CpuAffinity)]
        [DefaultValue(null)]
        [Editor(typeof(CpuAffinityEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CpuAffinityTypeConverter))]
        public IntPtr? CpuAffinity { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.IgnoreExisting, ArgumentExpectancy.No, DescriptionKey = Properties.Consts.IgnoreExisting + DescriptionKeyPostfix)]
        [UIOption(ShortOptions.IgnoreExisting)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool IgnoreExisting { get; set; }

        [Category(Categories.Options)]
        [Option(ShortOptions.IgnoreErrors, ArgumentExpectancy.No)]
        [UIOption(ShortOptions.IgnoreErrors)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
        public bool IgnoreErrors { get; set; }

        [Option(ShortOptions.SilentMode, ArgumentExpectancy.No)]
        [Browsable(false)]
        [DefaultValue(false)]
        public virtual bool Silent { get; set; }

        // ReSharper disable once MemberCanBeProtected.Global
        [Category(Categories.Options)]
#if DEBUG
        [Editor(typeof(CheckBoxGlyphEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
		[Option(ShortOptions.Test, ArgumentExpectancy.No)]
        [UIOption(ShortOptions.Test)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        public virtual bool Test { get; set; }

        [Option(ShortOptionType.None4, ArgumentExpectancy.No)]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Clean { get; set; }
    }
}
