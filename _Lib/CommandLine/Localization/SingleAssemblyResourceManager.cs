using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace CommandLine.Localization
{
    public class SingleAssemblyResourceManager : ResourceManager
    {
        private readonly Type _type;
        private readonly Dictionary<string, ResourceSet> _resourceSets = new Dictionary<string, ResourceSet>();

        public SingleAssemblyResourceManager(Type type)
            : base(type)
        {
            _type = type;
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            lock (_resourceSets)
            {
                var cultureName = culture.Name;

                ResourceSet resourceSet;

                if (_resourceSets.TryGetValue(cultureName, out resourceSet))
                {
                    return resourceSet;
                }

                var resourceFileName = GetResourceFileName(culture);

                var manifestResourceStream = MainAssembly.GetManifestResourceStream(_type, resourceFileName) ??
                                             MainAssembly.GetManifestResourceStream(resourceFileName);

                resourceSet = manifestResourceStream != null
                                ? new ResourceSet(manifestResourceStream)
                                : base.InternalGetResourceSet(culture, createIfNotExists, tryParents);

                _resourceSets.Add(cultureName, resourceSet);

                return resourceSet;
            }
        }
    }
}
