#nullable enable
using System;

namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591
public class CSharpTypeName : CSharpType
{
    public CSharpTypeName(string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        TypeName = typeName;
    }

    public string TypeName { get; }

    public override string ToString()
    {
        return TypeName;
    }
    
    /// <summary>
    /// Is the current type representing a <see cref="System.Threading.Tasks.Task"/>?
    /// </summary>
    public bool IsTask()
    {
        return TypeName is CSharpType.TaskFullTypeName or CSharpType.TaskShortTypeName;
    }

    protected bool Equals(CSharpTypeName other)
    {
        return TypeName == other.TypeName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeName)obj);
    }

    public override int GetHashCode()
    {
        return TypeName.GetHashCode();
    }
}