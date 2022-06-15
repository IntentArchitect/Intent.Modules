using System;
using System.Collections.Concurrent;
using System.Linq;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Kotlin.TypeResolvers;

/// <summary>
/// Kotlin <see cref="ICollectionFormatter"/> which recognizes collection types with generic parameters
/// as well as the <c>[]</c> syntax.
/// </summary>
public class KotlinCollectionFormatter : ICollectionFormatter
{
    private readonly KotlinResolvedTypeInfo _typeInfo;
    private static readonly ConcurrentDictionary<string, KotlinCollectionFormatter> Cache = new();

    private KotlinCollectionFormatter(KotlinResolvedTypeInfo typeInfo)
    {
        _typeInfo = typeInfo;
    }

    private KotlinCollectionFormatter(string collectionFormat)
    {
        static KotlinResolvedTypeInfo Parse(string type)
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

            var genericTypeParameters = Array.Empty<KotlinResolvedTypeInfo>();
            if (TryGetBracketedContentFromEnd('<', '>', type, out var angledBracketsContent))
            {
                genericTypeParameters = angledBracketsContent
                    .Split(',')
                    .Select(Parse)
                    .ToArray();

                type = type[..^(angledBracketsContent.Length + 2)];
            }

            var package = string.Empty;
            var lastIndexOfPeriod = type.LastIndexOf(".", StringComparison.Ordinal);
            if (lastIndexOfPeriod >= 0)
            {
                package = type[..lastIndexOfPeriod];
                type = type[(lastIndexOfPeriod + 1)..];
            }

            return KotlinResolvedTypeInfo.Create(
                name: type,
                package: package,
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
    /// Returns an instance of <see cref="KotlinCollectionFormatter"/> based on the provided
    /// <paramref name="collectionFormat"/>.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="KotlinCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    /// <param name="collectionFormat">The collection type</param>
    public static KotlinCollectionFormatter GetOrCreate(string collectionFormat)
    {
        return Cache.GetOrAdd(
            collectionFormat,
            _ => new KotlinCollectionFormatter(collectionFormat));
    }

    /// <summary>
    /// Returns an instance of <see cref="KotlinCollectionFormatter"/> constructed with the
    /// specified parameters.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="KotlinCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// <para>
    /// If any of the values of <see cref="KotlinResolvedTypeInfo.GenericTypeParameters"/> is null,
    /// they will be substituted by the provided <see cref="KotlinResolvedTypeInfo"/> in the
    /// <see cref="ApplyTo"/> method.
    /// </para>
    /// </remarks>
    /// <param name="typeInfo">The collection type</param>
    public static KotlinCollectionFormatter GetOrCreate(KotlinResolvedTypeInfo typeInfo)
    {
        return Cache.GetOrAdd(typeInfo.ToString(), _ => new KotlinCollectionFormatter(typeInfo));
    }

    /// <inheritdoc />
    public string Format(string type)
    {
        return type;
    }

    IResolvedTypeInfo ICollectionFormatter.ApplyTo(IResolvedTypeInfo typeInfo)
    {
        return ApplyTo((KotlinResolvedTypeInfo)typeInfo);
    }

    /// <summary>
    /// Returns a <see cref="KotlinResolvedTypeInfo"/> which is the type of collection for this
    /// instance of the <see cref="KotlinCollectionFormatter"/> of the provided
    /// <paramref name="typeInfo"/>.
    /// </summary>
    public KotlinResolvedTypeInfo ApplyTo(KotlinResolvedTypeInfo typeInfo)
    {
        if (_typeInfo == null)
        {
            return typeInfo;
        }

        var isNullable = typeInfo.IsNullable;
        typeInfo = typeInfo.WithIsNullable(false);

        return KotlinResolvedTypeInfo.Create(
            name: _typeInfo.Name,
            package: _typeInfo.Package,
            isPrimitive: _typeInfo.IsPrimitive,
            isNullable: isNullable,
            isCollection: true,
            typeReference: _typeInfo.TypeReference,
            template: _typeInfo.Template,
            genericTypeParameters: _typeInfo.GenericTypeParameters.Count == 0
                ? Array.Empty<KotlinResolvedTypeInfo>()
                : _typeInfo.GenericTypeParameters
                    .Select(genericTypeParameter => genericTypeParameter ?? typeInfo)
                    .ToArray());
    }
}