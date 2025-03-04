using System;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp;

/// <summary>
/// Type check extension methods for types defined in the Intent.Common.CSharp module.
/// </summary>
public static class TypeCheckExtensions
{
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Dictionary.
    /// </summary>
    public static bool IsDictionaryType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "ac2b65c3-6a8f-454a-b520-b583350c43ef", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Dictionary.
    /// </summary>
    public static bool HasDictionaryType(this ITypeReference typeReference)
    {
        return IsDictionaryType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Task.
    /// </summary>
    public static bool IsTaskType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "6cdaffae-7609-47ab-b8db-9d7282d2df6f", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Task.
    /// </summary>
    public static bool HasTaskType(this ITypeReference typeReference)
    {
        return IsTaskType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Task&lt;T&gt;.
    /// </summary>
    public static bool IsTaskOfTType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "5aed838d-ba89-4877-a007-49a249994abc", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Task&lt;T&gt;.
    /// </summary>
    public static bool HasTaskOfTType(this ITypeReference typeReference)
    {
        return IsTaskOfTType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a TimeSpan.
    /// </summary>
    public static bool IsTimeSpanType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "46dbdc6c-aaa7-4d2e-ba1f-81abdb87a888", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a TimeSpan.
    /// </summary>
    public static bool HasTimeSpanType(this ITypeReference typeReference)
    {
        return IsTimeSpanType(typeReference?.Element);
    }
}