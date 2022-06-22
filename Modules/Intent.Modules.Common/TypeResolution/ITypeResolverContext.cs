using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.TypeResolution
{
    public interface ITypeResolverContext
    {
        /// <summary>
        /// Resolves the type information for the specified <paramref name="classProvider"/>.
        /// </summary>
        IResolvedTypeInfo Get(IClassProvider classProvider);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        IResolvedTypeInfo Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="collectionFormat">The collection format to be applied if the typeInfo.IsCollection is true</param>
        IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="collectionFormatter">The collection formatter to be applied if the typeInfo.IsCollection is true</param>
        IResolvedTypeInfo Get(ITypeReference typeInfo, ICollectionFormatter collectionFormatter);

        /// <summary>
        /// Obsolete. This method will be removed, please notify Intent Architect should have a need for this.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        IResolvedTypeInfo Get(ITypeReference typeInfo, ITypeSource typeSource);

        /// <summary>
        /// Adds a <see cref="ITypeSource"/> that is used when resolving information about types provided by other templates.
        /// </summary>
        void AddTypeSource(ITypeSource typeSource);

        /// <summary>
        /// Default collection formatter to use when the typeInfo.IsCollection is true;
        /// </summary>
        void SetCollectionFormatter(ICollectionFormatter formatter);

        /// <summary>
        /// Default collection formatter to use when the typeInfo.IsCollection is true;
        /// </summary>
        void SetNullableFormatter(INullableFormatter formatter);

        /// <summary>
        /// Returns the list of added <see cref="ITypeSource"/>s
        /// </summary>
        IEnumerable<ITypeSource> TypeSources { get; }

        /// <summary>
        /// The default <see cref="ICollectionFormatter"/>.
        /// </summary>
        ICollectionFormatter DefaultCollectionFormatter { get; }

        /// <summary>
        /// The default <see cref="INullableFormatter"/>.
        /// </summary>
        INullableFormatter DefaultNullableFormatter { get; }
    }
}