using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

using CommandLine;

using Xps2Img.CommandLine.TypeConverters;

#if XPS2IMG_UI
using System;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;

using CommandLine.Validation;
using Parser = CommandLine.Validation.Parser;

using Xps2ImgUI.Controls.PropertyGridEx;
using Xps2ImgUI.Converters;
using Xps2ImgUI.Dialogs;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;
#else
using System.Drawing;
#endif

using Xps2Img.Xps2Img;

// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming

namespace Xps2Img.CommandLine
{
    #if !XPS2IMG_UI
    [Description("\nConverts XPS (XML Paper Specification) document to set of images.")]
    #endif
    public class Options
    #if XPS2IMG_UI
        : FilterablePropertyBase
    #endif
    {
        #if XPS2IMG_UI
        public Options()
        {
            ReflectionUtils.SetDefaultValues(this);
        }
        #endif

        public const string CategoryParameters  = "\tParameters";
        public const string CategoryOptions     = "Options";
        public const string EmptyOption         = "\"\"";

        private const string SrcFileDescription = "XPS file to process";

        #if XPS2IMG_UI
        public const string XPSFileDisplayName = "XPS File";
        #endif

        [UnnamedOption(SrcFileDescription)]
        #if XPS2IMG_UI
        [DisplayName(XPSFileDisplayName)]
        [Xps2ImgUI.Attributes.Options.UnnamedOption]
        [Category(CategoryParameters)]
        [Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
        [DefaultValue(null)]
        [TabbedDescription(SrcFileDescription + " (required)")]
        #endif
        public string SrcFile
        {
            get { return TrimFileName(_srcFile); }
            set { _srcFile = value; }
        }

        private string _srcFile;

        private const string OutDirDescription = "Output folder\n  new folder named as document will be created in folder where document is by default";

        #if XPS2IMG_UI
        public const string OutputFolderDisplayName = "Output Folder";
        #endif

        [UnnamedOption(OutDirDescription, false)]
        #if XPS2IMG_UI
        [DisplayName(OutputFolderDisplayName)]
        [Xps2ImgUI.Attributes.Options.UnnamedOption(false)]
        [Editor(typeof(SelectXpsFolderEditor), typeof(UITypeEditor))]
        [Category(CategoryParameters)]
        [DefaultValue(null)]
        [TabbedDescription(OutDirDescription)]
        #endif
        public string OutDir
        {
            get { return TrimFileName(_outDir); }
            set { _outDir = value; }
        }

        private string _outDir;

        #if XPS2IMG_UI

        public const string PostActionDisplayName   = "After Conversion";
        private const string PostActionDescription  = "Action to execute after conversion completed.";

        [DisplayName(PostActionDisplayName)]
        [Category(CategoryParameters)]
        //[TypeConverter(typeof(PostActionConverterWithUserApplication))]
        [TypeConverter(typeof(PostActionConverter))]
        [DefaultValue(PostActionConverter.Default)]
        [TabbedDescription(PostActionDescription)]
        public string PostAction
        {
            get; set;
        }

        public const string UserApplicationDisplayName  = "Program";
        private const string UserApplicationDescription = "Program to execute after conversion completed.";

        [DisplayName(UserApplicationDisplayName)]
        [Category(CategoryParameters)]
        [DefaultValue(null)]
        [TabbedDescription(UserApplicationDescription)]
        [Editor(typeof(SelectExecutableFileEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("PostAction", PostActionConverterWithUserApplication.UserApplication)]
        public string UserApplication
        {
            get; set;
        }

        public const string UserApplicationPostActionDisplayName = "After Program";
        private const string UserApplicationPostActionDescription = "Action to execute after program completed.";

        [DisplayName(UserApplicationPostActionDisplayName)]
        [Category(CategoryParameters)]
        [DefaultValue(PostActionConverter.Default)]
        [TypeConverter(typeof(PostActionConverter))]
        [TabbedDescription(UserApplicationPostActionDescription)]
        [DynamicPropertyFilter("PostAction", PostActionConverterWithUserApplication.UserApplication)]
        public string UserApplicationPostAction
        {
            get; set;
        }
        #endif

        private static readonly char[] FileNameTrimCharacters = "\"\x20\t\r\n".ToCharArray();

        private static string TrimFileName(string val)
        {
            return val == null ? null : val.Trim(FileNameTrimCharacters);
        }

        private const string RegexMatchEmptyString = 
        #if XPS2IMG_UI
        @"^$|"
        #else
        ""
        #endif
        ;

        private const string PagesDescription = "Page number(s)\n  all pages by default\nSyntax:\n  all:\t\t1-\n  single:\t1\n  set:\t\t1,3\n  range:\t1-10 or -10 or 10-\n  combined:\t1,3-5,7-9,15-";
        private const char PagesShortOption = 'p';
        private const string PagesValidationExpression = "/" + RegexMatchEmptyString + Interval.ValidationRegex + "/";

        [Option(
            PagesDescription,
            PagesShortOption,
        	#if !XPS2IMG_UI
            DefaultValue = "",
            ConverterType = typeof(IntervalTypeConverter),
        	#endif
            ValidationExpression = PagesValidationExpression
        )]
        #if XPS2IMG_UI
        [DisplayName("Page Number(s)")]
        [TabbedDescription(PagesDescription)]
        [Category(CategoryOptions)]
        [Xps2ImgUI.Attributes.Options.Option(PagesShortOption)]
        [DefaultValue(null)]
        public string Pages
        {
            get { return _pages; }
            set
            {
                ValidateProperty(value, PagesValidationExpression);
                _pages = value;
            }
        }
        private string _pages;
        #else
        public List<Interval> Pages { get; set; }
        #endif

        #if XPS2IMG_UI
        public const string FileTypeDisplayName = "Image Type";
        #endif

        private const string FileTypeDescription = "Image type";
        private const char FileTypeShortOption = 'f';

        [Option(FileTypeDescription, FileTypeShortOption, DefaultValue = "png")]
        #if XPS2IMG_UI
        [DisplayName(FileTypeDisplayName)]
        [TabbedDescription(FileTypeDescription)]
        [Xps2ImgUI.Attributes.Options.Option(FileTypeShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(ImageType.Png)]
        #endif
        public ImageType FileType { get; set; }

        private const string JpegQualityDescription = "JPEG quality level (10-100)";
        private const char JpegQualityShortOption = 'q';
        private const string JpegQualityValidationExpression = "10-100";

        [Option(JpegQualityDescription, JpegQualityShortOption, DefaultValue = "85", ValidationExpression = JpegQualityValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("JPEG Quality")]
        [TabbedDescription(JpegQualityDescription)]
        [Xps2ImgUI.Attributes.Options.Option(JpegQualityShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(85)]
        [DynamicPropertyFilter("FileType", "Jpeg")]
        [TypeConverter(typeof(NullableIntTypeConverter))]
        public int? JpegQuality
        {
            get { return _jpegQuality; }
            set
            {
                ValidateProperty(value, JpegQualityValidationExpression);
                _jpegQuality = value;
            }
        }
        private int? _jpegQuality;
        #else
        public int JpegQuality { get; set; }
        #endif

        private const string TiffCompressionDescription = "TIFF compression method";
        private const char TiffCompressionShortOption = 't';

        [Option(TiffCompressionDescription, TiffCompressionShortOption, DefaultValue = "zip")]
        #if XPS2IMG_UI
        [DisplayName("TIFF Compression")]
        [TabbedDescription(TiffCompressionDescription)]
        [Xps2ImgUI.Attributes.Options.Option(TiffCompressionShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(TiffCompressOption.Zip)]
        [DynamicPropertyFilter("FileType", "Tiff")]
        #endif
        public TiffCompressOption TiffCompression { get; set; }

        private const string RequiredSizeDescription = "Desired image size\n  DPI will be ignored if image size is specified \nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theight for portrait orientation";
        private const char RequiredSizeOption = 'r';
        private const string RequiredSizeValidationExpression = "/" + RegexMatchEmptyString + RequiredSizeTypeConverter.ValidationRegex + "/";

        [Option(
            RequiredSizeDescription,
            RequiredSizeOption,
            #if !XPS2IMG_UI
            ConverterType = typeof(RequiredSizeTypeConverter),
            #endif
            ValidationExpression = RequiredSizeValidationExpression
        )]
        #if XPS2IMG_UI
        [DisplayName("Image Size")]
        [TabbedDescription(RequiredSizeDescription)]
        [Xps2ImgUI.Attributes.Options.Option(RequiredSizeOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        public string RequiredSize
        {
            get { return _requiredSize; }
            set
            {
                ValidateProperty(value, RequiredSizeValidationExpression);
                _requiredSize = value;
            }
        }
        private string _requiredSize;
        #else
        public Size? RequiredSize { get; set; }
        #endif

        private const string DpiDescription = "Image DPI (16-2350)\n  DPI will be ignored if image size is specified";
        private const char DpiShortOption = 'd';
        private const string DpiValidationExpression = "16-2350";

        [Option(DpiDescription, DpiShortOption, DefaultValue = "120", ValidationExpression = DpiValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Image DPI")]
        [TabbedDescription(DpiDescription)]
        [Xps2ImgUI.Attributes.Options.Option(DpiShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(120)]
        [TypeConverter(typeof(NullableIntTypeConverter))]
        public int? Dpi
        {
            get { return _dpi; }
            set
            {
                ValidateProperty(value, DpiValidationExpression);
                _dpi = value;
            }
        }
        private int? _dpi;
        #else
        public int Dpi { get; set; }
        #endif

        private const string ImageNameDescription = "Image prefix\n  numeric if ommited: 01.png\n  name of src file if empty (-i \"\"): src_file-01.png";
        private const char ImageNameShortOption = 'i';
        private const string ImageNameValidationExpression = OptionsValidators.FileNameValidationRegex;

        [Option(ImageNameDescription, ImageNameShortOption, ValidationExpression = ImageNameValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Image Prefix")]
        [TabbedDescription(ImageNameDescription)]
        [Xps2ImgUI.Attributes.Options.Option(ImageNameShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(null)]
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                if (value != EmptyOption)
                {
                    ValidateProperty(value, ImageNameValidationExpression);
                }
                _imageName = value;
            }
        }
        private string _imageName;
        #else
        public string ImageName { get; set; }
        #endif

        private const string FirstPageIndexDescription = "Document body first page index";
        private const char FirstPageIndexShortOption = 'a';
        private const string FirstPageIndexValidationExpression = "1-1000000";

        [Option(FirstPageIndexDescription, FirstPageIndexShortOption, DefaultValue = "1", ValidationExpression = FirstPageIndexValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("First Page Index")]
        [TabbedDescription(FirstPageIndexDescription)]
        [Xps2ImgUI.Attributes.Options.Option(FirstPageIndexShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(1)]
        [TypeConverter(typeof(NullableIntTypeConverter))]
        public int? FirstPageIndex
        {
            get { return _firstPageIndex; }
            set
            {
                ValidateProperty(value, FirstPageIndexValidationExpression);
                _firstPageIndex = value;
            }
        }
        private int? _firstPageIndex;
        #else
        public int FirstPageIndex { get; set; }
        #endif

        private const string PrelimsPrefixDescription = "Preliminaries prefix." + OptionsValidators.FileNameCharactersNotAllowed;
        private const char PrelimsPrefixShortOption = 'x';
        private const string PrelimsPrefixDefaultValue = "$";
        private const string PrelimsPrefixValidationExpression = OptionsValidators.FileNameValidationRegex;

        [Option(PrelimsPrefixDescription, PrelimsPrefixShortOption, DefaultValue = PrelimsPrefixDefaultValue, ValidationExpression = PrelimsPrefixValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Preliminaries Prefix")]
        [TabbedDescription(PrelimsPrefixDescription)]
        [Xps2ImgUI.Attributes.Options.Option(PrelimsPrefixShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(PrelimsPrefixDefaultValue)]
        public string PrelimsPrefix
        {
            get { return _prelimsPrefix; }
            set
            {
                ValidateProperty(value, PrelimsPrefixValidationExpression);
                _prelimsPrefix = value;
            }
        }
        private string _prelimsPrefix;
        #else
        public string PrelimsPrefix { get; set; }
        #endif

        private const string TestDescription = "Test mode (no file operations will be performed)";
        private const char TestShortOption = 'e';

        #if !XPS2IMG_UI
        private const string SilentModeDescription = "Silent mode (no progress will be shown)";
        private const char SilentModeShortOption = 's';

        [Option(SilentModeDescription, SilentModeShortOption, ArgumentExpectancy.No)]
        public bool Silent { get; set; }
        #else
        [Browsable(false)]
        public bool Silent
        {
            get { return false; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }
        #endif

        #if !XPS2IMG_UI
        [Option("", ShortOptionType.None10, Flags = OptionFlags.Internal)]
        public string CancellationObjectIds { get; set; }
        #else

        private const string CancellationObjectIdName = "cancellation-object-ids";

        [Xps2ImgUI.Attributes.Options.Option(CancellationObjectIdName)]
        [Browsable(false)]
        public string CancellationObjectIds
        {
            get { return CancellationObjectIdStatic; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        private static readonly Func<string> GetGuidNamePart = () => Guid.NewGuid().ToString().Substring(0, 8);

        private static readonly string CancellationObjectIdStatic = String.Format("{0}-{1}", GetGuidNamePart(), GetGuidNamePart());

        #endif

        private IEnumerable<string> SyncObjectsNames { get { return CancellationObjectIds.Split('-'); } }

        #if XPS2IMG_UI
        [Browsable(false)]
        #endif
        public string CancellationEventName { get { return SyncObjectsNames.First(); } }

        #if XPS2IMG_UI
        [Browsable(false)]
        #endif
        public string ParentAppMutexName { get { return SyncObjectsNames.Last(); } }

        #if XPS2IMG_UI

        public const string AutoValue = "Auto";

        private const string ProcessorsDisplayName = "Processors";
        private const string ProcessorsNameDefaultValue = AutoValue;
        private const string ProcessorsName = "processors-number";

        [Option("", ShortOptionType.None12, DefaultValue = ProcessorsNameDefaultValue)]
        [Xps2ImgUI.Attributes.Options.Option(ProcessorsName)]
        [DisplayName(ProcessorsDisplayName)]
        [TabbedDescription("Number of simultaneously running document processors\n  number of logical CPUs by default")]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessorsNumberConverter))]
        [DefaultValue(ProcessorsNameDefaultValue)]
        public string ProcessorsNumber
        {
            get { return _processorsNumber; }

            set
            {
                var processesNumber = 1;

                _processorsNumber =
                    (String.CompareOrdinal(AutoValue, value) != 0 && !Int32.TryParse(value, out processesNumber)) || (processesNumber <= 0 || processesNumber > ProcessorsNumberConverter.ProcessorCount)
                    ? AutoValue
                    : value;
            }
        }

        private string _processorsNumber;

        [Browsable(false)]
        public int ActualProcessorsNumber
        {
            get
            {
                return AutoValue == ProcessorsNumber
                           ? ProcessorsNumberConverter.ProcessorCount
                           : Int32.Parse(ProcessorsNumber);
            }
        }

        private const string ProcessorsPriorityDisplayName = "Processors Priority";
        private const string ProcessorsPriorityNameDefaultValue = AutoValue;
        private const string ProcessorsPriorityName = "processors-priority";

        [Option("", ShortOptionType.None13, DefaultValue = ProcessorsPriorityNameDefaultValue)]
        [Xps2ImgUI.Attributes.Options.Option(ProcessorsPriorityName)]
        [DisplayName(ProcessorsPriorityDisplayName)]
        [TabbedDescription("Document processors priority\n  Normal by default")]
        [Category(CategoryOptions)]
        [TypeConverter(typeof(ProcessPriorityClassConverter))]
        [DefaultValue(ProcessorsPriorityNameDefaultValue)]
        public string ProcessorsPriority
        {
            get { return _processorsPriority; }

            set
            {
                _processorsPriority = (String.CompareOrdinal(AutoValue, value) != 0 && !Enum.IsDefined(typeof(ProcessPriorityClass), value.RemoveSpaces()))
                                        ? AutoValue
                                        : value;
            }
        }

        private string _processorsPriority;

        [Browsable(false)]
        public ProcessPriorityClass ActualProcessorsPriority
        {
            get
            {
                var processorsPriority = _processorsPriority.RemoveSpaces();
                return Enum.IsDefined(typeof(ProcessPriorityClass), processorsPriority)
                        ? (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), processorsPriority)
                        : ProcessPriorityClass.Normal;
            }
        }

        public static readonly string[] ExcludedOnSave = new[] { CancellationObjectIdName };
        public static readonly string[] ExcludedUIOptions = new[] { ProcessorsName, ProcessorsPriorityName };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { PagesShortOption.ToString(CultureInfo.InvariantCulture) }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

        public static readonly string[] ExcludeOnResumeCheck = new[] { ProcessorsDisplayName, ProcessorsPriorityDisplayName };

        private static void ValidateProperty(object propertyValue, string validatorExpresion)
        {
            if (propertyValue == null)
            {
                return;
            }

            try
            {
                var validator = Parser.Parse(validatorExpresion);
                validator.Validate(propertyValue.ToString());
            }
            catch (ValidationException ex)
            {
                var message = ex.Message.ToCharArray();
                message[0] = Char.ToUpper(message[0]);
                throw new ValidationException(new string(message));
            }
        }
        #endif

        [Option(TestDescription, TestShortOption, ArgumentExpectancy.No)]
        #if XPS2IMG_UI
        [DisplayName("Test Mode")]
        [TabbedDescription(TestDescription)]
        [Xps2ImgUI.Attributes.Options.Option(TestShortOption)]
        [Category(CategoryOptions)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        #endif
        public bool Test { get; set; }

        #if !XPS2IMG_UI
        [Option("Clean (delete images)", ShortOptionType.None1, ArgumentExpectancy.No)]
        public bool Clean { get; set; }
        #else
        public const string CleanOption = " --clean";
        #endif
    }

    public static class OptionsValidators
    {
        public const string FileNameCharactersNotAllowed = " <>:\"/\\|?* characters are not allowed.";
        public const string FileNameValidationRegex      = @"/^([^\x00-\x1F<>:""/\\|?*])*$/";
    }
}
