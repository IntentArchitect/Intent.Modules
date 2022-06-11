using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Kotlin.TypeResolvers;

/// <summary>
/// Kotlin specialization of <see cref="ResolvedTypeInfo"/>.
/// </summary>
public class KotlinResolvedTypeInfo : ResolvedTypeInfo
{
    private KotlinResolvedTypeInfo(
        string name,
        string package,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        ITemplate template,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters)
        : base(
            name: name,
            isPrimitive: isPrimitive,
            isNullable: isNullable,
            isCollection: isCollection,
            typeReference: typeReference,
            template: template,
            nullableFormatter: null,
            collectionFormatter: null,
            genericTypeParameters: genericTypeParameters)
    {
        Package = package;
    }

    /// <summary>
    /// Creates a new instance of <see cref="KotlinResolvedTypeInfo"/>.
    /// </summary>
    public static KotlinResolvedTypeInfo Create(
        string name,
        string package,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        ITemplate template = null,
        IReadOnlyList<KotlinResolvedTypeInfo> genericTypeParameters = null)
    {
        return new KotlinResolvedTypeInfo(
            name: name,
            package: package,
            isPrimitive: isPrimitive,
            isNullable: isNullable,
            isCollection: isCollection,
            typeReference: typeReference,
            template: template,
            genericTypeParameters: genericTypeParameters ?? Array.Empty<KotlinResolvedTypeInfo>());
    }

    /// <summary>
    /// Creates a new instance of <see cref="KotlinResolvedTypeInfo"/> from the provided <paramref name="resolvedTypeInfo"/>.
    /// </summary>
    public static KotlinResolvedTypeInfo Create(IResolvedTypeInfo resolvedTypeInfo)
    {
        var (name, package) = resolvedTypeInfo.Template is IClassProvider classProvider
            ? (classProvider.ClassName, classProvider.Namespace)
            : (resolvedTypeInfo.Name, string.Empty);

        return new KotlinResolvedTypeInfo(
            name: name,
            package: package,
            isPrimitive: resolvedTypeInfo.IsPrimitive,
            isNullable: resolvedTypeInfo.IsNullable,
            isCollection: resolvedTypeInfo.IsCollection,
            typeReference: resolvedTypeInfo.TypeReference,
            template: resolvedTypeInfo.Template,
            genericTypeParameters: resolvedTypeInfo.GenericTypeParameters.Select(Create).ToArray());
    }

    /// <summary>
    /// Creates a new instance of <see cref="KotlinResolvedTypeInfo"/> for an array resolved type.
    /// </summary>
    public static KotlinResolvedTypeInfo CreateForArray(
        KotlinResolvedTypeInfo forResolvedType,
        bool isNullable,
        int arrayDimensionCount)
    {
        if (arrayDimensionCount < 1)
        {
            throw new ArgumentException("Must be at least 1", nameof(arrayDimensionCount));
        }

        return new KotlinResolvedTypeInfo(
            name: string.Empty,
            package: string.Empty,
            isPrimitive: false,
            isNullable: isNullable,
            isCollection: true,
            typeReference: null,
            template: null,
            genericTypeParameters: new[] { forResolvedType });
    }

    /// <inheritdoc cref="IResolvedTypeInfo.WithIsNullable" />
    public new KotlinResolvedTypeInfo WithIsNullable(bool isNullable)
    {
        return (KotlinResolvedTypeInfo)WithIsNullableProtected(isNullable);
    }

    /// <inheritdoc />
    protected override IResolvedTypeInfo WithIsNullableProtected(bool isNullable)
    {
        return new KotlinResolvedTypeInfo(
            name: Name,
            package: Package,
            isPrimitive: IsPrimitive,
            isNullable: isNullable,
            isCollection: IsCollection,
            typeReference: TypeReference,
            template: Template,
            genericTypeParameters: GenericTypeParameters);
    }

    /// <summary>
    /// The package of the type.
    /// </summary>
    public string Package { get; }

    /// <inheritdoc cref="IResolvedTypeInfo.GenericTypeParameters"/>
    public new IReadOnlyList<KotlinResolvedTypeInfo> GenericTypeParameters =>
        (IReadOnlyList<KotlinResolvedTypeInfo>)base.GenericTypeParameters;

    /// <summary>
    /// Returns the the fully qualified name for this this type as well as the recursively acquired
    /// packages of this type's <see cref="GenericTypeParameters"/>.
    /// </summary>
    public IEnumerable<KotlinResolvedTypeInfo> GetAllResolvedTypes()
    {
        yield return this;

        foreach (var resolvedType in GenericTypeParameters.SelectMany(x => x.GetAllResolvedTypes()))
        {
            yield return resolvedType;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return GenericTypeParameters.Count > 0
            ? $"{Name}<{string.Join(", ", GenericTypeParameters.Select(x => x.ToString()))}>"
            : Name;
    }

    /// <summary>
    /// Gets the fully qualified type name.
    /// </summary>
    public string GetFullyQualifiedTypeName()
    {
        return string.IsNullOrWhiteSpace(Package)
            ? Name
            : $"{Package}.{Name}";
    }
}