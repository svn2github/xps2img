namespace Xps2ImgUI.Converters
{
    public class ProcessPriorityClassConverter : StandardValuesTypeConverter
    {
        private static readonly string[] Processors =
        {
            Xps2Img.CommandLine.Options.AutoValue,
            "Idle",
            "BelowNormal",
            "Normal",
            "AboveNormal",
            "High"
        };

        public override string[] Values
        {
            get { return Processors; }
        }
    }
}
