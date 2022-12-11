using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution;

/// <summary>
/// Implementation of <see cref="IResolvedTypeInfo"/>.
/// </summary>
public class ResolvedTypeInfo : IResolvedTypeInfo
{
    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    protected ResolvedTypeInfo(
        string name,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        ITemplate template,
        INullableFormatter nullableFormatter,
        ICollectionFormatter collectionFormatter,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters)
    {
        Name = name;
        IsPrimitive = isPrimitive && !isCollection;
        IsNullable = isNullable;
        IsCollection = isCollection;
        TypeReference = typeReference;
        Template = template;
        NullableFormatter = nullableFormatter;
        CollectionFormatter = collectionFormatter;
        GenericTypeParameters = genericTypeParameters ?? Array.Empty<IResolvedTypeInfo>();
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    public ResolvedTypeInfo(IResolvedTypeInfo typeInfo)
        : this(
            name: typeInfo.Template is IClassProvider classProvider
                ? classProvider.FullTypeName()
                : typeInfo.Name,
            isPrimitive: typeInfo.IsPrimitive,
            isNullable: typeInfo.IsNullable,
            isCollection: typeInfo.IsCollection,
            typeReference: typeInfo.TypeReference,
            template: typeInfo.Template,
            nullableFormatter: typeInfo.NullableFormatter,
            collectionFormatter: typeInfo.CollectionFormatter,
            genericTypeParameters: typeInfo.GenericTypeParameters)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    public ResolvedTypeInfo(
        IResolvedTypeInfo typeInfo,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters)
        : this(
            name: typeInfo.Template is IClassProvider classProvider
                ? classProvider.FullTypeName()
                : typeInfo.Name,
            isPrimitive: typeInfo.IsPrimitive,
            isNullable: typeInfo.IsNullable,
            isCollection: typeInfo.IsCollection,
            typeReference: typeInfo.TypeReference,
            template: typeInfo.Template,
            nullableFormatter: typeInfo.NullableFormatter,
            collectionFormatter: typeInfo.CollectionFormatter,
            genericTypeParameters: genericTypeParameters)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>. Automatically applies
    /// <see cref="IsCollection"/> and <see cref="IsNullable"/> from the provided
    /// <paramref name="typeReference"/>.
    /// </summary>
    public static ResolvedTypeInfo Create(
        string name,
        bool isPrimitive,
        ITypeReference typeReference,
        ITemplate template = null,
        INullableFormatter nullableFormatter = null,
        ICollectionFormatter collectionFormatter = null,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters = null)
    {
        return new ResolvedTypeInfo(
            name: name,
            isPrimitive: isPrimitive,
            isNullable: typeReference.IsNullable,
            isCollection: typeReference.IsCollection,
            typeReference: typeReference,
            template: template,
            nullableFormatter: nullableFormatter,
            collectionFormatter: collectionFormatter,
            genericTypeParameters: genericTypeParameters);
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    public static ResolvedTypeInfo Create(
        string name,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference = null,
        ITemplate template = null,
        INullableFormatter nullableFormatter = null,
        ICollectionFormatter collectionFormatter = null,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters = null)
    {
        return new ResolvedTypeInfo(
            name: name,
            isPrimitive: isPrimitive,
            isNullable: isNullable,
            isCollection: isCollection,
            typeReference: typeReference,
            template: template,
            nullableFormatter: nullableFormatter,
            collectionFormatter: collectionFormatter,
            genericTypeParameters: genericTypeParameters);
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/> for use with collections.
    /// </summary>
    public static ResolvedTypeInfo CreateForCollection(
        IResolvedTypeInfo forResolvedType,
        bool isNullable,
        INullableFormatter nullableFormatter = null,
        ICollectionFormatter collectionFormatter = null)
    {
        return new ResolvedTypeInfo(
            name: string.Empty,
            isPrimitive: false,
            isNullable: isNullable,
            isCollection: true,
            typeReference: null,
            template: null,
            nullableFormatter: nullableFormatter,
            collectionFormatter: collectionFormatter,
            genericTypeParameters: new[]
            {
                forResolvedType
            });
    }

    IResolvedTypeInfo IResolvedTypeInfo.WithIsNullable(bool isNullable)
    {
        return WithIsNullableProtected(isNullable);
    }

    /// <inheritdoc cref="IResolvedTypeInfo.WithIsNullable" />
    public ResolvedTypeInfo WithIsNullable(bool isNullable)
    {
        return (ResolvedTypeInfo)WithIsNullableProtected(isNullable);
    }

    /// <summary>
    /// Called by <see cref="IResolvedTypeInfo.WithIsNullable" />.
    /// </summary>
    /// <remarks>
    /// By having this method be the virtual overridable one, the "WithIsNullable" is left open to
    /// be safely hide-able in specializations of this class.
    /// </remarks>
    protected virtual IResolvedTypeInfo WithIsNullableProtected(bool isNullable)
    {
        return new ResolvedTypeInfo(
            name: Name,
            isPrimitive: IsPrimitive,
            isNullable: isNullable,
            isCollection: IsCollection,
            typeReference: TypeReference,
            template: Template,
            nullableFormatter: NullableFormatter,
            collectionFormatter: CollectionFormatter,
            genericTypeParameters: GenericTypeParameters);
    }


    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsPrimitive { get; }

    /// <inheritdoc />
    public bool IsNullable { get; }

    /// <inheritdoc />
    public bool IsCollection { get; }

    /// <inheritdoc />
    public ITemplate Template { get; }

    /// <inheritdoc />
    public ITypeReference TypeReference { get; }

    /// <inheritdoc />
    public IReadOnlyList<IResolvedTypeInfo> GenericTypeParameters { get; }

    /// <inheritdoc />
    public INullableFormatter NullableFormatter { get; }

    /// <inheritdoc />
    public ICollectionFormatter CollectionFormatter { get; }

    /// <inheritdoc />
    public IEnumerable<ITemplate> GetTemplateDependencies()
    {
        if (Template != null)
        {
            yield return Template;
        }

        foreach (var templateDependency in GenericTypeParameters.SelectMany(x => x.GetTemplateDependencies()))
        {
            yield return templateDependency;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var typeName = Name;

        if (IsCollection && CollectionFormatter != null)
        {
            typeName = CollectionFormatter.Format(GenericTypeParameters.Single().ToString());
        }
        else if (GenericTypeParameters.Count > 0)
        {
            typeName = $"{typeName}<{string.Join(", ", GenericTypeParameters.Select(x => x.ToString()))}>";
        }

        if (NullableFormatter != null)
        {
            typeName = NullableFormatter.AsNullable(this, typeName);
        }

        return typeName;
    }
}