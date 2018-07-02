using System;
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
                public static readonly OutOfMemoryStrategy Default = new OutOfMemoryStrategy(10, TimeSpan.FromSeconds(30));
                public static readonly OutOfMemoryStrategy Fit     = new OutOfMemoryStrategy(5,  TimeSpan.FromSeconds(2));

                public TimeSpan SleepInterval { get; private set; }

                public int Tries { get; private set; }

                public OutOfMemoryStrategy(int tries, TimeSpan sleepInterval)
                {
                    Tries = tries;
                    SleepInterval = sleepInterval;
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
            public PageCropThreshold PageCropThreshold { get; set; }
            public Size PageCropMargin { get; set; }

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

                ConverterOutOfMemoryStrategy = OutOfMemoryStrategy.Default;
            }
        }
    }
}
