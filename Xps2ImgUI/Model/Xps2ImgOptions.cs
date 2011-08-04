using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Media.Imaging;

using Xps2ImgUI.Attributes.Options;
using Xps2ImgUI.Dialogs;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Model
{
    public class Xps2ImgOptions : OptionsBase
    {
        public enum ImageType
        {
            Png,
            Jpeg,
            Tiff,
            Bmp,
            Gif
        }

        [UnnamedOption]
        [Editor(typeof(SelectXpsFileEditor), typeof(UITypeEditor))]
        [Description("XPS file to process")]
        [Category(Category.Parameters)]
        [DisplayName("XPS File*")]
        [DefaultValue("")]
        public string SrcFile { get; set; }

        [UnnamedOption(false)]
        [Editor(typeof(SelectFolderEditor), typeof(UITypeEditor))]
        [Description("Output folder (current folder by default)")]
        [Category(Category.Parameters)]
        [DisplayName("Output Folder")]
        [DefaultValue("")]
        public string OutDir { get; set; }

        [NamedOption("p")]
        [Description("  all:\t\t1-\n  single:\t1\n  set:\t\t1,3\n  range:\t1-10\n  combined:\t1,3-5,7-9,15-")]
        [Category(Category.Options)]
        [DisplayName("Page Number(s)")]
        [DefaultValue("")]
        public string Pages { get; set; }

        [NamedOption("i")]
        [Description("Output image type")]
        [Category(Category.Options)]
        [DisplayName("Image Type")]
        [DefaultValue(ImageType.Png)]
        public ImageType FileType { get; set; }

        [NamedOption("r")]
        [Description("Desired image size.\nDPI will be ignored if specified.\nSyntax:\n  width only:\t2000\n  height only:\tx1000\n  both:\t\t2000x1000\n\t\twidth for landscape orientation\n\t\theigth for portrait orientation\n")]
        [Category(Category.Options)]
        [DisplayName("Image Size")]
        [DefaultValue("")]
        public string RequiredSize { get; set; }

        private int? dpi;

        [NamedOption("d")]
        [Description("Image DPI (16-720)")]
        [Category(Category.Options)]
        [DisplayName("Image DPI")]
        [DefaultValue(120)]
        public int? Dpi
        {
            get { return dpi; }
            set
            {
                if (value != null && (value < 0 || value > 120))
                {
                    throw new NotSupportedException();
                }
                dpi = value;
            }
        }

        [NamedOption("n")]
        [Description("Beginning of the output image name\n  numeric if ommited: 01.png\n  name of src file if empty: src_file-01.png")]
        [Category(Category.Options)]
        [DisplayName("Image Name")]
        [DefaultValue("")]
        public string ImageName { get; set; }

        [NamedOption("q")]
        [Description("JPEG quality level (10-100)")]
        [Category(Category.Options)]
        [DisplayName("JPEG Quality")]
        [DefaultValue(85)]
        public int? JpegQuality { get; set; }

        [NamedOption("t")]
        [Description("TIFF compression method")]
        [Category(Category.Options)]
        [DisplayName("TIFF Compression")]
        [DefaultValue(TiffCompressOption.Zip)]
        public TiffCompressOption TiffCompression { get; set; }
    }

    public class SelectXpsFileEditor : SelectFileEditor
    {
        SelectXpsFileEditor() :
            base("XPS Files (*.xps)|*.xps|" + Utils.Filter.AllFiles)
        {
        }
    }
}