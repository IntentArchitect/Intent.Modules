#nullable enable
using System;

namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591
public class CSharpTypeArray : CSharpType
{
    public CSharpTypeArray(CSharpType elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType);
        ElementType = elementType;
    }
    
    public CSharpType ElementType { get; }

    public override string ToString()
    {
        return $"{ElementType}[]";
    }

    protected bool Equals(CSharpTypeArray other)
    {
        return ElementType.Equals(other.ElementType);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeArray)obj);
    }

    public override int GetHashCode()
    {
        return ElementType.GetHashCode();
    }
}