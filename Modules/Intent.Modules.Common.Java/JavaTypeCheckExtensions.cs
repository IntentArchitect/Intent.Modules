using System;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.Java;

public static class JavaTypeCheckExtensions
{
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Boolean. 
    /// </summary>
    public static bool IsJavaBooleanType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "d4c82b09-853f-417f-b9e3-dc3012fa1e84", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Boolean. 
    /// </summary>
    public static bool HasJavaBooleanType(this ITypeReference typeReference)
    {
        return IsJavaBooleanType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Byte. 
    /// </summary>
    public static bool IsJavaByteType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "1bff28a9-6cca-471c-b7e9-287402154651", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Byte. 
    /// </summary>
    public static bool HasJavaByteType(this ITypeReference typeReference)
    {
        return IsJavaByteType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Character. 
    /// </summary>
    public static bool IsJavaCharacterType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "b3f2263f-66bc-4792-9f93-3c44aac4580c", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Character. 
    /// </summary>
    public static bool HasJavaCharacterType(this ITypeReference typeReference)
    {
        return IsJavaCharacterType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Double. 
    /// </summary>
    public static bool IsJavaDoubleType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "29ba80af-7502-4f9d-b94d-be17cb4146d0", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Double. 
    /// </summary>
    public static bool HasJavaDoubleType(this ITypeReference typeReference)
    {
        return IsJavaDoubleType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Float. 
    /// </summary>
    public static bool IsJavaFloatType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "fbeecaa9-dc50-4067-88fc-a209f9a9a318", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Float. 
    /// </summary>
    public static bool HasJavaFloatType(this ITypeReference typeReference)
    {
        return IsJavaFloatType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Integer. 
    /// </summary>
    public static bool IsJavaIntegerType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "b3e5cb3b-8a26-4346-810b-9789afa25a82", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Integer. 
    /// </summary>
    public static bool HasJavaIntegerType(this ITypeReference typeReference)
    {
        return IsJavaIntegerType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Long. 
    /// </summary>
    public static bool IsJavaLongType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "e9e575eb-f8de-4ce4-9838-2d09665a752d", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Long. 
    /// </summary>
    public static bool HasJavaLongType(this ITypeReference typeReference)
    {
        return IsJavaLongType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Map. 
    /// </summary>
    public static bool IsJavaMapType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "49c6163a-3640-4df5-a7c9-ed4bab8af996", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Map. 
    /// </summary>
    public static bool HasJavaMapType(this ITypeReference typeReference)
    {
        return IsJavaMapType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java Short. 
    /// </summary>
    public static bool IsJavaShortType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "1dc92ff9-9334-4227-bbe2-6822c6df1694", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java Short. 
    /// </summary>
    public static bool HasJavaShortType(this ITypeReference typeReference)
    {
        return IsJavaShortType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Java String. 
    /// </summary>
    public static bool IsJavaStringType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "a99879cb-8843-4514-91c7-50d07c221bf9", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Java String. 
    /// </summary>
    public static bool HasJavaStringType(this ITypeReference typeReference)
    {
        return IsJavaStringType(typeReference?.Element);
    }
}