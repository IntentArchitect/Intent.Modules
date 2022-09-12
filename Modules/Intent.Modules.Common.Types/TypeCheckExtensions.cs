using System;
using Intent.Metadata.Models;

namespace Intent.Modules.Common;

public static class TypeCheckExtensions
{
    /// <summary>
    /// Checks if the <paramref name="type"/> is a string. 
    /// </summary>
    public static bool IsStringType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Name, "string", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a string. 
    /// </summary>
    public static bool HasStringType(this ITypeReference typeReference)
    {
        return IsStringType(typeReference?.Element);
    }
}