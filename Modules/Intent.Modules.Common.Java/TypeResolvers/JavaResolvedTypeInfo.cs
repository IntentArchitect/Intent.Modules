using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Java.TypeResolvers;

/// <summary>
/// Java specialization of <see cref="ResolvedTypeInfo"/>.
/// </summary>
public class JavaResolvedTypeInfo : ResolvedTypeInfo
{
    private JavaResolvedTypeInfo(
        string name,
        string package,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        ITemplate template,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters,
        int arrayDimensionCount)
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
        ArrayDimensionCount = arrayDimensionCount;
    }

    /// <summary>
    /// Creates a new instance of <see cref="JavaResolvedTypeInfo"/>.
    /// </summary>
    public static JavaResolvedTypeInfo Create(
        string name,
        string package,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        ITemplate template = null,
        IReadOnlyList<JavaResolvedTypeInfo> genericTypeParameters = null)
    {
        return new JavaResolvedTypeInfo(
            name: name,
            package: package,
            isPrimitive: isPrimitive,
            isNullable: isNullable,
            isCollection: isCollection,
            typeReference: typeReference,
            template: template,
            genericTypeParameters: genericTypeParameters ?? Array.Empty<JavaResolvedTypeInfo>(),
            arrayDimensionCount: 0);
    }

    /// <summary>
    /// Creates a new instance of <see cref="JavaResolvedTypeInfo"/> from the provided <paramref name="resolvedTypeInfo"/>.
    /// </summary>
    public static JavaResolvedTypeInfo Create(IResolvedTypeInfo resolvedTypeInfo)
    {
        var (name, package) = resolvedTypeInfo.Template is IClassProvider classProvider
            ? (classProvider.ClassName, classProvider.Namespace)
            : (resolvedTypeInfo.Name, string.Empty);

        return new JavaResolvedTypeInfo(
            name: name,
            package: package,
            isPrimitive: resolvedTypeInfo.IsPrimitive,
            isNullable: resolvedTypeInfo.IsNullable,
            isCollection: resolvedTypeInfo.IsCollection,
            typeReference: resolvedTypeInfo.TypeReference,
            template: resolvedTypeInfo.Template,
            genericTypeParameters: resolvedTypeInfo.GenericTypeParameters.Select(Create).ToArray(),
            arrayDimensionCount: 0);
    }

    /// <summary>
    /// Creates a new instance of <see cref="JavaResolvedTypeInfo"/> for an array resolved type.
    /// </summary>
    public static JavaResolvedTypeInfo CreateForArray(
        JavaResolvedTypeInfo forResolvedType,
        bool isNullable,
        int arrayDimensionCount)
    {
        if (arrayDimensionCount < 1)
        {
            throw new ArgumentException("Must be at least 1", nameof(arrayDimensionCount));
        }

        return new JavaResolvedTypeInfo(
            name: string.Empty,
            package: string.Empty,
            isPrimitive: false,
            isNullable: isNullable,
            isCollection: true,
            typeReference: null,
            template: null,
            genericTypeParameters: new[] { forResolvedType },
            arrayDimensionCount: arrayDimensionCount);
    }

    /// <inheritdoc cref="IResolvedTypeInfo.WithIsNullable" />
    public new JavaResolvedTypeInfo WithIsNullable(bool isNullable)
    {
        return (JavaResolvedTypeInfo)WithIsNullableProtected(isNullable);
    }

    /// <inheritdoc />
    protected override IResolvedTypeInfo WithIsNullableProtected(bool isNullable)
    {
        return new JavaResolvedTypeInfo(
            name: Name,
            package: Package,
            isPrimitive: IsPrimitive,
            isNullable: isNullable,
            isCollection: IsCollection,
            typeReference: TypeReference,
            template: Template,
            genericTypeParameters: GenericTypeParameters,
            arrayDimensionCount: ArrayDimensionCount);
    }

    /// <summary>
    /// The package of the type.
    /// </summary>
    public string Package { get; }

    /// <summary>
    /// The number of dimensions of the array for the type, or <c>0</c> if the type is not an array.
    /// </summary>
    public int ArrayDimensionCount { get; }

    /// <inheritdoc cref="IResolvedTypeInfo.GenericTypeParameters"/>
    public new IReadOnlyList<JavaResolvedTypeInfo> GenericTypeParameters =>
        (IReadOnlyList<JavaResolvedTypeInfo>)base.GenericTypeParameters;

    /// <summary>
    /// Returns the the fully qualified name for this this type as well as the recursively acquired
    /// packages of this type's <see cref="GenericTypeParameters"/>.
    /// </summary>
    public IEnumerable<string> GetAllFullyQualifiedTypeNames()
    {
        yield return GetFullyQualifiedTypeName();

        foreach (var fullyQualifiedTypeName in GenericTypeParameters.SelectMany(x => x.GetAllFullyQualifiedTypeNames()))
        {
            yield return fullyQualifiedTypeName;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ArrayDimensionCount > 0
            ? $"{GenericTypeParameters[0]}{string.Concat(Enumerable.Repeat("[]", ArrayDimensionCount))}"
            : GenericTypeParameters.Count > 0
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