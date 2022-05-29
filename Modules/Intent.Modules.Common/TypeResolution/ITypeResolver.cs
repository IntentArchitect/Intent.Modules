using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Used with <see cref="IResolvedTypeInfo"/> to format collections.
    /// </summary>
    public interface ICollectionFormatter
    {
        /// <summary>
        /// Returns the provided <paramref name="type"/> formatted as collection.
        /// </summary>
        string Format(string type);

        /// <summary>
        /// Returns a new <see cref="IResolvedTypeInfo"/> with the original
        /// <paramref name="typeInfo"/> embedded within it.
        /// </summary>
        IResolvedTypeInfo ApplyTo(IResolvedTypeInfo typeInfo);
    }

    /// <summary>
    /// Used with <see cref="IResolvedTypeInfo"/> to format nullable types.
    /// </summary>
    public interface INullableFormatter
    {
        /// <summary>
        /// Return the provided <paramref name="type"/> formatted as a nullable type while taking
        /// into account information in the provided <paramref name="typeInfo"/>.
        /// </summary>
        string AsNullable(IResolvedTypeInfo typeInfo, string type);
    }

    /// <summary>
    /// Abstraction for resolving types.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// The current default <see cref="ICollectionFormatter"/>.
        /// </summary>
        ICollectionFormatter DefaultCollectionFormatter { get; }

        /// <summary>
        /// The current default <see cref="INullableFormatter"/>.
        /// </summary>
        INullableFormatter DefaultNullableFormatter { get; }

        /// <summary>
        /// Sets the default <see cref="ICollectionFormatter"/> which is used for methods which
        /// receive a <see langword="null"/> <see cref="ICollectionFormatter"/> value.
        /// </summary>
        void SetDefaultCollectionFormatter(ICollectionFormatter formatter);

        /// <summary>
        /// Sets the default <see cref="INullableFormatter"/> which is used for methods which
        /// receive a <see langword="null"/> <see cref="INullableFormatter"/> value.
        /// </summary>
        void SetDefaultNullableFormatter(INullableFormatter formatter);

        /// <summary>
        /// Adds a default <see cref="ITypeSource"/> that is used when resolving type names of classes.
        /// </summary>
        void AddTypeSource(ITypeSource typeSource);

        /// <summary>
        /// Obsolete. Use <see cref="AddTypeSource(ITypeSource)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        void AddClassTypeSource(ITypeSource typeSource);

        /// <summary>
        /// Adds an <see cref="ITypeSource"/> that is only used to resolve type names when <see cref="InContext(string)"/> is called for the specific <see cref="contextName"/>.
        /// </summary>
        void AddTypeSource(ITypeSource typeSource, string contextName);

        /// <summary>
        /// Obsolete. Use <see cref="AddTypeSource(ITypeSource,string)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        void AddClassTypeSource(ITypeSource typeSource, string contextName);

        /// <summary>
        /// Resolves the type information for the specified <paramref name="classProvider"/>
        /// </summary>
        IResolvedTypeInfo Get(IClassProvider classProvider);

        /// <summary>
        /// Resolves the type information for the specified <paramref name="typeInfo"/>
        /// </summary>
        IResolvedTypeInfo Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type information for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo">The type for which to resolve.</param>
        /// <param name="collectionFormat">The collection format provided if the typeInfo.IsCollection is true</param>
        IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat);

        /// <summary>
        /// Resolves the type information for the specified <paramref name="element"/>
        /// </summary>
        IResolvedTypeInfo Get(ICanBeReferencedType element);

        /// <summary>
        /// Resolves the type information for the specified <paramref name="element"/>
        /// </summary>
        /// <param name="element">The type for which to resolve.</param>
        /// <param name="collectionFormat">The collection format provided if the typeInfo.IsCollection is true</param>
        IResolvedTypeInfo Get(ICanBeReferencedType element, string collectionFormat);

        /// <summary>
        /// Returns a <see cref="ITypeResolverContext"/> that resolves the type using the
        /// <see cref="ITypeSource"/> added to the specified "<paramref name="contextName"/>"
        /// </summary>
        ITypeResolverContext InContext(string contextName);

        /// <summary>
        /// Returns a collection of template dependencies discovered while discovering type names from templates.
        /// </summary>
        IEnumerable<ITemplateDependency> GetTemplateDependencies();
    }
}
