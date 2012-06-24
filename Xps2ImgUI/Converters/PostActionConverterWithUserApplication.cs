using System.Linq;

namespace Xps2ImgUI.Converters
{
    public class PostActionConverterWithUserApplication : PostActionConverter
    {
        public const string UserApplication = "Execute Program";

        public override string[] Values
        {
            get { return base.Values.Concat(new[] { UserApplication }).ToArray(); }
        }
    }
}
