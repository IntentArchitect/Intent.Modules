#nullable enable
using System;

namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591
public class CSharpTypeNullable : CSharpType
{
    public CSharpTypeNullable(CSharpType elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType);
        ElementType = elementType;
    }
    
    public CSharpType ElementType { get; }

    public override string ToString()
    {
        return $"{ElementType}?";
    }

    protected bool Equals(CSharpTypeNullable other)
    {
        return ElementType.Equals(other.ElementType);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeNullable)obj);
    }

    public override int GetHashCode()
    {
        return ElementType.GetHashCode();
    }
}