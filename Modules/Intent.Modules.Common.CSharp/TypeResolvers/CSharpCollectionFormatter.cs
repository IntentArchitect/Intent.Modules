using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.TypeResolution;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.TypeResolvers;

/// <summary>
/// C# <see cref="ICollectionFormatter"/> which recognizes collection types with generic parameters
/// as well as the <c>[]</c> syntax.
/// </summary>
public class CSharpCollectionFormatter : ICollectionFormatter
{
    private readonly CSharpResolvedTypeInfo _typeInfo;
    private static readonly ConcurrentDictionary<string, CSharpCollectionFormatter> Cache = new();

    private CSharpCollectionFormatter(CSharpResolvedTypeInfo typeInfo)
    {
        _typeInfo = typeInfo;
    }

    private CSharpCollectionFormatter(string collectionFormat)
    {
        static CSharpResolvedTypeInfo Parse(string type)
        {
            static bool TryGetBracketedContentFromEnd(char openingBracket, char closingBracket, string type, out string innerContent)
            {
                innerContent = null;
                if (type[^1] != closingBracket)
                {
                    return false;
                }

                var depth = 0;
                for (var index = type.Length - 2; index >= 0; index--)
                {
                    if (type[index] == openingBracket && depth == 0)
                    {
                        innerContent = type.Substring(index + 1, type.Length - index - 2);
                        return true;
                    }

                    if (type[index] == openingBracket)
                    {
                        depth--;
                        continue;
                    }

                    if (type[index] == closingBracket)
                    {
                        depth++;
                    }
                }

                throw new Exception($"Mismatch of {openingBracket}{closingBracket} brackets in \"{type}\"");
            }

            type = type.Trim();
            if (string.IsNullOrWhiteSpace(type) || type == "{0}")
            {
                return null;
            }

            var isNullable = false;
            if (type[^1] == '?')
            {
                type = type[..^1];
                isNullable = true;
            }

            var jaggedArrays = new List<CSharpJaggedArray>();
            while (TryGetBracketedContentFromEnd('[', ']', type, out var squareBracketsContent))
            {
                var arrayDimensionCount = squareBracketsContent.Count(@char => @char == ',');

                jaggedArrays.Add(new CSharpJaggedArray(arrayDimensionCount));

                type = type[..^(squareBracketsContent.Length + 2)];
            }

            if (jaggedArrays.Count > 0)
            {
                return CSharpResolvedTypeInfo.CreateForArray(
                    forResolvedType: Parse(type),
                    isNullable: isNullable,
                    jaggedArrays: jaggedArrays);
            }

            var genericTypeParameters = Array.Empty<CSharpResolvedTypeInfo>();
            if (TryGetBracketedContentFromEnd('<', '>', type, out var angledBracketsContent))
            {
                genericTypeParameters = angledBracketsContent
                    .Split(',')
                    .Select(Parse)
                    .ToArray();

                type = type[..^(angledBracketsContent.Length + 2)];
            }

            var @namespace = string.Empty;
            var lastIndexOfPeriod = type.LastIndexOf(".", StringComparison.Ordinal);
            if (lastIndexOfPeriod >= 0)
            {
                @namespace = type[..lastIndexOfPeriod];
                type = type[(lastIndexOfPeriod + 1)..];
            }

            return CSharpResolvedTypeInfo.Create(
                name: type,
                @namespace: @namespace,
                isPrimitive: false,
                isNullable: isNullable,
                isCollection: true,
                typeReference: null,
                template: null,
                nullableFormatter: null,
                genericTypeParameters: genericTypeParameters);
        }

        _typeInfo = Parse(collectionFormat);
    }

