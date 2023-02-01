using System;
using System.Collections.Concurrent;
using System.Linq;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Dart.TypeResolvers;

/// <summary>
/// Dart <see cref="ICollectionFormatter"/> which recognizes collection types with generic parameters.
/// </summary>
public class DartCollectionFormatter : ICollectionFormatter
{
    private readonly DartResolvedTypeInfo _typeInfo;
    private static readonly ConcurrentDictionary<string, DartCollectionFormatter> Cache = new();

    private DartCollectionFormatter(DartResolvedTypeInfo typeInfo)
    {
        _typeInfo = typeInfo;
    }

    private DartCollectionFormatter(string collectionFormat)
    {
        static DartResolvedTypeInfo Parse(string type)
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

            var genericTypeParameters = Array.Empty<DartResolvedTypeInfo>();
            if (TryGetBracketedContentFromEnd('<', '>', type, out var angledBracketsContent))
            {
                genericTypeParameters = angledBracketsContent
                    .Split(',')
                    .Select(Parse)
                    .ToArray();

                type = type[..^(angledBracketsContent.Length + 2)];
            }

            return DartResolvedTypeInfo.Create(
                name: type,
                importSource: null,
                isPrimitive: false,
                isNullable: false,
                isCollection: true,
                typeReference: null,
                template: null,
                genericTypeParameters: genericTypeParameters);
        }

        _typeInfo = Parse(collectionFormat);
    }

    /// <summary>
    /// Returns a <see cref="DartResolvedTypeInfo"/> which is the type of collection for this
    /// instance of the <see cref="DartCollectionFormatter"/> of the provided
    /// <paramref name="typeInfo"/>.
    /// </summary>
    public DartResolvedTypeInfo ApplyTo(DartResolvedTypeInfo typeInfo)
    {
        if (_typeInfo == null)
        {
            return typeInfo;
        }

        var isNullable = typeInfo.IsNullable;
        typeInfo = typeInfo.WithIsNullable(false);

        return DartResolvedTypeInfo.Create(
            name: _typeInfo.Name,
            importSource: _typeInfo.ImportSource,
            isPrimitive: _typeInfo.IsPrimitive,
            isNullable: isNullable,
            isCollection: true,
            typeReference: _typeInfo.TypeReference,
            template: _typeInfo.Template,
            genericTypeParameters: _typeInfo.GenericTypeParameters.Count == 0
                ? Array.Empty<DartResolvedTypeInfo>()
                : _typeInfo.GenericTypeParameters
                    .Select(genericTypeParameter => genericTypeParameter ?? typeInfo)
                    .ToArray());
    }

    /// <summary>
    /// Returns an instance of <see cref="DartCollectionFormatter"/> constructed with the
    /// specified parameters.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="DartCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// <para>
    /// If any of the values of <see cref="DartResolvedTypeInfo.GenericTypeParameters"/> is null,
    /// they will be substituted by the provided <see cref="DartResolvedTypeInfo"/> in the
    /// <see cref="ApplyTo"/> method.
    /// </para>
    /// </remarks>
    /// <param name="typeInfo">The collection type</param>
    public static DartCollectionFormatter Create(DartResolvedTypeInfo typeInfo)
    {
        return Cache.GetOrAdd(typeInfo.ToString(), _ => new DartCollectionFormatter(typeInfo));
    }

    /// <summary>
    /// Returns an instance of <see cref="DartCollectionFormatter"/> based on the provided
    /// <paramref name="collectionFormat"/>.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="DartCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    /// <param name="collectionFormat">The collection type</param>
    public static DartCollectionFormatter Create(string collectionFormat)
    {
        return Cache.GetOrAdd(
            collectionFormat,
            _ => new DartCollectionFormatter(collectionFormat));
    }

    /// <inheritdoc />
    public string Format(string type)
    {
        return type;
    }

    IResolvedTypeInfo ICollectionFormatter.ApplyTo(IResolvedTypeInfo typeInfo)
    {
        return ApplyTo((DartResolvedTypeInfo)typeInfo);
    }
}