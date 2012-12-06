using Xps2Img.Shared.CommandLine;

namespace Xps2Img.Shared.TypeConverters
{
    public class ProcessPriorityClassTypeConverter : StandardValuesTypeConverter
    {
        private static readonly string[] Processors =
        {
            Validation.AutoValue,
            "Idle",
            "Below Normal",
            "Normal",
            "Above Normal",
            "High"
        };

        public override string[] Values
        {
            get { return Processors; }
        }
    }
}
