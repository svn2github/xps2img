using System.ComponentModel;
using System.Windows.Media.Imaging;
using Xps2Img.CommandLine.TypeConverters;

#if XPS2IMG_UI
using System;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;

using CommandLine.Validation;

using Xps2ImgUI.Attributes.Options;
using Xps2ImgUI.Converters;
using Xps2ImgUI.Dialogs;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;
#else
using System.Drawing;
using System.Collections.Generic;
#endif

using Xps2Img.Xps2Img;

// ReSharper disable LocalizableElement

namespace Xps2Img.CommandLine
{
    #if !XPS2IMG_UI
    [Description("\nConverts XPS (XML Paper Specification) document to set of images.")]
    #endif
    public class Options
    #if XPS2IMG_UI
        : OptionsBase
    #endif
    {
        private const string SrcFileDescription = "XPS file to process";

        [global::CommandLine.UnnamedOption(SrcFileDescription)]
        #if XPS2IMG_UI
        [DisplayName("XPS File")]
        [UnnamedOption]
        [Category(Category.Parameters)]
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

        private const string OutDirDescription = "Output folder\n  current folder by default";

        [global::CommandLine.UnnamedOption(OutDirDescription, false)]
        #if XPS2IMG_UI
        [DisplayName("Output Folder")]
        [UnnamedOption(false)]
        [Editor(typeof(SelectFolderEditor), typeof(UITypeEditor))]
        [Category(Category.Parameters)]
        [DefaultValue(null)]
        [TabbedDescription(OutDirDescription)]
        #endif
        public string OutDir
        {
            get { return TrimFileName(_outDir); }
            set { _outDir = value; }
        }

        private string _outDir;

        private static readonly char[] _fileNameTrimCharacters = "\"\x20\t\r\n".ToCharArray();

        private static string TrimFileName(string val)
        {
            return val == null ? null : val.Trim(_fileNameTrimCharacters);
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

        [global::CommandLine.Option(
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
        [Category(Category.Options)]
        [Option(PagesShortOption)]
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

        private const string FileTypeDescription = "Image type";
        private const char FileTypeShortOption = 'f';

        [global::CommandLine.Option(FileTypeDescription, FileTypeShortOption, DefaultValue = "png")]
        #if XPS2IMG_UI
        [DisplayName("Image Type")]
        [TabbedDescription(FileTypeDescription)]
        [Option(FileTypeShortOption)]
        [Category(Category.Options)]
        [DefaultValue(ImageType.Png)]
        #endif
        public ImageType FileType { get; set; }

        private const string RequiredSizeDescription = "Desired image size\n  DPI will be ignored if specified \nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theight for portrait orientation";
        private const char RequiredSizeOption = 'r';
        private const string RequiredSizeValidationExpression = "/" + RegexMatchEmptyString + RequiredSizeTypeConverter.ValidationRegex + "/";

        [global::CommandLine.Option(
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
        [Option(RequiredSizeOption)]
        [Category(Category.Options)]
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

        private const string DpiDescription = "Image DPI (16-2350)";
        private const char DpiShortOption = 'd';
        private const string DpiValidationExpression = "16-2350";

        [global::CommandLine.Option(DpiDescription, DpiShortOption, DefaultValue = "120", ValidationExpression = DpiValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Image DPI")]
        [TabbedDescription(DpiDescription)]
        [Option(DpiShortOption)]
        [Category(Category.Options)]
        [DefaultValue(120)]
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

        [global::CommandLine.Option(ImageNameDescription, ImageNameShortOption, ValidationExpression = ImageNameValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Image Prefix")]
        [TabbedDescription(ImageNameDescription)]
        [Option(ImageNameShortOption)]
        [Category(Category.Options)]
        [DefaultValue(null)]
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                if (value != Option.Empty)
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

        private const string JpegQualityDescription = "JPEG quality level (10-100)";
        private const char JpegQualityShortOption = 'q';
        private const string JpegQualityValidationExpression = "10-100";

        [global::CommandLine.Option(JpegQualityDescription, JpegQualityShortOption, DefaultValue = "85", ValidationExpression = JpegQualityValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("JPEG Quality")]
        [TabbedDescription(JpegQualityDescription)]
        [Option(JpegQualityShortOption)]
        [Category(Category.Options)]
        [DefaultValue(85)]
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

        [global::CommandLine.Option(TiffCompressionDescription, TiffCompressionShortOption, DefaultValue = "zip")]
        #if XPS2IMG_UI
        [DisplayName("TIFF Compression")]
        [TabbedDescription(TiffCompressionDescription)]
        [Option(TiffCompressionShortOption)]
        [Category(Category.Options)]
        [DefaultValue(TiffCompressOption.Zip)]
        #endif
        public TiffCompressOption TiffCompression { get; set; }

        private const string FirstPageIndexDescription = "Document body first page index";
        private const char FirstPageIndexShortOption = 'a';
        private const string FirstPageIndexValidationExpression = "1-1000000";

        [global::CommandLine.Option(FirstPageIndexDescription, FirstPageIndexShortOption, DefaultValue = "1", ValidationExpression = FirstPageIndexValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("First Page Index")]
        [TabbedDescription(FirstPageIndexDescription)]
        [Option(FirstPageIndexShortOption)]
        [Category(Category.Options)]
        [DefaultValue(1)]
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

        private const string PrelimsPrefixDescription = "Preliminaries prefix";
        private const char PrelimsPrefixShortOption = 'x';
        private const string PrelimsPrefixDefaultValue = "$";
        private const string PrelimsPrefixValidationExpression = OptionsValidators.FileNameValidationRegex;

        [global::CommandLine.Option(PrelimsPrefixDescription, PrelimsPrefixShortOption, DefaultValue = PrelimsPrefixDefaultValue, ValidationExpression = PrelimsPrefixValidationExpression)]
        #if XPS2IMG_UI
        [DisplayName("Preliminaries Prefix")]
        [TabbedDescription(PrelimsPrefixDescription)]
        [Option(PrelimsPrefixShortOption)]
        [Category(Category.Options)]
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

        private const string TestDescription = "Test mode (no files will be written)";
        private const char TestShortOption = 'e';

        #if !XPS2IMG_UI
        private const string SilentModeDescription = "Silent mode (no progress will be shown)";
        private const char SilentModeShortOption = 's';

        [global::CommandLine.Option(SilentModeDescription, SilentModeShortOption, global::CommandLine.ArgumentExpectancy.No)]
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
        [global::CommandLine.Option("", global::CommandLine.ShortOptionType.None1, Flags = global::CommandLine.OptionFlags.Internal)]
        public string CancellationObjectId { get; set; }
        #else

        private const string CancellationObjectIdName = "cancellation-object-id";

        [Option(CancellationObjectIdName)]
        [Browsable(false)]
        public string CancellationObjectId
        {
            get { return CancellationObjectIdStatic; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        private static readonly string CancellationObjectIdStatic = Guid.NewGuid().ToString();

        #endif

        #if XPS2IMG_UI

        public const string AutoValue = "Auto";

        private const string ProcessorsNameDefaultValue = AutoValue;
        private const string ProcessorsName = "processors-number";

        [global::CommandLine.Option("", global::CommandLine.ShortOptionType.None2, DefaultValue = ProcessorsNameDefaultValue)]
        [Option(ProcessorsName)]
        [DisplayName("Processors")]
        [TabbedDescription("Number of simultaneously running document processors\n  number of logical CPUs by default")]
        [Category(Category.Options)]
        [TypeConverter(typeof(ProcessorsNumberConverter))]
        [DefaultValue(ProcessorsNameDefaultValue)]
        public string ProcessorsNumber
        {
            get { return _processorsNumber; }

            set
            {
                var processesNumber = 1;

                _processorsNumber =
                    (AutoValue.CompareTo(value) != 0 && !Int32.TryParse(value, out processesNumber)) || (processesNumber <= 0 || processesNumber > ProcessorsNumberConverter.ProcessorCount)
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
                           : int.Parse(ProcessorsNumber);
            }
        }

        private const string ProcessorsPriorityNameDefaultValue = AutoValue;
        private const string ProcessorsPriorityName = "processors-priority";

        [global::CommandLine.Option("", global::CommandLine.ShortOptionType.None3, DefaultValue = ProcessorsPriorityNameDefaultValue)]
        [Option(ProcessorsPriorityName)]
        [DisplayName("Processors Priority")]
        [TabbedDescription("Document processors priority\n  Normal by default")]
        [Category(Category.Options)]
        [TypeConverter(typeof(ProcessPriorityClassConverter))]
        [DefaultValue(ProcessorsPriorityNameDefaultValue)]
        public string ProcessorsPriority
        {
            get { return _processorsPriority; }

            set
            {
               _processorsPriority = (AutoValue.CompareTo(value) != 0 && !Enum.IsDefined(typeof(ProcessPriorityClass), value))
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
                return Enum.IsDefined(typeof(ProcessPriorityClass), _processorsPriority)
                        ? (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), _processorsPriority)
                        : ProcessPriorityClass.Normal;
            }
        }

        public static readonly string[] ExcludedOnSave = new[] { CancellationObjectIdName };
        public static readonly string[] ExcludedUIOptions = new[] { ProcessorsName, ProcessorsPriorityName };
        public static readonly string[] ExcludedOnLaunch = ExcludedUIOptions.Concat(new[] { PagesShortOption.ToString() }).ToArray();
        public static readonly string[] ExcludedOnView = ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();

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

        [global::CommandLine.Option(TestDescription, TestShortOption, global::CommandLine.ArgumentExpectancy.No)]
        #if XPS2IMG_UI
        [DisplayName("Test Mode")]
        [TabbedDescription(TestDescription)]
        [Option(TestShortOption)]
        [Category(Category.Options)]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoConverter))]
        #endif
        public bool Test { get; set; }
    }

    public static class OptionsValidators
    {
        public const string FileNameValidationRegex = @"/^([^<>:""/\\|?*])*$/";
    }
}
