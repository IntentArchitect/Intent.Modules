#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591
public class CSharpTypeGeneric : CSharpType
{
    public CSharpTypeGeneric(string typeName, IReadOnlyList<CSharpType> typeArgumentList)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        ArgumentNullException.ThrowIfNull(typeArgumentList);

        TypeName = typeName;
        TypeArgumentList = typeArgumentList;
    }

    public string TypeName { get; }
    public IReadOnlyList<CSharpType> TypeArgumentList { get; }

    public override string ToString()
    {
        return $"{TypeName}<{string.Join(", ", TypeArgumentList.Select(s => s.ToString()))}>";
    }
    
    public bool IsTask()
    {
        return TypeName is CSharpType.TaskFullTypeName or CSharpType.TaskShortTypeName;
    }

    protected bool Equals(CSharpTypeGeneric other)
    {
        return TypeName == other.TypeName && TypeArgumentList.SequenceEqual(other.TypeArgumentList);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeGeneric)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TypeName, TypeArgumentList);
    }
}