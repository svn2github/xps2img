using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Attributes.Options
{
    public class OptionsBase
    {
        public OptionsBase()
        {
            ReflectionUtils.SetDefaultValues(this);
        }
    }
}
