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
    
    [Option("Output image type", DefaultValue = "Png")]
    public ImageType FileType { get; set; }

    [Option("Desired image size",
            ConverterType = typeof(RequiredTypeConverter),
            ValidationExpression = "/" + RequiredTypeConverter.ValidationRegex + "/"
    )]
    public Size? RequiredSize { get; set; }
    
    [Option("Image DPI (16-720)", DefaultValue = "120", ValidationExpression = "16-720")]
    public int Dpi { get; set; }

    [Option(
      "Beginning of the output image name\n  numeric if ommited: 01.png\n  name of src file if empty: src_file-01.png",
      ValidationExpression = @"/^([^<>:""/\\|?*])*$/"
    )]
    public string ImageName { get; set; }

    [Option("JPEG quality level (10-100)", 'q', DefaultValue = "85", ValidationExpression = "10-100")]
    public int JpegQuality { get; set; }

    [Option("TIFF compression method", DefaultValue = "Zip")]
    public TiffCompressOption TiffCompression { get; set; }

    [Option("Silent mode (no progress will be shown)", ArgumentExpectancy.No)]
    public bool Silent { get; set; }

    [Option(null, 'y', ArgumentExpectancy.No, Flags = OptionFlags.Internal)]
    public bool IsWorker { get; set; }
    
    [Option(null, 'w', ArgumentExpectancy.Required, Flags = OptionFlags.Internal)]
    public ConverterState WorkerCoverterState { get; set; }
  }
}
