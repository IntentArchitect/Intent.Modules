using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    public abstract class TypeResolverContextBase : ITypeResolverContext
    {
        private readonly List<ITypeSource> _typeSources = new List<ITypeSource>();
        private ICollectionFormatter _defaultCollectionFormatter;
        private INullableFormatter _defaultNullableFormatter;

        protected TypeResolverContextBase(ICollectionFormatter defaultCollectionFormatter, INullableFormatter nullableFormatter)
        {
            _defaultCollectionFormatter = defaultCollectionFormatter;
            _defaultNullableFormatter = nullableFormatter;
        }

        public void AddTypeSource(ITypeSource typeSource)
        {
            _typeSources.Add(typeSource);
        }

        public void SetCollectionFormatter(ICollectionFormatter formatter)
        {
            _defaultCollectionFormatter = formatter;
        }

        public void SetNullableFormatter(INullableFormatter formatter)
        {
            _defaultNullableFormatter = formatter;
        }

        public IEnumerable<ITypeSource> TypeSources => _typeSources;

        public IResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, default(ICollectionFormatter));
        }

        public virtual IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            return Get(typeInfo, !string.IsNullOrWhiteSpace(collectionFormat) ? new CollectionFormatter(collectionFormat) : null);
        }

        public virtual IResolvedTypeInfo Get(ITypeReference typeInfo, ICollectionFormatter collectionFormatter)
        {
            if (typeInfo == null)
            {
                return null;
            }

            ICollectionFormatter typeSourceCollectionFormatter = null;
            INullableFormatter nullableFormatter = null;
            ResolvedTypeInfo type = null;
            foreach (var classLookup in _typeSources)
            {
                var foundClass = classLookup.GetType(typeInfo);
                if (foundClass != null)
                {
                    typeSourceCollectionFormatter = classLookup.CollectionFormatter;
                    nullableFormatter = classLookup.NullableFormatter;
                    type = new ResolvedTypeInfo(foundClass);
                    break;
                }
            }

            type = type ?? ResolveType(typeInfo);
            if (typeInfo.GenericTypeParameters.Any())
            {
                var resolvedGenerics = typeInfo.GenericTypeParameters.Select(x => Get(x, collectionFormatter)).ToList();
                type.GenericTypes = resolvedGenerics;
                type.Name = FormatGenerics(type, resolvedGenerics);
            }

            if (typeInfo.IsCollection)
            {
                type.Name = (collectionFormatter ?? typeSourceCollectionFormatter ?? _defaultCollectionFormatter).AsCollection(type);
            }

            if (typeInfo.IsNullable)
            {
                nullableFormatter = nullableFormatter ?? _defaultNullableFormatter;
                type.Name = nullableFormatter.AsNullable(type);
            }

            return type;
        }

        public virtual IResolvedTypeInfo Get(ITypeReference typeInfo, ITypeSource typeSource)
        {
            if (typeInfo == null)
            {
                return null;
            }

            ICollectionFormatter collectionFormatter = null;
            INullableFormatter nullableFormatter = null;
            ResolvedTypeInfo type = null;
            var foundClass = typeSource.GetType(typeInfo);
            if (foundClass != null)
            {
                collectionFormatter = typeSource.CollectionFormatter;
                type = new ResolvedTypeInfo(foundClass);
            }
            else
            {
                foreach (var classLookup in _typeSources)
                {
                    foundClass = classLookup.GetType(typeInfo);
                    if (foundClass != null)
                    {
                        collectionFormatter = classLookup.CollectionFormatter;
                        nullableFormatter = classLookup.NullableFormatter;
                        type = new ResolvedTypeInfo(foundClass);
                        break;
                    }
                }
            }

            type = type ?? ResolveType(typeInfo);
            if (typeInfo.GenericTypeParameters.Any())
            {
                type.Name = FormatGenerics(type, typeInfo.GenericTypeParameters.Select(x => Get(x, typeSource)));
            }

            if (typeInfo.IsCollection)
            {
                collectionFormatter = collectionFormatter ?? _defaultCollectionFormatter;
                type.Name = collectionFormatter.AsCollection(type);
            }

            if (typeInfo.IsNullable)
            {
                nullableFormatter = nullableFormatter ?? _defaultNullableFormatter;
                type.Name = nullableFormatter.AsNullable(type);
            }

            return type;
        }

        protected abstract string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes);

        protected abstract ResolvedTypeInfo ResolveType(ITypeReference typeInfo);
    }
}