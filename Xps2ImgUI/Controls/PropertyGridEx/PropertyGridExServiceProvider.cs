using System;
using System.Windows.Forms.Design;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public class PropertyGridExServiceProvider : IServiceProvider
    {
        private readonly PropertyGridExUIService _service;

        public PropertyGridExServiceProvider(PropertyGridEx grid)
        {
            _service = new PropertyGridExUIService(grid);
        }

        public object GetService(Type serviceType)
        {
            return serviceType == typeof(IUIService) ? _service : null;
        }
    }
}

