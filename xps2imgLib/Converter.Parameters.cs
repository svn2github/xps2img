using System.Drawing;

namespace Xps2ImgLib
{
    public partial class Converter
    {
        public class Parameters
        {
            public class RenderOptions
            {
                public bool HighQualityBitmapScalingMode { get; set; }

                // Improves (sometimes) text rendering quality as of .NET 4.0+.
                public bool AnimatedVisualTextHintingMode { get; set; }

                public RenderOptions()
                {
                    HighQualityBitmapScalingMode = true;
                    AnimatedVisualTextHintingMode = true;
                }
            }

            public bool Silent { get; set; }
            public bool IgnoreExisting { get; set; }
            public bool IgnoreErrors { get; set; }
            public bool Test { get; set; }

            public int StartPage { get; set; }
            public int EndPage { get; set; }

            public ImageType ImageType { get; set; }
            public bool ShortenExtension { get; set; }
            public ImageOptions ImageOptions { get; set; }

            public Size? RequiredSize { get; set; }
            public int Dpi { get; set; }

            public string OutputDir { get; set; }
            public string BaseImageName { get; set; }

            public int FirstPageIndex { get; set; }
            public string PrelimsPrefix { get; set; }

            public bool Clean { get; set; }

            public RenderOptions XpsRenderOptions { get; set; }

            public Parameters()
            {
                FirstPageIndex = 1;
                PrelimsPrefix = "$";
                XpsRenderOptions = new RenderOptions();
            }
        }
    }
}
