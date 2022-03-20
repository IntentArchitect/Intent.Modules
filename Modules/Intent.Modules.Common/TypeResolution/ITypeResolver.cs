using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.TypeResolution
{
    public interface ICollectionFormatter
    {
        string AsCollection(IResolvedTypeInfo typeInfo);
    }

    public interface INullableFormatter
    {
        string AsNullable(IResolvedTypeInfo typeInfo);
    }

    public interface ITypeResolver
    {
        /// <summary>
        /// Default format to use when the typeInfo.IsCollection is true.
        /// </summary>
        string DefaultCollectionFormat { set; }

        void SetDefaultCollectionFormatter(ICollectionFormatter formatter);

        void SetDefaultNullableFormatter(INullableFormatter formatter);

        /// <summary>
        /// Adds a default <see cref="ITypeSource"/> that is used when resolving type names of classes.
        /// </summary>
        void AddTypeSource(ITypeSource typeSource);
        [Obsolete("Use AddTypeSource")]
        void AddClassTypeSource(ITypeSource typeSource);

        /// <summary>
        /// Adds an <see cref="ITypeSource"/> that is only used to resolve type names when <see cref="InContext(string)"/> is called for the specific <see cref="contextName"/>.
        /// </summary>
        void AddTypeSource(ITypeSource typeSource, string contextName);
        [Obsolete("Use AddTypeSource")]
        void AddClassTypeSource(ITypeSource typeSource, string contextName);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        IResolvedTypeInfo Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="collectionFormat">The collection format provided if the typeInfo.IsCollection is true</param>
        IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="element"/>
        /// </summary>
        IResolvedTypeInfo Get(ICanBeReferencedType element);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="element"/>
        /// </summary>
        /// <param name="collectionFormat">The collection format provided if the typeInfo.IsCollection is true</param>
        [FixFor_Version4("Makes no sense to have this overload - the collectionFormat will never be used.")]
        IResolvedTypeInfo Get(ICanBeReferencedType element, string collectionFormat);

        /// <summary>
        /// Returns a <see cref="ITypeResolverContext"/> that resolves the type using the <see cref="IClassTypeSource"/> added to the specified "<paramref name="contextName"/>"
        /// </summary>
        ITypeResolverContext InContext(string contextName);

        /// <summary>
        /// Returns a collection of template dependencies discovered while discovering type names from templates.
        /// </summary>
        IEnumerable<ITemplateDependency> GetTemplateDependencies();
    }
}
