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
        return string.Equals(type?.Id, ElementId.Binary, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Bool, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Byte, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Char, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Date, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.DateTime, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.DateTimeOffset, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Decimal, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Double, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Float, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Guid, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Int, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Long, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Object, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.Short, StringComparison.OrdinalIgnoreCase);
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
        return string.Equals(type?.Id, ElementId.String, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the <paramref name="typeReference"/>'s <see cref="ITypeReference.Element"/> is a String. 
    /// </summary>
    public static bool HasStringType(this ITypeReference typeReference)
    {
        return IsStringType(typeReference?.Element);
    }
}