using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    public abstract class TypeResolverContextBase : ITypeResolverContext
    {
        private readonly List<ITypeSource> _classTypeSources = new List<ITypeSource>();
        private ICollectionFormatter _defaultCollectionFormatter;

        protected TypeResolverContextBase(ICollectionFormatter defaultCollectionFormatter)
        {
            _defaultCollectionFormatter = defaultCollectionFormatter;
        }

        public void AddTypeSource(ITypeSource typeSource)
        {
            _classTypeSources.Add(typeSource);
        }

        public void SetCollectionFormatter(ICollectionFormatter formatter)
        {
            _defaultCollectionFormatter = formatter;
        }

        public IEnumerable<ITypeSource> TypeSources => _classTypeSources;

        public IResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, null);
        }

        public IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            if (typeInfo == null)
            {
                return null;
            }

            ICollectionFormatter collectionFormatter = null;
            ResolvedTypeInfo type = null;
            foreach (var classLookup in _classTypeSources)
            {
                var foundClass = classLookup.GetType(typeInfo);
                if (foundClass != null)
                {
                    collectionFormatter = classLookup.CollectionFormatter;
                    type = new ResolvedTypeInfo(foundClass);
                    break;
                }
            }

            type = type ?? ResolveType(typeInfo);
            if (typeInfo.GenericTypeParameters.Any())
            {
                type.Name = FormatGenerics(type, typeInfo.GenericTypeParameters.Select(x => Get(x, collectionFormat)));
            }

            if (typeInfo.IsCollection)
            {
                collectionFormatter = (collectionFormat != null) ? new CollectionFormatter(collectionFormat) : (collectionFormatter ?? _defaultCollectionFormatter);
                type.IsPrimitive = false;
                type.Name = collectionFormatter.AsCollection(type);
            }

            return type;
        }

        protected abstract string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes);

        protected abstract ResolvedTypeInfo ResolveType(ITypeReference typeInfo);
    }
}