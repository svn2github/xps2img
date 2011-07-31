﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;

using CommandLine;

namespace Xps2Img.CommandLine
{
	[Description("\nConverts XPS (XML Paper Specification) document to set of images.")]
	public class Options
	{
		[UnnamedOption("XPS file to process")]
		public string SrcFile { get; set; }

		[UnnamedOption("Output folder\n  current folder by default", false)]
		public string OutDir { get; set; }

		[Option(
		  "Page number(s)\n  all pages by default\nSyntax:\n  all:\t\t1-\n  single:\t1\n  set:\t\t1,3\n  range:\t1-10\n  combined:\t1,3-5,7-9,15-",
		  DefaultValue = "",
		  ConverterType = typeof(IntervalTypeConverter),
		  ValidationExpression = "/" + Interval.ValidationRegex + "/"
		)]
		public List<Interval> Pages { get; set; }

		[Option("Image type", DefaultValue = "png")]
		public ImageType FileType { get; set; }

		[Option(
		  "Desired image size\n  DPI will be ignored if specified \nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theight for portrait orientation",
		  ConverterType = typeof(RequiredSizeTypeConverter),
		  ValidationExpression = "/" + RequiredSizeTypeConverter.ValidationRegex + "/"
		)]
		public Size? RequiredSize { get; set; }

		[Option("Image DPI (16-1500)", DefaultValue = "120", ValidationExpression = "16-1500")]
		public int Dpi { get; set; }

		[Option("Image prefix\n  numeric if ommited: 01.png\n  name of src file if empty: src_file-01.png", ValidationExpression = OptionsValidators.FileNameValidationRegex)]
		public string ImageName { get; set; }

		[Option("JPEG quality level (10-100)", 'q', DefaultValue = "85", ValidationExpression = "10-100")]
		public int JpegQuality { get; set; }

		[Option("TIFF compression method", DefaultValue = "zip")]
		public TiffCompressOption TiffCompression { get; set; }

		[Option("Document body first page index", 'a', DefaultValue = "1", ValidationExpression = "1-1000000")]
		public int FirstPageIndex { get; set; }

		[Option("Preliminaries prefix", 'x', DefaultValue = "$", ValidationExpression = OptionsValidators.FileNameValidationRegex)]
		public char PrelimsPrefix { get; set; }

		[Option("Test mode (no files will be written)", 'e', ArgumentExpectancy.No)]
		public bool Test { get; set; }

		[Option("Silent mode (no progress will be shown)", ArgumentExpectancy.No)]
		public bool Silent { get; set; }
	}

	public static class OptionsValidators
	{
		public const string FileNameValidationRegex = @"/^([^<>:""/\\|?*])*$/";
	}
}
