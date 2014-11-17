using System.ComponentModel;
using System.Resources;

namespace Xps2Img.Shared.Localization
{
    public static class LocalizableTypeDescriptionProviderInstaller
    {
        public static LocalizableTypeDescriptionProvider AddProvider(object obj, ResourceManager resourceManager, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy = null)
        {
            var parentProvider = TypeDescriptor.GetProvider(obj);
            var localizableTypeDescriptor = new LocalizableTypeDescriptor(parentProvider.GetTypeDescriptor(obj), resourceManager, localizablePropertyDescriptorStrategy ?? DefaultLocalizablePropertyDescriptorStrategy.Instance);
            var localizableTypeDescriptionProvider = new LocalizableTypeDescriptionProvider(parentProvider, localizableTypeDescriptor);
            TypeDescriptor.AddProvider(localizableTypeDescriptionProvider, obj);
            return localizableTypeDescriptionProvider;
        }

        public static LocalizableTypeDescriptionProvider AddProvider<T>(ResourceManager resourceManager, ILocalizablePropertyDescriptorStrategy localizablePropertyDescriptorStrategy = null)
        {
            var parentProvider = TypeDescriptor.GetProvider(typeof(T));
            var localizableTypeDescriptor = new LocalizableTypeDescriptor(parentProvider.GetTypeDescriptor(typeof(T)), resourceManager, localizablePropertyDescriptorStrategy ?? DefaultLocalizablePropertyDescriptorStrategy.Instance);
            var localizableTypeDescriptionProvider = new LocalizableTypeDescriptionProvider(parentProvider, localizableTypeDescriptor);
            TypeDescriptor.AddProvider(localizableTypeDescriptionProvider, typeof(T));
            return localizableTypeDescriptionProvider;
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

