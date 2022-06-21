using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Base abstract implementation of <see cref="ITypeResolverContext"/>.
    /// </summary>
    public abstract class TypeResolverContextBase<TCollectionFormatter, TResolvedTypeInfo> : ITypeResolverContext
        where TCollectionFormatter : class, ICollectionFormatter
        where TResolvedTypeInfo : class, IResolvedTypeInfo
    {
        private readonly List<ITypeSource> _typeSources = new();

        /// <summary>
        /// Creates a new instance of <see cref="TypeResolverContextBase"/>.
        /// </summary>
        protected TypeResolverContextBase(TCollectionFormatter defaultCollectionFormatter, INullableFormatter nullableFormatter)
        {
            DefaultCollectionFormatter = defaultCollectionFormatter;
            DefaultNullableFormatter = nullableFormatter;
        }

        /// <inheritdoc cref="ITypeResolverContext.DefaultCollectionFormatter"/>
        public TCollectionFormatter DefaultCollectionFormatter { get; private set; }

        ICollectionFormatter ITypeResolverContext.DefaultCollectionFormatter => DefaultCollectionFormatter;

        /// <inheritdoc />
        public INullableFormatter DefaultNullableFormatter { get; private set; }

        /// <inheritdoc />
        public void AddTypeSource(ITypeSource typeSource)
        {
            _typeSources.Add(typeSource);
        }

        /// <inheritdoc cref="ITypeResolverContext.SetCollectionFormatter(ICollectionFormatter)"/>
        protected void SetCollectionFormatter(TCollectionFormatter formatter)
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

        /// <inheritdoc cref="ITypeResolverContext.Get(IClassProvider)"/>
        protected abstract TResolvedTypeInfo Get(IClassProvider classProvider);


        /// <inheritdoc cref="ITypeResolverContext.Get(ITypeReference)"/>
        protected virtual TResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, default(TCollectionFormatter));
        }

        /// <inheritdoc cref="ITypeResolverContext.Get(ITypeReference,string)"/>
        protected abstract TResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat);

        /// <inheritdoc cref="ITypeResolverContext.Get(ITypeReference,ICollectionFormatter)"/>
        protected virtual TResolvedTypeInfo Get(ITypeReference typeReference, TCollectionFormatter collectionFormatter)
        {
            if (typeReference == null)
            {
                return null;
            }

            TResolvedTypeInfo resolvedTypeInfo = null;
            foreach (var classLookup in TypeSources)
            {
                var foundClass = classLookup.GetType(typeReference);
                if (foundClass == null)
                {
                    continue;
                }

                collectionFormatter ??= (TCollectionFormatter)classLookup.CollectionFormatter;
                resolvedTypeInfo = Get(foundClass);
                break;
            }

            collectionFormatter ??= DefaultCollectionFormatter;

            resolvedTypeInfo ??= ResolveType(
                typeReference: typeReference,
                nullableFormatter: DefaultNullableFormatter,
                collectionFormatter: collectionFormatter);

            if (typeReference.IsCollection)
            {
                resolvedTypeInfo = (TResolvedTypeInfo)collectionFormatter.ApplyTo(resolvedTypeInfo);
            }

            return resolvedTypeInfo;
        }

        /// <inheritdoc cref="ITypeResolverContext.Get(ITypeReference,ITypeSource)"/>
        protected virtual TResolvedTypeInfo Get(ITypeReference typeReference, ITypeSource typeSource)
        {
            if (typeReference == null)
            {
                return null;
            }

            TCollectionFormatter collectionFormatter = null;
            TResolvedTypeInfo resolvedTypeInfo = null;
            var foundClass = (TResolvedTypeInfo)typeSource.GetType(typeReference);
            if (foundClass != null)
            {
                collectionFormatter = (TCollectionFormatter)typeSource.CollectionFormatter;
                resolvedTypeInfo = Get(foundClass);
            }
            else
            {
                foreach (var classLookup in TypeSources)
                {
                    foundClass = (TResolvedTypeInfo)classLookup.GetType(typeReference);
                    if (foundClass == null)
                    {
                        continue;
                    }

                    collectionFormatter = (TCollectionFormatter)classLookup.CollectionFormatter;
                    resolvedTypeInfo = Get(foundClass);
                    break;
                }
            }

            collectionFormatter ??= DefaultCollectionFormatter;

            resolvedTypeInfo ??= ResolveType(
                typeReference: typeReference,
                nullableFormatter: DefaultNullableFormatter,
                collectionFormatter: collectionFormatter);

            if (typeReference.IsCollection)
            {
                resolvedTypeInfo = (TResolvedTypeInfo)collectionFormatter.ApplyTo(resolvedTypeInfo);
            }

            return resolvedTypeInfo;
        }

        /// <summary>
        /// Resolve a <typeparamref name="TResolvedTypeInfo"/> from the provided <paramref name="resolvedTypeInfo"/>.
        /// </summary>
        /// <remarks>
        /// Override this method to return a different specialized implementation of <typeparamref name="TResolvedTypeInfo"/>.
        /// </remarks>
        protected abstract TResolvedTypeInfo Get(IResolvedTypeInfo resolvedTypeInfo);

        /// <summary>
        /// Resolve a <typeparamref name="TResolvedTypeInfo"/> for the provided <paramref name="typeReference"/>.
        /// </summary>
        protected abstract TResolvedTypeInfo ResolveType(
            ITypeReference typeReference,
            INullableFormatter nullableFormatter,
            TCollectionFormatter collectionFormatter);

        #region ITypeResolverContext explicit implementations

        IResolvedTypeInfo ITypeResolverContext.Get(ITypeReference typeInfo) => Get(typeInfo);

        IResolvedTypeInfo ITypeResolverContext.Get(IClassProvider classProvider) => Get(classProvider);

        IResolvedTypeInfo ITypeResolverContext.Get(ITypeReference typeInfo, ICollectionFormatter collectionFormatter) => Get(typeInfo, (TCollectionFormatter)collectionFormatter);

        IResolvedTypeInfo ITypeResolverContext.Get(ITypeReference typeInfo, string collectionFormat) => Get(typeInfo, collectionFormat);

        IResolvedTypeInfo ITypeResolverContext.Get(ITypeReference typeReference, ITypeSource typeSource) => Get(typeReference, typeSource);

        void ITypeResolverContext.SetCollectionFormatter(ICollectionFormatter formatter) => SetCollectionFormatter((TCollectionFormatter)formatter);

        #endregion
    }
}