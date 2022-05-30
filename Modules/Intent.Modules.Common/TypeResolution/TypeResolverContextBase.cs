using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Base abstract implementation of <see cref="ITypeResolverContext"/>.
    /// </summary>
    public abstract class TypeResolverContextBase : ITypeResolverContext
    {
        private readonly List<ITypeSource> _typeSources = new();

        /// <summary>
        /// Creates a new instance of <see cref="TypeResolverContextBase"/>.
        /// </summary>
        protected TypeResolverContextBase(ICollectionFormatter defaultCollectionFormatter, INullableFormatter nullableFormatter)
        {
            DefaultCollectionFormatter = defaultCollectionFormatter;
            DefaultNullableFormatter = nullableFormatter;
        }

        /// <inheritdoc />
        public ICollectionFormatter DefaultCollectionFormatter { get; private set; }

        /// <inheritdoc />
        public INullableFormatter DefaultNullableFormatter { get; private set; }

        /// <inheritdoc />
        public void AddTypeSource(ITypeSource typeSource)
        {
            _typeSources.Add(typeSource);
        }

        /// <inheritdoc />
        public void SetCollectionFormatter(ICollectionFormatter formatter)
        {
            DefaultCollectionFormatter = formatter;
        }

        /// <inheritdoc />
        public void SetNullableFormatter(INullableFormatter formatter)
        {
            DefaultNullableFormatter = formatter;
        }

        /// <inheritdoc />
        public IEnumerable<ITypeSource> TypeSources => _typeSources;

        /// <inheritdoc />
        public virtual IResolvedTypeInfo Get(IClassProvider classProvider)
        {
            return ResolvedTypeInfo.Create(
                name: classProvider.FullTypeName(),
                isPrimitive: false,
                isNullable: false,
                isCollection: false,
                typeReference: null,
                template: classProvider,
                nullableFormatter: null,
                collectionFormatter: null);
        }

        /// <inheritdoc />
        public IResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, default(ICollectionFormatter));
        }

        /// <inheritdoc />
        public virtual IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                ? CollectionFormatter.GetOrCreate(collectionFormat)
                : null;

            return Get(typeInfo, collectionFormatter);
        }

        /// <inheritdoc />
        public virtual IResolvedTypeInfo Get(ITypeReference typeReference, ICollectionFormatter collectionFormatter)
        {
            if (typeReference == null)
            {
                return null;
            }

            IResolvedTypeInfo resolvedTypeInfo = null;
            foreach (var classLookup in TypeSources)
            {
                var foundClass = classLookup.GetType(typeReference);
                if (foundClass == null)
                {
                    continue;
                }

                collectionFormatter ??= classLookup.CollectionFormatter;
                resolvedTypeInfo = Get(foundClass);
                break;
            }

            collectionFormatter ??= DefaultCollectionFormatter;

            resolvedTypeInfo ??= ResolveType(
                typeReference: typeReference,
                nullableFormatter: DefaultNullableFormatter);

            if (typeReference.IsCollection)
            {
                resolvedTypeInfo = collectionFormatter.ApplyTo(resolvedTypeInfo);
            }

            return resolvedTypeInfo;
        }

        /// <inheritdoc />
        public virtual IResolvedTypeInfo Get(ITypeReference typeReference, ITypeSource typeSource)
        {
            if (typeReference == null)
            {
                return null;
            }

            ICollectionFormatter collectionFormatter = null;
            IResolvedTypeInfo resolvedTypeInfo = null;
            var foundClass = typeSource.GetType(typeReference);
            if (foundClass != null)
            {
                collectionFormatter = typeSource.CollectionFormatter;
                resolvedTypeInfo = Get(foundClass);
            }
            else
            {
                foreach (var classLookup in TypeSources)
                {
                    foundClass = classLookup.GetType(typeReference);
                    if (foundClass == null)
                    {
                        continue;
                    }

                    collectionFormatter = classLookup.CollectionFormatter;
                    resolvedTypeInfo = Get(foundClass);
                    break;
                }
            }

            collectionFormatter ??= DefaultCollectionFormatter;

            resolvedTypeInfo ??= ResolveType(
                typeReference: typeReference,
                nullableFormatter: DefaultNullableFormatter);

            if (typeReference.IsCollection)
            {
                resolvedTypeInfo = collectionFormatter.ApplyTo(resolvedTypeInfo);
            }

            return resolvedTypeInfo;
        }

        /// <summary>
        /// Resolve a <see cref="IResolvedTypeInfo"/> from the provided <paramref name="resolvedTypeInfo"/>.
        /// </summary>
        /// <remarks>
        /// Override this method to return a different specialized implementation of <see cref="IResolvedTypeInfo"/>.
        /// </remarks>
        protected virtual IResolvedTypeInfo Get(IResolvedTypeInfo resolvedTypeInfo)
        {
            return new ResolvedTypeInfo(resolvedTypeInfo);
        }

        /// <summary>
        /// Resolve a <see cref="ResolvedTypeInfo"/> for the provided <paramref name="typeReference"/>.
        /// </summary>
        protected abstract IResolvedTypeInfo ResolveType(
            ITypeReference typeReference,
            INullableFormatter nullableFormatter);
    }
}