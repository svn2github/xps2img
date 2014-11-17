using System;
using System.ComponentModel;

namespace Xps2Img.Shared.Localization
{
    public class LocalizableTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly ICustomTypeDescriptor _customTypeDescriptor;

        public LocalizableTypeDescriptionProvider()
        {
        }

        public LocalizableTypeDescriptionProvider(TypeDescriptionProvider parentTypeDescriptionProvider)
            : base(parentTypeDescriptionProvider)
        {
        }

        public LocalizableTypeDescriptionProvider(TypeDescriptionProvider parentTypeDescriptionProvider, ICustomTypeDescriptor customTypeDescriptor)
            : base(parentTypeDescriptionProvider)
        {
            _customTypeDescriptor = customTypeDescriptor;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return _customTypeDescriptor;
        }
    }
}

