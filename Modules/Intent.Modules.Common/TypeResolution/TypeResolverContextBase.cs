using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Base abstract implementation of <see cref="ITypeResolverContext"/>.
    /// </summary>
    public abstract class TypeResolverContextBase : ITypeResolverContext
    {
        private readonly List<ITypeSource> _typeSources = new();
        private ICollectionFormatter _defaultCollectionFormatter;
        private INullableFormatter _defaultNullableFormatter;

        /// <summary>
        /// Creates a new instance of <see cref="TypeResolverContextBase"/>.
        /// </summary>
        protected TypeResolverContextBase(ICollectionFormatter defaultCollectionFormatter, INullableFormatter nullableFormatter)
        {
            _defaultCollectionFormatter = defaultCollectionFormatter;
            _defaultNullableFormatter = nullableFormatter;
        }

        /// <inheritdoc />
        public void AddTypeSource(ITypeSource typeSource)
        {
            _typeSources.Add(typeSource);
        }

        /// <inheritdoc />
        public void SetCollectionFormatter(ICollectionFormatter formatter)
        {
            _defaultCollectionFormatter = formatter;
        }

        /// <inheritdoc />
        public void SetNullableFormatter(INullableFormatter formatter)
        {
            _defaultNullableFormatter = formatter;
        }

        /// <inheritdoc />
        public IEnumerable<ITypeSource> TypeSources => _typeSources;

        /// <inheritdoc />
        public IResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, default(ICollectionFormatter));
        }

        /// <inheritdoc />
        public virtual IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            return Get(typeInfo, !string.IsNullOrWhiteSpace(collectionFormat) ? new CollectionFormatter(collectionFormat) : null);
        }

        /// <inheritdoc />
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

            type ??= ResolveType(typeInfo);
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
                nullableFormatter ??= _defaultNullableFormatter;
                type.Name = nullableFormatter.AsNullable(type);
            }

            return type;
        }

        /// <inheritdoc />
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

            type ??= ResolveType(typeInfo);
            if (typeInfo.GenericTypeParameters.Any())
            {
                type.Name = FormatGenerics(type, typeInfo.GenericTypeParameters.Select(x => Get(x, typeSource)));
            }

            if (typeInfo.IsCollection)
            {
                collectionFormatter ??= _defaultCollectionFormatter;
                type.Name = collectionFormatter.AsCollection(type);
            }

            if (typeInfo.IsNullable)
            {
                nullableFormatter ??= _defaultNullableFormatter;
                type.Name = nullableFormatter.AsNullable(type);
            }

            return type;
        }

        /// <summary>
        /// Return the type as a string with generic type parameters formatted correctly for the
        /// particular language being used.
        /// </summary>
        protected abstract string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes);

        /// <summary>
        /// Resolve a <see cref="ResolvedTypeInfo"/> for the provided <paramref name="typeInfo"/>.
        /// </summary>
        protected abstract ResolvedTypeInfo ResolveType(ITypeReference typeInfo);
    }
}