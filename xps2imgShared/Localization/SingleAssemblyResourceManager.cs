using System;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Xps2Img.Shared.Localization
{
    // http://stackoverflow.com/questions/1952638/single-assembly-multi-language-windows-forms-deployment-ilmerge-and-satellite-a
    public class SingleAssemblyResourceManager : ResourceManager
    {
        private readonly Type _contextTypeInfo;
        private CultureInfo _neutralResourcesCulture;

        private readonly MethodInfo _addResourceSetMethodInfo;

        public SingleAssemblyResourceManager(Type type) : base(type)
        {
            _contextTypeInfo = type;

            _addResourceSetMethodInfo = typeof(ResourceManager).GetMethod("AddResourceSet", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Hashtable), typeof(CultureInfo), typeof(ResourceSet).MakeByRefType() }, null);
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            var resourceSet = (ResourceSet)ResourceSets[culture];

            if (resourceSet != null)
            {
                return resourceSet;
            }

            if (_neutralResourcesCulture == null)
            {
                _neutralResourcesCulture = GetNeutralResourcesLanguage(MainAssembly);
            }

            if (_neutralResourcesCulture.Equals(culture))
            {
                culture = CultureInfo.InvariantCulture;
            }

            var resourceFileName = GetResourceFileName(culture);

            var store = MainAssembly.GetManifestResourceStream(_contextTypeInfo, resourceFileName);

            if (store == null)
            {
                return base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            }

            var parameters = new object[]{ ResourceSets, culture, new ResourceSet(store) };

            _addResourceSetMethodInfo.Invoke(this, parameters);

            return (ResourceSet)parameters.Last();
        }
    }
}
