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

            public class OutOfMemoryStrategy
            {
                private int _triesTotal;

                public int TriesSleepInterval { get; set; }

                public int TriesTotal
                {
                    get { return _triesTotal; }
                    set
                    {
                        _triesTotal = value;
                        MaxTries = TriesTotal*10;
                    }
                }

                public int MaxTries { get; private set; }

                public OutOfMemoryStrategy()
                {
                    TriesSleepInterval = 1000;
                    TriesTotal = 30;
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

            public PageCrop PageCrop { get; set; }

            public string OutputDir { get; set; }
            public string BaseImageName { get; set; }

            public int FirstPageIndex { get; set; }
            public string PrelimsPrefix { get; set; }

            public bool Clean { get; set; }

            public bool XpsRenderOptionsEnabled { get; set; }
            public RenderOptions XpsRenderOptions { get; set; }

            public bool OutOfMemoryStrategyEnabled { get; set; }
            public OutOfMemoryStrategy ConverterOutOfMemoryStrategy { get; set; }

            public Parameters()
            {
                FirstPageIndex = 1;
                PrelimsPrefix = "$";

                XpsRenderOptionsEnabled = true;
                XpsRenderOptions = new RenderOptions();

                ConverterOutOfMemoryStrategy = new OutOfMemoryStrategy();
            }
        }
    }
}