    /// <summary>
    /// Returns a <see cref="CSharpResolvedTypeInfo"/> which is the type of collection for this
    /// instance of the <see cref="CSharpCollectionFormatter"/> of the provided
    /// <paramref name="typeInfo"/>.
    /// </summary>
    public CSharpResolvedTypeInfo ApplyTo(CSharpResolvedTypeInfo typeInfo)
    {
        if (_typeInfo == null)
        {
            return typeInfo;
        }

        var isNullable = typeInfo.IsNullable;
        typeInfo = typeInfo.WithIsNullable(false);

        if (_typeInfo.JaggedArrays != null)
        {
            return CSharpResolvedTypeInfo.CreateForArray(
                forResolvedType: typeInfo,
                isNullable: isNullable,
                jaggedArrays: _typeInfo.JaggedArrays,
                typeInfo.NullableFormatter);
        }

        return CSharpResolvedTypeInfo.Create(
            name: _typeInfo.Name,
            @namespace: _typeInfo.Namespace,
            isPrimitive: _typeInfo.IsPrimitive,
            isNullable: isNullable,
            isCollection: true,
            typeReference: _typeInfo.TypeReference,
            template: _typeInfo.Template,
            nullableFormatter: typeInfo.NullableFormatter,
            genericTypeParameters: _typeInfo.GenericTypeParameters.Count == 0
                ? Array.Empty<CSharpResolvedTypeInfo>()
                : _typeInfo.GenericTypeParameters
                    .Select(genericTypeParameter => genericTypeParameter ?? typeInfo)
                    .ToArray());
    }

    /// <summary>
    /// Returns an instance of <see cref="CSharpCollectionFormatter"/> constructed with the
    /// specified parameters.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="CSharpCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// <para>
    /// If any of the values of <see cref="CSharpResolvedTypeInfo.GenericTypeParameters"/> is null,
    /// they will be substituted by the provided <see cref="CSharpResolvedTypeInfo"/> in the
    /// <see cref="ApplyTo"/> method.
    /// </para>
    /// </remarks>
    /// <param name="typeInfo">The collection type, for example:
    /// <c>System.Collection.Generic.Dictionary&lt;TKey, TValue&gt;</c>.</param>
    public static CSharpCollectionFormatter Create(CSharpResolvedTypeInfo typeInfo)
    {
        return Cache.GetOrAdd(typeInfo.ToString(), _ => new CSharpCollectionFormatter(typeInfo));
    }

    /// <summary>
    /// Returns an instance of <see cref="CSharpCollectionFormatter"/> based on the provided
    /// <paramref name="collectionFormat"/>.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="CSharpCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    /// <param name="collectionFormat">The collection type, for example:
    /// <c>System.Collection.Generic.List&lt;T&gt;</c>.</param>
    public static CSharpCollectionFormatter Create(string collectionFormat)
    {
        return Cache.GetOrAdd(
            collectionFormat,
            _ => new CSharpCollectionFormatter(collectionFormat));
    }

    /// <summary>
    /// Returns an instance of <see cref="CSharpCollectionFormatter"/> for <see cref="System.Collections.Generic.ICollection{T}"/>
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="CSharpCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    public static CSharpCollectionFormatter CreateICollection()
    {
        return Create("System.Collections.Generic.ICollection<{0}>");
    }

    /// <summary>
    /// Returns an instance of <see cref="CSharpCollectionFormatter"/> for <see cref="System.Collections.Generic.IList{T}"/>
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="CSharpCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    public static CSharpCollectionFormatter CreateIList()
    {
        return Create("System.Collections.Generic.IList<{0}>");
    }

    /// <summary>
    /// Returns an instance of <see cref="CSharpCollectionFormatter"/> for <see cref="System.Collections.Generic.List{T}"/>
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="CSharpCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    public static CSharpCollectionFormatter CreateList()
    {
        return Create("System.Collections.Generic.List<{0}>");
    }

    /// <inheritdoc />
    public string Format(string type)
    {
        return type;
    }

    /// <summary>
    /// Obsolete. Please use <see cref="Create(CSharpResolvedTypeInfo)"/>
    /// </summary>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static CSharpCollectionFormatter GetOrCreate(CSharpResolvedTypeInfo typeInfo)
    {
        return Create(typeInfo);
    }

    /// <summary>
    /// Obsolete. Use <see cref="Create(string)"/> instead.
    /// </summary>
    /// <param name="collectionFormat">The collection type, for example:
    /// <c>System.Collection.Generic.List&lt;T&gt;</c>.</param>
    [Obsolete(WillBeRemovedIn.Version4)]
    public static CSharpCollectionFormatter GetOrCreate(string collectionFormat)
    {
        return Create(collectionFormat);
    }

    IResolvedTypeInfo ICollectionFormatter.ApplyTo(IResolvedTypeInfo typeInfo)
    {
        return ApplyTo((CSharpResolvedTypeInfo)typeInfo);
    }
}