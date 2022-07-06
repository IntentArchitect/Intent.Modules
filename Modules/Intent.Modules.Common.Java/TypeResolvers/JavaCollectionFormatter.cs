using System;
using System.Collections.Concurrent;
using System.Linq;
using Intent.Modules.Common.TypeResolution;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.Java.TypeResolvers;

/// <summary>
/// Java <see cref="ICollectionFormatter"/> which recognizes collection types with generic parameters
/// as well as the <c>[]</c> syntax.
/// </summary>
public class JavaCollectionFormatter : ICollectionFormatter
{
    private readonly JavaResolvedTypeInfo _typeInfo;
    private static readonly ConcurrentDictionary<string, JavaCollectionFormatter> Cache = new();

    private JavaCollectionFormatter(JavaResolvedTypeInfo typeInfo)
    {
        _typeInfo = typeInfo;
    }

    private JavaCollectionFormatter(string collectionFormat)
    {
        static JavaResolvedTypeInfo Parse(string type, bool disallowPrimitives = false)
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

            var arrayDimensionCount = 0;
            while (TryGetBracketedContentFromEnd('[', ']', type, out var squareBracketsContent))
            {
                arrayDimensionCount++;
                type = type[..^(squareBracketsContent.Length + 2)];
            }

            if (arrayDimensionCount > 0)
            {
                return JavaResolvedTypeInfo.CreateForArray(
                    forResolvedType: Parse(type),
                    isNullable: false,
                    arrayDimensionCount: arrayDimensionCount);
            }

            var genericTypeParameters = Array.Empty<JavaResolvedTypeInfo>();
            if (TryGetBracketedContentFromEnd('<', '>', type, out var angledBracketsContent))
            {
                genericTypeParameters = angledBracketsContent
                    .Split(',')
                    .Select(unparsedType => Parse(unparsedType, disallowPrimitives: true))
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

            return JavaResolvedTypeInfo.Create(
                name: disallowPrimitives && JavaTypeResolver.TryGetWrapperTypeName(type, out var wrapperTypeName)
                    ? wrapperTypeName
                    : type,
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
    /// Returns a <see cref="JavaResolvedTypeInfo"/> which is the type of collection for this
    /// instance of the <see cref="JavaCollectionFormatter"/> of the provided
    /// <paramref name="typeInfo"/>.
    /// </summary>
    public JavaResolvedTypeInfo ApplyTo(JavaResolvedTypeInfo typeInfo)
    {
        if (_typeInfo == null)
        {
            return typeInfo;
        }

        var isNullable = typeInfo.IsNullable;
        typeInfo = typeInfo.WithIsNullable(false);

        if (_typeInfo.ArrayDimensionCount > 0)
        {
            return JavaResolvedTypeInfo.CreateForArray(
                forResolvedType: typeInfo,
                isNullable: isNullable,
                arrayDimensionCount: _typeInfo.ArrayDimensionCount);
        }

        return JavaResolvedTypeInfo.Create(
            name: _typeInfo.Name,
            package: _typeInfo.Package,
            isPrimitive: false,
            isNullable: isNullable,
            isCollection: true,
            typeReference: _typeInfo.TypeReference,
            template: _typeInfo.Template,
            genericTypeParameters: _typeInfo.GenericTypeParameters.Count == 0
                ? Array.Empty<JavaResolvedTypeInfo>()
                : _typeInfo.GenericTypeParameters
                    .Select(genericTypeParameter =>
                        genericTypeParameter ??
                        (typeInfo.IsPrimitive
                            ? JavaTypeResolver.ToNonPrimitive(typeInfo)
                            : typeInfo))
                    .ToArray());
    }

    /// <summary>
    /// Returns an instance of <see cref="JavaCollectionFormatter"/> constructed with the
    /// specified parameters.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="JavaCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// <para>
    /// If any of the values of <see cref="JavaResolvedTypeInfo.GenericTypeParameters"/> is null,
    /// they will be substituted by the provided <see cref="JavaResolvedTypeInfo"/> in the
    /// <see cref="ApplyTo"/> method.
    /// </para>
    /// </remarks>
    /// <param name="typeInfo">The collection type</param>
    public static JavaCollectionFormatter Create(JavaResolvedTypeInfo typeInfo)
    {
        return Cache.GetOrAdd(typeInfo.ToString(), _ => new JavaCollectionFormatter(typeInfo));
    }

    /// <summary>
    /// Returns an instance of <see cref="JavaCollectionFormatter"/> based on the provided
    /// <paramref name="collectionFormat"/>.
    /// </summary>
    /// <remarks>
    /// A cache of <see cref="JavaCollectionFormatter"/> instances is first checked for an
    /// already existing instance, if an instance is found then that is returned, otherwise a new
    /// instance is created, placed in the cache and returned.
    /// </remarks>
    /// <param name="collectionFormat">The collection type</param>
    public static JavaCollectionFormatter Create(string collectionFormat)
    {
        return Cache.GetOrAdd(
            collectionFormat,
            _ => new JavaCollectionFormatter(collectionFormat));
    }

    /// <inheritdoc />
    public string Format(string type)
    {
        return type;
    }

    IResolvedTypeInfo ICollectionFormatter.ApplyTo(IResolvedTypeInfo typeInfo)
    {
        return ApplyTo((JavaResolvedTypeInfo)typeInfo);
    }
}