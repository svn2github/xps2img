using System;
using System.ComponentModel;
using System.Resources;

namespace Xps2Img.Shared.Localization
{
    public static class LocalizableTypeDescriptionProviderInstaller
    {
        public static LocalizableTypeDescriptionProvider AddProvider(object obj, ResourceManager resourceManager, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy = null)
        {
            var type = obj as Type;
            var isType = type != null;

            var parentProvider = isType ? TypeDescriptor.GetProvider(type) : TypeDescriptor.GetProvider(obj);
            var localizableTypeDescriptor = new LocalizableTypeDescriptor(isType ? parentProvider.GetTypeDescriptor(type) : parentProvider.GetTypeDescriptor(obj), resourceManager, localizablePropertyDescriptorStrategy ?? DefaultLocalizablePropertyDescriptorStrategy.Instance);
            var localizableTypeDescriptionProvider = new LocalizableTypeDescriptionProvider(parentProvider, localizableTypeDescriptor);

            if (isType)
            {
                TypeDescriptor.AddProvider(localizableTypeDescriptionProvider, type);
            }
            else
            {
                TypeDescriptor.AddProvider(localizableTypeDescriptionProvider, obj);
            }

            return localizableTypeDescriptionProvider;
        }

        public static LocalizableTypeDescriptionProvider AddProvider<T>(ResourceManager resourceManager, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy = null)
        {
            return AddProvider(typeof (T), resourceManager, localizablePropertyDescriptorStrategy);
        }

        public static void RemoveProvider(object obj, LocalizableTypeDescriptionProvider localizableTypeDescriptionProvider)
        {
            TypeDescriptor.RemoveProvider(localizableTypeDescriptionProvider, obj);
        }

        public static void RemoveProvider<T>(LocalizableTypeDescriptionProvider localizableTypeDescriptionProvider)
        {
            TypeDescriptor.RemoveProvider(localizableTypeDescriptionProvider, typeof(T));
        }
    }
}

