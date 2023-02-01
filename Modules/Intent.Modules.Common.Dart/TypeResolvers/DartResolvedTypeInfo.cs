using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Dart.TypeResolvers;

/// <summary>
/// Dart specialization of <see cref="ResolvedTypeInfo"/>.
/// </summary>
public class DartResolvedTypeInfo : ResolvedTypeInfo
{
    private DartResolvedTypeInfo(
        string name,
        string importSource,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        INullableFormatter nullableFormatter,
        ITemplate template,
        IReadOnlyList<IResolvedTypeInfo> genericTypeParameters)
        : base(
            name: name,
            isPrimitive: isPrimitive,
            isNullable: isNullable,
            isCollection: isCollection,
            typeReference: typeReference,
            template: template,
            nullableFormatter: nullableFormatter,
            collectionFormatter: null,
            genericTypeParameters: genericTypeParameters)
    {
        ImportSource = importSource;
    }

    /// <summary>
    /// Creates a new instance of <see cref="DartResolvedTypeInfo"/>.
    /// </summary>
    public static DartResolvedTypeInfo Create(
        string name,
        string importSource,
        bool isPrimitive,
        bool isNullable,
        bool isCollection,
        ITypeReference typeReference,
        INullableFormatter nullableFormatter = null,
        ITemplate template = null,
        IReadOnlyList<DartResolvedTypeInfo> genericTypeParameters = null)
    {
        return new DartResolvedTypeInfo(
            name: name,
            importSource: importSource,
            isPrimitive: isPrimitive,
            isNullable: isNullable,
            isCollection: isCollection,
            typeReference: typeReference,
            nullableFormatter: nullableFormatter,
            template: template,
            genericTypeParameters: genericTypeParameters ?? Array.Empty<DartResolvedTypeInfo>());
    }

    /// <summary>
    /// Creates a new instance of <see cref="DartResolvedTypeInfo"/> from the provided <paramref name="resolvedTypeInfo"/>.
    /// </summary>
    public static DartResolvedTypeInfo Create(
        IResolvedTypeInfo resolvedTypeInfo,
        IReadOnlyList<DartResolvedTypeInfo> genericTypeParameters)
    {
        if (resolvedTypeInfo is DartResolvedTypeInfo dartResolvedTypeInfo)
        {
            return dartResolvedTypeInfo;
        }

        var (name, importSource) = resolvedTypeInfo.Template is IClassProvider classProvider
            ? (classProvider.ClassName, classProvider.Namespace)
            : (resolvedTypeInfo.Name, string.Empty);

        return new DartResolvedTypeInfo(
            name: name,
            importSource: importSource,
            isPrimitive: resolvedTypeInfo.IsPrimitive,
            isNullable: resolvedTypeInfo.IsNullable,
            isCollection: resolvedTypeInfo.IsCollection,
            typeReference: resolvedTypeInfo.TypeReference,
            template: resolvedTypeInfo.Template,
            nullableFormatter: resolvedTypeInfo.NullableFormatter,
            genericTypeParameters: genericTypeParameters);
    }

    /// <summary>
    /// Creates a new instance of <see cref="DartResolvedTypeInfo"/> for an collection resolved type.
    /// </summary>
    public static DartResolvedTypeInfo CreateForCollection(
        DartResolvedTypeInfo forResolvedType,
        bool isNullable,
        INullableFormatter nullableFormatter = null)
    {
        return new DartResolvedTypeInfo(
            name: string.Empty,
            importSource: string.Empty,
            isPrimitive: false,
            isNullable: isNullable,
            isCollection: true,
            typeReference: null,
            template: null,
            nullableFormatter: nullableFormatter,
            genericTypeParameters: new[] { forResolvedType });
    }

    /// <inheritdoc cref="IResolvedTypeInfo.WithIsNullable" />
    public new DartResolvedTypeInfo WithIsNullable(bool isNullable)
    {
        return (DartResolvedTypeInfo)WithIsNullableProtected(isNullable);
    }

    /// <inheritdoc />
    protected override IResolvedTypeInfo WithIsNullableProtected(bool isNullable)
    {
        return new DartResolvedTypeInfo(
            name: Name,
            importSource: ImportSource,
            isPrimitive: IsPrimitive,
            isNullable: isNullable,
            isCollection: IsCollection,
            typeReference: TypeReference,
            template: Template,
            nullableFormatter: NullableFormatter,
            genericTypeParameters: GenericTypeParameters);
    }

    /// <summary>
    /// The import source of the type.
    /// </summary>
    public string ImportSource { get; }

    /// <inheritdoc cref="IResolvedTypeInfo.GenericTypeParameters"/>
    public new IReadOnlyList<DartResolvedTypeInfo> GenericTypeParameters =>
        (IReadOnlyList<DartResolvedTypeInfo>)base.GenericTypeParameters;

    /// <summary>
    /// Returns the the fully qualified name for this this type as well as the recursively acquired
    /// import sources of this type's <see cref="GenericTypeParameters"/>.
    /// </summary>
    public IEnumerable<DartResolvedTypeInfo> GetAllResolvedTypes()
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
        var typeName = GenericTypeParameters.Count > 0
            ? $"{Name}<{string.Join(", ", GenericTypeParameters.Select(x => x.ToString()))}>"
            : Name;

        if (NullableFormatter != null)
        {
            typeName = NullableFormatter.AsNullable(this, typeName);
        }

        return typeName;
    }
}