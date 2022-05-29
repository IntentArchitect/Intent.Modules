using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
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
    public ResolvedTypeInfo(
        string name,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        ITemplate template,
        INullableFormatter nullableFormatter,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters = null)
    {
        Name = name;
        IsPrimitive = isPrimitive && !typeReference.IsCollection;
        IsNullable = isNullable;
        IsCollection = isCollection;
        TypeReference = typeReference;
        Template = template;
        NullableFormatter = nullableFormatter;
        GenericTypeParameters = genericTypeParameters ?? Array.Empty<IResolvedTypeInfo>();
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    public ResolvedTypeInfo(
        string name,
        bool isPrimitive,
        ITypeReference typeReference,
        ITemplate template,
        INullableFormatter nullableFormatter)
        : this(
            name,
            isPrimitive,
            typeReference.IsNullable,
            typeReference.IsCollection,
            typeReference,
            template,
            nullableFormatter)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    public ResolvedTypeInfo(IResolvedTypeInfo typeInfo)
        : this(
            name: typeInfo.Name,
            isPrimitive: typeInfo.IsPrimitive,
            isNullable: typeInfo.IsNullable,
            isCollection: typeInfo.IsCollection,
            typeReference: typeInfo.TypeReference,
            template: typeInfo.Template,
            nullableFormatter: typeInfo.NullableFormatter,
            genericTypeParameters: typeInfo.GenericTypeParameters)
    {
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

        if (GenericTypeParameters.Count > 0)
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