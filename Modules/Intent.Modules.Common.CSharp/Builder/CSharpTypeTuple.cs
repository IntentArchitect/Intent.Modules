using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpTupleElement
{
    public CSharpTupleElement(CSharpType type, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(type);
        Type = type;
        Name = name;
    }
    
    public CSharpType Type { get; }
    public string? Name { get; }

    protected bool Equals(CSharpTupleElement other)
    {
        return Type.Equals(other.Type) && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTupleElement)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Name);
    }
}

public class CSharpTypeTuple : CSharpType
{
    public CSharpTypeTuple(IReadOnlyList<CSharpTupleElement> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);
        Elements = elements;
    }

    public IReadOnlyList<CSharpTupleElement> Elements { get; }

    public override string ToString()
    {
        return $"({string.Join(", ", Elements.Select(s => !string.IsNullOrWhiteSpace(s.Name) ? $"{s.Type} {s.Name}" : s.Type.ToString()))})";
    }

    protected bool Equals(CSharpTypeTuple other)
    {
        return Elements.SequenceEqual(other.Elements);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeTuple)obj);
    }

    public override int GetHashCode()
    {
        return Elements.GetHashCode();
    }
}