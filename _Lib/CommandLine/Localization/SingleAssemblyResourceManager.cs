using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace CommandLine.Localization
{
    // http://stackoverflow.com/questions/1952638/single-assembly-multi-language-windows-forms-deployment-ilmerge-and-satellite-a
    public class SingleAssemblyResourceManager : ResourceManager
    {
        private readonly Type _contextTypeInfo;
        private CultureInfo _neutralResourcesCulture;

        private static readonly Type ResourceManagerType = typeof(ResourceManager);

        private static readonly MethodInfo AddResourceSetMethodInfo;
        private static readonly bool IsNet4;

        private readonly FieldInfo _resourceSetsFieldInfo;

        static SingleAssemblyResourceManager()
        {
            Func<Type, Type, MethodInfo> getAddResourceSetMethodInfo = (p1, p2) => ResourceManagerType.GetMethod("AddResourceSet", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { p1, p2, typeof(ResourceSet).MakeByRefType() }, null);

            AddResourceSetMethodInfo = getAddResourceSetMethodInfo(typeof(Hashtable), typeof(CultureInfo));

            IsNet4 = AddResourceSetMethodInfo == null;
            if (IsNet4)
            {
                AddResourceSetMethodInfo = getAddResourceSetMethodInfo(typeof(Dictionary<string, ResourceSet>), typeof(string));
            }
        }

        public SingleAssemblyResourceManager(Type type) : base(type)
        {
            _contextTypeInfo = type;

            if (IsNet4)
            {
                _resourceSetsFieldInfo = ResourceManagerType.GetField("_resourceSets", BindingFlags.Instance | BindingFlags.NonPublic);
            }
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

            if (store == null || AddResourceSetMethodInfo == null)
            {
                return base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            }

            var parameters = new [] { IsNet4 ? _resourceSetsFieldInfo.GetValue(this) : ResourceSets, IsNet4 ? (object)culture.Name : culture, new ResourceSet(store) };

            AddResourceSetMethodInfo.Invoke(this, parameters);

            return (ResourceSet)parameters.Last();
        }
    }
}
