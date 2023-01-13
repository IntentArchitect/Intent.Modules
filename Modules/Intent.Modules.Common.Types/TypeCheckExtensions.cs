using System;
using Intent.Metadata.Models;

namespace Intent.Modules.Common;

public static class TypeCheckExtensions
{
    /// <summary>
    /// Checks if the <paramref name="type"/> is a Binary. 
    /// </summary>
    public static bool IsBinaryType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "013af2c5-3c32-4752-8f59-db5691050aef", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Binary. 
    /// </summary>
    public static bool HasBinaryType(this ITypeReference typeReference)
    {
        return IsBinaryType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Boolean. 
    /// </summary>
    public static bool IsBoolType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "e6f92b09-b2c5-4536-8270-a4d9e5bbd930", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Boolean. 
    /// </summary>
    public static bool HasBoolType(this ITypeReference typeReference)
    {
        return IsBoolType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Byte. 
    /// </summary>
    public static bool IsByteType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "A4E9102F-C1C8-4902-A417-CA418E1874D2", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Byte. 
    /// </summary>
    public static bool HasByteType(this ITypeReference typeReference)
    {
        return IsByteType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Character. 
    /// </summary>
    public static bool IsCharType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "C1B3A361-B1C6-48C3-B34C-7999B3E071F0", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Character. 
    /// </summary>
    public static bool HasCharType(this ITypeReference typeReference)
    {
        return IsCharType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Date. 
    /// </summary>
    public static bool IsDateType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "1fbaa056-b666-4f25-b8fd-76fe3165acc8", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Date. 
    /// </summary>
    public static bool HasDateType(this ITypeReference typeReference)
    {
        return IsDateType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a DateTime. 
    /// </summary>
    public static bool IsDateTimeType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "a4107c29-7851-4121-9416-cf1236908f1e", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a DateTime. 
    /// </summary>
    public static bool HasDateTimeType(this ITypeReference typeReference)
    {
        return IsDateTimeType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a DateTimeOffset. 
    /// </summary>
    public static bool IsDateTimeOffsetType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "f1ba4df3-a5bc-427e-a591-4f6029f89bd7", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a DateTimeOffset. 
    /// </summary>
    public static bool HasDateTimeOffsetType(this ITypeReference typeReference)
    {
        return IsDateTimeOffsetType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Decimal. 
    /// </summary>
    public static bool IsDecimalType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "675c7b84-997a-44e0-82b9-cd724c07c9e6", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Decimal. 
    /// </summary>
    public static bool HasDecimalType(this ITypeReference typeReference)
    {
        return IsDecimalType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Double. 
    /// </summary>
    public static bool IsDoubleType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "24A77F70-5B97-40DD-8F9A-4208AD5F9219", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Double. 
    /// </summary>
    public static bool HasDoubleType(this ITypeReference typeReference)
    {
        return IsDoubleType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Float. 
    /// </summary>
    public static bool IsFloatType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "341929E9-E3E7-46AA-ACB3-B0438421F4C4", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Float. 
    /// </summary>
    public static bool HasFloatType(this ITypeReference typeReference)
    {
        return IsFloatType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Guid. 
    /// </summary>
    public static bool IsGuidType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "6b649125-18ea-48fd-a6ba-0bfff0d8f488", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Guid. 
    /// </summary>
    public static bool HasGuidType(this ITypeReference typeReference)
    {
        return IsGuidType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is an Integer. 
    /// </summary>
    public static bool IsIntType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is an Integer. 
    /// </summary>
    public static bool HasIntType(this ITypeReference typeReference)
    {
        return IsIntType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Long. 
    /// </summary>
    public static bool IsLongType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "33013006-E404-48C2-AC46-24EF5A5774FD", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Long. 
    /// </summary>
    public static bool HasLongType(this ITypeReference typeReference)
    {
        return IsLongType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is an Object. 
    /// </summary>
    public static bool IsObjectType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "341DD965-D06C-4A40-9437-9516ADA77FF5", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is an Object. 
    /// </summary>
    public static bool HasObjectType(this ITypeReference typeReference)
    {
        return IsObjectType(typeReference?.Element);
    }

    /// <summary>
    /// Checks if the <paramref name="type"/> is a Short. 
    /// </summary>
    public static bool IsShortType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "2ABF0FD3-CD56-4349-8838-D120ED268245", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a Short. 
    /// </summary>
    public static bool HasShortType(this ITypeReference typeReference)
    {
        return IsShortType(typeReference?.Element);
    }
    
    /// <summary>
    /// Checks if the <paramref name="type"/> is a String. 
    /// </summary>
    public static bool IsStringType(this ICanBeReferencedType type)
    {
        return string.Equals(type?.Id, "d384db9c-a279-45e1-801e-e4e8099625f2", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a String. 
    /// </summary>
    public static bool HasStringType(this ITypeReference typeReference)
    {
        return IsStringType(typeReference?.Element);
    }
}