using System.ComponentModel;
using System.Windows.Media.Imaging;

#if XPS2IMG_UI
using System.Drawing.Design;
using Xps2ImgUI.Attributes.Options;
using Xps2ImgUI.Converters;
using Xps2ImgUI.Dialogs;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;
#else
using System.Drawing;
using System.Collections.Generic;
using CommandLine;
using Xps2Img.CommandLine.TypeConverters;
#endif

using Xps2Img.Xps2Img;

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
		#if XPS2IMG_UI
		[DisplayName("XPS File")]
		[UnnamedOption]
		[Category(Category.Parameters)]
		[Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
		[TabbedDescription
		#else
		[UnnamedOption
		#endif
		("XPS file to process"
		#if XPS2IMG_UI
		+ " (required)"
		#endif
		)]
		public string SrcFile { get; set; }

		#if XPS2IMG_UI
		[DisplayName("Output Folder")]
		[UnnamedOption(false)]
		[Editor(typeof(SelectFolderEditor), typeof(UITypeEditor))]
		[Category(Category.Parameters)]
		[TabbedDescription
		#else
		[UnnamedOption
		#endif
		("Output folder\n  current folder by default"
		#if XPS2IMG_UI
		)]
		#else
		, false)]
		#endif
		public string OutDir { get; set; }

		private const string PagesDescription = "Page number(s)\n  all pages by default\nSyntax:\n  all:\t\t1-\n  single:\t1\n  set:\t\t1,3\n  range:\t1-10 or -10 or 10-\n  combined:\t1,3-5,7-9,15-";
		private const char PagesShortOption = 'p';

		#if XPS2IMG_UI
		[DisplayName("Page Number(s)")]
		[TabbedDescription(PagesDescription)]
		[Category(Category.Options)]
		[Option(PagesShortOption)]
		public string Pages { get; set; }
		#else
		[Option(
		  PagesDescription,
		  PagesShortOption,
		  DefaultValue = "",
		  ConverterType = typeof(IntervalTypeConverter),
		  ValidationExpression = "/" + Interval.ValidationRegex + "/"
		)]
		public List<Interval> Pages { get; set; }
		#endif

		private const string FileTypeDescription = "Image type";
		private const char FileTypeShortOption = 'f';

		#if XPS2IMG_UI
		[DisplayName("Image Type")]
		[TabbedDescription(FileTypeDescription)]
		[Option(FileTypeShortOption)]
		[Category(Category.Options)]
		[DefaultValue(ImageType.Png)]
		#else
		[Option(FileTypeDescription, FileTypeShortOption, DefaultValue = "png")]
		#endif
		public ImageType FileType { get; set; }

		private const string RequiredSizeDescription = "Desired image size\n  DPI will be ignored if specified \nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theight for portrait orientation";
		private const char RequiredSizeOption = 'r';

		#if XPS2IMG_UI
		[DisplayName("Image Size")]
		[TabbedDescription(RequiredSizeDescription)]
		[Option(RequiredSizeOption)]
		[Category(Category.Options)]
		public string RequiredSize { get; set; }
		#else
		[Option(
		  RequiredSizeDescription,
		  RequiredSizeOption,
		  ConverterType = typeof(RequiredSizeTypeConverter),
		  ValidationExpression = "/" + RequiredSizeTypeConverter.ValidationRegex + "/"
		)]
		public Size? RequiredSize { get; set; }
		#endif

		private const string DpiDescription = "Image DPI (16-1500)";
		private const char DpiShortOption = 'd';

		#if XPS2IMG_UI
		[DisplayName("Image DPI")]
		[TabbedDescription(DpiDescription)]
		[Option(DpiShortOption)]
		[Category(Category.Options)]
		[DefaultValue(120)]
		public int? Dpi { get; set; }
		#else
		[Option(DpiDescription, DpiShortOption, DefaultValue = "120", ValidationExpression = "16-1500")]
		public int Dpi { get; set; }
		#endif

		private const string ImageNameDescription = "Image prefix\n  numeric if ommited: 01.png\n  name of src file if empty (-i \"\"): src_file-01.png";
		private const char ImageNameShortOption = 'i';

		#if XPS2IMG_UI
		[DisplayName("Image Prefix")]
		[TabbedDescription(ImageNameDescription)]
		[Option(ImageNameShortOption)]
		[Category(Category.Options)]
		#else
		[Option(ImageNameDescription, ImageNameShortOption, ValidationExpression = OptionsValidators.FileNameValidationRegex)]
		#endif
		public string ImageName { get; set; }

		private const string JpegQualityDescription = "JPEG quality level (10-100)";
		private const char JpegQualityShortOption = 'q';

		#if XPS2IMG_UI
		[DisplayName("JPEG Quality")]
		[TabbedDescription(JpegQualityDescription)]
		[Option(JpegQualityShortOption)]
		[Category(Category.Options)]
		[DefaultValue(85)]
		public int? JpegQuality { get; set; }
		#else
		[Option(JpegQualityDescription, JpegQualityShortOption, DefaultValue = "85", ValidationExpression = "10-100")]
		public int JpegQuality { get; set; }
		#endif

		private const string TiffCompressionDescription = "TIFF compression method";
		private const char TiffCompressionShortOption = 't';

		#if XPS2IMG_UI
		[DisplayName("TIFF Compression")]
		[TabbedDescription(TiffCompressionDescription)]
		[Option(TiffCompressionShortOption)]
		[Category(Category.Options)]
		[DefaultValue(TiffCompressOption.Zip)]
		#else
		[Option(TiffCompressionDescription, TiffCompressionShortOption, DefaultValue = "zip")]
		#endif
		public TiffCompressOption TiffCompression { get; set; }

		private const string FirstPageIndexDescription = "Document body first page index";
		private const char FirstPageIndexShortOption = 'a';

		#if XPS2IMG_UI
		[DisplayName("First Page Index")]
		[TabbedDescription(FirstPageIndexDescription)]
		[Option(FirstPageIndexShortOption)]
		[Category(Category.Options)]
		[DefaultValue(1)]
		public int? FirstPageIndex { get; set; }
		#else
		[Option(FirstPageIndexDescription, FirstPageIndexShortOption, DefaultValue = "1", ValidationExpression = "1-1000000")]
		public int FirstPageIndex { get; set; }
		#endif

		private const string PrelimsPrefixDescription = "Preliminaries prefix";
		private const char PrelimsPrefixShortOption = 'x';

		#if XPS2IMG_UI
		[DisplayName("Preliminaries Prefix")]
		[TabbedDescription(PrelimsPrefixDescription)]
		[Option(PrelimsPrefixShortOption)]
		[Category(Category.Options)]
		[DefaultValue('$')]
		public char? PrelimsPrefix { get; set; }
		#else
		[Option(PrelimsPrefixDescription, PrelimsPrefixShortOption, DefaultValue = "$", ValidationExpression = OptionsValidators.FileNameValidationRegex)]
		public char PrelimsPrefix { get; set; }
		#endif

		private const string TestDescription = "Test mode (no files will be written)";
		private const char TestShortOption = 'e';

		#if XPS2IMG_UI
		[DisplayName("Test Mode")]
		[TabbedDescription(TestDescription)]
		[Option(TestShortOption)]
		[Category(Category.Options)]
		[DefaultValue(false)]
		[TypeConverter(typeof(YesNoConverter))]
		#else
		[Option(TestDescription, TestShortOption, ArgumentExpectancy.No)]
		#endif
		public bool Test { get; set; }

		#if !XPS2IMG_UI
		private const string SilentModeDescription = "Silent mode (no progress will be shown)";
		private const char SilentModeShortOption = 's';

		[Option(SilentModeDescription, SilentModeShortOption, ArgumentExpectancy.No)]
		public bool Silent { get; set; }
		#endif
	}

	#if !XPS2IMG_UI
	public static class OptionsValidators
	{
		public const string FileNameValidationRegex = @"/^([^<>:""/\\|?*])*$/";
	}
	#endif
}
